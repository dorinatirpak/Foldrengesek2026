using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers;

[Authorize]
public class BooksController : Controller
{
    private readonly DataService _data;

    public BooksController(DataService data)
    {
        _data = data;
    }

    // DEMO: Konyvek listazasa - MUKODO
    public IActionResult Index()
    {
        var books = _data.GetBooks();
        var vm = new BookSearchViewModel { Results = books };
        foreach (var b in books)
            vm.AvailableCopies[b.Id] = _data.GetAvailableCopies(b.Id);
        return View(vm);
    }

    // DEMO: Konyvek keresese - MUKODO
    [HttpPost]
    public IActionResult Search(BookSearchViewModel vm)
    {
        vm.Results = _data.SearchBooks(vm.SearchField, vm.SearchText);
        foreach (var b in vm.Results)
            vm.AvailableCopies[b.Id] = _data.GetAvailableCopies(b.Id);
        return View("Index", vm);
    }

    // DEMO: Uj konyv felvetele - MUKODO
    public IActionResult Create() => View(new Book());

    [HttpPost]
    public IActionResult Create(Book book)
    {
        if (!ModelState.IsValid) return View(book);
        _data.AddBook(book);
        TempData["Success"] = "Konyv sikeresen felveve!";
        return RedirectToAction(nameof(Index));
    }

    // TODO 3. merfoldk.: Konyv szerkesztese
    public IActionResult Edit(int id)
    {
        TempData["Warning"] = "Ez a funkcio a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Edit(Book book)
    {
        TempData["Warning"] = "Ez a funkcio a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    // TODO 3. merfoldk.: Konyv torlese
    public IActionResult Delete(int id)
    {
        TempData["Warning"] = "Ez a funkcio a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id, bool deleteAll)
    {
        TempData["Warning"] = "Ez a funkcio a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    // TODO 3. merfoldk.: Konyv reszletei
    public IActionResult Details(int id)
    {
        TempData["Warning"] = "Ez a funkcio a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }
}
