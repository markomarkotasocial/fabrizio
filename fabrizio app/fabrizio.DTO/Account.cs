namespace fabrizio.DTO
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
		public required string Email { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }

	}

	public class POSTAccount
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
		public string Name { get; set; } = string.Empty;
	}

	public class PUTAccount
	{
		public string Name { get; set; } = string.Empty;

	}


}
