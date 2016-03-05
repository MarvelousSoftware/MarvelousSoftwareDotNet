using System;
using System.Collections.Generic;
using System.Linq;

namespace MarvelousSoftware.Examples.Samples
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private static readonly Lazy<IQueryable<Project>> _projects = new Lazy<IQueryable<Project>>(() => new List<Project>()
        {
            new Project {Id = 1, Name = "Athena"},
            new Project {Id = 2, Name = "Project X"},
            new Project {Id = 3, Name = "Reality"},
            new Project {Id = 4, Name = "Foobar"}
        }.AsQueryable());

        public static IQueryable<Project> GetAll()
        {
            return _projects.Value;
        } 
    }
}