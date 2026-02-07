
using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;
using fabrizio.Shared.DTO;
using fabrizio.Repository;

namespace fabrizio.BLL
{
	public interface IAccountService
	{
		Task<Account?> ValidateCredentials(string email, string password);
		Task<PagedResult<GETAccount>> GetAll(int skip = 0, int take = 100, string? name = null, string? email = null);
		Task<GETAccount> GetById(int id);
		Task<Account> Create(POSTAccount dto);
		Task Update(int id, PUTAccount dto);
		Task Activate(string token);
		Task Delete(int id);
	}

	public class AccountService : IAccountService
	{
		private readonly IAccountRepository _repository;
		private readonly AppDbContext _context;

		public AccountService(IAccountRepository repository, AppDbContext context)
		{
			_repository = repository;
			_context = context;
		}



		public async Task<Account?> ValidateCredentials(string email, string password)
		{
			#region Validate

			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("E-mail must be provided.", nameof(email));

			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException("Password must be provided.", nameof(password));

			#endregion Validate

			var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.Status == AccountStatuses.Active);
			if (account == null) return null;
			return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash) ? account : null;
		}

		public async Task<GETAccount> GetById(int id)
		{
			#region Validate

			if (id < 0) throw new ArgumentException("Id must be non-negative.", nameof(id));

			Account? account = await _repository.GetById(id);
			if (account == null) throw new KeyNotFoundException("There is no account with specified ID!");

			#endregion Validate

			return new GETAccount
			{
				Id = account.Id,
				Name = account.Name,
				Email = account.Email,
				Status = (int)account.Status,
				CreatedAt = account.Audit.AddTime
			};
		}

		public async Task<PagedResult<GETAccount>> GetAll(int skip = 0, int take = 100, string? name = null, string? email = null)
		{
			#region Validate

			if (take <= 0)
				throw new ArgumentException("Take must be greater than zero.", nameof(take));

			#endregion Validate

			var query = _repository.QueryAll().Where(x => x.Status != AccountStatuses.Deleted);

			#region Filters

			if (!string.IsNullOrWhiteSpace(name))
			{
				var trimmedName = name.Trim();
				if (trimmedName.Length > 0)
					query = query.Where(t => t.Name.Contains(trimmedName));
			}

			if (!string.IsNullOrWhiteSpace(email))
			{
				var trimmedEmail = email.Trim();
				if (trimmedEmail.Length > 0)
					query = query.Where(t => t.Email.Contains(trimmedEmail));
			}

			#endregion Filters

			// Paging
			var totalCount = await query.CountAsync();
			var items = await query.Skip(skip).Take(take).ToListAsync();

			// Map to DTOs
			var dtoItems = items.Select(account => new GETAccount
			{
				Id = account.Id,
				Name = account.Name,
				Email = account.Email,
			    Status = (int)account.Status,
				CreatedAt = account.Audit.AddTime
			});

			return new PagedResult<GETAccount>
			{
				TotalCount = totalCount,
				Items = dtoItems
			};
		}

		public async Task<Account> Create(POSTAccount dto)
		{
			#region Validate

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			if (string.IsNullOrWhiteSpace(dto.Password))
				throw new ArgumentException("Password must be provided.", nameof(dto.Password));

			if (string.IsNullOrWhiteSpace(dto.Email))
				throw new ArgumentException("E-mail must be provided.", nameof(dto.Email));

			if (await _repository.AnyAsync(dto.Email))
				throw new InvalidOperationException("Email is already in use.");

			#endregion Validate

			var account = new Account
			{
				Email = dto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
				Name = dto.Name,
			};

			_repository.Add(account);
			await _repository.SaveChangesAsync();
			return account;
		}

		public async Task Update(int id, PUTAccount dto)
		{
			#region Validate

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			if (id < 0)
				throw new ArgumentException("Id must be non-negative.", nameof(id));

			var account = await _repository.GetById(id);
			if (account == null)
				throw new KeyNotFoundException("There is no account with the specified ID.");

			#endregion Validate

			account.Name = dto.Name;
			await _repository.SaveChangesAsync();
		}

		public async Task Activate(string token)
		{
			throw new NotImplementedException();


			//if (string.IsNullOrWhiteSpace(token))
			//	throw new ArgumentException("Activation token is required.", nameof(token));

			//var account = await _repository.GetByActivationTokenAsync(token);
			//if (account == null || account.ActivationTokenExpiry < DateTime.UtcNow)
			//	throw new InvalidOperationException("Invalid or expired activation token.");

			//account.Status = AccountStatuses.Active;
			//account.ActivationToken = null; // clear it so it can't be reused
			//account.ActivationTokenExpiry = null;

			//await _repository.SaveChangesAsync();
		}

		public async Task Delete(int id)
		{
			#region Validate

			if (id < 0)
				throw new ArgumentException("Id must be non-negative.", nameof(id));

			var account = await _repository.GetById(id);
			if (account == null)
				throw new KeyNotFoundException("There is no account with the specified ID.");

			#endregion Validate

			account.Status = AccountStatuses.Deleted;
			await _repository.SaveChangesAsync();
		}
	}

}
