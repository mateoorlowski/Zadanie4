using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Repositories;

public interface IWarehouseRepository
{
    Task<int> CreateProductWarehouseRecord(ProductWarehouseDto request);
    Task<bool> IsProductIdExisting(int id);
    Task<bool> IsWarehouseIdExisting(int id);
    Task<int> IsOrderExisting(int id, int amount, DateTime createDate);
    Task<bool> IsOrderCompleted(int orderId);
    Task<bool> UpdateOrderFullfilledAt(int orderId, DateTime dateTime);
    Task<int> FulfillOrder(ProductWarehouseDto request, int orderId);
    Task<int> AddProductToWarehouseWithStoredProcedure(ProductWarehouseDto request);
}