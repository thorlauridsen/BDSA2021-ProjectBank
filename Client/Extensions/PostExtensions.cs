using ProjectBank.Core;

namespace ProjectBank.Client.Extensions;

public static class PostExtensions
{
    public static IEnumerable<string> GetTagsFromPosts(this List<PostDto> posts)
    {
        return posts.SelectMany(p => p.Tags);
    }
}