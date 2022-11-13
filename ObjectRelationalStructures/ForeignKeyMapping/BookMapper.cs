using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using System.Linq;
using DesignPatterns;

namespace ObjectRelationalStructures.ForeignKeyMapping
{
    public class BookMapper
    {
        private List<Book> books = new List<Book>();
        public bool Updated { get; set; } = false;
        private static BookMapper? instance = null;
        public static BookMapper GetInstance()
        {
            if (instance == null)
                instance = new BookMapper();
            return instance;
        }
        private BookMapper(){}

        public void Fetch()
        {
            var authorMapper = AuthorMapper.GetInstance();
            books.Clear();

            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("SELECT book_id, title, author_id, price, currency FROM Books", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            var author = authorMapper.FindID(reader.GetInt32(2));
                            if (author is null)
                                throw new Exception("Author not found");
                            books.Add(new Book(reader.GetInt32(0), reader.GetString(1), author, new Price(reader.GetDecimal(3), reader.GetString(4))));
                        }
                }
            }
            Updated = true;
        }

        public List<Book> FindByTitle(string title)
        {
            return books.Where(book => book.Title == title).ToList();
        }

        public List<Book> FindAll()
        {
            return books;
        }


        public Book? FindID(int id)
        {
            return books.Find(book => book.Id == id);
        }

        public void Insert(Book book)
        {
            books.Add(book);
            Updated = false;
        }

        public void Update(Book book)
        {
            books.Where(b => b.Id == book.Id).ToList().ForEach(b => b = book);
            Updated = false;
        }

        public void Delete(int id)
        {
            books.RemoveAll(book => book.Id == id);
            Updated = false;
        }

        public void Save()
        {
            if (!AuthorMapper.GetInstance().Updated)
                AuthorMapper.GetInstance().Save();

            using (SqliteConnection connection = new SqliteConnection(Config.ConnectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("DELETE FROM Books", connection))
                    command.ExecuteNonQuery();
                foreach (var book in books)
                {
                    if (book.Id is null)
                    {
                        AuthorMapper.GetInstance().InsertOrUpdate(book.Author);

                        using (SqliteCommand command = new SqliteCommand("INSERT INTO Books (title, author_id, price, currency) VALUES ($title, $author_id, $price, $currency)", connection))
                        {
                            command.Parameters.AddWithValue("$title", book.Title);
                            command.Parameters.AddWithValue("$author_id", book.Author.Id);
                            command.Parameters.AddWithValue("$price", book.Price.Value);
                            command.Parameters.AddWithValue("$currency", book.Price.Currency);
                            command.ExecuteNonQuery();
                        }

                    }
                    else
                    {
                        AuthorMapper.GetInstance().InsertOrUpdate(book.Author);

                        using (SqliteCommand command = new SqliteCommand("UPDATE Books SET title = $title, author_id = $author_id, price = $price, currency = $currency WHERE book_id = $book_id", connection))
                        {
                            command.Parameters.AddWithValue("$title", book.Title);
                            command.Parameters.AddWithValue("$author_id", book.Author.Id);
                            command.Parameters.AddWithValue("$price", book.Price.Value);
                            command.Parameters.AddWithValue("$currency", book.Price.Currency);
                            command.Parameters.AddWithValue("$book_id", book.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            this.Fetch();
        }
    }
}