using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace sys.Models.Approval
{
    [Table("UserParentChild")]
    public class UserParentChild
    {
        [Key, Column(Order = 0)]
        public string ParentId { get; set; }

        [Key, Column(Order = 1)]
        public string ChildId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual ApplicationUser Parent { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual ApplicationUser Child { get; set; }
    }
}