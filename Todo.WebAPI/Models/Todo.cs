using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Todo.WebAPI.Models
{
    public class Todo
    {
        [Key]
        public Guid TodoId { get; set; }
        public TodoUser TodoUser { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool UseAlert { get; set; }
        public DateTime? AlertTime { get; set; }
        public int AlertBeforeMinutes { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string FormId { get; set; }
        public bool IsDone { get; set; }
    }
}