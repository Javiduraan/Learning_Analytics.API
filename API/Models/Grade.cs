using System.Collections.Generic;

namespace API.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public string GradeName { get; set; }
        public string Section { get; set; }
        public ICollection<Student> Students { get; set; }


    }
}