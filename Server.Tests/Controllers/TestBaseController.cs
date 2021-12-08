using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ProjectBank.Server.Tests.Controllers
{
    public class TestBaseController<S>
    {
        protected Mock<ILogger<S>> logger
            = new Mock<ILogger<S>>();

        protected static T? GetCreatedResultContent<T>(ActionResult<T> result)
            => (T?)(result.Result as CreatedAtRouteResult)?.Value;

        protected static T? GetOkResultContent<T>(ActionResult<T> result)
            => (T?)(result.Result as OkObjectResult)?.Value;
    }
}
