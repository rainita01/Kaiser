using Core_Layer.Dtos.ContactUsDto;

namespace Core_Layer.Repository.ContactUs;

public interface IContactUsRepo
{
    public Task<ActionResult> Add(ContactUsDto contactUsDto);
    public Task<ActionResult> Remove(int id);
    public Task<List<ContactUsDto>> Get();
}