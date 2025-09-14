using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

using fabrizio.DAL.Entities;


namespace fabrizio.DAL
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


		public DbSet<Entities.Account> Accounts { get; set; }
		public DbSet<Entities.Trip> Trips { get; set; }
		public DbSet<Entities.TravelBooking> TravelBookings { get; set; }
		public DbSet<Entities.AccommodationBooking> AccommodationBookings { get; set; }




		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>().OwnsOne(e => e.Audit);
			modelBuilder.Entity<Trip>().OwnsOne(e => e.Audit);
			modelBuilder.Entity<AccommodationBooking>().OwnsOne(e => e.Audit);
			modelBuilder.Entity<TravelBooking>().OwnsOne(e => e.Audit);

			// Apply all IEntityTypeConfiguration<T> in this assembly
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			base.OnModelCreating(modelBuilder);
		}


		public override int SaveChanges()
		{
			ApplyEntityRules();
			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			ApplyEntityRules();
			return await base.SaveChangesAsync(cancellationToken);
		}

		#region Update specific entity rules

		private void ApplyEntityRules()
		{
			// Find all entities that are being Added or Modified, now using only the base classes
			var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntityGuid || e.Entity is BaseEntityInt);

			foreach (var entry in entries)
			{
				var entity = entry.Entity;

				if (entity is BaseEntityGuid guidEntity)
				{
					UpdateAudit(guidEntity, entry.State);
				}
				else if (entity is BaseEntityInt intEntity)
				{
					UpdateAudit(intEntity, entry.State);
				}
			}
		}

		private void UpdateAudit(dynamic entity, EntityState state)
		{
			if (state == EntityState.Added)
			{
				entity.Audit.AddTime = DateTime.UtcNow;
				entity.Audit.EditTime = DateTime.UtcNow;
			}
			else if (state == EntityState.Modified)
			{
				entity.Audit.EditTime = DateTime.UtcNow;
			}
		}



		#endregion Update specific entity rules

	}



	public class AccountConfiguration : IEntityTypeConfiguration<Entities.Account>
	{
		public void Configure(EntityTypeBuilder<Entities.Account> builder)
		{
			builder.ToTable("Accounts");

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
			  .ValueGeneratedOnAdd().UseIdentityColumn(100000, 1);

			builder.Property(a => a.Email)
				   .IsRequired()
				   .HasMaxLength(255);

			builder.HasIndex(a => a.Email)
				   .IsUnique();

			builder.Property(a => a.PasswordHash)
				   .IsRequired()
				   .HasMaxLength(512);

			builder.Property(a => a.Name)
				   .HasMaxLength(100);

			builder.Property(a => a.Status)
				   .IsRequired();
		}
	}

	public class TripConfiguration : IEntityTypeConfiguration<Entities.Trip>
	{
		public void Configure(EntityTypeBuilder<Entities.Trip> builder)
		{
			builder.ToTable("Trips");

			builder.HasKey(t => t.Id);

			builder.Property(a => a.Name)
				   .HasMaxLength(100);

			builder.Property(t => t.Destination)
				   .HasMaxLength(200);
		}
	}


	public class TravelBookingConfiguration : IEntityTypeConfiguration<Entities.TravelBooking>
	{
		public void Configure(EntityTypeBuilder<Entities.TravelBooking> builder)
		{
			builder.ToTable("TravelBookings");

			builder.HasKey(a => a.Id);

			builder.Property(t => t.Reference)
				   .HasMaxLength(50);

			builder.Property(t => t.Origin)
				   .HasMaxLength(200);

			builder.Property(t => t.Destination)
				   .HasMaxLength(200);

			builder.Property(t => t.Carrier)
				   .HasMaxLength(100);

			builder.Property(t => t.Note)
				.HasMaxLength(1000);

			builder.Property(a => a.Type)
				   .IsRequired();
		}
	}

	public class AccommodationBookingConfiguration : IEntityTypeConfiguration<Entities.AccommodationBooking>
	{
		public void Configure(EntityTypeBuilder<Entities.AccommodationBooking> builder)
		{
			builder.ToTable("AccommodationBookings");

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Name)
				   .HasMaxLength(100);

			builder.Property(t => t.Reference)
				   .HasMaxLength(50);

			builder.Property(t => t.Note)
				.HasMaxLength(1000);

			builder.Property(a => a.Type)
				   .IsRequired();

		}
	}


}







