using System;
using Microsoft.Data.Sqlite;
using DesignPatterns;

namespace DataAccess.RowDataGateway
{
    public class PersonRowGateway
    {
        public int Id { get; set; }
        private Person person;

        public PersonRowGateway(int id, string lastName, string firstName, decimal balance)
        {
            Id = id;
            person = new Person(lastName, firstName, balance);
        }

        public void Update()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("UPDATE People SET FirstName = @firstName, lastName = @lastName, balance = @balance WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@firstName", person.FirstName);
                    command.Parameters.AddWithValue("@lastName", person.LastName);
                    command.Parameters.AddWithValue("@balance", person.Balance);
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
                    command.Parameters.AddWithValue("@firstName", person.FirstName);
                    command.Parameters.AddWithValue("@lastName", person.LastName);
                    command.Parameters.AddWithValue("@balance", person.Balance);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Person GetPerson()
        {
            return person;
        }

    }
}