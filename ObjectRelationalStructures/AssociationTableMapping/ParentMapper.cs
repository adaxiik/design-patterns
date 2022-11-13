using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using DesignPatterns;

namespace ObjectRelationalStructures.AssociationTableMapping
{
    public class ParentMapper
    {
        private List<Parent> parents = new List<Parent>();
        public bool Updated { get; set; } = false;

        private static ParentMapper? instance = null;
        public static ParentMapper GetInstance()
        {

            if (instance == null)
                instance = new ParentMapper();
            return instance;

        }
        private ParentMapper() { }

        public void Fetch()
        {
            parents.Clear();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT parent_id, firstName, lastName FROM Parents", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            parents.Add(new Parent(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                }
            }

            var links = new List<(int, int)>();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT parent_id, child_id FROM ParentChild", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            links.Add((reader.GetInt32(0), reader.GetInt32(1)));
                }
            }

            foreach (var link in links)
            {
                var parent = parents.Find(p => p.Id == link.Item1);
                var child = ChildMapper.GetInstance().FindID(link.Item2);
                if (parent != null && child != null)
                    parent.AddChild(child);
            }


            Updated = true;
        }

        public List<Parent> FindByLastName(string lastName)
        {
            return parents.Where(parent => parent.LastName == lastName).ToList();
        }


        public Parent? FindID(int id)
        {
            return parents.Find(parent => parent.Id == id);
        }

        public List<Parent> FindAll()
        {
            return parents;
        }

        public void Insert(Parent author)
        {
            parents.Add(author);
            Updated = false;
        }


        public void Update(Parent parent)
        {
            parents.Where(p => p.Id == parent.Id).ToList().ForEach(p => p = parent);
            Updated = false;
        }

        public void Delete(int id)
        {
            parents.RemoveAll(p => p.Id == id);
            Updated = false;
        }

        public void Save()
        {

            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();

                foreach (var parent in parents)
                {
                    if(parent.Id is null)
                    {
                        using (SqliteCommand command = new SqliteCommand("INSERT INTO Parents (firstName, lastName) VALUES (@firstName, @lastName)", connection))
                        {
                            command.Parameters.AddWithValue("@firstName", parent.FirstName);
                            command.Parameters.AddWithValue("@lastName", parent.LastName);
                            command.ExecuteNonQuery();
                        }
                        using (SqliteCommand command = new SqliteCommand("SELECT last_insert_rowid()", connection))
                        {
                            parent.Id = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    else
                    {
                        using (SqliteCommand command = new SqliteCommand("UPDATE Parents SET firstName = @firstName, lastName = @lastName WHERE parent_id = @id", connection))
                        {
                            command.Parameters.AddWithValue("@firstName", parent.FirstName);
                            command.Parameters.AddWithValue("@lastName", parent.LastName);
                            command.Parameters.AddWithValue("@id", parent.Id);
                            command.ExecuteNonQuery();
                        }
                    }

                    using (SqliteCommand command = new SqliteCommand("DELETE FROM ParentChild WHERE parent_id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", parent.Id);
                        command.ExecuteNonQuery();
                    }

                    foreach (var child in parent.Children)
                    {
                        if(child.Id is null)
                            throw new Exception("Child not in database");

                        using (SqliteCommand command = new SqliteCommand("INSERT INTO ParentChild (parent_id, child_id) VALUES (@parentId, @childId)", connection))
                        {
                            command.Parameters.AddWithValue("@parentId", parent.Id);
                            command.Parameters.AddWithValue("@childId", child.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                this.Fetch();
            }
        }

    }
}