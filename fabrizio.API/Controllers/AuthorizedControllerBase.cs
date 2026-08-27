using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace fabrizio.API.Controllers
{
	/// <summary>
	/// Base controller for endpoints that operate on the authenticated account.
	/// Centralizes reading the account id carried in the "accountId" JWT claim.
	/// </summary>
	public abstract class AuthorizedControllerBase : ControllerBase
	{
		/// <summary>
		/// Reads the authenticated account id from the "accountId" claim.
		/// Returns <c>false</c> when the claim is missing or is not a positive integer.
		/// </summary>
		protected bool TryGetAccountId(out int accountId)
		{
			accountId = 0;
			var claim = User.FindFirstValue("accountId");
			return int.TryParse(claim, out accountId) && accountId > 0;
		}
	}
}
