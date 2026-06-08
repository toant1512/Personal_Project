using MediaArchive.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaArchive.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<MediaItem> MediaItems => Set<MediaItem>();

    public DbSet<DownloadJob> DownloadJobs => Set<DownloadJob>();
}