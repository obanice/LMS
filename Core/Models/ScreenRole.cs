using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ScreenRole
    {
        [Key]
        public int Id { get; set; }
        public string? RoleId { get; set; }
        public bool ScreenChecked { get; set; }
        public int? Priority { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
        public int? ScreenId { get; set; }

        [ForeignKey(nameof(ScreenId))]
        public virtual Screen? Screen { get; set; }
       
    }
}
