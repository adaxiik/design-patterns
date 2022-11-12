using System.Collections.Generic;
using System;

using ObjectRelationalBehavior.DAO;

namespace ObjectRelationalBehavior.LazyLoad
{
    public class LazyCemetry
    {
        private List<PersonRowGateway>? people = null;

        private void CheckPeople()
        {
            if (people is null)
            {
                Console.WriteLine("Loading people from database");
                people = PersonFinder.FindAll();
            }

            Console.WriteLine("Returning people from memory");
        }

        public List<PersonRowGateway> GetAllDeadPeople()
        {
            CheckPeople();
            return people!;
        }

        public PersonRowGateway? GetDeadPersonByID(int id)
        {
            CheckPeople();

            foreach (var person in people!)
                if (person.Id == id)
                    return person;

                
            return null;
        }
    }
}