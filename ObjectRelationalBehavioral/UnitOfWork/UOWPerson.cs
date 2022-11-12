using System;

namespace ObjectRelationalBehavioral.UnitOfWork
{
    public class UOWPerson : Person
    {

        public UOWPerson(int? id, string lastName, string firstName, decimal balance) : base(id, lastName, firstName, balance)
        {
            UnitOfWork.GetInstance().RegisterNew(this);
        }


        public void Delete()
        {
            UnitOfWork.GetInstance().RegisterDeleted(this);
        }

        public override void SetLastName(string lastName)
        {
            base.SetLastName(lastName);
            UnitOfWork.GetInstance().RegisterDirty(this);
        }

        public override void SetFirstName(string firstName)
        {
            base.SetFirstName(firstName);
            UnitOfWork.GetInstance().RegisterDirty(this);
        }

        public override void SetBalance(decimal balance)
        {
            base.SetBalance(balance);
            UnitOfWork.GetInstance().RegisterDirty(this);
        }

        public override void SetId(int? id)
        {
            base.SetId(id);
            UnitOfWork.GetInstance().RegisterDirty(this);
        }

    }

}