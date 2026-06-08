using Core_Layer.Dtos.Category;
using Core_Layer.Repository.Category;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

[ApiController]
public class CategoryController(ICategoryRepo categoryRepo) : ControllerBase
{

    [HttpGet("/CategoryManager")]
    public async Task<IActionResult> CategoryManager()
    {
        var categories = await categoryRepo.GetCategoriesAsync();
        return Ok(categories);
    }
    [HttpPost("CategoryManager/add")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto dto)
    {
        var result = await categoryRepo.AddAsync(dto);
        if (result.Success)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
    [HttpDelete("CategoryManager/Remove/{id}")]
    public async Task<IActionResult> RemoveCategory(int id)
    {
        var result = await categoryRepo.RemoveAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
    [HttpPut("CategoryManager/Update")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto dto)
    {
        var result = await categoryRepo.UpdateAsync(dto);
        if (result.Success)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

}