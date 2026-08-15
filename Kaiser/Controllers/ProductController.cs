
using Busines_Layer.Dtos.ImageDto;
using Busines_Layer.Dtos.Product;
using Busines_Layer.Dtos.ViewsDto;

using Busines_Layer.Repository.Image;
using Busines_Layer.Repository.Product;
using Busines_Layer.Repository.Visitors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

[ApiController]
public class ProductController(IProductRepo productRepo, IViewsRepo viewsRepo, IImageRepo imageRepo)
    : ControllerBase
{
    [HttpGet("Products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] int? pageSize,
        [FromQuery] decimal? minPrice,  
        [FromQuery] decimal? maxPrice,
        [FromQuery] SortProduct? sort,
        [FromQuery] int? categoryId,
        [FromQuery] bool? haveCost,
        [FromQuery] bool? isBestSale,
        [FromQuery] int page = 1)
    {
        var result = await productRepo.GetProductPagesAsync(page, pageSize, minPrice, maxPrice, sort, search, categoryId,isBestSale,haveCost);
        return Ok(result);
    }

    [HttpGet("Products/BestSales")]
    public async Task<IActionResult> GetBestSales(
        [FromQuery] int? pageSize,
        [FromQuery] int page = 1
    )
    {
        var products = await productRepo.GetBestSalesProducts(pageSize, page);
        return Ok(products);
    }
    [HttpGet("Products/Detail/{id}/{slug}")]
   
    public async Task<IActionResult> ProductDetail(int id, string slug)
    {
        var product = await productRepo.GetProductAsync(id);
        string? ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        await viewsRepo.AddAsync(new AddViewDto()
        {
            ProductId = id,
            ViewAt = DateTime.Now,
            SesstionId = Guid.NewGuid().ToString(),
            IpAddress = ipaddress
        });
        return Ok(product);
    }

    [HttpPost("ProductManager/add")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddProduct([FromForm] AddProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("مدل ناقص میباشد ");
        }

        var result = await productRepo.AddAsync(dto);
        if (result.Success)
        {
            return Created();
        }

        return BadRequest(result.Message);
    }

    [HttpDelete("ProductManager/Remove")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveProduct(int id)
    {
        var result = await productRepo.DeleteAsync(id);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

    [HttpGet("ProductManager/EditProduct")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await productRepo.GetUpdateProductAsync(id);
        
        return Ok(result);
    }


    [HttpPut("ProductManager/EditProduct")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit([FromForm] UpdateProductDto dto)
    {
        var result = await productRepo.UpdateAsync(dto);

        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

    [HttpDelete("ProductManager/EditProduct/RemoveImage")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var result = await imageRepo.RemoveAsync(id);

        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

    [HttpPost("ProductManager/EditProduct/AddImage")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteImage(AddImageDto dto)
    {
        if (dto.ProductId == null)
        {
            return BadRequest("ای دی محصول نال میباشد");
        }

        var result = await imageRepo.AddAsync(dto.Image, (int)dto.ProductId);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

   

 

}