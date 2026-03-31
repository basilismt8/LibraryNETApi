using Library.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
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
                .HasDefaultValue(0); // Sets default value of 0

            modelBuilder.Entity<Book>()
                .Property(b => b.totalCopies)
                .HasDefaultValue(0); // Sets default value of 0

            // BookCopy configuration
            modelBuilder.Entity<BookCopy>()
                .Property(bc => bc.copyCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("VARCHAR(50)");

            modelBuilder.Entity<BookCopy>()
                .HasIndex(bc => bc.copyCode)
                .IsUnique(); // Unique index for copyCode

            modelBuilder.Entity<BookCopy>()
                .Property(bc => bc.status)
                .HasDefaultValue(CopyStatus.Available)
                .HasConversion<string>(); // Store CopyStatus as string in database

            modelBuilder.Entity<Loan>()
                .Property(l => l.loanDate)
                .HasColumnType("date") // Ensures SQL DATE type
                .HasDefaultValueSql("GETDATE()"); // Uses SQL Server's current date

            modelBuilder.Entity<Loan>()
                .Property(l => l.dueDate)
                .IsRequired(); // Ensures dueDate is NOT NULL

            modelBuilder.Entity<Loan>()
                .Property(l => l.status)
                .HasDefaultValue(LoanStatus.borrowed) // Default at database level
                .HasConversion<string>(); // Store as string in database

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

            // Relationships

            // Book:BookCopy 1:N (Cascade delete)
            modelBuilder.Entity<BookCopy>()
                .HasOne(bc => bc.Book) // BookCopy has one Book
                .WithMany(b => b.BookCopies) // Book has many BookCopies
                .HasForeignKey(bc => bc.bookId) // Foreign key in BookCopy table
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete from Book to BookCopies

            // BookCopy:Loan 1:N (Cascade delete)
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.BookCopy) // Loan has one BookCopy
                .WithMany(bc => bc.Loans) // BookCopy has many Loans
                .HasForeignKey(l => l.bookCopyId) // Foreign key in Loan table
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete from BookCopy to Loans

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
