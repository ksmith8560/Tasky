using System;
using System.Linq;
using System.Collections.Generic;
using Tasky.Core.SQLite;

namespace Tasky.Core {
	/// <summary>
	/// TaskDatabase builds on SQLite.Net and represents a specific database, in our case, the Task DB.
	/// It contains methods for retrieval and persistance as well as db creation, all based on the 
	/// underlying ORM.
	/// </summary>
	public class TaskDatabase : SQLiteConnection {
		static object locker = new object ();

		/// <summary>
		/// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public TaskDatabase (string path) : base (path)
		{
			// create the tables
			CreateTable<Task> ();
		}
		
		public IEnumerable<T> GetItems<T> () where T : Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                return (from i in Table<T> () select i).ToList ();
            }
		}

		public T GetItem<T> (int id) where T : Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
				return Table<T>().FirstOrDefault(x => x.ID == id);
            }
		}

		public int SaveItem<T> (T item) where T : Contracts.IBusinessEntity
		{
            lock (locker) {
                if (item.ID != 0) {
                    Update (item);
                    return item.ID;
                } else {
                    return Insert (item);
                }
            }
		}
		
		public int DeleteItem<T>(int id) where T : Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
				return Delete<T> (id); // primaryKey
				//return Delete (new Task() {ID = id}); // would also have worked
            }
		}
	}
}