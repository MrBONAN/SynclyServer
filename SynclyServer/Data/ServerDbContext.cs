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
            .HasForeignKey(ta => ta.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureTopTracksTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TopTracks>()
            .HasKey(ut => new { ut.UserId, ut.TrackId });
        
        modelBuilder.Entity<TopTracks>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.TopTracks)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder.Entity<TopTracks>()
        //     .HasOne(ut => ut.Track)
        //     .WithMany()
        //     .HasForeignKey(ut => ut.TrackId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureTopArtistsTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TopArtists>()
            .HasKey(ua => new { ua.UserId, ua.ArtistId});
        
        modelBuilder.Entity<TopArtists>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.TopArtists)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder.Entity<TopArtists>()
        //     .HasOne(ut => ut.Artist)
        //     .WithMany()
        //     .HasForeignKey(ut => ut.ArtistId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureRecentlyTracksTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecentlyTracks>()
            .HasKey(ut => new { ut.UserId, ut.TrackId});
        
        modelBuilder.Entity<RecentlyTracks>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.RecentlyTracks)
            .HasForeignKey(ut => ut.UserId)
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
}