﻿using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using DesignPatterns;

namespace ObjectRelationalStructures.ForeignKeyMapping
{
    public class AuthorMapper
    {
        private List<Author> authors = new List<Author>();
        public bool Updated { get; set; } = false;

        private static AuthorMapper? instance = null;
        public static AuthorMapper GetInstance()
        {

            if (instance == null)
                instance = new AuthorMapper();
            return instance;
            
        }

        public void Fetch()
        {
            authors.Clear();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT author_id, firstName, lastName FROM Authors", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            authors.Add(new Author(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                }
            }
            Updated = true;
        }

        public List<Author> FindByLastName(string lastName)
        {
            return authors.Where(author => author.LastName == lastName).ToList();
        }


        public Author? FindID(int id)
        {
            return authors.Find(author => author.Id == id);
        }

        public List<Author> FindAll()
        {
            return authors;
        }

        public void Insert(Author author)
        {
            authors.Add(author);
            Updated = false;
        }

        public void InsertOrUpdate(Author author)
        {
            if(author.Id is null)
            {
                Insert(author);
                return;
            }
            var authorToUpdate = FindID(author.Id.Value);
            if(authorToUpdate is null)
            {
                Insert(author);
                return;
            }
            Update(author);
        }

        public void Update(Author author)
        {
            authors.Where(a => a.Id == author.Id).ToList().ForEach(a => a = author);
            Updated = false;
        }

        public void Delete(int id)
        {
            authors.RemoveAll(author => author.Id == id);
            Updated = false;
        }

        public void Save()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("DELETE FROM Authors", connection))
                    command.ExecuteNonQuery();
                foreach (Author author in authors)
                {
                    if (author.Id is null)
                    {
                        using (SqliteCommand command = new SqliteCommand("INSERT INTO Authors (firstName, lastName) VALUES (@firstName, @lastName)", connection))
                        {
                            command.Parameters.AddWithValue("@firstName", author.FirstName);
                            command.Parameters.AddWithValue("@lastName", author.LastName);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqliteCommand command = new SqliteCommand("UPDATE Authors SET firstName = @firstName, lastName = @lastName WHERE author_id = @id", connection))
                        {
                            command.Parameters.AddWithValue("@firstName", author.FirstName);
                            command.Parameters.AddWithValue("@lastName", author.LastName);
                            command.Parameters.AddWithValue("@id", author.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                authors.Clear();
                this.Fetch();
            }

        }

    }
}