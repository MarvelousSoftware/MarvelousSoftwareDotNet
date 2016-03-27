using System;

namespace MarvelousSoftware.Examples.Samples
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfResidence { get; set; }
        public int ProjectId { get; set; }
        public DateTime DueDate { get; set; }
        public int? Rating { get; set; }

        private string _userName;

        public string UserName
        {
            get { return _userName ?? FirstName[0] + LastName; }
            set { _userName = value; }
        }

        public int Age => DateTime.UtcNow.Year - DateOfBirth.Year;
    }
}