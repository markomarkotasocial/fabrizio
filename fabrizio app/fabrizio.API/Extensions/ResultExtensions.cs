using Microsoft.AspNetCore.Mvc;
using fabrizio.BLL;

namespace fabrizio.API.Extensions
{
	public static class ResultExtensions
	{
		public static IActionResult ToProblem(this Result result)
		{
			var error = result.Error;
			if (error == null) throw new InvalidOperationException("Cannot create ProblemDetails from a successful result.");

			var problem = new ProblemDetails
			{
				Type = error.Code,
				Title = "Business error",
				Detail = error.Message,
				Status = error.HttpStatusCode
			};

			return new ObjectResult(problem)
			{
				StatusCode = error.HttpStatusCode
			};
		}
	}
}
