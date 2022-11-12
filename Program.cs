using System;
using Microsoft.Data.Sqlite;

// Data Access
using DataAccess.TableDataGateway;
using DataAccess.RowDataGateway;
using DataAccess.ActiveRecord;
using DataAccess.DataMapper;

// Object Relational Behavioral
using ObjectRelationalBehavior.UnitOfWork;
using ObjectRelationalBehavior.IdentityMap;
using ObjectRelationalBehavior.LazyLoad;


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
        static void DataAccessDemo()
        {
            ClearDatabase("People");
            Console.WriteLine("Data Access: ");
            // Table Data Gateway
            {
                var p0 = new DataAccess.Person("Silná", "Eliška", 1000);
                var p1 = new DataAccess.Person("Vláček", "Tomáš", 10);
                var p2 = new DataAccess.Person("Pták", "Martin", 20);
                var p3 = new DataAccess.Person("Doplachtil", "Ujo", 800);
                var p4 = new DataAccess.Person("Křup", "Robin", 6969);
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
                DataAccess.Person p = mapper.FindByLastName("Doplachtil")[0];
                p.FirstName = "Tonda";
                mapper.UpdateByLastName(p);
                mapper.Save();
            }
        }

        static void ObjectRelationalBehavioralDemo()
        {
            ClearDatabase("People");
            Console.WriteLine("Object Relational Behavioral: ");
            // Unit of Work
            {
                UnitOfWork uow = UnitOfWork.GetInstance();
                var p1 = new UOWPerson(null, "Křup", "Robin", 6969);
                var p2 = new UOWPerson(null, "Doplachtil", "Ujo", 800);
                var p3 = new UOWPerson(null, "Vláček", "Tomáš", 10);
                var p4 = new UOWPerson(null, "Pták", "Martin", 20);
                var p5 = new UOWPerson(null, "Silná", "Eliška", 1000);
                uow.Commit();
                Console.WriteLine("Database after first commit:");
                foreach (var person in new ObjectRelationalBehavior.DAO.PersonGateway().FindAll())
                    Console.WriteLine(person);


                p1.SetFirstName("Robinette");
                p2.SetBalance(420);
                p4.Delete();
                uow.Commit();

                Console.WriteLine();
                Console.WriteLine("Database after second commit:");
                foreach (var person in new ObjectRelationalBehavior.DAO.PersonGateway().FindAll())
                    Console.WriteLine(person);
            }
            Console.WriteLine();

            // Identity Map
            {
                // Debug texts are in FindById method
                var p1 = IMPersonFinder.FindById(3); // Tomáš Vláček
                var p2 = IMPersonFinder.FindById(1); // Eliška Silná
                var p3 = IMPersonFinder.FindById(3); // Tomáš Vláček
                p3!.Balance = 1_000_000;
                p3!.Update();
            }
            Console.WriteLine();

            // Lazy load initialization
            {
                var cemetry = new LazyCemetry();
                var person = cemetry.GetDeadPersonByID(1)!;
                person.Balance = 0;
                person.Update();
                Console.WriteLine(person.GetPerson());

            }
        }


        static void Main(string[] args)
        {

            
            // DataAccessDemo();
            ObjectRelationalBehavioralDemo();



        }
    }
}
