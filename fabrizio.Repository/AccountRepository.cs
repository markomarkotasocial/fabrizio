using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface IAccountRepository : IRepository<Account>
	{
		IQueryable<Account> QueryAll();
		Task<Account?> GetById(int id);
		Task<Account?> GetByIdWithInfo(int id);
		Task<Account?> GetActiveByEmailAsync(string email);
		Task<bool> AnyAsync(string email);
	}


	public class AccountRepository : RepositoryBase<Account>, IAccountRepository
	{
		public AccountRepository(AppDbContext context) : base(context) { }

		public IQueryable<Account> QueryAll()
		{
			return Context.Accounts.AsNoTracking();
		}

		public async Task<Account?> GetById(int id)
		{
			return await Context.Accounts.FindAsync(id);
		}

		public async Task<Account?> GetByIdWithInfo(int id)
		{
			return await Context.Accounts.Include(a => a.AccountInfo).SingleOrDefaultAsync(a => a.Id == id);
		}

		public async Task<Account?> GetActiveByEmailAsync(string email)
		{
			return await Context.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.Status == AccountStatuses.Active);
		}

		public async Task<bool> AnyAsync(string email)
		{
			return await Context.Accounts.AnyAsync(a => a.Email == email);
		}
	}
}
