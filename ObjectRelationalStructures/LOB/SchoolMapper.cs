using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using DesignPatterns;

namespace ObjectRelationalStructures.LOB
{
    public class SchoolMapper
    {
        private List<School> schools = new List<School>();

        private static SchoolMapper? instance = null;
        public static SchoolMapper GetInstance()
        {

            if (instance == null)
                instance = new SchoolMapper();
            return instance;

        }
        private SchoolMapper() { }

        public void Fetch()
        {
            schools.Clear();
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT school_id, name, classes FROM Schools", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            var s = new School(reader.GetInt32(0), reader.GetString(1));
                            s.DeserializeClasses(reader.GetString(2));
                            schools.Add(s);
                        }
                }
            }


        }

        public List<School> FindByName(string name)
        {
            return schools.Where(school => school.Name == name).ToList();
        }

        public School? FindID(int id)
        {
            return schools.Find(school => school.Id == id);
        }

        public List<School> FindAll()
        {
            return schools;
        }

        public void Insert(School author)
        {
            schools.Add(author);
        }



        public void Update(School school)
        {
            schools.Where(s => s.Id == school.Id).ToList().ForEach(s => s = school);
        }

        public void Delete(int id)
        {
            schools.RemoveAll(school => school.Id == id);
        }

        public void Save()
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();

                foreach (School school in schools)
                {
                    if (school.Id is null)
                    {
                        using (SqliteCommand command = new SqliteCommand("INSERT INTO Schools (name, classes) VALUES (@name, @classes)", connection))
                        {
                            command.Parameters.AddWithValue("@name", school.Name);
                            command.Parameters.AddWithValue("@classes", school.SerializeClasses());
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqliteCommand command = new SqliteCommand("UPDATE Schools SET name = @name, classes = @classes WHERE school_id = @id", connection))
                        {
                            command.Parameters.AddWithValue("@name", school.Name);
                            command.Parameters.AddWithValue("@classes", school.SerializeClasses());
                            command.Parameters.AddWithValue("@id", school.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}