using System;

namespace Todo.WebAPI.Models
{
    public class TodoSetDoneRequest : RequestBase
    {
        public Guid TodoId { get; set; }
    }
}