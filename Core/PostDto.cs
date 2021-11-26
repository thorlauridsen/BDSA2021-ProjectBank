using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record PostDto(
        int Id,
        string Title,
        string Content,
        DateTime DateAdded,
        int SupervisorId,
        HashSet<string> Tags
    );

    public record PostDetailsDto(
        int Id,
        string Title,
        string Content,
        DateTime DateAdded,
        int SupervisorId,
        HashSet<string> Tags
    ) : PostDto(Id, Title, Content, DateAdded, SupervisorId, Tags);

    public record PostCreateDto
    {
        public PostCreateDto(
            string title,
            string content,
            DateTime dateAdded,
            int supervisorId,
            HashSet<string> tags)
        {
            Title = title;
            Content = content;
            DateAdded = dateAdded;
            SupervisorId = supervisorId;
            Tags = tags;
        }

        [StringLength(50)]
        public string Title { get; init; }
        public string Content { get; init; }
        public DateTime DateAdded { get; init; }
        public int SupervisorId { get; init; }
        public HashSet<string> Tags { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public PostUpdateDto(
            string title,
            string content,
            DateTime dateAdded,
            int supervisorId,
            HashSet<string> tags
        ) : base(title, content, dateAdded, supervisorId, tags) { }

        public int Id { get; init; }
    }
}
