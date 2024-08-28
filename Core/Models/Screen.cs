using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Screen
    {
        public Screen()
        {
            Active = true;
            DateCreated = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Class { get; set; }
        public string? Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Active { get; set; }
    }
}
