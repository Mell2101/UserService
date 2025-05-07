using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DBConnection.Enum;

namespace DBConnection.Entity;

[Table("core_user")]
public class User
{
    [Column("guid_id")]
    public Guid GuidId { get; set; } = Guid.NewGuid();

    [Column("login")]
    public string Login { get; set; }

    [Column("password")]
    public string Password { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("gender")]
    public UserGenderEnum Gender { get; set; } = UserGenderEnum.Unknown; // По умолчанию "неизвестно"

    [Column("birthday")]
    public DateTime? Birthday { get; set; }

    [Column("admin")]
    public bool Admin { get; set; }

    [Column("created_on")]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string CreatedBy { get; set; }

    [Column("modified_on")]
    public DateTime ModifiedOn { get; set; }

    [Column("modified_by")]
    public string ModifiedBy { get; set; }

    [Column("revoked_on")]
    public DateTime? RevokedOn { get; set; }

    [Column("revoked_by")]
    public string? RevokedBy { get; set; }
}
