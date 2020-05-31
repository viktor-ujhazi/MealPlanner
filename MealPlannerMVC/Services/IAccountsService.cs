using MealPlannerMVC.Models;

namespace MealPlannerMVC.Services
{
    public interface IAccountsService
    {
        public void Register(RegisterModel account);
        public Account Login(string email, string password);
        int GetUserId(string email);
    }
}