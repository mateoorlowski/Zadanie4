using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Repositories;

public interface IWarehouseRepository
{
    Task<int> GetOrderIdByProductWarehouse(Order order);
    Task<decimal> GetPriceByProductId(int idProduct);
    Task<bool> CheckIfWarehouseExists(int idWarehouse);
    Task AddProductToWarehouse(ProductWarehouse productWarehouse);
    Task AddProductToWarehouseByProcedure(ProductWarehouseDto productWarehouseDto);
    Task<int> GetLastCreatedProductWarehouseId();
}