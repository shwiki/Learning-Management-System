using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using sys.Models;      // ApprovedStudent, Teacher, Message, MessageRead, ApplicationDbContext
using sys.Models.Chat;
using sys.Models.ViewModels;
using sys.ViewModels;
using static sys.Models.Chat.MessageAttachment; // ChatThreadViewModel, ChatViewModel, MessageBubbleViewModel

namespace sys.Controllers
{
    [Authorize]
    public class StudentMessagesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // --------------------------------------------------------------------
        // GET: /StudentMessages/Threads
        // --------------------------------------------------------------------
        public async Task<ActionResult> Threads()
        {
            var meId = User.Identity.GetUserId();
            var meUser = await _db.Users.SingleOrDefaultAsync(u => u.Id == meId);
            var meStu = await _db.ApprovedStudents
                          .SingleOrDefaultAsync(s => s.Email == meUser.UserName);
            if (meStu == null) return HttpNotFound("Student record not found.");

            var grade = meStu.ClassName;
            var threads = new List<ChatThreadViewModel>();

            // --- Add teacher contact ---
            var teacher = await _db.CreateTeacherViewModel.SingleOrDefaultAsync(t => t.ClassName == grade);
            if (teacher != null)
            {
                var tUser = await _db.Users.SingleOrDefaultAsync(u => u.Email == teacher.Email);
                if (tUser != null)
                    await AddOrUpdateThreadAsync(threads,
                        meId, tUser.Id, teacher.Email, teacher.PhotoPath);
            }

            // --- Add all peers ---
            var peers = await _db.ApprovedStudents
                          .Where(s => s.ClassName == grade && s.Email != meUser.UserName)
                          .ToListAsync();
            foreach (var peer in peers)
            {
                var pUser = await _db.Users.SingleOrDefaultAsync(u => u.Email == peer.Email);
                if (pUser != null)
                {
                    var name = $"{peer.FirstName} {peer.LastName}";
                    await AddOrUpdateThreadAsync(threads,
                        meId, pUser.Id, name, peer.PhotoPath);
                }
            }

            // sort by time, nulls (never chatted) go last
            threads = threads
                .OrderByDescending(t => t.LastMessageTime ?? DateTime.MinValue)
                .ToList();

            return View("Threads", threads);
        }
        private async Task AddOrUpdateThreadAsync(
          List<ChatThreadViewModel> list,
          string meId,
          string partnerId,
          string partnerName,
          string partnerPhoto
      )
        {
            // get most recent message between us
            var last = await _db.Messages
                .Where(m =>
                    (m.SenderId == meId && m.RecipientId == partnerId) ||
                    (m.SenderId == partnerId && m.RecipientId == meId)
                )
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            // calculate unread count
            var unreadCount = last == null
              ? 0
              : await _db.Messages
                    .Where(m =>
                        m.SenderId == partnerId &&
                        m.RecipientId == meId &&
                        !m.ReadReceipts.Any(rr => rr.UserId == meId)
                    ).CountAsync();

            // build a safe snippet
            var text = last?.TextContent ?? "";
            string snippet;
            if (string.IsNullOrWhiteSpace(text))
            {
                snippet = "Tap to chat";
            }
            else if (text.Length <= 30)
            {
                snippet = text;
            }
            else
            {
                snippet = text.Substring(0, 30) + "…";
            }

            list.Add(new ChatThreadViewModel
            {
                ConversationPartnerId = partnerId,
                ConversationPartnerName = partnerName,
                PartnerPhotoPath = partnerPhoto,
                LastMessageSnippet = snippet,
                LastMessageTime = last?.SentAt,
                UnreadCount = unreadCount
            });
        }
        // helper that populates a single ChatThreadViewModel and adds it to the list
        private async Task AddThreadAsync(
            List<ChatThreadViewModel> list,
            string meId,
            string partnerId,
            string partnerName,
            string partnerPhoto
        )
        {
            // last message between us
            var last = await _db.Messages
                .Where(m =>
                    (m.SenderId == meId && m.RecipientId == partnerId) ||
                    (m.SenderId == partnerId && m.RecipientId == meId)
                )
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
            if (last == null) return;

            // unread for me
            var unreadCount = await _db.Messages
                .Where(m =>
                    m.SenderId == partnerId &&
                    m.RecipientId == meId &&
                    !m.ReadReceipts.Any(rr => rr.UserId == meId)
                )
                .CountAsync();

            list.Add(new ChatThreadViewModel
            {
                ConversationPartnerId = partnerId,
                ConversationPartnerName = partnerName,
                PartnerPhotoPath = partnerPhoto,
                LastMessageSnippet = last.TextContent.Length > 30
                                           ? last.TextContent.Substring(0, 30) + "…"
                                           : last.TextContent,
                LastMessageTime = last.SentAt,
                UnreadCount = unreadCount
            });
        }

