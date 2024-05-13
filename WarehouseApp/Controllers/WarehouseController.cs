using System.Data.SqlClient;
using APBD_Zadanie_6.Models;
using APBD_Zadanie_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Zadanie_6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController(IWarehouseService _warehouseService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddProductWarehouse(ProductWarehouseDto productWarehouseDto)
    {
        int result = await _warehouseService.AddProductWarehouse(productWarehouseDto);
        if (result == -1)
        {
            return BadRequest("Invalid request parameters");
        }

        return Ok("ID of inserted row: " + result);
    }
    
    [HttpPost]
    [Route("sp")]
    public async Task<IActionResult> AddProductWarehouseByProcedure(ProductWarehouseDto productWarehouseDto)
    {
        try
        {
            Decimal result = await _warehouseService.AddProductWarehouseByProcedure(productWarehouseDto);
            return Ok("ID of inserted row: " + result);
        }
        catch (SqlException e)
        {
            return BadRequest("Invalid request parameters");
        }
    }
}