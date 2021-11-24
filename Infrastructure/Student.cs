namespace ProjectBank.Infrastructure
{
    public class Student : User
    {
        [StringLength(50)]
        public string Course { get; set; }

        public Student(string name, string course)
        {
            Name = name;
            Course = course;
        }
    }
}
