using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholarPrj_Back.Domain.Entities
{
    [Table("fs_users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string UserName { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        public string LastName { get; set; } = null!;

        [Column("gender")]
        public string Gender { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
