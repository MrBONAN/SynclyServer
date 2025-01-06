using Microsoft.EntityFrameworkCore;
using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Data;

public class ServerDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Links> Links { get; set; }
    public DbSet<CurrentTrack> CurrentTracks { get; set; }
    public DbSet<RecentlyTrack> RecentlyTracks { get; set; }
    public DbSet<Location> Locations { get; set; }
    
    //Join tables
    public DbSet<TrackArtist> TrackArtists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTrackArtistTable(modelBuilder);
        //ConfigureLinks(modelBuilder);
    }

    private void ConfigureTrackArtistTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackArtist>()
            .HasKey(ta => new { ta.TrackId, ta.ArtistId });

        modelBuilder.Entity<TrackArtist>()
            .HasOne(ta => ta.Track)
            .WithMany(t => t.TrackArtists)
            .HasForeignKey(ta => ta.TrackId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TrackArtist>()
            .HasOne(ta => ta.Artist)
            .WithMany()
            .HasForeignKey(ta => ta.ArtistId);
    }

    private void ConfigureLinks(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<>()
    }
}