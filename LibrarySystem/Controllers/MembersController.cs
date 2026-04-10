using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers;

[Authorize]
public class MembersController : Controller
{
    private readonly DataService _data;

    public MembersController(DataService data)
    {
        _data = data;
    }

    // DEMO: Tagok listazasa - MUKODO
    public IActionResult Index()
    {
        var vm = new MemberSearchViewModel { Results = _data.GetMembers() };
        return View(vm);
    }

    // TODO 3. merfoldk.: Tag keresese
    [HttpPost]
    public IActionResult Search(MemberSearchViewModel vm)
    {
        TempData["Warning"] = "A kereses a kovetkezo verzioban lesz elerheto.";
        vm.Results = _data.GetMembers();
        return View("Index", vm);
    }

    // TODO 3. merfoldk.: Uj tag felvetele
    public IActionResult Create()
    {
        TempData["Warning"] = "Az uj tag felvetele a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Create(CreateMemberViewModel vm)
    {
        TempData["Warning"] = "Az uj tag felvetele a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    // TODO 3. merfoldk.: Tag szerkesztese
    public IActionResult Edit(int id)
    {
        TempData["Warning"] = "A tag szerkesztese a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Edit(int id, string name, string address, string contact)
    {
        TempData["Warning"] = "A tag szerkesztese a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    // TODO 3. merfoldk.: Tag torlese
    [HttpPost]
    public IActionResult Delete(int id)
    {
        TempData["Warning"] = "A tag torlese a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }

    // TODO 3. merfoldk.: Tag kolcsonzeseinek megtekintese
    public IActionResult Loans(int id)
    {
        TempData["Warning"] = "A tag kolcsonzeseinek reszletes megtekintese a kovetkezo verzioban lesz elerheto.";
        return RedirectToAction(nameof(Index));
    }
}
