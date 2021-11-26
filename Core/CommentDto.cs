using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record CommentDto(
        int Id,
        string Content,
        DateTime DateAdded,
        int UserId,
        int PostId
    );

    public record CommentDetailsDto(
        int Id,
        string Content,
        DateTime DateAdded,
        int UserId,
        int PostId
    ) : CommentDto(Id, Content, DateAdded, UserId, PostId);

    public record CommentCreateDto
    {
        public CommentCreateDto(
            string title,
            string content,
            DateTime dateAdded,
            int userId,
            int postId)
        {
            Title = title;
            Content = content;
            DateAdded = dateAdded;
            UserId = userId;
            PostId = postId;
        }

        [StringLength(50)]
        public string Title { get; init; }
        public string Content { get; init; }
        public DateTime DateAdded { get; init; }
        public int UserId { get; init; }
        public int PostId { get; init; }
    }

    public record CommentUpdateDto : CommentCreateDto
    {
        public CommentUpdateDto(
            string title,
            string content,
            DateTime dateAdded,
            int userId,
            int postId
        ) : base(title, content, dateAdded, userId, postId) { }

        public int Id { get; init; }
    }
}
