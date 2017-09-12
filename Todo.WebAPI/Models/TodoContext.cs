using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Todo.WebAPI.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext() :
            base("TodoContext")
        { }

        public DbSet<TodoUser> TodoUsers { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}