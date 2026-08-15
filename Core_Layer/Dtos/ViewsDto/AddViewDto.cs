namespace Busines_Layer.Dtos.ViewsDto;

public class AddViewDto
{
    public int ProductId { get; set; }
    public string SesstionId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime ViewAt { get; set; }    
}