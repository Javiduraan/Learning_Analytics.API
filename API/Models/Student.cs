namespace API.Models
{
    public class Student
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GradeId { get; set; }
        public Grade Grade { get; set; }
        public long TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}