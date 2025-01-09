using Microsoft.EntityFrameworkCore;
using SynclyServer.Models;
using SynclyServer.Models.Authorization;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Data;

public class ServerDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Links> Links { get; set; }
    public DbSet<CurrentTrack> CurrentTracks { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Registration> Registrations { get; set; }


    //Join tables
    public DbSet<TrackArtist> TrackArtists { get; set; }
    public DbSet<TopTracks> TopTracks { get; set; }
    public DbSet<TopArtists> TopArtists { get; set; }
    public DbSet<RecentlyTracks> RecentlyTracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTrackArtistTable(modelBuilder);
        ConfigureTopTracksTable(modelBuilder);
        ConfigureTopArtistsTable(modelBuilder);
        ConfigureRecentlyTracksTable(modelBuilder);
        ConfigureUserRegistrationRelationship(modelBuilder);
        ConfigureUserLocationRelationship(modelBuilder);
    }

    private void ConfigureTrackArtistTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackArtist>()
            .HasKey(ta => ta.Id);
        
        modelBuilder.Entity<TrackArtist>()
            .HasIndex(ta => new { ta.TrackId, ta.ArtistId })
            .IsUnique();

        modelBuilder.Entity<TrackArtist>()
            .HasOne(ta => ta.Track)
            .WithMany(t => t.TrackArtists)
            .HasForeignKey(ta => ta.TrackId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TrackArtist>()
            .HasOne(ta => ta.Artist)
            .WithMany()
            .HasForeignKey(ta => ta.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureTopTracksTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TopTracks>()
            .HasKey(tt => tt.Id);

        modelBuilder.Entity<TopTracks>()
            .HasIndex(tt => new { tt.UserId, tt.TrackId })
            .IsUnique();

        modelBuilder.Entity<TopTracks>()
            .HasOne(tt => tt.User)
            .WithMany(u => u.TopTracks)
            .HasForeignKey(tt => tt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TopTracks>()
            .HasOne(tt => tt.Track)
            .WithMany()
            .HasForeignKey(tt => tt.TrackId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureTopArtistsTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TopArtists>()
            .HasKey(ta => ta.Id);

        modelBuilder.Entity<TopArtists>()
            .HasIndex(ta => new { ta.UserId, ta.ArtistId })
            .IsUnique();

        modelBuilder.Entity<TopArtists>()
            .HasOne(ta => ta.User)
            .WithMany(u => u.TopArtists)
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TopArtists>()
            .HasOne(ta => ta.Artist)
            .WithMany()
            .HasForeignKey(ta => ta.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureRecentlyTracksTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecentlyTracks>()
            .HasKey(rt => rt.Id);

        modelBuilder.Entity<RecentlyTracks>()
            .HasIndex(rt => new { rt.UserId, rt.TrackId })
            .IsUnique();

        modelBuilder.Entity<RecentlyTracks>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RecentlyTracks)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecentlyTracks>()
            .HasOne(rt => rt.Track)
            .WithMany()
            .HasForeignKey(rt => rt.TrackId)
            .OnDelete(DeleteBehavior.Restrict);

    }
    private void ConfigureUserRegistrationRelationship(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithOne(u => u.Registration)
            .HasForeignKey<User>(u => u.RegistrationId);
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Registration)
            .WithOne(r => r.User)
            .HasForeignKey<Registration>(r => r.UserId);
    }
    
    private void ConfigureUserLocationRelationship(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>()
            .HasOne(l => l.User)
            .WithOne(u => u.Location)
            .HasForeignKey<User>(u => u.LocationId);
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Location)
            .WithOne(l => l.User)
            .HasForeignKey<Location>(l => l.UserId);
    }
}