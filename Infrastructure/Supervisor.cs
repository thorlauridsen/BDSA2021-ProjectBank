namespace ProjectBank.Infrastructure
{
    public class Supervisor : User
    {
        public int Salary { get; set; }
        public ICollection<Post> posts { get; set; } = null!;
        public Supervisor(string name, int salary)
        {
            Name = name;
            Salary = salary;
        }
    }
}
