namespace ProjectBank.Core
{
    public record CommentDto(
        int Id,
        string Content,
        DateTime DateAdded,
        string UserId,
        int PostId
    );

    public record CommentDetailsDto(
        int Id,
        string Content,
        DateTime DateAdded,
        string UserId,
        int PostId
    ) : CommentDto(Id, Content, DateAdded, UserId, PostId);

    public record CommentCreateDto
    {
        public string Content { get; init; }
        public string UserId { get; init; }
        public int PostId { get; init; }
    }

    public record CommentUpdateDto : CommentCreateDto
    {
        public int Id { get; init; }
    }
}
