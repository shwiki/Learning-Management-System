using sys.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.ViewModels
{
    public class MessageBubbleViewModel
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }

        // May be null if it was purely an attachment
        public string TextContent { get; set; }

        // List of any images or audio files attached to this message
        public List<MessageAttachmentViewModel> Attachments { get; set; }
            = new List<MessageAttachmentViewModel>();

        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}