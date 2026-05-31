using Microsoft.AspNetCore.Identity;

namespace Data_Layer.Entities;

public class Role :IdentityRole
{
    public List<User> Users { get; set; }   
}