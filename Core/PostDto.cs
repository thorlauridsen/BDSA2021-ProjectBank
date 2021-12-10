using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record PostDetailsDto
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Content { get; init; }
        public DateTime DateAdded { get; init; }
        public string UserOid { get; init; }
        public HashSet<string> Tags { get; init; }
        public PostState PostState { get; init; }
        public int ViewCount { get; init; }
    }

    public record PostCreateDto
    {
        [RegularExpression(@"[a-zA-Z0-9 \-\/]+")]
        [StringLength(255)]
        public string Title { get; init; }

        public string Content { get; init; }
        public string UserOid { get; init; }
        public HashSet<string> Tags { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public int Id { get; init; }

        public PostState PostState { get; init; }

        public int ViewCount { get; init; }
    }
}
