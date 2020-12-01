using ClubScansub.App_Data.ScansubModels;
using ClubScansub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ClubScansub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Diveorganization> Diveorganizations { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<UserCertificat> UserCertificates { get; set; }
        public DbSet<Divelocation> Divelocations { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<ApplicationUserAccountEntry> ApplicationUserAccountEntry { get; set; }
        public DbSet<CourseTemplate> CourseTemplates { get; set; }
        public DbSet<CourseSessionTemplate> CourseSessionTemplates { get; set; }
        public DbSet<DiveorgCertificate> DiveorgCertificates { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSession> CourseSessions { get; set; }
        public DbSet<CourseSessionInstructor> CourseSessionInstructors { get; set; }
        public DbSet<ClubSettings> ClubSettings { get; set; }
        public DbSet<CourseSignup> CourseSignups { get; set; }
        //public DbSet<Site> Divesites { get; set; }
        //public DbSet<medlemsdata> medlemsdata { get; private set; }
        //public DbSet<kursistdata> kursistdata { get; private set; }
        //public DbSet<saldooplysning> saldooplysning { get; private set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EventUser>().HasKey(ue => new { ue.ApplicationUserId, ue.EventId });

            builder.Entity<EventUser>()
                .HasOne(u => u.Event)
                .WithMany(c => c.Participants)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.EventId)
                ;

            builder.Entity<EventUser>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(c => c.Events)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(c => c.ApplicationUserId)
                ;

            builder.Entity<CourseSignup>().HasKey(ue => new { ue.ApplicationUserId, ue.CourseId });

            builder.Entity<CourseSignup>()
                .HasOne(u => u.Course)
                .WithMany(c => c.Signups)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.CourseId)
                ;

            builder.Entity<CourseSignup>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(c => c.Courses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(c => c.ApplicationUserId)
                ;

            builder.Entity<ApplicationUserAccountEntry>()
                .HasOne(c => c.Event)
                .WithMany(c => c.AccountTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<DiveorgCertificate>()
                .HasKey(dc => new { dc.DiveorganizationId, dc.CertificateId });

            builder.Entity<DiveorgCertificate>()
                .HasOne(c => c.Diveorg)
                .WithMany(c => c.Certificates)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(c => c.DiveorganizationId)
                ;

            builder.Entity<DiveorgCertificate>()
                .HasOne(c => c.Certificate)
                .WithMany(c => c.Diveorgs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(c => c.CertificateId)
                ;

            builder.Entity<UserCertificat>()
                .HasOne(c => c.Certificate)
                .WithMany(c => c.UserCertificate)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserCertificat>()
                .HasOne(c => c.User)
                .WithMany(c => c.Certificates)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Course>()
                .HasMany(x => x.CourseSessions)
                .WithOne(x => x.Course)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<CourseSessionInstructor>()
                .HasKey(ue => new { ue.CourseSessionId, ue.InstructorId });

            builder.Entity<CourseSessionInstructor>()
                .HasOne(x => x.CourseSession)
                .WithMany(x => x.SessionInstructors)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseSessionInstructor>()
                .HasOne(x => x.Instructor)
                .WithMany(x => x.InstructorSessions)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Event>()
                .HasOne(x => x.Divelocation)
                .WithMany(x => x.Events)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
