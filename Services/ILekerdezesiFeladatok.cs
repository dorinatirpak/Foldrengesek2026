using Földrengések2026.ViewModels;

namespace Földrengések2026.Services
{
    public interface ILekerdezesiFeladatok
    {
        IQueryable<string> SomogyTelepulesNevek();
        IQueryable<Feladat3ViewModel> VarmegyeiRengesSzamok();
        Feladat4ViewModel? LegnagyobbMagnitudo();
    }
}