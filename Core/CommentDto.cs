namespace ProjectBank.Core
{
    public record CommentDto(
        int Id,
        string Content,
        DateTime DateAdded,
        string UserOid
    );

    public record CommentDetailsDto(
        int Id,
        string Content,
        DateTime DateAdded,
        string UserOid
    ) : CommentDto(Id, Content, DateAdded, UserOid);

    public record CommentCreateDto
    {
        public string Content { get; init; }
        public string UserOid { get; init; }

        public int postid { get; init; }
    }
}
