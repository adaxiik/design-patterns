using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using DesignPatterns;
using ObjectRelationalBehavioral.DAO;

namespace ObjectRelationalBehavioral.IdentityMap
{
    public class IMPersonFinder
    {
        private static IdentityMap identityMap = new IdentityMap();

        public static PersonRowGateway? FindById(int id)
        {

            var person = identityMap.GetPerson(id);
            if (person != null)
            {
                Console.WriteLine("Entity, with id {0}, found in identity map", id);
                return person;
            }
            
        
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT id, lastName, firstName, balance FROM People WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        if (reader.Read())
                            person = new PersonRowGateway(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDecimal(3));
                }
            }
            if (person != null)
            {
                identityMap.AddPerson(person);
                Console.WriteLine("Entity, with id {0}, added to identity map", id);
            }   
                
            
            return person;
        }
    
        // FindByLastName is not searching in map
        public static List<PersonRowGateway> FindByLastName(string lastName)
        {
            List<PersonRowGateway> result = new List<PersonRowGateway>();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT id, lastName, firstName, balance FROM People WHERE LastName = @lastName", connection))
                {
                    command.Parameters.AddWithValue("@lastName", lastName);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            result.Add(new PersonRowGateway(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDecimal(3)));
                }
            }

            foreach (var person in result)
                identityMap.AddPerson(person);

            return result;
        }
    }
}