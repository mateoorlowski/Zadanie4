using APBD_Zadanie_6.Models;
using APBD_Zadanie_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Zadanie_6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController(IWarehouseService warehouseService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductWarehouseDto product)
    {
        try
        {
            var idProductWarehouse = await warehouseService.AddProduct(product);
            return Ok(idProductWarehouse);
        }
        catch (Exception e)
        {
            return BadRequest(e.StackTrace);
        }
    }
}