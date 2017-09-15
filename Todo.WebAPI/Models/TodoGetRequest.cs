using System;

namespace Todo.WebAPI.Models
{
    public class TodoGetRequest : RequestBase
    {
        public Guid TodoId { get; set; }
    }
}