using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Services;

public interface IWarehouseService
{
    Task<int> AddProductWarehouse(ProductWarehouseDto productWarehouseDto);
    Task<int> AddProductWarehouseByProcedure(ProductWarehouseDto product);
}