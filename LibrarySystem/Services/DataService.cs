using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Data;
using LibrarySystem.Models;

namespace LibrarySystem.Services;

public class DataService
{
    private readonly LibraryDbContext _db;

    public DataService(LibraryDbContext db)
    {
        _db = db;
    }

    // -- Auth --

    public static string HashPassword(string password)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }

    public Librarian? Authenticate(string username, string password)
    {
#pragma warning disable CA1862
        return _db.Librarians.FirstOrDefault(l =>
            l.Username.ToLower() == username.ToLower() &&
            l.PasswordHash == HashPassword(password));
#pragma warning restore CA1862
    }

    // -- Books --

    public List<Book> GetBooks(bool includeDeleted = false)
    {
        return _db.Books
              .Where(b => includeDeleted || !b.IsDeleted)
              .OrderBy(b => b.Title)
              .ToList();
    }

    public Book? GetBook(int id)
    {
        return _db.Books.FirstOrDefault(b => b.Id == id);
    }

    public Book AddBook(Book book)
    {
        var existing = _db.Books.FirstOrDefault(b => b.ISBN == book.ISBN && !b.IsDeleted);
        if (existing != null)
        {
            existing.CopyCount++;
            _db.SaveChanges();
            return existing;
        }
        _db.Books.Add(book);
        _db.SaveChanges();
        return book;
    }

    public void UpdateBook(Book book)
    {
        _db.Books.Update(book);
        _db.SaveChanges();
    }

    public (bool success, string message) DeleteBookCopy(int bookId, bool deleteAll)
    {
        var book = GetBook(bookId);
        if (book == null) return (false, "A konyv nem talalhato.");

        int loanedCount = _db.Loans.Count(l => l.BookId == bookId && l.ReturnDate == null);

        if (deleteAll)
        {
            if (loanedCount > 0)
                return (false, $"Nem torolheto: {loanedCount} peldany ki van kolcsonozve.");
            book.IsDeleted = true;
            book.CopyCount = 0;
        }
        else
        {
            int available = book.CopyCount - loanedCount;
            if (available <= 0)
                return (false, "Nincs szabad peldany. Minden ki van kolcsonozve.");
            book.CopyCount--;
            if (book.CopyCount == 0) book.IsDeleted = true;
        }

        _db.SaveChanges();
        return (true, "Sikeres torles.");
    }

    public List<Book> SearchBooks(string field, string text)
    {
        return _db.Books
            .Where(b => !b.IsDeleted)
            .AsEnumerable()
            .Where(b => field switch
            {
                "Author" => b.Author.Contains(text, StringComparison.OrdinalIgnoreCase),
                "ISBN"   => b.ISBN.Contains(text, StringComparison.OrdinalIgnoreCase),
                "Id"     => b.Id.ToString() == text,
                _        => b.Title.Contains(text, StringComparison.OrdinalIgnoreCase)
            })
            .ToList();
    }

    public int GetAvailableCopies(int bookId)
    {
        var book = GetBook(bookId);
        if (book == null) return 0;
        int loaned = _db.Loans.Count(l => l.BookId == bookId && l.ReturnDate == null);
        return Math.Max(0, book.CopyCount - loaned);
    }

    // -- Members --

    public List<LibraryMember> GetMembers(bool includeDeleted = false)
    {
        return _db.Members
              .Where(m => includeDeleted || !m.IsDeleted)
              .OrderBy(m => m.Name)
              .ToList();
    }

    public LibraryMember? GetMember(int id)
    {
        return _db.Members.FirstOrDefault(m => m.Id == id);
    }

    public LibraryMember AddMember(LibraryMember member)
    {
        _db.Members.Add(member);
        _db.SaveChanges();
        return member;
    }

    public void UpdateMember(LibraryMember member)
    {
        _db.Members.Update(member);
        _db.SaveChanges();
    }

    public (bool success, string message) DeleteMember(int id)
    {
        var member = GetMember(id);
        if (member == null) return (false, "A tag nem talalhato.");
        member.IsDeleted = true;
        _db.SaveChanges();
        return (true, "Tag sikeresen torolve.");
    }

    public List<LibraryMember> SearchMembers(string field, string text)
    {
        return _db.Members
            .Where(m => !m.IsDeleted)
            .AsEnumerable()
            .Where(m => field switch
            {
                "Address" => m.Address.Contains(text, StringComparison.OrdinalIgnoreCase),
                _         => m.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            })
            .ToList();
    }

    // -- Loans --

    public List<Loan> GetLoans()
    {
        return _db.Loans
              .Include(l => l.Book)
              .Include(l => l.Member)
              .OrderByDescending(l => l.LoanDate)
              .ToList();
    }

    public Loan? GetLoan(int id)
    {
        return _db.Loans.FirstOrDefault(l => l.Id == id);
    }

    public (bool success, string message, Loan? loan) CreateLoan(int bookId, int memberId, DateTime loanDate)
    {
        var book = GetBook(bookId);
        if (book == null) return (false, "A konyv nem talalhato.", null);
        if (!book.IsLoanable) return (false, "Ez a konyv nem kolcsonozheto.", null);
        if (GetAvailableCopies(bookId) <= 0)
            return (false, "Nincs szabad peldany.", null);

        var member = GetMember(memberId);
        if (member == null) return (false, "A tag nem talalhato.", null);

        int currentLoans = _db.Loans.Count(l => l.MemberId == memberId && l.ReturnDate == null);
        if (member.MaxBooks != int.MaxValue && currentLoans >= member.MaxBooks)
            return (false, $"A tag elerte a kolcsonzesi limitet ({member.MaxBooks} konyv).", null);

        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            LoanDate = loanDate,
            DueDate = loanDate.AddDays(member.LoanDays)
        };

        _db.Loans.Add(loan);
        _db.SaveChanges();
        return (true, "Sikeres kolcsonzes.", loan);
    }

    public (bool success, string message, bool overdue, int overdueDays) ReturnBook(int loanId)
    {
        var loan = GetLoan(loanId);
        if (loan == null) return (false, "A kolcsonzes nem talalhato.", false, 0);
        if (loan.ReturnDate.HasValue) return (false, "Ez a konyv mar vissza lett hozva.", false, 0);

        bool overdue = DateTime.Today > loan.DueDate;
        int days = overdue ? (DateTime.Today - loan.DueDate).Days : 0;

        loan.ReturnDate = DateTime.Today;
        _db.SaveChanges();

        string msg = overdue
            ? $"Konyv visszaveve. Keses: {days} nap!"
            : "Konyv sikeresen visszaveve.";

        return (true, msg, overdue, days);
    }

    public List<Loan> GetMemberLoans(int memberId, bool? returned = null)
    {
        return _db.Loans
              .Include(l => l.Book)
              .Where(l => l.MemberId == memberId &&
                  (returned == null ||
                   (returned == true  ? l.ReturnDate != null : l.ReturnDate == null)))
              .OrderByDescending(l => l.LoanDate)
              .ToList();
    }

    // -- Dashboard --

    public DashboardViewModel GetDashboard()
    {
        List<Loan> activeLoans = _db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .ToList();

        List<Loan> recent = _db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .Take(5)
            .ToList();

        return new DashboardViewModel
        {
            TotalBooks = _db.Books.Count(b => !b.IsDeleted),
            TotalMembers = _db.Members.Count(m => !m.IsDeleted),
            ActiveLoans = activeLoans.Count,
            OverdueLoans = activeLoans.Count(l => l.IsOverdue),
            RecentLoans = recent.Select(l => new LoanViewModel
            {
                Loan   = l,
                Book   = l.Book,
                Member = l.Member
            }).ToList()
        };
    }
}
