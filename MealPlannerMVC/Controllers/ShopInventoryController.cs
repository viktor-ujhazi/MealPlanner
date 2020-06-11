using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealPlannerMVC.Models;
using MealPlannerMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MealPlannerMVC.Controllers
{
    [Authorize]
    public class ShopInventoryController : Controller
    {
        private readonly ILogger<ShopInventoryController> _logger;
        private readonly IShopInventoryService _inventoryService;

        public ShopInventoryController(ILogger<ShopInventoryController> logger, IShopInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }
        public IActionResult Inventory(List<ShopInventoryModel> inventoryItems)
        {
            int shopID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            if (inventoryItems.Count == 0)
            {
                inventoryItems = _inventoryService.GetAllItem(shopID);
            }
            return View(inventoryItems);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var inventoryItem = _inventoryService.GetItemByID(id);


            return View(inventoryItem);
        }
        [HttpPost]
        public IActionResult Edit(int id, ShopInventoryModel inventoryItem)
        {
            
            _inventoryService.UpdateItem(id, inventoryItem);

            return RedirectToAction("Inventory", "ShopInventory");
        }
        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(ShopInventoryModel inventoryItem)
        {
            var shopID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            inventoryItem.ShopID = shopID;
            _inventoryService.AddItem(inventoryItem);

            return RedirectToAction("Inventory", "ShopInventory");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _inventoryService.DeleteItem(id);


            return RedirectToAction("Inventory", "ShopInventory");
        }
    }
}