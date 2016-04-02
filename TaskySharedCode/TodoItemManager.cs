using System;
using System.Collections.Generic;
using Parse;

namespace Tasky.Shared 
{
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class TodoItemManager 
	{
		static TodoItemManager ()
		{
			ParseClient.Initialize(new ParseClient.Configuration {
				ApplicationId = "5RbUg6fakfKVJxNCcXDgHTBYeNFAuH3YbGbQ3eMv",
				WindowsKey = "cj53vpGoiNkNzi0VGrdUskKXjjZ1wo0pr8DnLfYi",
				Server = "https://parseapi.back4app.com"
			});
		}

		static ParseObject GetTaskObject(int id) {
			var query = from taskQuery in ParseObject.GetQuery ("Task")
					where taskQuery.Get<int> ("ID") == id
				select taskQuery;
			return query.FirstOrDefaultAsync().GetAwaiter().GetResult();
		}
		
		public static TodoItem GetTask(int id)
		{
			ParseObject task = GetTaskObject(id);
			if (task != null) {
				return new TodoItem {
					ID = task.Get<int> ("ID"),
					Name = task.Get<string> ("Name"),
					Notes = task.Get<string> ("Notes"),
					Done = task.Get<bool> ("Done")
				};
			} else {
				return null;
			}
		}
		
		public static IList<TodoItem> GetTasks ()
		{
			List<TodoItem> results = new List<TodoItem>();
			var query = ParseObject.GetQuery ("Task").OrderBy ("Name");
			IEnumerable<ParseObject> tasks = query.FindAsync ().GetAwaiter().GetResult();
			foreach(ParseObject task in tasks){
				results.Add(new TodoItem {
					ID = task.Get<int>("ID"),
					Name = task.Get<string>("Name"),
					Notes = task.Get<string>("Notes"),
					Done = task.Get<bool>("Done")
				});
			}
			return results;
		}

		static int SaveObject(ParseObject task, TodoItem item) {
			task ["Name"] = item.Name;
			task ["Notes"] = item.Notes;
			task ["Done"] = item.Done;
			task.SaveAsync ().GetAwaiter ().GetResult ();
			return 1;
		}

		public static int SaveTask (TodoItem item)
		{
			if (item.ID <= 0) {				
				ParseObject task = new ParseObject ("Task");
				task ["ID"] = (new Random ()).Next (1, int.MaxValue);
				return SaveObject(task, item);
			} else {
				ParseObject task = GetTaskObject (item.ID);
				if (task != null) {					
					return SaveObject(task, item);
				} else {
					return 0;
				}
			}
		}
		
		public static int DeleteTask(int id)
		{
			ParseObject task = GetTaskObject (id);
			if (task != null) {	
				task.DeleteAsync ().GetAwaiter ().GetResult ();
				return 1;
			} else {
				return 0;
			}
		}
	}
}