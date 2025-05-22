using sys.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.Models.ViewModels
{
    public class GroupChatViewModel
    {
        // All attachments & text messages for the class
        public List<MessageBubbleViewModel> Messages { get; set; }
            = new List<MessageBubbleViewModel>();

        // Optional text
        [Display(Name = "Text Message")]
        public string NewMessageContent { get; set; }

        // Optional audio or image upload
        [Display(Name = "Record Audio")]
        public HttpPostedFileBase AudioUpload { get; set; }

        [Display(Name = "Upload Image")]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
}