        // ----------------------------------------------------------
        // GET: /StudentMessages/Chat?partnerId=XYZ
        // ----------------------------------------------------------
        public async Task<ActionResult> Chat(string partnerId)
        {
            if (String.IsNullOrWhiteSpace(partnerId))
                return RedirectToAction("Threads");

            var meId = User.Identity.GetUserId();
            var meUser = await _db.Users.SingleOrDefaultAsync(u => u.Id == meId);
            if (meUser == null) return HttpNotFound();

            // mark incoming as read…
            var incoming = await _db.Messages
                .Where(m => m.SenderId == partnerId
                         && m.RecipientId == meId
                         && !m.ReadReceipts.Any(rr => rr.UserId == meId))
                .ToListAsync();
            foreach (var m in incoming)
                _db.MessageReads.Add(new MessageRead
                {
                    MessageId = m.Id,
                    UserId = meId,
                    ReadAt = DateTime.UtcNow
                });
            await _db.SaveChangesAsync();

            // load messages **with** attachments
            var msgs = await _db.Messages
                .Include(m => m.ReadReceipts)
                .Include(m => m.Attachments)
                .Where(m =>
                    (m.SenderId == meId && m.RecipientId == partnerId) ||
                    (m.SenderId == partnerId && m.RecipientId == meId)
                )
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // project into bubbles
            var bubbles = msgs.Select(m => new MessageBubbleViewModel
            {
                SenderId = m.SenderId,
                SenderName = m.Sender.UserName,
                TextContent = m.TextContent,
                SentAt = m.SentAt,
                IsRead = m.SenderId == meId
                                 ? m.ReadReceipts.Any(rr => rr.UserId == partnerId)
                                 : true,
                Attachments = m.Attachments
                    .Select(a => new MessageAttachmentViewModel
                    {
                        FilePath = a.FilePath,
                        Type = a.Type
                    })
                    .ToList()
            }).ToList();

            var partnerUser = await _db.Users.SingleOrDefaultAsync(u => u.Id == partnerId);
            if (partnerUser == null) return RedirectToAction("Threads");

            var vm = new ChatViewModel
            {
                RecipientId = partnerId,
                RecipientName = partnerUser.UserName,
                RecipientPhoto = "", // or lookup photo path
                Messages = bubbles,
                NewMessageContent = String.Empty
            };

            return View("Chat", vm);
        }
        // ----------------------------------------------------------
        // POST: /StudentMessages/Chat
        // ----------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Chat(ChatViewModel vm)
        {
            // require at least one of text/audio/image
            if (String.IsNullOrWhiteSpace(vm.NewMessageContent)
             && (vm.AudioUpload == null || vm.AudioUpload.ContentLength == 0)
             && (vm.ImageUpload == null || vm.ImageUpload.ContentLength == 0))
            {
                ModelState.AddModelError("", "Please enter text or attach audio/image.");
            }
            if (!ModelState.IsValid)
                return View("Chat", vm);

            var meId = User.Identity.GetUserId();
            var meUser = await _db.Users.SingleAsync(u => u.Id == meId);
            var meStu = await _db.ApprovedStudents
                            .SingleAsync(s => s.Email == meUser.UserName);

            // create base message
            var msg = new Message
            {
                ClassId = meStu.ClassName,
                SenderId = meId,
                RecipientId = vm.RecipientId,
                TextContent = vm.NewMessageContent,
                SentAt = DateTime.UtcNow
            };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            // handle audio
            if (vm.AudioUpload?.ContentLength > 0)
            {
                // ensure folder exists
                var audioDir = Server.MapPath("~/Uploads/Audio");
                if (!Directory.Exists(audioDir))
                    Directory.CreateDirectory(audioDir);

                var audioName = Guid.NewGuid() + Path.GetExtension(vm.AudioUpload.FileName);
                var audioPath = Path.Combine(audioDir, audioName);
                vm.AudioUpload.SaveAs(audioPath);

                _db.MessageAttachments.Add(new MessageAttachment
                {
                    MessageId = msg.Id,
                    Type = AttachmentType.Audio,
                    FilePath = "/Uploads/Audio/" + audioName
                });
            }

            // ——— IMAGE upload ———
            if (vm.ImageUpload?.ContentLength > 0)
            {
                // ensure folder exists
                var imageDir = Server.MapPath("~/Uploads/Images");
                if (!Directory.Exists(imageDir))
                    Directory.CreateDirectory(imageDir);

                var imgName = Guid.NewGuid() + Path.GetExtension(vm.ImageUpload.FileName);
                var imgPath = Path.Combine(imageDir, imgName);
                vm.ImageUpload.SaveAs(imgPath);

                _db.MessageAttachments.Add(new MessageAttachment
                {
                    MessageId = msg.Id,
                    Type = AttachmentType.Image,
                    FilePath = "/Uploads/Images/" + imgName
                });
            }

            await _db.SaveChangesAsync();

            // mark own message as read …
            _db.MessageReads.Add(new MessageRead
            {
                MessageId = msg.Id,
                UserId = meId,
                ReadAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Chat), new { partnerId = vm.RecipientId });
        }
        // --------------------------------------------------------------------
        // GET: /StudentMessages/GroupChat
        // --------------------------------------------------------------------
        public async Task<ActionResult> GroupChat()
        {
            var meId = User.Identity.GetUserId();
            var meUser = await _db.Users.SingleOrDefaultAsync(u => u.Id == meId);
            if (meUser == null) return HttpNotFound("User not found.");

            // mark all incoming (RecipientId==null) as read
            var unread = await _db.Messages
                .Where(m => m.RecipientId == null
                         && m.SenderId != meId
                         && !m.ReadReceipts.Any(rr => rr.UserId == meId))
                .ToListAsync();
            foreach (var m in unread)
                _db.MessageReads.Add(new MessageRead
                {
                    MessageId = m.Id,
                    UserId = meId,
                    ReadAt = DateTime.UtcNow
                });
            await _db.SaveChangesAsync();

            // load all class‐wide posts
            var meStu = await _db.ApprovedStudents
                           .SingleOrDefaultAsync(s => s.Email == meUser.UserName);
            if (meStu == null) return HttpNotFound("Student record not found.");

            var msgs = await _db.Messages
                .Include(m => m.ReadReceipts)
                .Include(m => m.Attachments)
                .Where(m => m.ClassId == meStu.ClassName && m.RecipientId == null)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            var bubbles = msgs.Select(m => new MessageBubbleViewModel
            {
                SenderId = m.SenderId,
                SenderName = m.Sender.UserName,
                TextContent = m.TextContent,
                SentAt = m.SentAt,
                IsRead = m.SenderId == meId
                                 ? m.ReadReceipts.Any(rr => rr.UserId != meId)
                                 : true,
                Attachments = m.Attachments
                    .Select(a => new MessageAttachmentViewModel
                    {
                        FilePath = a.FilePath,
                        Type = a.Type
                    }).ToList()
            }).ToList();

            var vm = new GroupChatViewModel
            {
                Messages = bubbles
            };
            return View("GroupChat", vm);
        }

