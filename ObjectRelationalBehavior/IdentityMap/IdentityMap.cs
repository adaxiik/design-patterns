using System.Collections.Generic;
using ObjectRelationalBehavior.DAO;
using System;

namespace ObjectRelationalBehavior.IdentityMap
{
    public class IdentityMap
    {
        private Dictionary<int, PersonRowGateway> people = new Dictionary<int, PersonRowGateway>();

        public PersonRowGateway? GetPerson(int id)
        {
            if (people.ContainsKey(id))
                return people[id];
            return null;
        }

        public void AddPerson(PersonRowGateway person)
        {
            if (!people.ContainsKey(person.Id))
                people.Add(person.Id, person);
        }
    }
}