using System;
using Microsoft.Data.Sqlite;
using DesignPatterns;

namespace ObjectRelationalBehavior.DAO
{
    public class PersonRowGateway
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }

        public PersonRowGateway(int id, string lastName, string firstName, decimal balance)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Balance = balance;
        }

        public void Update()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("UPDATE People SET FirstName = @firstName, lastName = @lastName, balance = @balance WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@firstName", FirstName);
                    command.Parameters.AddWithValue("@lastName", LastName);
                    command.Parameters.AddWithValue("@balance", Balance);
                    command.ExecuteNonQuery();
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
            }
        }

        public Person GetPerson()
        {
            return new Person(Id,LastName,FirstName, Balance);
        }

    }
}