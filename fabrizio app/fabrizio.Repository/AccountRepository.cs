using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface IAccountRepository
	{
		Task SaveChangesAsync();

		IQueryable<Account> QueryAll();
		Task<Account?> GetById(int id);
		Task<bool> AnyAsync(string email);
		void Add(Account account);
		void Delete(Account account);
	}


	public class AccountRepository : IAccountRepository
	{

		private readonly AppDbContext _context;

		public AccountRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}




		public IQueryable<Account> QueryAll()
		{
			return _context.Accounts.AsNoTracking();
		}

		public async Task<Account?> GetById(int id)
		{
			return await _context.Accounts.FindAsync(id);
		}

		public async Task<bool> AnyAsync(string email)
		{
			return await _context.Accounts.AnyAsync(a => a.Email == email);
		}

		public void Add(Account account)
		{
			_context.Accounts.Add(account);
		}

		public void Delete(Account account)
		{
			_context.Accounts.Remove(account);
		}


	}
}
