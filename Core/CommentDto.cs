namespace ProjectBank.Core
{
    public record CommentDto(
        int Id,
        string Content,
        DateTime DateAdded,
        string UserId
    );

    public record CommentDetailsDto(
        int Id,
        string Content,
        DateTime DateAdded,
        string UserId
    ) : CommentDto(Id, Content, DateAdded, UserId);

    public record CommentCreateDto
    {
        public string Content { get; init; }
        public string UserId { get; init; }
        
        public int postid { get; init; }
    }

    public record CommentUpdateDto : CommentCreateDto
    {
        public int Id { get; init; }
    }
}
