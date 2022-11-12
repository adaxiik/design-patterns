using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace DataAccess.ActiveRecord
{
    public class PersonActiveRecord
    {
        public int Id { get; set; } = -1;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public decimal Balance { get; set; } 

        public PersonActiveRecord(string lastName, string firstName, decimal balance)
        {
            LastName = lastName;
            FirstName = firstName;
            Balance = balance;
        }


        public void Update()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("UPDATE People SET firstName = @firstName, lastName = @lastName, balance = @balance WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@firstName", FirstName);
                    command.Parameters.AddWithValue("@lastName", LastName);
                    command.Parameters.AddWithValue("@balance", Balance);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Insert()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("INSERT INTO People (firstName, lastName, balance) VALUES (@firstName, @lastName, @balance)", connection))
                {
                    command.Parameters.AddWithValue("@firstName", FirstName);
                    command.Parameters.AddWithValue("@lastName", LastName);
                    command.Parameters.AddWithValue("@balance", Balance);
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = new SqliteCommand("SELECT last_insert_rowid()", connection))
                {
                    Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }

        }

        public void Delete()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("DELETE FROM People WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", Id);
                    command.ExecuteNonQuery();
                }
            }
        }



        public static PersonActiveRecord? Find(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT * FROM People WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PersonActiveRecord(reader.GetString(2), reader.GetString(1), reader.GetDecimal(3))
                            {
                                Id = reader.GetInt32(0)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static List<PersonActiveRecord> FindByLastName(string lastName)
        {
            List<PersonActiveRecord> result = new List<PersonActiveRecord>();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT * FROM People WHERE LastName = @lastName", connection))
                {
                    command.Parameters.AddWithValue("@lastName", lastName);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new PersonActiveRecord(reader.GetString(2), reader.GetString(1), reader.GetDecimal(3))
                            {
                                Id = reader.GetInt32(0)
                            });
                        }
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            return $"LN: {LastName}, FN: {FirstName} ({Balance}) Id: {Id}";
        }
    }
}