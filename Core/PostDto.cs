using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record PostDto(
        int Id,
        string Title,
        string Content,
        DateTime DateAdded,
        string SupervisorOid,
        HashSet<string> Tags,
        PostState PostState,
        int ViewCount
    );

    public record PostDetailsDto(
        int Id,
        string Title,
        string Content,
        DateTime DateAdded,
        string SupervisorOid,
        HashSet<string> Tags,
        PostState PostState,
        int ViewCount
    ) : PostDto(Id, Title, Content, DateAdded, SupervisorOid, Tags, PostState, ViewCount);

    public record PostCreateDto
    {
        [RegularExpression(@"[a-zA-Z0-9 \-\/]+")]
        [StringLength(255)]
        public string Title { get; init; }

        public string Content { get; init; }
        public string SupervisorOid { get; init; }
        public HashSet<string> Tags { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public int Id { get; init; }

        public PostState PostState { get; init; }

        public int ViewCount { get; init; }
    }
}
