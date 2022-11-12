using System;

namespace ObjectRelationalBehavioral
{
    public class Person
    {
        private int? id;
        private string  lastName;
        private string  firstName;
        private decimal balance; 

        public Person(int? id, string lastName, string firstName, decimal balance)
        {
            this.id = id;
            this.lastName = lastName;
            this.firstName = firstName;
            this.balance = balance;
        }

        public override string ToString()
        {
            return $"LN: {lastName}, FN: {firstName} ({balance}) [{id}]";
        }

        public int? GetId() => id;
        public string GetLastName() => lastName;
        public string GetFirstName() => firstName;
        public decimal GetBalance() => balance;

        public virtual void SetLastName(string lastName) => this.lastName = lastName;
        public virtual void SetFirstName(string firstName) => this.firstName = firstName;
        public virtual void SetBalance(decimal balance) => this.balance = balance;
        public virtual void SetId(int? id) => this.id = id;
    }
}