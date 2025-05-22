using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static sys.Models.Chat.MessageAttachment;

namespace sys.Models.ViewModels
{
    public class MessageAttachmentViewModel
    {
        // The URL or relative path to the saved file
        public string FilePath { get; set; }

        // “Audio” or “Image”
        public AttachmentType Type { get; set; }
    }
}