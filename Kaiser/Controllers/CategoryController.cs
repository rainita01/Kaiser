using Busines_Layer.Dtos.Category;
using Busines_Layer.Repository.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
public class CategoryController(ICategoryRepo categoryRepo) : ControllerBase
{

    [HttpGet("/CategoryManager")]
    [AllowAnonymous]
    public async Task<IActionResult> CategoryManager()
    {
        var categories = await categoryRepo.GetCategoriesAsync();
        return Ok(categories);
    }
    [HttpPost("CategoryManager/add")]
    
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto dto)
    {
        var result = await categoryRepo.AddAsync(dto);
        if (result.Success)
            return Created();
        return BadRequest();
        
    }
    [HttpDelete("CategoryManager/Remove/{id}")]
    public async Task<IActionResult> RemoveCategory(int id)
    {
        var result = await categoryRepo.RemoveAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest();
        
    }
    [HttpPut("CategoryManager/Update")]
    
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto dto)
    {
        var result = await categoryRepo.UpdateAsync(dto);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest();
        
    }

}