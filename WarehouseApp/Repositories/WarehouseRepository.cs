using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Repositories;

public class WarehouseRepository(IConfiguration _configuration) : IWarehouseRepository
{
    public async Task<int> CreateProductWarehouseRecord(ProductWarehouseDto request)
    {
        return 0;
    }

    public async Task<bool> IsProductIdExisting(int id)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT IdProduct FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", id);

        var updateObjects = command.ExecuteScalarAsync();
        return updateObjects != null && (int) await updateObjects > 0;
    }

    public async Task<bool> IsWarehouseIdExisting(int id)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT *FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        command.Parameters.AddWithValue("@IdWarehouse", id);
        
        var updateObjects = command.ExecuteScalarAsync();
        var result = await updateObjects;
        return result != null && (int) result > 0;

    }

    public async Task<int> IsOrderExisting(int id, int amount, DateTime createDate)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreateDate";
        command.Parameters.AddWithValue("@IdProduct", id);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreateDate", createDate);

        using (var dr = await command.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                return (int)dr["IdOrder"];
            }
        }

        return -1;
    }

    public async Task<bool> IsOrderCompleted(int orderId)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        command.Parameters.AddWithValue("@IdOrder", orderId);
        
        var result = await command.ExecuteScalarAsync();
        return result != null && (int) result > 0;
    }

    public async Task<bool> UpdateOrderFullfilledAt(int orderId, DateTime dateTime)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE [Order] SET FullfilledAt = @FullfilledAt WHERE IdOrder = @IdOrder";
        command.Parameters.AddWithValue("@IdOrder", orderId);
        command.Parameters.AddWithValue("@FullfilledAt", dateTime);
        
        var updatedObjects = command.ExecuteNonQueryAsync();
        return await updatedObjects > 0;
    }
    
    public async Task<int> InsertProductWarehouseRow(ProductWarehouseDto request, int orderId)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        var price = GetPriceFromProduct(request.IdProduct);
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        
        command.CommandText = "INSERT INTO Product_Warehouse VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); SELECT CAST(scope_identity() AS int)";
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", orderId);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@Price", request.Amount * price.Result);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        var id = command.ExecuteScalarAsync();
        return (int) await id;
    }
    
    public async Task<Decimal> GetPriceFromProduct(int productId)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", productId);

        using (var dr = await command.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                return (decimal)dr["Price"];
            }
        }
        
        return Decimal.MinusOne;
    }

    public async Task<int> FulfillOrder(ProductWarehouseDto request, int orderId)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        DbTransaction tran = await connection.BeginTransactionAsync();
        command.Transaction = (SqlTransaction)tran;
        try
        {
            UpdateOrderFullfilledAt(orderId, DateTime.Now);
            int id = await InsertProductWarehouseRow(request, orderId);
            await tran.CommitAsync();
            return id;
        }
        catch (SqlException exc)
        { 
            await tran.RollbackAsync();
            return -1;
        }
        catch (Exception exc)
        {
            await tran.RollbackAsync();
            return -1;
        }
    }

    public async Task<int> AddProductToWarehouseWithStoredProcedure(ProductWarehouseDto request)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var cmd = new SqlCommand("AddProductToWarehouse", connection);
        cmd.CommandType = CommandType.StoredProcedure; 
        cmd.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        cmd.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse); 
        cmd.Parameters.AddWithValue("@Amount", request.Amount); 
        cmd.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
        
        var id = cmd.ExecuteScalarAsync();
        return Decimal.ToInt32((Decimal) await id);
    }
}