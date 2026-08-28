using fabrizio.App;
using fabrizio.App.Services;
using Microsoft.Maui.Storage;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

public class TokenHandler : DelegatingHandler
{
	private readonly AuthService _authService;

	public TokenHandler(AuthService authService)
	{
		_authService = authService;
	}


	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var token = await SecureStorage.GetAsync("jwt_token");

		if (!string.IsNullOrEmpty(token))
		{
			request.Headers.Authorization =	new AuthenticationHeaderValue("Bearer", token);
		}

		var response = await base.SendAsync(request, cancellationToken);

		// Single place that reacts to an expired/invalid session: sign the user out.
		// Callers just see the 401 turn into a failed Result.
		if (response.StatusCode == HttpStatusCode.Unauthorized)
			await _authService.LogoutAsync();

		return response;
	}

}
