using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Todo.WebAPI.Models
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class TodoContext : DbContext
    {
        public TodoContext() :
            base("TodoContext")
        {
            Configuration.ValidateOnSaveEnabled = false;
        }

        static TodoContext()
        {
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
        }

        public DbSet<TodoUser> TodoUsers { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}