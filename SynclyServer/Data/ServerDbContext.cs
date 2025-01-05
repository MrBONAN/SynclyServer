using Microsoft.EntityFrameworkCore;
using SynclyServer.Models;

namespace SynclyServer.Data;

public class ServerDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Links> Links { get; set; }
    public DbSet<CurrentTrack> CurrentTracks { get; set; }
    public DbSet<RecentlyTrack> RecentlyTracks { get; set; }
    public DbSet<UserLocation> UserLocations { get; set; }
}