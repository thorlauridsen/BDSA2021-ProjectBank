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
        [StringLength(50)]
        public string Title { get; init; }
        public string Content { get; init; }
        public DateTime DateAdded { get; init; }
        public int SupervisorId { get; init; }
        public HashSet<string> Tags { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public int Id { get; init; }
    }
}
