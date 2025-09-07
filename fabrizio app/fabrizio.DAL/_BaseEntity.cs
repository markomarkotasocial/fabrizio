using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace fabrizio.DAL.Entities
{

	// For GUID-based entities
	public class BaseEntityGuidConfiguration<T> : IEntityTypeConfiguration<T>
		where T : BaseEntityGuid
	{
		public void Configure(EntityTypeBuilder<T> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id)
				   .ValueGeneratedNever(); // EF should not generate, we assign

			builder.OwnsOne(e => e.Audit, audit =>
			{
				audit.Property(a => a.AddTime).IsRequired();
				audit.Property(a => a.EditTime).IsRequired();
			});
		}
	}

	// For INT-based entities
	public class BaseEntityIntConfiguration<T> : IEntityTypeConfiguration<T>
		where T : BaseEntityInt
	{
		public void Configure(EntityTypeBuilder<T> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id)
				   .ValueGeneratedOnAdd(); // identity/auto increment

			builder.OwnsOne(e => e.Audit, audit =>
			{
				audit.Property(a => a.AddTime).IsRequired();
				audit.Property(a => a.EditTime).IsRequired();
			});
		}
	}

}
