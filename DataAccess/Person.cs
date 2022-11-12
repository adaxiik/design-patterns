using System;

namespace DataAccess
{
    public class Person
    {
        public string  LastName {get;set;}
        public string  FirstName {get;set;}
        public decimal Balance {get;set;}

        public Person(string lastName, string firstName, decimal balance)
        {
            LastName = lastName;
            FirstName = firstName;
            Balance = balance;
        }

        public override string ToString()
        {
            return $"LN: {LastName}, FN: {FirstName} ({Balance})";
        }
    }
}