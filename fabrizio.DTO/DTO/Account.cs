using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class LoginDto
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
	}

	public class LoginResponseDto
	{
		public string Token { get; set; } = string.Empty;
	}

	public class AccountDto
	{
		public int Id { get; set; }


		// from Account
		public string Email { get; set; } = string.Empty;
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }


		// from AccountInfo
		public string PreferredLanguage { get; set; } = string.Empty;
		public string PreferredCurrency { get; set; } = string.Empty;
		public string TimeZone { get; set; } = string.Empty;
		public bool IsDarkMode { get; set; }
		public Guid? HomeLocationId { get; set; }

	}

	public class CreateAccountRequest
	{
		public required string Email { get; init; }
		public required string Password { get; init; }
		public string Name { get; set; } = string.Empty;
	}

	public class UpdateAccountProfileRequest
	{
		public string? Name { get; init; }
		public string? PreferredLanguage { get; init; }
		public string? PreferredCurrency { get; init; }
		public string? TimeZone { get; init; }

	}


}
