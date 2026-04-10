using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers;

[Authorize]
public class LoansController : Controller
{
    private readonly DataService _data;

    public LoansController(DataService data)
    {
        _data = data;
    }

    // DEMO: Kolcsonzes rogzitese - MUKODO
    public IActionResult Create(int? bookId, int? memberId)
    {
        List<Book> books = _data.GetBooks()
            .Where(b => b.IsLoanable && _data.GetAvailableCopies(b.Id) > 0)
            .ToList();

        var vm = new CreateLoanViewModel
        {
            AvailableBooks = books,
            Members = _data.GetMembers(),
            LoanDate = DateTime.Today,
            BookId = bookId ?? 0,
            MemberId = memberId ?? 0
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(CreateLoanViewModel vm)
    {
        var (success, message, loan) = _data.CreateLoan(vm.BookId, vm.MemberId, vm.LoanDate);
        if (success)
        {
            TempData["Success"] = message;
            return RedirectToAction("Index", "Home");
        }

        vm.ErrorMessage = message;
        vm.AvailableBooks = _data.GetBooks()
            .Where(b => b.IsLoanable && _data.GetAvailableCopies(b.Id) > 0)
            .ToList();
        vm.Members = _data.GetMembers();
        return View(vm);
    }

    // TODO 3. merfoldk.: Konyv visszavetele
    [HttpPost]
    public IActionResult Return(int id)
    {
        TempData["Warning"] = "A konyv visszavetele a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction("Index", "Home");
    }

    // TODO 3. merfoldk.: Osszes kolcsonzes listazasa
    public IActionResult All()
    {
        TempData["Warning"] = "Az osszes kolcsonzes listazasa a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction("Index", "Home");
    }
}
