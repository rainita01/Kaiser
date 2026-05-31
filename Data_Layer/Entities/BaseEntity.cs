using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
    public bool IsDeleted { get; set; }    = false;

}