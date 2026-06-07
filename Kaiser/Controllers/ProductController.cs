using Core_Layer.Dtos.ImageDto;
using Core_Layer.Dtos.Product;
using Core_Layer.Dtos.ViewsDto;
using Core_Layer.Repository.Image;
using Core_Layer.Repository.Product;
using Core_Layer.Repository.Visitors;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class ProductController(IProductRepo productRepo,IViewsRepo viewsRepo) : ControllerBase
{
    [HttpGet("Products")]
    public async Task<IActionResult> GetProducts([FromQuery] string? search = null, [FromQuery] int? categoryId = null, [FromQuery] int pageNumber = 1)
    {
        List<ProductCardDto> products;

        if (!string.IsNullOrEmpty(search))
        {
            products = await productRepo.GetProductPagesAsync(pageNumber, search);
        }
        else if (categoryId.HasValue && categoryId.Value > 0)
        {
            products = await productRepo.GetProductPagesAsync(pageNumber, categoryId.Value);
        }
        else
        {
            products = await productRepo.GetProductPagesAsync(pageNumber);
        }

        return Ok(products);
    }

    [HttpGet("Products/Detail/{id}/{slug}")]
    public async Task<IActionResult> ProductDetail(int id, string slug)
    {
        var product = await productRepo.GetProductAsync(id);

        await viewsRepo.AddAsync(new AddViewDto()
        {
            ProductId = id,
            ViewAt = DateTime.Now,
            SesstionId = HttpContext.Session.Id,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
        return Ok(product);
    }
    [HttpPost("ProductManager/add")]
    public async Task<IActionResult> AddProduct(AddProductDto dto)
    {   
        var result = await productRepo.AddAsync(dto);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest();
    }
    [HttpDelete("ProductManager/Remove")]
    public async Task<IActionResult> RemoveProduct(int id)
    {
        var result = await productRepo.DeleteAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpPut("ProductManager/EditProduct")]
    public async Task<IActionResult> Edit(UpdateProductDto dto)
    {
        var result = await productRepo.UpdateAsync(dto);

        if (result.Success)
        {
            return Ok();
        }
        return BadRequest();
    }

    

}