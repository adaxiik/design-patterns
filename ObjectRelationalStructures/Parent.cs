using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectRelationalStructures
{
    public class Parent
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
         public List<Child> Children { get; set; } = new List<Child>();

        public Parent(int? id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public Parent AddChild(Child child)
        {
            Children.Add(child);
            return this;
        }

        public Parent AddChildren(List<Child> children)
        {
            foreach (var child in children)
                AddChild(child);
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{FirstName} {LastName} has children: ");
            foreach (var child in Children)
                sb.Append($"{child.FirstName} {child.LastName}, ");
            return sb.ToString();
        }
    }
}