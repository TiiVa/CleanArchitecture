using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CleanArchitecture.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
		 

	}

	public DbSet<EventRegistrationForm> UserMeetingRegistrationForms { get; set; }
	public DbSet<Story> Stories { get; set; }
	public DbSet<Resource> Resources { get; set; }
	public DbSet<Document> Documents { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<EventResource> EventResources { get; set; }
	public DbSet<ExcelFile> ExcelFiles { get; set; }

	public DbSet<Invitation> Invitations { get; set; }
	public DbSet<InvitationSection> InvitationSections { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Document>()
			.HasOne<Resource>(d => d.Resource)
			.WithMany(r => r.Documents)
			.HasForeignKey(d => d.ResourceId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder.Entity<Event>()
			.HasOne<ApplicationUser>(e => e.ApplicationUser)
			.WithMany(ap => ap.Events)
			.HasForeignKey(e => e.ApplicationUserId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder.Entity<Story>()
			.HasOne<Event>(e => e.Event)
			.WithMany(ap => ap.Stories)
			.HasForeignKey(e => e.EventId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder.Entity<EventResource>().HasKey(er => new { er.EventId, er.ResourceId });

		modelBuilder.Entity<Invitation>()
			.HasMany(i => i.Sections)
			.WithOne()
			.HasForeignKey(s => s.InvitationId)
			.OnDelete(DeleteBehavior.Cascade);




		base.OnModelCreating(modelBuilder);
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
	}

}