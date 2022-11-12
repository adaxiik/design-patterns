using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace DataAccess.RowDataGateway
{
    public class PersonFinder
    {
        public static PersonRowGateway? FindById(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT id, firstName, lastName, balance FROM People WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        if (reader.Read())
                            return new PersonRowGateway(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDecimal(3));

                }
            }
            return null;
        }

        public static List<PersonRowGateway> FindByLastName(string lastName)
        {
            List<PersonRowGateway> result = new List<PersonRowGateway>();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT id, firstName, lastName, balance FROM people WHERE lastName = @lastName", connection))
                {
                    command.Parameters.AddWithValue("@lastName", lastName);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            result.Add(new PersonRowGateway(reader.GetInt32(0), reader.GetString(2), reader.GetString(1), reader.GetDecimal(3)));

                }
            }
            return result;
        }
    }
}