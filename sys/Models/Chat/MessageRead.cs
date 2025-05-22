using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.Models.Chat
{
    public class MessageRead
    {
        [Key, Column(Order = 0)]
        public int MessageId { get; set; }
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        [Required]
        public DateTime ReadAt { get; set; }

        public virtual Message Message { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}