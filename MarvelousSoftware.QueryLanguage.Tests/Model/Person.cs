using System;

namespace MarvelousSoftware.QueryLanguage.Tests.Model
{
    public class Person : IEquatable<Person>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int S { get; set; }

        public bool Equals(Person p)
        {
            if (p == null)
            {
                return false;
            }
            return p.FirstName == FirstName && p.LastName == LastName && p.S == S;
        }

        public override bool Equals(object obj)
        {
            var p = obj as Person;
            return Equals(p);
        }
    }
}
