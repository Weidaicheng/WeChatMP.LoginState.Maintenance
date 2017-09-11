using System;

namespace Todo.Model
{
    public class TodoRecord
    {
        public Guid Id { get; set; }
        public Guid TodoUserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool UseAlert { get; set; }
        public DateTime? AlertTime { get; set; }
        public int AlertBeforeMinutes { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
