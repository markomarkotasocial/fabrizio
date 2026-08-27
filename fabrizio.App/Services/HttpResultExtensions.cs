using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using fabrizio.Shared.Contracts;

namespace fabrizio.App.Services
{
	/// <summary>
	/// Turns HttpClient calls against the API into <see cref="Result"/> / <see cref="Result{T}"/>.
	/// Success (2xx) yields the deserialized body (or an empty <see cref="Result"/>);
	/// a non-2xx response is read as <c>ProblemDetails</c> and surfaced as a <see cref="BusinessError"/>;
	/// any transport exception becomes a <c>network_error</c>.
	/// </summary>
	internal static class HttpResultExtensions
	{
		private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

		private static BusinessError NetworkError() => new("network_error", "Unable to reach server.", 0);

		public static async Task<Result<T>> GetResultAsync<T>(this HttpClient http, string url)
		{
			try { return await ReadResultAsync<T>(await http.GetAsync(url)); }
			catch (Exception) { return Result<T>.Fail(NetworkError()); }
		}

		public static async Task<Result<T>> PostResultAsync<T>(this HttpClient http, string url, object body)
		{
			try { return await ReadResultAsync<T>(await http.PostAsync(url, AsJson(body))); }
			catch (Exception) { return Result<T>.Fail(NetworkError()); }
		}

		public static async Task<Result<T>> PutResultAsync<T>(this HttpClient http, string url, object body)
		{
			try { return await ReadResultAsync<T>(await http.PutAsync(url, AsJson(body))); }
			catch (Exception) { return Result<T>.Fail(NetworkError()); }
		}

		public static async Task<Result> PostResultAsync(this HttpClient http, string url, object body)
		{
			try { return await ReadResultAsync(await http.PostAsync(url, AsJson(body))); }
			catch (Exception) { return Result.Fail(NetworkError()); }
		}

		public static async Task<Result> PutResultAsync(this HttpClient http, string url, object body)
		{
			try { return await ReadResultAsync(await http.PutAsync(url, AsJson(body))); }
			catch (Exception) { return Result.Fail(NetworkError()); }
		}

		public static async Task<Result> DeleteResultAsync(this HttpClient http, string url)
		{
			try { return await ReadResultAsync(await http.DeleteAsync(url)); }
			catch (Exception) { return Result.Fail(NetworkError()); }
		}

		// Pre-serialize into a StringContent so the request carries a Content-Length.
		// JsonContent streams without a length (chunked transfer-encoding), which the
		// Android HTTP stack can drop, leaving the API with an empty request body.
		private static HttpContent AsJson(object body)
			=> new StringContent(JsonSerializer.Serialize(body, body.GetType(), JsonOptions), Encoding.UTF8, "application/json");

		private static async Task<Result<T>> ReadResultAsync<T>(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
				return Result<T>.Fail(await ReadErrorAsync(response));

			var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
			return value is null
				? Result<T>.Fail(new BusinessError("parse_error", "Unable to parse response.", 0))
				: Result<T>.Success(value);
		}

		private static async Task<Result> ReadResultAsync(HttpResponseMessage response)
			=> response.IsSuccessStatusCode
				? Result.Success()
				: Result.Fail(await ReadErrorAsync(response));

		private static async Task<BusinessError> ReadErrorAsync(HttpResponseMessage response)
		{
			try
			{
				var problem = await response.Content.ReadFromJsonAsync<ApiProblem>(JsonOptions);
				if (problem is not null)
					return new BusinessError(
						problem.Type ?? "api_error",
						problem.Detail ?? problem.Title ?? "Unknown API error",
						problem.Status ?? (int)response.StatusCode);
			}
			catch (Exception)
			{
				// response body was not problem+json; fall through to a generic error
			}

			return new BusinessError("api_error", $"Request failed ({(int)response.StatusCode}).", (int)response.StatusCode);
		}
	}
}
