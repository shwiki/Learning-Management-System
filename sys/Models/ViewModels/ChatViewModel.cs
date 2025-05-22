using sys.Models.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.ViewModels
{
    public class ChatViewModel
    {
        [Required]
        public string RecipientId { get; set; }

        public string RecipientName { get; set; }
        public string RecipientPhoto { get; set; }

        public List<MessageBubbleViewModel> Messages { get; set; }
            = new List<MessageBubbleViewModel>();

        // Text entry (optional if you send an attachment)
        [Display(Name = "Text Message")]
        public string NewMessageContent { get; set; }

        // File uploads: either/or (we’ll handle “at least one” in the controller)
        [Display(Name = "Record Audio")]
        public HttpPostedFileBase AudioUpload { get; set; }

        [Display(Name = "Upload Image")]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
}