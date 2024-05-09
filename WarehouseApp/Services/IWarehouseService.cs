using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Services;

public interface IWarehouseService
{
    Task<int> AddProduct(ProductWarehouseDto productWarehouseDto);
    Task<int> AddProductByProcedure(ProductWarehouseDto product);
}