using System;
using Microsoft.Data.Sqlite;

// Data Access
using DataAccess.TableDataGateway;
using DataAccess.RowDataGateway;
using DataAccess.ActiveRecord;
using DataAccess.DataMapper;

// Object Relational Behavior
using ObjectRelationalBehavior.UnitOfWork;
using ObjectRelationalBehavior.IdentityMap;
using ObjectRelationalBehavior.LazyLoad;

// Object Relational Structures
using ObjectRelationalStructures;
using ObjectRelationalStructures.ForeignKeyMapping;
using ObjectRelationalStructures.AssociationTableMapping;
using ObjectRelationalStructures.DependentMapping;


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
                prg1.GetPerson().Balance = 420;
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
        static void ObjectRelationalBehaviorDemo()
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
        // NOTE!
        // Foreign key mapping obsahuje i ukázku embedded value (viz Book.cs Price) a identification field 
        static void ObjectRelationalStructuresDemo()
        {
            ClearDatabase("Authors");
            ClearDatabase("Books");
            ClearDatabase("Parents");
            ClearDatabase("Children");
            ClearDatabase("ParentChild");

            Console.WriteLine("Object Relational Behavioral: ");

            // Foreign key mapping
            {
                // NOTE!
                // author.Books is unused here .. 
                // its used later with dependent mapping
                var authorMapper = ObjectRelationalStructures.ForeignKeyMapping.AuthorMapper.GetInstance();
                authorMapper.Fetch();
                authorMapper.Insert(new Author(null, "Jack", "Kerouac"));
                authorMapper.Insert(new Author(null, "Arthur", "Rimbaud"));
                authorMapper.Insert(new Author(null, "Charles", "Baudelaire"));
                authorMapper.Insert(new Author(null, "Antoine", "de Saint-Exupéry"));
                authorMapper.Save();

                authorMapper.FindID(2)!.FirstName = "Arthurek";
                authorMapper.Save();

                var bookMapper = ObjectRelationalStructures.ForeignKeyMapping.BookMapper.GetInstance();
                bookMapper.Fetch();
                bookMapper.Insert(new Book(null, "On the road", authorMapper.FindByLastName("Kerouac")[0], new Price(100, "CZK")));
                bookMapper.Insert(new Book(null, "Flowers of Evil", authorMapper.FindByLastName("Baudelaire")[0], new Price(200, "CZK")));
                bookMapper.Insert(new Book(null, "The Little Prince", authorMapper.FindByLastName("de Saint-Exupéry")[0], new Price(300, "CZK")));
                bookMapper.Save();

                Console.WriteLine("Authors saved to database: ");
                foreach (var author in authorMapper.FindAll())
                    Console.WriteLine(author);

                Console.WriteLine();
                Console.WriteLine("Books saved to database: ");
                foreach (var book in bookMapper.FindAll())
                    Console.WriteLine(book);
            }

            // Association table mapping
            {
                ParentMapper parentMapper = ParentMapper.GetInstance();
                parentMapper.Fetch();

                var p1 = new Parent(null, "Tomáš", "Vláček");
                var p2 = new Parent(null, "Eliška", "Vláčková");

                parentMapper.Insert(p1);
                parentMapper.Insert(p2);
                parentMapper.Save();

                ChildMapper childMapper = ChildMapper.GetInstance();
                childMapper.Fetch();
                childMapper.Insert(new Child(null, "Robin", "Vláček", 10));
                childMapper.Insert(new Child(null, "Ujo", "Vláček", 8));
                childMapper.Insert(new Child(null, "Amálie", "Vláčková", 5));
                childMapper.Save();

                parentMapper.FindByLastName("Vláček")[0].AddChildren(childMapper.FindByLastname("Vláček"))
                                                        .AddChildren(childMapper.FindByLastname("Vláčková"));

                parentMapper.FindByLastName("Vláčková")[0].AddChildren(childMapper.FindByLastname("Vláček"))
                                                          .AddChildren(childMapper.FindByLastname("Vláčková"));
                parentMapper.Save();

                parentMapper.FindByLastName("Vláček")[0].Children[0].FirstName = "Robinette";
                parentMapper.Save();

                Console.WriteLine();
                Console.WriteLine("Parents saved to database: ");
                foreach (var parent in parentMapper.FindAll())
                    Console.WriteLine(parent);

            }
            Console.WriteLine();

            //Dependent mapping
            {
                var authorMapper = ObjectRelationalStructures.DependentMapping.AuthorMapper.GetInstance();
                authorMapper.Fetch(); // authors are already in database from Foreign key mapping demo

                Author exupery = authorMapper.FindByLastName("de Saint-Exupéry")[0];
                // NOTE!
                // book.Author is unused with dependent mapping
                exupery.AddBook(new Book(null, "Night Flight", null, new Price(4, "EUR")));

                authorMapper.Save();


                Console.WriteLine("Authors in database: ");
                foreach (var author in authorMapper.FindAll())
                {
                    Console.WriteLine(author);
                    foreach (var book in author.Books)
                        Console.WriteLine("\t" + book);
                }

            }




        }
        static void Main(string[] args)
        {


            // DataAccessDemo();
            // ObjectRelationalBehaviorDemo();
            ObjectRelationalStructuresDemo();



        }
    }
}
