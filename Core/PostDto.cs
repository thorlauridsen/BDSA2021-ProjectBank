using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record PostDto(int Id, string Title, string Content);

    public record PostDetailsDto(int Id, string Title, string Content) : PostDto(Id, Title, Content);

    public record PostCreateDto
    {
        [StringLength(50)]
        public string? Title { get; init; }
        public string? Content { get; init; }
    }

    public record PostUpdateDto : PostCreateDto
    {
        public int Id { get; init; }
    }
}
