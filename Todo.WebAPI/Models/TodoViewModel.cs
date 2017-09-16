using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Todo.WebAPI.Models
{
    public class TodoViewModel
    {
        public Guid TodoId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool UseAlert { get; set; }
        public string AlertDate { get; set; }
        public string AlertTime { get; set; }
        public int AlertBeforeMinutes { get; set; }
    }
}