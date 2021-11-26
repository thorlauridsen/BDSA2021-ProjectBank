namespace ProjectBank.Infrastructure
{
    public class Tag
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Post> Post { get; set; } = null!;

        public Tag(string name)
        {
            Name = name;
        }
    }
}
