using System.Data;
using System.Data.SqlClient;
using APBD_Zadanie_6.Models;

namespace APBD_Zadanie_6.Repositories;

public class WarehouseRepository(IConfiguration configuration) : IWarehouseRepository
{
    public async Task<int> GetOrderIdByProductWarehouse(Order order)
    {
        var connectionString = configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand();

        command.Connection = connection;

        await connection.OpenAsync();
        command.CommandText = "SELECT TOP 1 [Order].IdOrder FROM [Order] " +
                              "LEFT JOIN Product_Warehouse ON [Order].IdOrder = Product_Warehouse.IdOrder " +
                              "WHERE [Order].IdProduct = @IdProduct " +
                              "AND [Order].Amount = @Amount " +
                              "AND Product_Warehouse.IdProductWarehouse IS NULL " +
                              "AND [Order].CreatedAt < @CreatedAt";

        command.Parameters.AddWithValue("@IdProduct", order.IdProduct);
        command.Parameters.AddWithValue("@Amount", order.Amount);
        command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) throw new Exception("No order found for given product criteria");

        await reader.ReadAsync();
        var idOrder = (int)reader["IdOrder"];
        await reader.CloseAsync();
        await connection.CloseAsync();

        return idOrder;
    }

    public async Task<decimal> GetPriceByProductId(int idProduct)
    {
        var connectionString = configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand();

        command.Connection = connection;

        await connection.OpenAsync();
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", idProduct);

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) throw new Exception("No product found with given id");

        await reader.ReadAsync();
        var price = (decimal)reader["Price"];
        await reader.CloseAsync();
        await connection.CloseAsync();

        return price;
    }

    public async Task<bool> CheckIfWarehouseExists(int idWarehouse)
    {
        var connectionString = configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand();

        command.Connection = connection;

        await connection.OpenAsync();
        command.CommandText = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);

        var reader = await command.ExecuteReaderAsync();
        var result = reader.HasRows;
        await reader.CloseAsync();
        await connection.CloseAsync();

        return result;
    }

    public async Task AddProductToWarehouse(ProductWarehouse productWarehouse)
    {
        var connectionString = configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand();

        command.Connection = connection;

        await connection.OpenAsync();
        var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        command.Transaction = transaction;

        try
        {
            command.CommandText = "UPDATE [Order] SET FulfilledAt = @CreatedAt WHERE IdOrder = @IdOrder";
            command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            command.Parameters.AddWithValue("@IdOrder", productWarehouse.IdOrder);

            var rowsUpdated = await command.ExecuteNonQueryAsync();

            if (rowsUpdated < 1) throw new Exception("Error while updating order");

            command.Parameters.Clear();

            command.CommandText =
                "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                "VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", productWarehouse.IdOrder);
            command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            command.Parameters.AddWithValue("@Price", productWarehouse.Price);
            command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

            var rowsInserted = await command.ExecuteNonQueryAsync();

            if (rowsInserted < 1) throw new Exception("Error while adding product to warehouse");

            await transaction.CommitAsync();
            await connection.CloseAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            await connection.CloseAsync();
            throw new Exception("Error while adding product to warehouse", e);
        }
    }

    public async Task AddProductToWarehouseByProcedure(ProductWarehouseDto productWarehouseDto)
    {
        var connectionString = configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand("AddProductToWarehouse", connection);

        await connection.OpenAsync();
        var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        command.Transaction = transaction;
        command.Connection = connection;

        try
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@IdProduct", productWarehouseDto.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouseDto.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", productWarehouseDto.Amount);
            command.Parameters.AddWithValue("@CreatedAt", productWarehouseDto.CreatedAt);

            var rowsInserted = await command.ExecuteNonQueryAsync();

            if (rowsInserted < 1) throw new Exception();

            await transaction.CommitAsync();
            await connection.CloseAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            await connection.CloseAsync();
            throw new Exception("Error while adding product to warehouse by procedure");
        }
    }

    public async Task<int> GetLastCreatedProductWarehouseId()
    {
        var connectionString = configuration.GetConnectionString("Database");
        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand();

        command.Connection = connection;

        await connection.OpenAsync();
        command.CommandText = "SELECT TOP 1 IdProductWarehouse FROM Product_Warehouse ORDER BY IdProductWarehouse DESC";

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) throw new Exception("Id of newly created product warehouse not found");

        await reader.ReadAsync();
        var idProductWarehouse = (int)reader["IdProductWarehouse"];
        await reader.CloseAsync();
        await connection.CloseAsync();

        return idProductWarehouse;
    }
}