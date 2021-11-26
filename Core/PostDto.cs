using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record PostDto(
        int Id,
        string Title,
        string Content,
        DateTime DateAdded,
        int SupervisorId,
        HashSet<string> Tags,
        HashSet<int> Comments
    );

    public record PostDetailsDto(
        int Id,
        string Title,
        string Content,
        DateTime DateAdded,
        int SupervisorId,
        HashSet<string> Tags,
        HashSet<int> Comments
    ) : PostDto(Id, Title, Content, DateAdded, SupervisorId, Tags, Comments);

    public record PostCreateDto
    {
        public PostCreateDto(
            string title,
            string content,
            DateTime dateAdded,
            int supervisorId,
            HashSet<string> tags,
            HashSet<int> comments)
        {
            Title = title;
            Content = content;
            DateAdded = dateAdded;
            SupervisorId = supervisorId;
            Tags = tags;
            Comments = comments;
        }

        [StringLength(50)]
        public string Title { get; init; }
        public string Content { get; init; }
        public DateTime DateAdded { get; init; }
        public int SupervisorId { get; init; }
        public HashSet<string> Tags { get; init; }
        public HashSet<int> Comments { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public PostUpdateDto(
            string title,
            string content,
            DateTime dateAdded,
            int supervisorId,
            HashSet<string> tags,
            HashSet<int> comments
        ) : base(title, content, dateAdded, supervisorId, tags, comments) { }

        public int Id { get; init; }
    }
}
