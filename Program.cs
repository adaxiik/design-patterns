using System;
using Microsoft.Data.Sqlite;
using DataAccess;
using DataAccess.TableDataGateway;
using DataAccess.RowDataGateway;
using DataAccess.ActiveRecord;
using DataAccess.DataMapper;

namespace DesignPatterns
{
    class Program
    {

        static void ClearDatabase(string table)
        {
            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand($"DELETE FROM {table}", connection))
                    command.ExecuteNonQuery();
                
                // reset autoincrement
                using (SqliteCommand command = new SqliteCommand($"UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='{table}'", connection))
                    command.ExecuteNonQuery();
                
            }
        }

        static void Main(string[] args)
        {
            ClearDatabase("People");

            // Table Data Gateway
            {
                var p0 = new Person("Silná", "Eliška", 1000);
                var p1 = new Person("Vláček", "Tomáš", 10);
                var p2 = new Person("Pták", "Martin", 20);
                var p3 = new Person("Doplachtil", "Ujo", 800);
                var p4 = new Person("Křup", "Robin", 6969);
                PersonGateway gateway = new PersonGateway();
                gateway.Insert(p0);
                gateway.Insert(p1);
                gateway.Insert(p2);
                gateway.Insert(p3);
                gateway.Insert(p4);

                // var p = gateway.Find(1);
                // Console.WriteLine(p?.FirstName); // id v db jsou autoincrement => při testování jsem vygeneroval spoustu id, takže nejsou od 1

                var people = gateway.FindByLastName("Doplachtil");
                foreach (var person in people)
                    Console.WriteLine(person);
            }

            // Row Data Gateway
            {
                PersonRowGateway prg1 = PersonFinder.FindByLastName("Vláček")[0];
                prg1.Balance = 420;
                prg1.Update();
                Console.WriteLine(prg1.GetPerson());
            }

            // Active Record
            {
                PersonActiveRecord par1 = PersonActiveRecord.FindByLastName("Křup")[0];
                par1.LastName = "Hňup";
                par1.Update();
                Console.WriteLine(par1);

                PersonActiveRecord par2 = new PersonActiveRecord("Banánová", "Amálie", 6969);
                par2.Insert();
                Console.WriteLine(par2);
            }

            // Data Mapper
            {
                PersonMapper mapper = new PersonMapper();
                mapper.Fetch();
                Person p = mapper.FindByLastName("Doplachtil")[0];
                p.FirstName = "Tonda";
                mapper.UpdateByLastName(p);
                mapper.Save();
            }

        }
    }
}