        // --------------------------------------------------------------------
        // POST: /StudentMessages/GroupChat
        // --------------------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> GroupChat(GroupChatViewModel vm)
        {
            // require something to send
            if (string.IsNullOrWhiteSpace(vm.NewMessageContent)
             && (vm.AudioUpload == null || vm.AudioUpload.ContentLength == 0)
             && (vm.ImageUpload == null || vm.ImageUpload.ContentLength == 0))
            {
                ModelState.AddModelError("", "Please enter text or attach audio/image.");
            }
            if (!ModelState.IsValid)
                return View("GroupChat", vm);

            var meId = User.Identity.GetUserId();
            var meUser = await _db.Users.SingleAsync(u => u.Id == meId);
            var meStu = await _db.ApprovedStudents
                            .SingleAsync(s => s.Email == meUser.UserName);

            // base message
            var msg = new Message
            {
                ClassId = meStu.ClassName,
                SenderId = meId,
                RecipientId = null,             // null = group
                TextContent = vm.NewMessageContent,
                SentAt = DateTime.UtcNow
            };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            // ensure folders exist & save attachments
            var audioDir = Server.MapPath("~/Uploads/Audio");
            if (!Directory.Exists(audioDir)) Directory.CreateDirectory(audioDir);

            if (vm.AudioUpload?.ContentLength > 0)
            {
                var name = Guid.NewGuid() + Path.GetExtension(vm.AudioUpload.FileName);
                var full = Path.Combine(audioDir, name);
                vm.AudioUpload.SaveAs(full);
                _db.MessageAttachments.Add(new MessageAttachment
                {
                    MessageId = msg.Id,
                    Type = AttachmentType.Audio,
                    FilePath = "/Uploads/Audio/" + name
                });
            }

            var imageDir = Server.MapPath("~/Uploads/Images");
            if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);

            if (vm.ImageUpload?.ContentLength > 0)
            {
                var name = Guid.NewGuid() + Path.GetExtension(vm.ImageUpload.FileName);
                var full = Path.Combine(imageDir, name);
                vm.ImageUpload.SaveAs(full);
                _db.MessageAttachments.Add(new MessageAttachment
                {
                    MessageId = msg.Id,
                    Type = AttachmentType.Image,
                    FilePath = "/Uploads/Images/" + name
                });
            }

            await _db.SaveChangesAsync();

            // mark own as read
            _db.MessageReads.Add(new MessageRead
            {
                MessageId = msg.Id,
                UserId = meId,
                ReadAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(GroupChat));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
