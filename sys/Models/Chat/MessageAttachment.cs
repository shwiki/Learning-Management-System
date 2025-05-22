using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.Models.Chat
{
    public class MessageAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        public AttachmentType Type { get; set; }

        [Required]
        public string FilePath { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; }
        public enum AttachmentType
        {
            Audio,
            Image
        }
    }
}