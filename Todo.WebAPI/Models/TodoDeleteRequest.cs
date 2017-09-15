using System;

namespace Todo.WebAPI.Models
{
    public class TodoDeleteRequest : RequestBase
    {
        public Guid TodoId { get; set; }
    }
}