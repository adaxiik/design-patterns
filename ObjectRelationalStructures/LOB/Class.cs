using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectRelationalStructures.LOB
{
    public class Class
    {
        public string identificator { get; set; }
        public List<Student> Students { get; set; }

        public Class(string identificator)
        {
            this.identificator = identificator;
            Students = new List<Student>();
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append($"{identificator}/");
            sb.Append(Students.Count).Append("/");
            foreach (var student in Students)
                sb.Append($"{student.Serialize()}/");
            return sb.ToString();
        }
        public static Class Deserialize(string serialized)
        {
            var parts = serialized.Split('/');
            var class_ = new Class(parts[0]);
            var count = int.Parse(parts[1]);
            for (int i = 0; i < count; i++)
                class_.Students.Add(Student.Deserialize(parts[i + 2]));
            return class_;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{identificator} has students: ");

            foreach (var student in Students)
                sb.AppendLine($"\t\t {student}");

            return sb.ToString();
        }

    }
}