
using fabrizio.DAL;
using fabrizio.DAL.Entities;
using fabrizio.Repository;
using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace fabrizio.BLL
{
	public interface IAccountService
	{
		Task<Account?> ValidateCredentials(string email, string password);
		Task<Result<PagedResult<AccountDto>>> GetAll(int skip = 0, int take = 100, string? name = null, string? email = null);
		Task<Result<AccountDto>> GetAccountInfoById(int id);
		Task<Account> Create(CreateAccountRequest dto);
		Task<Result> Update(int id, UpdateAccountProfileRequest dto);
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

		public async Task<Result<AccountDto>> GetAccountInfoById(int id)
		{
			#region Validate

			if (id < 0) throw new ArgumentException("Id must be non-negative.", nameof(id));
			Account? account = await _repository.GetByIdWithInfo(id);
			if (account == null)
			{
				return Result<AccountDto>.Fail(new BusinessError("account_not_found", "There is no account with specified ID.", 404));
			}

			#endregion Validate

			return Result<AccountDto>.Success(new AccountDto
			{
				Id = account.Id,
				Name = account.Name,
				Email = account.Email,
				Status = (int)account.Status,
				CreatedAt = account.Audit.AddTime,

				PreferredLanguage = account.AccountInfo?.PreferredLanguage,
				PreferredCurrency = account.AccountInfo?.PreferredCurrency,
				TimeZone = account.AccountInfo?.TimeZone,
				IsDarkMode = account.AccountInfo?.IsDarkMode ?? false,
				HomeLocationId = account.AccountInfo?.HomeLocationId
			});
		}

		public async Task<Result<PagedResult<AccountDto>>> GetAll(int skip = 0, int take = 100, string? name = null, string? email = null)
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
			var dtoItems = items.Select(account => new AccountDto
			{
				Id = account.Id,
				Name = account.Name,
				Email = account.Email,
			    Status = (int)account.Status,
				CreatedAt = account.Audit.AddTime
			});

			return Result<PagedResult<AccountDto>>.Success(new PagedResult<AccountDto>
			{
				TotalCount = totalCount,
				Items = dtoItems
			});
		}

		public async Task<Account> Create(CreateAccountRequest dto)
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

		public async Task<Result> Update(int id, UpdateAccountProfileRequest dto)
		{
			#region Validate

			if (id <= 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(id));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result.Fail(new BusinessError("account_name_required", "Name must be provided.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.PreferredLanguage))
			{
				return Result.Fail(new BusinessError("account_preferredlanguage_required", "Preferred language must be provided.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.PreferredCurrency))
			{
				return Result.Fail(new BusinessError("account_preferredcurrency_required", "Preferred currency must be provided.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.TimeZone))
			{
				return Result.Fail(new BusinessError("account_timezone_required", "Time zone must be provided.", 400));
			}

			var account = await _repository.GetByIdWithInfo(id);
			if (account == null)
			{
				return Result.Fail(new BusinessError("account_not_found", "There is no account with specified ID.", 404));
			}

			#endregion Validate

			account.Name = dto.Name;

			// defensive ensure that AccountInfo exists before trying to update it, even AccountInfo should always be created together with Account
			account.AccountInfo ??= new AccountInfo	{ AccountId = account.Id };
			account.AccountInfo.PreferredCurrency = dto.PreferredCurrency;
			account.AccountInfo.PreferredLanguage = dto.PreferredLanguage;
			account.AccountInfo.TimeZone = dto.TimeZone;

			await _repository.SaveChangesAsync();
			return Result.Success();
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
