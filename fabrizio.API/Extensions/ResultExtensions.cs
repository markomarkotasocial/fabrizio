using Microsoft.AspNetCore.Mvc;
using fabrizio.Shared.Contracts;

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

		/// <summary>
		/// Maps a non-generic <see cref="Result"/> to <c>204 No Content</c> on success,
		/// or a <see cref="ProblemDetails"/> response on failure.
		/// </summary>
		public static IActionResult ToActionResult(this Result result)
			=> result.IsSuccess ? new NoContentResult() : result.ToProblem();

		/// <summary>
		/// Maps a <see cref="Result{T}"/> to <c>200 OK</c> with the value on success,
		/// or a <see cref="ProblemDetails"/> response on failure.
		/// </summary>
		public static IActionResult ToActionResult<T>(this Result<T> result)
			=> result.IsSuccess ? new OkObjectResult(result.Value) : result.ToProblem();
	}
}
