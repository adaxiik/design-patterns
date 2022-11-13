using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using DesignPatterns;

namespace ObjectRelationalStructures.AssociationTableMapping
{
    public class ChildMapper
    {
        private List<Child> children = new List<Child>();
        public bool Updated { get; set; } = false;
        private static ChildMapper? instance = null;
        public static ChildMapper GetInstance()
        {
            if (instance == null)
                instance = new ChildMapper();
            return instance;
        }
        private ChildMapper() { }

        public void Fetch()
        {
            children.Clear();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT child_id, firstName, lastName, age FROM Children", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            children.Add(new Child(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3)));
                }
            }
            Updated = true;
        }

        public List<Child> FindByLastname(string lastName)
        {
            return children.Where(child => child.LastName == lastName).ToList();
        }

        public List<Child> FindAll()
        {
            return children;
        }


        public Child? FindID(int id)
        {
            return children.Find(child => child.Id == id);
        }

        public void Insert(Child child)
        {
            children.Add(child);
            Updated = false;
        }

        public void Update(Child child)
        {
            children.Where(c => c.Id == child.Id).ToList().ForEach(c => c = child);
            Updated = false;
        }

        public void Delete(int id)
        {
            children.RemoveAll(child => child.Id == id);
            Updated = false;
        }

        public void Save()
        {

            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                foreach (Child child in children)
                {
                    if(child.Id is null)
                        using (SqliteCommand command = new SqliteCommand("INSERT INTO Children (firstName, lastName, age) VALUES (@firstName, @lastName, @age)", connection))
                        {
                            command.Parameters.AddWithValue("@firstName", child.FirstName);
                            command.Parameters.AddWithValue("@lastName", child.LastName);
                            command.Parameters.AddWithValue("@age", child.Age);
                            command.ExecuteNonQuery();
                        }
                    else
                        using (SqliteCommand command = new SqliteCommand("UPDATE Children SET firstName = @firstName, lastName = @lastName, age = @age WHERE child_id = @id", connection))
                        {
                            command.Parameters.AddWithValue("@firstName", child.FirstName);
                            command.Parameters.AddWithValue("@lastName", child.LastName);
                            command.Parameters.AddWithValue("@age", child.Age);
                            command.Parameters.AddWithValue("@id", child.Id);
                            command.ExecuteNonQuery();
                        }
                }
            }
            this.Fetch();

        }

    }
}