using Library.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Fine> Fines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(b => b.title)
                .IsRequired() // Ensures title is NOT NULL
                .HasMaxLength(256) // Ensures max length of 256 characters
                .HasColumnType("VARCHAR(255)"); // Explicitly sets VARCHAR instead of NVARCHAR

            modelBuilder.Entity<Book>()
                .Property(b => b.copiesAvailable)
                .HasDefaultValue(1); // Sets default value of 1

            modelBuilder.Entity<Book>()
                .Property(b => b.totalCopies)
                .HasDefaultValue(1); // Sets default value of 1

            modelBuilder.Entity<Loan>()
                .Property(l => l.loanDate)
                .HasColumnType("date") // Ensures SQL DATE type
                .HasDefaultValueSql("GETDATE()"); // Uses SQL Server's current date

            modelBuilder.Entity<Loan>()
                .Property(l => l.dueDate)
                .IsRequired(); // Ensures title is NOT NULL

            modelBuilder.Entity<Loan>()
                .Property(l => l.status)
                .HasDefaultValue(LoanStatus.borrowed) // Default at database level
                .HasConversion<string>(); // Store as string in database (optional)

            modelBuilder.Entity<Fine>()
                .Property(f => f.fineDate)
                .HasColumnType("date") // Ensures SQL DATE type
                .HasDefaultValueSql("GETDATE()"); // Uses SQL Server's current date

            modelBuilder.Entity<Fine>()
                .Property(f => f.amount)
                .HasColumnType("decimal(6,2)") // Maps to SQL DECIMAL(6,2)
                .IsRequired(); // Ensures NOT NULL

            modelBuilder.Entity<Fine>()
                .Property(f => f.paid)
                .HasColumnType("bit") // Ensures it maps to SQL Server BIT type
                .HasDefaultValue(false) // Default is FALSE (0)
                .IsRequired(); // Ensures NOT NULL

            modelBuilder.Entity<Fine>()
                .Property(f => f.fineDate)
                .HasDefaultValue("GETDATE()"); // Default is FALSE (0)

            // Relationships

            // Book:Loan 1:N
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book) // Loan has one Book
                .WithMany(b => b.Loans) // Book has many Loans
                .HasForeignKey(l => l.bookId) // Foreign key in Loan table
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // Loan:Fine 1:1
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Loan) // Fine has one Loan
                .WithOne(l => l.Fine) // Loan has one Fine
                .HasForeignKey<Fine>(f => f.loanId) // Fine has FK loanId
                .IsRequired() // Ensures a fine must be linked to a Loan
                .OnDelete(DeleteBehavior.Cascade); // If Loan is deleted, delete Fine too

            // Only call the base method once
            base.OnModelCreating(modelBuilder);
        }

    }
}
