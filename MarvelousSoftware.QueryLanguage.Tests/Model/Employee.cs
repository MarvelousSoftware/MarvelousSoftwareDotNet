using System;
using Newtonsoft.Json;

namespace MarvelousSoftware.QueryLanguage.Tests.Model
{
    public class Employee : IEquatable<Employee>
    {
        public decimal? Salary { get; set; }
        public Person Person { get; set; }
        public DateTime? Created { get; set; }
        public int FunLevel { get; set; }
        public int TestCase { get; set; }
        public int TestCase2 { get; set; }
        public bool HasPersonInfo => Person != null;
        public bool? IsAwesome { get; set; }
        public string Foo { get; set; }

        public string FirstName => Person?.FirstName;

        public string LastName => Person?.LastName;

        public bool Equals(Employee e)
        {
            if (e == null)
            {
                return false;
            }

            if (e.Person == null && Person != null)
            {
                return false;
            }

            if (e.Person != null && e.Person.Equals(Person) == false)
            {
                return false;
            }

            return e.Salary == Salary;
        }

        public override bool Equals(object obj)
        {
            var e = obj as Employee;
            return Equals(e);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Salary.GetHashCode() * 397) ^ (Person != null ? Person.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}