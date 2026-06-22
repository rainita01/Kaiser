namespace Data_Layer.Entities;

public class ContactMessage :BaseEntity
{
    public string Fullname { get; set; }    
    public string Text { get; set; }
    public string  PhoneNumber { get; set; }
    public string Subject { get; set; }
    public DateTime SendTime { get; set; }  
}