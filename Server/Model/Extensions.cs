using Microsoft.AspNetCore.Mvc;
using ProjectBank.Core;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Model
{
    public static class Extensions
    {
        public static T GetResultContent<T>(this ActionResult<T> result)
            => (T)(result.Result as CreatedAtRouteResult).Value;

        public static IActionResult ToActionResult(this Status status) => status switch
        {
            Updated => new NoContentResult(),
            Deleted => new NoContentResult(),
            NotFound => new NotFoundResult(),
            Conflict => new ConflictResult(),
            _ => throw new NotSupportedException($"{status} not supported")
        };

        public static ActionResult<T> ToActionResult<T>(this Option<T> option) where T : class
            => option.IsSome ? option.Value : new NotFoundResult();
    }
}
