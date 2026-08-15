using Data_Layer.Entities;

namespace Busines_Layer.Dtos.PaymentDto;

public class CheckoutDto
{
    public double ShippingPrice { get; set; }   
    public double ProductsPrice { get; set; }
    public double TotalPrice => ProductsPrice + ShippingPrice;
    public SnapShot Snapshot { get; set; }    
}