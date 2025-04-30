using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static sys.Models.Student.PendingUser;

namespace sys.Models
{
    public class Notes
    {
        public int Id { get; set; }
        public SubjectSelects Subject { get; set; }

        public GradeNo ClassName { get; set; }
        public string Topic { get; set; }
        public string KeyContent { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public enum SubjectSelects
        {
            Shona,
            Maths,
            FAREME,
            PE,
            English,
            ICT
        }
        public Notes() { }
    }
}