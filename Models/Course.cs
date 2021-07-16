using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoursesAssignment.Models
{
    public class Course
    {
        public int Id { get; set; }
        public List<Slide> Slides { get; set; }
    }
}