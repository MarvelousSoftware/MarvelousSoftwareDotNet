using System;
using System.Collections.Generic;
using System.Linq;

namespace MarvelousSoftware.QueryLanguage.Tests.Model
{
    public static class DataSample
    {
        public static IQueryable<Employee> GetEmployees()
        {
            return new List<Employee>
            {
                new Employee
                {
                    Salary = 25000, 
                    Created = new DateTime(2015, 1, 25, 10, 10, 10),
                    Person = new Person {FirstName = "Damian", LastName = "Kaminski"},
                    IsAwesome = true,
                    TestCase = 100
                },
                new Employee
                {
                    Salary = 2000,
                    Created = new DateTime(2015, 3, 2),
                    Person = new Person {FirstName = "Jan", LastName = "Kowalski"},
                    IsAwesome = false,
                    TestCase = 100
                },
                new Employee
                {
                    Salary = 6000,
                    Created = new DateTime(2015, 2, 15),
                    Person = new Person {FirstName = "Dariusz", LastName = "Kaminski"},
                    IsAwesome = true,
                    TestCase = 200,
                    Foo = "bar"
                },
                new Employee
                {
                    Salary = 14000, 
                    Person = new Person {FirstName = "Damian", LastName = "Kaminski"},
                    TestCase = 300,
                    Foo = "test"
                },
                new Employee
                {
                    Salary = 14000,
                    Person = new Person {FirstName = "Damian Dariusz", LastName = "Kaminski"},
                    TestCase = 300
                },
                new Employee
                {
                    Created = new DateTime(2015, 8, 5),
                    Salary = 5000.5M,
                    IsAwesome = true
                },
                new Employee
                {
                    Salary = 1200, 
                    Person = new Person {LastName = "Marian"}
                },
                new Employee
                {
                    Salary = 3000,
                    Created = new DateTime(2015, 1, 24),
                    Person = new Person {FirstName = "Łukasz", LastName = "Boczek"}
                },
                new Employee
                {
                    Salary = 121000, 
                },
                new Employee
                {
                    Created = new DateTime(2015, 1, 25, 10, 10, 0),
                    Salary = 19000, 
                },
                new Employee()
                {
                    Person = new Person(),
                    Salary = 5000
                },
                new Employee
                {
                    Created = new DateTime(2015, 1, 25),
                    Person = new Person {LastName = "Kaminski"}
                },
                new Employee
                {
                    Created = new DateTime(2015, 10, 28),
                    Person = new Person {FirstName = "Damian"},
                    IsAwesome = false
                },
                new Employee
                {
                    Person = new Person {FirstName = "Zdzislaw", LastName = "Krecina"}
                },
                new Employee
                {
                    Person = new Person {FirstName = "", LastName = ""}
                }
            }.AsQueryable();
        } 
    }
}