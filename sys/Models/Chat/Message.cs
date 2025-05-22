using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static sys.Models.Student.PendingUser;

namespace sys.Models.Chat
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public GradeNo ClassId { get; set; }

        [Required]
        public string SenderId { get; set; }

        public string RecipientId { get; set; }   // null = class‐wide

        public string TextContent { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual ApplicationUser Sender { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public virtual ApplicationUser Recipient { get; set; }

        public virtual ICollection<MessageRead> ReadReceipts { get; set; }
        public virtual ICollection<MessageAttachment> Attachments { get; set; }

        public Message()
        {
            ReadReceipts = new HashSet<MessageRead>();
            Attachments = new HashSet<MessageAttachment>();
        }
    }
}