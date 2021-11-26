using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record PostDto(int Id, string Title, string Content, int SupervisorId, HashSet<string> Tags);

    public record PostDetailsDto(int Id, string Title, string Content, int SupervisorId, HashSet<string> Tags) : PostDto(Id, Title, Content, SupervisorId, Tags);

    public record PostCreateDto
    {
        public PostCreateDto(string title, string content, int supervisorId, HashSet<string> tags)
        {
            Title = title;
            Content = content;
            SupervisorId = supervisorId;
            Tags = tags;
        }

        [StringLength(50)]
        public string Title { get; init; }
        public string Content { get; init; }
        public int SupervisorId { get; init; }
        public HashSet<string> Tags { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public PostUpdateDto(string title, string content, int supervisorId, HashSet<string> tags) : base(title, content, supervisorId, tags) { }

        public int Id { get; init; }
    }
}
