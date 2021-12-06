namespace ProjectBank.Infrastructure
{
    public class User
    {
        [Key]
        public string oid { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public bool IsSupervisor { get; set; }
    }
}
