using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Model;

namespace Todo.DAL
{
    public class TodoContext : DbContext
    {
        public TodoContext() :
            base("TodoContext")
        { }

        public DbSet<TodoUser> TodoUsers { get; set; }
        public DbSet<TodoRecord> TodoRecords { get; set; }
    }
}
