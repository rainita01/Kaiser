namespace Core_Layer.Dtos.ContactUsDto;

public class ContactUsDto
{
    public int? Id { get; set; }
    public string Fullname { get; set; }
    public string Text { get; set; }
    public string PhoneNumber { get; set; }
    public string Subject { get; set; }
    public DateTime SendTime { get; set; } = DateTime.UtcNow;
}