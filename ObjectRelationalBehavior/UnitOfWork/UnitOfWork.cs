using System;
using System.Collections.Generic;
using ObjectRelationalBehavior.DAO;
namespace ObjectRelationalBehavior.UnitOfWork
{
    public class UnitOfWork
    {
        private List<Person> newRegistry = new List<Person>();
        private List<Person> dirtyRegistry = new List<Person>();
        private List<Person> deletedRegistry = new List<Person>();

        private static UnitOfWork? self = null;

        public static UnitOfWork GetInstance()
        {
            if (self == null)
                self = new UnitOfWork();
            return self;
        }
        private UnitOfWork()
        {
        }

        public void RegisterNew(Person person)
        {
            if(newRegistry.Contains(person))
                throw new Exception("Person already registered new");
            
            if(dirtyRegistry.Contains(person))
                throw new Exception("Person already registered dirty");
            
            if(deletedRegistry.Contains(person))
                throw new Exception("Person already registered deleted");
            
            newRegistry.Add(person);  
        }

        public void RegisterDirty(Person person)
        {
            if(!newRegistry.Contains(person) && !dirtyRegistry.Contains(person))
                dirtyRegistry.Add(person);
        }

        public void RegisterDeleted(Person person)
        {
            if(newRegistry.Contains(person))
            {
                newRegistry.Remove(person);
                return;
            }
            
            if(dirtyRegistry.Contains(person))
                dirtyRegistry.Remove(person);
            
            if(!deletedRegistry.Contains(person))
                deletedRegistry.Add(person);
        }

        public void Commit()
        {
            PersonGateway gateway = new PersonGateway();
            foreach (Person person in newRegistry)
                gateway.Insert(person);
            
            foreach (Person person in dirtyRegistry)
                gateway.Update(person);
            
            foreach (Person person in deletedRegistry)
                gateway.Remove(person);
            
            newRegistry.Clear();
            dirtyRegistry.Clear();
            deletedRegistry.Clear();
        }
    }
}