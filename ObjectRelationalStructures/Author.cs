using System;
using System.Collections.Generic;

namespace ObjectRelationalStructures
{
    public class Author
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // NOTE!
        // This is used only with Dependent mapping
        public List<Book> Books { get; set; } = new List<Book>();

        public Author(int? id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return $"Author: FN: {FirstName}, LN: {LastName} ({Id})";
        }

        // NOTE!
        // This is used only with Dependent mapping
        public void AddBook(Book book)
        {
            Books.Add(book);
        }
    }
}