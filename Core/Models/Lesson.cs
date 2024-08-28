using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? VideoUrl { get; set; }
        public string? Content { get; set; }
        public int? ModuleId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public virtual Module? Module { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
