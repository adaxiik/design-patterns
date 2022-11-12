using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using DesignPatterns;

namespace DataAccess.DataMapper
{
    public class PersonMapper
    {
        private List<Person> people = new List<Person>();

        public void Fetch()
        {
            people.Clear();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT * FROM People", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            people.Add(new Person(reader.GetString(1), reader.GetString(2), reader.GetDecimal(3)));
                }
            }
        }

        public List<Person> FindByLastName(string lastName)
        {
            return people.FindAll(p => p.LastName == lastName);
        }

        public Person? Find(int id)
        {
            if(id < 0 || id >= people.Count)
                return null;
            return people[id];
        }

        public void Insert(Person person)
        {
            people.Add(person);
        }
        
        public void UpdateByLastName(Person person)
        {
            for(int i = 0; i < people.Count; i++)
                if(people[i].LastName == person.LastName)
                    people[i] = person;
        }

        public void Delete(int id)
        {
            if(id >= 0 && id < people.Count)
                people.RemoveAt(id);
        }

        public void Save()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("DELETE FROM People", connection))
                    command.ExecuteNonQuery();
                for (int i = 0; i < people.Count; i++)
                {
                    using (SqliteCommand command = new SqliteCommand("INSERT INTO People (LastName, FirstName, Balance) VALUES (@lastName, @firstName, @balance)", connection))
                    {
                        command.Parameters.AddWithValue("@lastName", people[i].LastName);
                        command.Parameters.AddWithValue("@firstName", people[i].FirstName);
                        command.Parameters.AddWithValue("@balance", people[i].Balance);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}