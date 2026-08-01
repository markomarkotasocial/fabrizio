using fabrizio.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fabrizio.API.Services
{
	public interface IJwtTokenService
	{
		string GenerateToken(Account account);
	}


	public class JwtTokenService : IJwtTokenService
	{
		private readonly IConfiguration _config;

		public JwtTokenService(IConfiguration config)
		{
			_config = config;
		}

		public string GenerateToken(Account account)
		{
			var claims = new[]
			{
				//new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
				new Claim("accountId", account.Id.ToString()),			
				new Claim(JwtRegisteredClaimNames.Email, account.Email),
				new Claim("name", account.Name),
				new Claim("status", account.Status.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(24),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
