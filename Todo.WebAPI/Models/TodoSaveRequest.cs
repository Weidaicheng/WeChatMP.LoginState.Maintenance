using System;

namespace Todo.WebAPI.Models
{
    public class TodoSaveRequest : RequestBase
    {
        public Guid? TodoId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool UseAlert { get; set; }
        public DateTime? AlertTime { get; set; }
        public int AlertBeforeMinutes { get; set; }
        public string FormId { get; set; }
    }
}