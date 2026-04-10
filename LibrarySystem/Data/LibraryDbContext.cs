using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<LibraryMember> Members { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Librarian> Librarians { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryMember>()
            .HasDiscriminator<MemberType>("MemberType")
            .HasValue<StudentMember>(MemberType.Student)
            .HasValue<ProfessorMember>(MemberType.Professor)
            .HasValue<ExternalMember>(MemberType.ExternalUniversity)
            .HasValue<OtherMember>(MemberType.Other);

        modelBuilder.Entity<LibraryMember>()
            .Ignore(m => m.MaxBooks)
            .Ignore(m => m.LoanDays)
            .Ignore(m => m.MemberTypeDisplay);

        modelBuilder.Entity<Loan>()
            .Ignore(l => l.IsReturned)
            .Ignore(l => l.IsOverdue)
            .Ignore(l => l.OverdueDays);

        // Kapcsolatok
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Oszlop-korlatok
        modelBuilder.Entity<Book>(b =>
        {
            b.Property(x => x.Author).HasMaxLength(200).IsRequired();
            b.Property(x => x.Title).HasMaxLength(300).IsRequired();
            b.Property(x => x.Publisher).HasMaxLength(200).IsRequired();
            b.Property(x => x.Edition).HasMaxLength(50).IsRequired();
            b.Property(x => x.ISBN).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<LibraryMember>(m =>
        {
            m.Property(x => x.Name).HasMaxLength(200).IsRequired();
            m.Property(x => x.Address).HasMaxLength(300).IsRequired();
            m.Property(x => x.Contact).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Librarian>(l =>
        {
            l.Property(x => x.Username).HasMaxLength(100).IsRequired();
            l.Property(x => x.PasswordHash).IsRequired();
            l.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            l.HasIndex(x => x.Username).IsUnique();
        });

        // Seed adatok - Konyvtaros (jelszo: admin123 -> SHA-256 hex)
        modelBuilder.Entity<Librarian>().HasData(new Librarian
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9",
            FullName = "Konyvtaros"
        });

        // Seed adatok - Tagok
        modelBuilder.Entity<StudentMember>().HasData(
            new StudentMember
            {
                Id = 1,
                Name = "Kovacs Anna",
                Address = "Budapest, Fo u. 1.",
                Contact = "kovacs.anna@edu.hu"
            },
            new StudentMember
            {
                Id = 4,
                Name = "Szabo Petra",
                Address = "Szeged, Tisza Lajos krt. 12.",
                Contact = "szabo.petra@edu.hu"
            });

        modelBuilder.Entity<ProfessorMember>().HasData(
            new ProfessorMember
            {
                Id = 2,
                Name = "Dr. Nagy Peter",
                Address = "Debrecen, Kossuth u. 5.",
                Contact = "nagy.peter@univ.hu"
            });

        modelBuilder.Entity<ExternalMember>().HasData(
            new ExternalMember
            {
                Id = 3,
                Name = "Toth Gabor",
                Address = "Pecs, Szechenyi ter 8.",
                Contact = "toth.gabor@bme.hu"
            });

        modelBuilder.Entity<OtherMember>().HasData(
            new OtherMember
            {
                Id = 5,
                Name = "Kiss Zoltan",
                Address = "Gyor, Baross u. 20.",
                Contact = "kiss.zoltan@gmail.com"
            });

        // Seed adatok - Konyvek
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Author = "Knuth, Donald E.",
                Title = "The Art of Computer Programming",
                Publisher = "Addison-Wesley",
                Year = 1968,
                Edition = "3rd",
                ISBN = "978-0201038040",
                IsLoanable = true,
                CopyCount = 3
            },
            new Book
            {
                Id = 2,
                Author = "Martin, Robert C.",
                Title = "Clean Code",
                Publisher = "Prentice Hall",
                Year = 2008,
                Edition = "1st",
                ISBN = "978-0132350884",
                IsLoanable = true,
                CopyCount = 2
            },
            new Book
            {
                Id = 3,
                Author = "Gamma, Erich et al.",
                Title = "Design Patterns",
                Publisher = "Addison-Wesley",
                Year = 1994,
                Edition = "1st",
                ISBN = "978-0201633610",
                IsLoanable = true,
                CopyCount = 1
            },
            new Book
            {
                Id = 4,
                Author = "Cormen, Thomas H.",
                Title = "Introduction to Algorithms",
                Publisher = "MIT Press",
                Year = 2009,
                Edition = "3rd",
                ISBN = "978-0262033848",
                IsLoanable = true,
                CopyCount = 4
            },
            new Book
            {
                Id = 5,
                Author = "Tanenbaum, Andrew S.",
                Title = "Modern Operating Systems",
                Publisher = "Pearson",
                Year = 2014,
                Edition = "4th",
                ISBN = "978-0133591620",
                IsLoanable = true,
                CopyCount = 2
            },
            new Book
            {
                Id = 6,
                Author = "Silberschatz, Abraham",
                Title = "Database System Concepts",
                Publisher = "McGraw-Hill",
                Year = 2019,
                Edition = "7th",
                ISBN = "978-0078022159",
                IsLoanable = false,
                CopyCount = 1
            },
            new Book
            {
                Id = 7,
                Author = "Fowler, Martin",
                Title = "Refactoring",
                Publisher = "Addison-Wesley",
                Year = 2018,
                Edition = "2nd",
                ISBN = "978-0134757599",
                IsLoanable = true,
                CopyCount = 2
            }
        );

        // Seed adatok - Kolcsonzesek (demo adatok)
        modelBuilder.Entity<Loan>().HasData(
            new Loan
            {
                Id = 1,
                BookId = 1,
                MemberId = 1,
                LoanDate = new DateTime(2026, 3, 1),
                DueDate = new DateTime(2026, 4, 30),
                ReturnDate = null
            },
            new Loan
            {
                Id = 2,
                BookId = 2,
                MemberId = 2,
                LoanDate = new DateTime(2026, 2, 15),
                DueDate = new DateTime(2027, 2, 15),
                ReturnDate = null
            },
            new Loan
            {
                Id = 3,
                BookId = 3,
                MemberId = 1,
                LoanDate = new DateTime(2026, 1, 10),
                DueDate = new DateTime(2026, 3, 11),
                ReturnDate = new DateTime(2026, 3, 5)
            },
            new Loan
            {
                Id = 4,
                BookId = 4,
                MemberId = 3,
                LoanDate = new DateTime(2026, 3, 15),
                DueDate = new DateTime(2026, 4, 14),
                ReturnDate = null
            },
            new Loan
            {
                Id = 5,
                BookId = 5,
                MemberId = 4,
                LoanDate = new DateTime(2026, 2, 1),
                DueDate = new DateTime(2026, 4, 2),
                ReturnDate = null
            }
        );
    }
}
