using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {

    //[HttpGet("/items/{categoryId}/items/new")]
    //public ActionResult New(int categoryId)
    //{
    //    Category category = Category.Find(categoryId);
    //    return View(category);
    //}

    //[HttpGet("/items/{categoryId}/items/{itemId}")]
    //public ActionResult Show(int categoryId, int itemId)
    //{
    //    Item item = Item.Find(itemId);
    //    Dictionary<string, object> model = new Dictionary<string, object>();
    //    Category category = Category.Find(categoryId);
    //    model.Add("item", item);
    //    model.Add("category", category);
    //    return View(model);
    //}

    //[HttpPost("/items/{categoryId}/items/{itemId}")]
    //public ActionResult Update(int categoryId, int itemId)
    //{
    //  Item item = Item.Find(itemId);
    //  Dictionary<string, object> model = new Dictionary<string, object>();
    //  Category category = Category.Find(categoryId);
    //  model.Add("category", category);
    //  model.Add("item", item);
    //  return View("Show", model);
    //}

    //[HttpGet("/items/{categoryId}/items/{itemId}/edit")]
    //public ActionResult Edit(int categoryId, int itemId)
    //{
    //  Dictionary<string, object> model = new Dictionary<string, object>();
    //  Category category = Category.Find(categoryId);
    //  model.Add("category", category);
    //  Item item = Item.Find(itemId);
    //  model.Add("item", item);
    //  return View(model);
    //}

    //[HttpGet("/items/{categoryId}/items/{itemId}/delete")]
    //public ActionResult Delete(int categoryId, int itemId)
    //{
    //    Dictionary<string, object> model = new Dictionary<string, object>();
    //    Category category = Category.Find(categoryId);
    //    Item item = Item.Find(itemId);
    //    model.Add("category", category);
    //    model.Add("item", item);
    //    return View(model);
    //}


        [HttpGet("/items")]
        public ActionResult Index()
        {
            List<Item> allItems = Item.GetAll();
            return View(allItems);
        }

        [HttpGet("/items/new")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost("/items")]
        public ActionResult Create(string description)
        {
            Item newItem = new Item(description, DateTime.Now.AddDays(1));
            newItem.Save();
            List<Item> allItems = Item.GetAll();
            return View("Index", allItems);
        }

        [HttpGet("/items/{id}")]
        public ActionResult Show(int id)
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            //Item selectedItem = Item.Find(id);
            //List<Category> itemCategories = selectedItem.GetCategory();
            //List<Category> allCategories = Category.GetAll();
            //model.Add("selectedItem", selectedItem);
            //model.Add("itemCategories", itemCategories);
            //model.Add("allCategories", allCategories);
            return View(model);
        }

        //[HttpPost("/items/{itemId}/categories/new")]
        //public ActionResult AddCategory(int itemId, int categoryId)
        //{
        //    Item item = Item.Find(itemId);
        //    Category category = Category.Find(categoryId);
        //    item.AddCategory(category);
        //    return RedirectToAction("Show", new { id = itemId });
        //}

        [HttpPost("/items/delete")]
    public ActionResult DeleteAll()
    {
      Item.ClearAll();
      return View();
    }
  }
}
