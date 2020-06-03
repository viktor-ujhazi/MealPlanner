using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealPlannerMVC.Models;
using MealPlannerMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MealPlannerMVC.Controllers
{
    public class UserInventoryController : Controller
    {
        private readonly ILogger<UserInventoryController> _logger;
        private readonly IUserInventoryService _inventoryService;
        public IActionResult Index()
        {
            return View();
        }
        public UserInventoryController(ILogger<UserInventoryController> logger, IUserInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }
        public IActionResult Inventory(List<UserInventoryModel> inventoryItems)
        {
            int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            if (inventoryItems.Count == 0)
            {
                inventoryItems = _inventoryService.GetAllItem(userID);
            }
            return View(inventoryItems);
        }
    }
}