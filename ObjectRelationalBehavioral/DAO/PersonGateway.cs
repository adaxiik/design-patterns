using System;
using DataAccess;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using DesignPatterns;

namespace ObjectRelationalBehavioral.DAO
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
                            return new Person(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDecimal(3));
                }
            }
            return null;
        }
        public List<Person> FindAll()
        {
            List<Person> people = new List<Person>();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT * FROM People", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            people.Add(new Person(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDecimal(3)));
                }
            }
            return people;
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
                            result.Add(new Person(reader.GetInt32(0),reader.GetString(1), reader.GetString(2), reader.GetDecimal(3)));
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
                    command.Parameters.AddWithValue("@lastName", person.GetLastName());
                    command.Parameters.AddWithValue("@firstName", person.GetFirstName());
                    command.Parameters.AddWithValue("@balance", person.GetBalance());
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand("SELECT last_insert_rowid()", connection))
                {
                    person.SetId(Convert.ToInt32(command.ExecuteScalar()));
                }
            }  
        }

        public void Update(Person person)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("UPDATE People SET LastName = @lastName, FirstName = @firstName, Balance = @balance WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@lastName", person.GetLastName());
                    command.Parameters.AddWithValue("@firstName", person.GetFirstName());
                    command.Parameters.AddWithValue("@balance", person.GetBalance());
                    command.Parameters.AddWithValue("@id", person.GetId());
                    command.ExecuteNonQuery();
                }
            }  
        }

        public void Remove(Person person)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("DELETE FROM People WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", person.GetId());
                    command.ExecuteNonQuery();
                }
            }  
        }
    }
}