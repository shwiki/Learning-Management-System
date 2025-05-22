using sys.Models.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.ViewModels
{
    public class ChatThreadViewModel
    {
        public string ConversationPartnerId { get; set; } // AspNetUsers.Id
        public string ConversationPartnerName { get; set; } // e.g. email or FirstName+LastName
        public string PartnerPhotoPath { get; set; } // student or teacher photo
        public string LastMessageSnippet { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}