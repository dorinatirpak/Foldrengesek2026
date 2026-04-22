namespace Földrengések2026.Services
{
    public interface ILekerdezesiFeladatok
    {
        IQueryable<string> SomogyTelepulesNevek();
    }
}
