using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Todo.WebAPI.Models
{
    public class TodoUser
    {
        [Key]
        public Guid TodoUserId { get; set; }
        public string OpenId { get; set; }
    }
}