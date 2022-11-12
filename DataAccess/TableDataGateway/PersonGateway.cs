using System;
using DataAccess;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
namespace DataAccess.TableDataGateway
{
    public class PersonGateway
     {
        public Person? Find(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT * FROM People WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        if (reader.Read())
                            return new Person(reader.GetString(1), reader.GetString(2), reader.GetDecimal(3));
                }
            }
            return null;
        }
        
        public List<Person> FindByLastName(string lastName)
        {
            List<Person> result = new List<Person>();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT * FROM People WHERE LastName = @lastName", connection))
                {
                    command.Parameters.AddWithValue("@lastName", lastName);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            result.Add(new Person(reader.GetString(1), reader.GetString(2), reader.GetDecimal(3)));
                }
            }
            return result;
        }

        public void Insert(Person person)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("INSERT INTO People (LastName, FirstName, Balance) VALUES (@lastName, @firstName, @balance)", connection))
                {
                    command.Parameters.AddWithValue("@lastName", person.LastName);
                    command.Parameters.AddWithValue("@firstName", person.FirstName);
                    command.Parameters.AddWithValue("@balance", person.Balance);
                    command.ExecuteNonQuery();
                }
            }  
        }

        public void Update(Person person, int id)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("UPDATE People SET LastName = @lastName, FirstName = @firstName, Balance = @balance WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@lastName", person.LastName);
                    command.Parameters.AddWithValue("@firstName", person.FirstName);
                    command.Parameters.AddWithValue("@balance", person.Balance);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }  
        }

        public void Remove(Person person)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("DELETE FROM People WHERE LastName = @lastName AND FirstName = @firstName AND Balance = @balance", connection))
                {
                    command.Parameters.AddWithValue("@lastName", person.LastName);
                    command.Parameters.AddWithValue("@firstName", person.FirstName);
                    command.Parameters.AddWithValue("@balance", person.Balance);
                    command.ExecuteNonQuery();
                }
            }  
        }
    }
}