using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace ProjectBank.Client.Extensions;

public static class URLExtensions
{
    /*CREDIT: https://jasonwatmore.com/post/2020/08/09/blazor-webassembly-get-query-string-parameters-with-navigation-manager*/
    // get entire querystring name/value collection
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    // get single querystring value with specified key
    public static string? QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key];
    }
}