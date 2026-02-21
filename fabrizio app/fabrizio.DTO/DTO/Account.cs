using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class GoogleLoginDto
	{
		public required string IdToken { get; set; }
	}

	public class LoginDto
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
	}

	public class ActivateAccount
	{
		public string Token { get; set; } = string.Empty;
	}

	public class GETAccount
	{
		public int Id { get; set; }


		// from Account
		public string Email { get; set; } = string.Empty;
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }


		// from AccountInfo
		public string PreferredLanguage { get; init; }
		public string PreferredCurrency { get; init; }
		public string TimeZone { get; init; }
		public bool IsDarkMode { get; set; }
		public Guid? HomeLocationId { get; set; }

	}

	public class POSTAccount
	{
		public required string Email { get; init; }
		public required string Password { get; init; }
		public string Name { get; set; } = string.Empty;
	}

	public class UpdateAccountProfileRequest
	{
		public required string Name { get; init; }
		public required string PreferredLanguage { get; init; }
		public required string PreferredCurrency { get; init; }
		public required string TimeZone { get; init; }

	}


}
