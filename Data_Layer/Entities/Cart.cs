namespace Data_Layer.Entities;

public class Cart : BaseEntity  
{


    #region Relations

    public string UserId { get; set; }
    public User User { get; set; }
    public List<CartItem>? CartItems { get; set; }   

    #endregion
}