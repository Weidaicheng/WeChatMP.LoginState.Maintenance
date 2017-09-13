using System;

namespace Todo.WebAPI.Models
{
    public class TodoAddRequest : RequestBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool UseAlert { get; set; }
        public DateTime? AlertTime { get; set; }
        public int AlertBeforeMinutes { get; set; }
    }
}