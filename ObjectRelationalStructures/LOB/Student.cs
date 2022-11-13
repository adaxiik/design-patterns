using System;

namespace ObjectRelationalStructures.LOB
{
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Student(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string Serialize()
        {
            return $"{FirstName};{LastName}";
        }
        public static Student Deserialize(string serialized)
        {
            var parts = serialized.Split(';');
            return new Student(parts[0], parts[1]);
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}