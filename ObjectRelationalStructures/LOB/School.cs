using System.Collections.Generic;
using System;
using System.Text;

namespace ObjectRelationalStructures.LOB
{
    public class School
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<Class> Classes { get; set; }

        public School(int? id, string name)
        {
            Id = id;
            Name = name;
            Classes = new List<Class>();
        }

        public string SerializeClasses()
        {
            var sb = new StringBuilder();
            sb.Append(Classes.Count).Append("^");
            foreach (var class_ in Classes)
                sb.Append($"{class_.Serialize()}^");
            return sb.ToString();
        }

        public void DeserializeClasses(string serialized)
        {
            var parts = serialized.Split('^');
            var count = int.Parse(parts[0]);
            for (int i = 0; i < count; i++)
                Classes.Add(Class.Deserialize(parts[i + 1]));

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Name} has classes: ");

            foreach (var class_ in Classes)
                sb.AppendLine($"\t {class_}");
                
            return sb.ToString();
        }
    }
}