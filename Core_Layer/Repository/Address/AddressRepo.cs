using System.Data.SqlTypes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Busines_Layer.Dtos.AddressDto;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Busines_Layer.Repository.Address;

public class AddressRepo(ILogger<AddressRepo> logger,IMapper mapper,Context context) : IAddressRepo
{
    public async Task<ActionResult> AddAsync(AddAddressDto dto,string userId)
    {
        
        try
        {
            var address = mapper.Map<Data_Layer.Entities.Address>(dto);
            address.UserId = userId; 
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            logger.LogInformation("user {userId} address added successfully",userId);
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "user {UserId} tried to add address {@AddressDto} but failed", userId, dto);
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
            address.city_code = dto.city_code ?? address.city_code;
            address.city_name = dto.city_name ?? address.city_name;
            address.province_code = dto.province_code ?? address.province_code;
            address.province_name = dto.province_name ?? address.province_name;
            address.Firstname = dto.Firstname ?? address.Firstname;
            address.Lastname = dto.Lastname ?? address.Lastname;
            address.FullAddress = dto.FullAddress ?? address.FullAddress;
            address.PostCode = dto.PostCode ?? address.PostCode;
            await context.SaveChangesAsync();


            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"Error while updating address {@dto}",dto);
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
            logger.LogError(e, "Error while deleting address: {id}", id);
            return ActionResult.Failed(e.Message);
        }
    }

    public Task<ActionResult> AddAddressToOrderAsync(AddAddressToOrderDto id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AddressDto>?> GetUserAddresses(string userId)
    {
        try
        {
            return await context.Addresses
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .ProjectTo<AddressDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while get user addresses user: {userId}",userId);
            throw;
        }
    }

    public async Task<AddressDto> FindAddressAsync(string userId, int addressId)
    {
        try
        {
            var address = await context.Addresses
                .ProjectTo<AddressDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);
            if (address == null)
                throw new SqlNullValueException("ادرس پیدا نشد");
            return address;
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while finding address:{address} for user:{user}",addressId,userId);
            throw;
        }
    }
    private async Task<Data_Layer.Entities.Address?> GetAddressByIdAsync(int id)
    {
        return await context.Addresses.FirstOrDefaultAsync(e => e.Id == id);
    }

}