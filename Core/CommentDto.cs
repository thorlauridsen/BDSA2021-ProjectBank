namespace ProjectBank.Core
{
    public record CommentDetailsDto
    {
        public int Id { get; init; }
        public string Content { get; init; }
        public DateTime DateAdded { get; init; }
        public string UserOid { get; init; }
    }

    public record CommentCreateDto
    {
        public string Content { get; init; }
        public string UserOid { get; init; }

        public int postid { get; init; }
    }
}
