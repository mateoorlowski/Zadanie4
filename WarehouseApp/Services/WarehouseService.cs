using System.Data;
using APBD_Zadanie_6.Models;
using APBD_Zadanie_6.Repositories;

namespace APBD_Zadanie_6.Services;

public class WarehouseService(IWarehouseRepository warehouseRepository) : IWarehouseService
{
    public async Task<int> AddProduct(ProductWarehouseDto productWarehouseDto)
    {
        var order = new Order
        {
            IdProduct = productWarehouseDto.IdProduct,
            Amount = productWarehouseDto.Amount,
            CreatedAt = productWarehouseDto.CreatedAt
        };

        var idOrder = await warehouseRepository.GetOrderIdByProductWarehouse(order);
        var price = await warehouseRepository.GetPriceByProductId(productWarehouseDto.IdProduct);
        var warehouseExists = await warehouseRepository.CheckIfWarehouseExists(productWarehouseDto.IdWarehouse);

        if (!warehouseExists) throw new DataException("Warehouse with given id does not exist");

        var productWarehouse = new ProductWarehouse
        {
            IdProduct = productWarehouseDto.IdProduct,
            IdWarehouse = productWarehouseDto.IdWarehouse,
            IdOrder = idOrder,
            Amount = productWarehouseDto.Amount,
            Price = productWarehouseDto.Amount * price,
            CreatedAt = productWarehouseDto.CreatedAt
        };

        await warehouseRepository.AddProductToWarehouse(productWarehouse);
        return await warehouseRepository.GetLastCreatedProductWarehouseId();
    }

    public async Task<int> AddProductByProcedure(ProductWarehouseDto product)
    {
        await warehouseRepository.AddProductToWarehouseByProcedure(product);
        return await warehouseRepository.GetLastCreatedProductWarehouseId();
    }
}