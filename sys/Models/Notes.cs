using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models
{
    public class Notes
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public int Level { get; set; }
        public string Topic { get; set; }
        public string KeyContent { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Notes() { }
    }
}