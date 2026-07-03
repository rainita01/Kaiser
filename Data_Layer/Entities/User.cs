using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Data_Layer.Entities;

public class User :IdentityUser
{
    [PersonalData]
    [MaxLength(250)]
    public string? FirstName { get; set; }
    [PersonalData]
    [MaxLength(250)]
    public string? LastName { get; set; }

    #region Relations
        
    public List<Role>? Roles { get; set; }   
    public List<Comment>? Comments { get; set; }
    public List<Order>? Orders { get; set; }
    public List<Cart>? Carts { get; set; }
    public List<Address>? Addresses { get; set; }

    public List<SnapShot> SnapShots { get; set; }
    #endregion
}