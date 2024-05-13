using APBD_Zadanie_6.Models;
using APBD_Zadanie_6.Repositories;

namespace APBD_Zadanie_6.Services;

public class WarehouseService(IWarehouseRepository _warehouseRepository) : IWarehouseService
{
    public async Task<int> AddProductWarehouse(ProductWarehouseDto productWarehouseDto)
    {
        if(!await ValidateRequest(productWarehouseDto))
        {
            return -1;
        }

        Task<int> orderId = _warehouseRepository.IsOrderExisting(productWarehouseDto.IdProduct,
            productWarehouseDto.Amount, productWarehouseDto.CreatedAt);
        Task<bool> isOrderCompleted = _warehouseRepository.IsOrderCompleted(await orderId);

        if (await isOrderCompleted)
        {
            return -1;
        }

        return await _warehouseRepository.FulfillOrder(productWarehouseDto, await orderId);
    }

    public Task<int> AddProductWarehouseByProcedure(ProductWarehouseDto productWarehouseDto)
    {
        return _warehouseRepository.AddProductToWarehouseWithStoredProcedure(productWarehouseDto);
    }
    
    private async Task<bool> ValidateRequest(ProductWarehouseDto dto)
    {
        Task<bool> isProductExisting = _warehouseRepository.IsProductIdExisting(dto.IdProduct);
        Task<bool> isWarehouseExisting = _warehouseRepository.IsWarehouseIdExisting(dto.IdWarehouse);

        await Task.WhenAll([isProductExisting, isWarehouseExisting]);

        return isProductExisting.Result && isWarehouseExisting.Result && dto.Amount > 0;
    }
}