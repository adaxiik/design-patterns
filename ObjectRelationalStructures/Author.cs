using System;

namespace ObjectRelationalStructures
{
    public class Author
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

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
    }
}