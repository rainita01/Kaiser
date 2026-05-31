using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Payment
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string TransactionId { get; set; }
    public string PaymentMethod { get; set; }   
    public PaymentStatus Status { get; set; }


    #region Relations
    [Required]
    public Order Order { get; set; }

    #endregion
}

public enum PaymentStatus
{
    Success,
    Failed,
    Refunded

}