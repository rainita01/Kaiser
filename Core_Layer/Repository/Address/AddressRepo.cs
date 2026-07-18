using System.Data.SqlTypes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.AddressDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Core_Layer.Repository.Address;

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
            address.CityId = dto.CityId ?? address.CityId;
            address.ProvinceId = dto.ProvinceId ?? address.ProvinceId;
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

    public async Task<ActionResult> AddProvinceAsync(string name)
    {
        try
        {
            if (await context.Provinces.AnyAsync(e=>e.Name == name))
            {
                return ActionResult.Failed("استان با همچین نامی وجود دارد");

            }

            var provice = new Province() { Name = name };

            await context.Provinces.AddAsync(provice);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while adding province : {name}", name);
            return ActionResult.Failed(e.Message);
        }

       
    }

    public async Task<ActionResult> AddCityAsync(string name,int provinceId)
    {
        try
        {
            if (await context.Cities.AnyAsync(e => e.Name == name) && await context.Provinces.AnyAsync(s=>s.Id == provinceId))
            {
                return ActionResult.Failed("شهری با همچین نامی وجود دارد یا استان پیدا نشد");
            }

            var city = new City { Name = name,ProvinceId = provinceId};

            await context.Cities.AddAsync(city);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while adding city name:{name}, provinceId:{provinceId}", name,provinceId);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> DeleteCityAsync(int id)
    {
        try
        {
            var result = await context.Cities.Where(e => e.Id == id).ExecuteDeleteAsync();
            if (result == 0)
            {
                return ActionResult.Failed("شهر پیدا نشد");
            }
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while deleting city id: {id}", id);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> DeleteProvinceAsync(int id)
    {
        try
        {
            var result = await context.Provinces.Where(e => e.Id == id).ExecuteDeleteAsync();
            if (result == 0)
            {
                return ActionResult.Failed("استان پیدا نشد");
            }
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while deleting province id: {id}", id);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<AddressDto> FindAddressAsync(string userId, int addressId)
    {
        try
        {
            var address = await context.Addresses
                .Include(address => address.City)
                .Include(address => address.Province)
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
    public async Task<List<ProviceDto>> GetProvinceAsync()
    {
        try
        {
            return await context.Provinces.AsNoTracking()
                .ProjectTo<ProviceDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
          logger.LogError(e,"error while getting provinces");
            throw;
        }
    }

    public async Task<List<CityDto>> GetCitiesAsync()
    {
        try
        {
            return await context.Cities.AsNoTracking()
                .ProjectTo<CityDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "error while getting cities");
            throw;
        }
    }

    private async Task<Data_Layer.Entities.Address?> GetAddressByIdAsync(int id)
    {
        return await context.Addresses.FirstOrDefaultAsync(e => e.Id == id);
    }

}