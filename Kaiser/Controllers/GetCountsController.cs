using Core_Layer.Services.GetServices;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

[Route("GetCounts")]
public class GetCountsController(IGetCountServices getCountServices) :ControllerBase
{
    [HttpGet("AllCounts")]
    public async Task<IActionResult> GetAllCounts()
    {
        var productsCount = await getCountServices.GetProductsCountAsync();
        var bestSaleProductsCount = await getCountServices.GetProductsWithBestSalesCountAsync();
        var discountProductsCount = await getCountServices.GetProductsWithDiscountCountAsync();
        var commentsCount = await getCountServices.GetCommentsCountAsync();
        var approvedCommentsCount = await getCountServices.GetApprovedCommentsCountAsync();
        var usersCount =await getCountServices.GetUsersCountAsync();
        var categoryCounts = await getCountServices.GetCategoryCountAsync();
        var messageCounts = await getCountServices.GetMessagesCountAsync();
        var getVisitsCounts = await getCountServices.GetVisitsCountAsync();


        return Ok(new
        {
             productsCount,
             bestSaleProductsCount,
             discountProductsCount,
             commentsCount,
             approvedCommentsCount,
             usersCount,
             categoryCounts,
             messageCounts,
             getVisitsCounts


        });
    }
}