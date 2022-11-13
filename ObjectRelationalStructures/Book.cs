using System;

namespace ObjectRelationalStructures
{
    public struct Price
    {
        public decimal Value { get; set; }
        public string Currency { get; set; }

        public Price(decimal value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public override string ToString()
        {
            return $"{Value} {Currency}";
        }
    }

    public class Book
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        // NOTE!
        // Author isn't used with Dependent mapping
        public Author? Author { get; set; }
        public Price Price { get; set; }

        public Book(int? id, string title, Author? author, Price price)
        {
            Id = id;
            Title = title;
            Author = author;
            Price = price;
        }

        public override string ToString()
        {
            return $"Book: {Title}, [{Author}], {Price} ({Id})";
        }
    }
}