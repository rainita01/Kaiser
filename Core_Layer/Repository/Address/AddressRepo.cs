using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.AddressDto;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;


namespace Core_Layer.Repository.Address;

public class AddressRepo(IMapper mapper,Context context) : IAddressRepo
{
    public async Task<ActionResult> AddAsync(AddAddressDto dto,string userId)
    {
        
        try
        {
            var address = mapper.Map<Data_Layer.Entities.Address>(dto);
            address.UserId = userId;
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> UpdateAsync(UpdateAddressDto dto)
    {
        try
        {
            var address = await GetAddressByIdAsync(dto.Id);
            if (address == null)
            {
                return ActionResult.Failed("ادرس پیدا نشد");
            }

            address.PhoneNumber = dto.PhoneNumber ?? address.PhoneNumber;
            address.City = dto.City ?? address.City;
            address.Province = dto.Province ?? address.Province;
            address.Firstname = dto.Firstname ?? address.Firstname;
            address.Lastname = dto.Lastname ?? address.Lastname;
            address.FullAddress = dto.FullAddress ?? address.FullAddress;
            address.PostCode = dto.PostCode ?? address.PostCode;
            await context.SaveChangesAsync();


            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }



    }

    public async Task<ActionResult> DeleteAsync(int id)
    {
        try
        {
            var address =await GetAddressByIdAsync(id);
            if (address == null)
                return ActionResult.Failed("ادرس پیدا نشد");
            context.Addresses.Remove(address);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public Task<ActionResult> AddAddressToOrderAsync(AddAddressToOrderDto id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AddressDto>?> GetUserAddresses(string userId)
    {
        return await context.Addresses
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .ProjectTo<AddressDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    private async Task<Data_Layer.Entities.Address?> GetAddressByIdAsync(int id)
    {
        return await context.Addresses.FirstOrDefaultAsync(e => e.Id == id);
    }
}