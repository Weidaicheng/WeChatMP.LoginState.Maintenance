using System;

namespace Todo.Model
{
    public class TodoUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
    }
}
