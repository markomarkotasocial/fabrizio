using fabrizio.DAL;

namespace fabrizio.Repository
{
	/// <summary>
	/// Persistence operations shared by every aggregate repository.
	/// Aggregate-specific queries live on the concrete <c>I*Repository</c> interfaces.
	/// </summary>
	public interface IRepository<TEntity> where TEntity : class
	{
		void Add(TEntity entity);
		void Delete(TEntity entity);
		Task SaveChangesAsync();
	}

	public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
	{
		protected readonly AppDbContext Context;

		protected RepositoryBase(AppDbContext context)
		{
			Context = context;
		}

		public void Add(TEntity entity) => Context.Set<TEntity>().Add(entity);

		public void Delete(TEntity entity) => Context.Set<TEntity>().Remove(entity);

		public Task SaveChangesAsync() => Context.SaveChangesAsync();
	}
}
