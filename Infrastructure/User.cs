namespace ProjectBank.Infrastructure
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }
    }
}
