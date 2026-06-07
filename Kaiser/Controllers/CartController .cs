using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class CartController : ControllerBase
{
    [HttpGet("Cart")]
    public async Task<IActionResult> GetCart()
    {
        throw new Exception();
    }
    [HttpPost]
    public async Task<IActionResult> AddCartItem()  
    {
        throw new Exception();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveCartItem()
    {
        throw new Exception();
    }





}