using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace ToDoList.Models
{
  public class Category
  {
    private string _name;
    private int _id;
    // private List<Item> _items;

    public Category(string categoryName, int id = 0)
    {
      _name = categoryName;
      _id = id;
      // _items = new List<Item>{};
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @" DELETE FROM categories_items; DELETE FROM category;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM category;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int CategoryId = rdr.GetInt32(0);
        string CategoryName = rdr.GetString(1);
        Category newCategory = new Category(CategoryName, CategoryId);
        allCategories.Add(newCategory);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCategories;
    }

    public static Category Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM category WHERE id = (@searchId);";
      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int CategoryId = 0;
      string CategoryName = "";
      while(rdr.Read())
      {
        CategoryId = rdr.GetInt32(0);
        CategoryName = rdr.GetString(1);
      }
      Category newCategory = new Category(CategoryName, CategoryId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newCategory;
    }

    public List<Item> GetItems(string sortBy = "")
      {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
          if (sortBy == "")
          {
            cmd.CommandText = @"SELECT items.*, categories_items.due_date, categories_items.is_complete FROM category
                JOIN categories_items ON (category.id = categories_items.category_id)
                JOIN items ON (categories_items.item_id = items.id)
                WHERE category.id = @CategoryId;";
            }
            else {
              cmd.CommandText = @"SELECT items.*, categories_items.due_date, categories_items.is_complete FROM categories
                    JOIN categories_items ON (category.id = categories_items.category_id)
                    JOIN items ON (categories_items.item_id = items.id)
                    WHERE category.id = @CategoryId ORDER BY items." + sortBy + ";";
            }

          MySqlParameter categoryIdParameter = new MySqlParameter();
          categoryIdParameter.ParameterName = "@CategoryId";
          categoryIdParameter.Value = _id;
          cmd.Parameters.Add(categoryIdParameter);
          MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
          List<Item> items = new List<Item>{};
          while(rdr.Read())
          {
            int thisItemId = rdr.GetInt32(0);
            string itemDescription = rdr.GetString(1);
            DateTime itemDueDate = rdr.GetDateTime(2);
                bool isComplete = rdr.GetBoolean(3);
                Item foundItem = new Item(itemDescription, itemDueDate, id: thisItemId, complete: isComplete);
            items.Add(foundItem);
          }
          conn.Close();
          if (conn != null)
          {
            conn.Dispose();
          }
          return items;
      }

    public override bool Equals(System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        bool idEquality = this.GetId().Equals(newCategory.GetId());
        bool nameEquality = this.GetName().Equals(newCategory.GetName());
        return (idEquality && nameEquality);
      }
    }

    public override int GetHashCode()
    {
        return this.GetId().GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO category (name) VALUES (@name);";
      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);
      cmd.ExecuteNonQuery();
      _id = (int)cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

        public void SaveItem(Item item)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO categories_items (category_id, item_id, due_date) VALUES (@categoryId, @itemId, @dueDate);";
            MySqlParameter categoryId = new MySqlParameter();
            categoryId.ParameterName = "@categoryId";
            categoryId.Value = this._id;
            MySqlParameter itemId = new MySqlParameter();
            itemId.ParameterName = "@itemId";
            itemId.Value = item.GetId();
            MySqlParameter dueDate = new MySqlParameter();
            dueDate.ParameterName = "@dueDate";
            dueDate.Value = item.GetDueDate();
            cmd.Parameters.Add(categoryId);
            cmd.Parameters.Add(itemId);
            cmd.Parameters.Add(dueDate);
            cmd.ExecuteNonQuery();
            _id = (int)cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand("DELETE FROM categories_items WHERE category_id = @CategoryId; DELETE FROM category WHERE id = @CategoryId;", conn);
      MySqlParameter categoryIdParameter = new MySqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetId();
      cmd.Parameters.Add(categoryIdParameter);
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddItem(Item newItem)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories_items (category_id, item_id, due_date, is_complete) VALUES (@CategoryId, @ItemId, @dueDate, @isComplete);";
      MySqlParameter category_id = new MySqlParameter();
      category_id.ParameterName = "@CategoryId";
      category_id.Value = _id;
      cmd.Parameters.Add(category_id);
      MySqlParameter item_id = new MySqlParameter();
      item_id.ParameterName = "@ItemId";
      item_id.Value = newItem.GetId();
      cmd.Parameters.Add(item_id);
            MySqlParameter due_date = new MySqlParameter();
            due_date.ParameterName = "@dueDate";
            due_date.Value = newItem.GetDueDate();
            cmd.Parameters.Add(due_date);
            MySqlParameter is_complete = new MySqlParameter();
            is_complete.ParameterName = "@isComplete";
            is_complete.Value = newItem.GetComplete();
            cmd.Parameters.Add(is_complete);
            cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

  }
}
