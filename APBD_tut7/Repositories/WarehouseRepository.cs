using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace APBD_tut7.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> DoesProductExist(int id)
    {
        var query = "SELECT 1 FROM Product WHERE IdProduct = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<bool> DoesWarehouseExist(int id)
    {
        var query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    //We can add a product to the warehouse only if there is a productpurchase order in the Order table. Therefore, we check if there is arecord in the Order table with IdProduct and Amount that matches ourrequest. The CreatedAt of the order should be lower than the CreatedAtin the request.
    public async Task<int> getOrderId(int idProduct, int amount, DateTime createdAt)
    {
        var query = "SELECT IdOrder FROM [Order] WHERE IdProduct = @IDProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IDProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return (int)res;
    }
    public async Task<bool> DoesOrderExist(int idProduct, int amount, DateTime createdAt)
    {
        var query = "SELECT 1 FROM [Order] WHERE IdProduct = @IDProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IDProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    //We check whether this order has been completed by any chance. Wecheck if there is no row with the given IdOrder in the Product_Warehousetable.
    public async Task<bool> DoesOrderProductExist(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IDOrder";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IDOrder", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    //We update the FullfilledAt column of the order with the current date andtime. (UPDATE)
    public async Task UpdateOrder(int idOrder)
    {
        var query = "UPDATE [Order] SET FulfilledAt = @FullfilledAt WHERE IdOrder = @IDOrder";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IDOrder", idOrder);
        command.Parameters.AddWithValue("@FullfilledAt", DateTime.Now);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }
    //We insert a record into the Product_Warehouse table. The Price columnshould corresponds to the price of the product multiplied by amount valuefrom our request. Moreover, we insert the CreatedAt value according tothe current time. (INSERT)
    public async Task AddProductToWarehouse(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        var query = "INSERT INTO Product_Warehouse (IdWarehouse, IdOrder, IdProduct, Amount, CreatedAt, Price) VALUES (@IdWarehouse, @IdOrder, @IdProduct, @Amount, @CreatedAt, @Price)";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        command.Parameters.AddWithValue("@IdOrder", await getOrderId(idProduct, amount, createdAt));
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);
        command.Parameters.AddWithValue("@Price", amount * (float)await GetProductPrice(idProduct));

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> GetProductWarehouseID(int productId, int warehouseId, int orderId)
    {
        var query = "SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdProduct = @IDProduct AND IdWarehouse = @IDWarehouse AND IdOrder = @IDOrder";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IDProduct", productId);
        command.Parameters.AddWithValue("@IDWarehouse", warehouseId);
        command.Parameters.AddWithValue("@IDOrder", orderId);
        
        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return (int)res;


    }

    public async Task<float> GetProductPrice(int idProduct)
    {
        var query = "SELECT Price FROM Product WHERE IdProduct = @IDProduct";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IDProduct", idProduct);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        if (res == null)
        {
            throw new Exception("Product with the given ID does not exist.");
        }

        return Convert.ToSingle(res);
    }
    
    ////As a result of the operation, we return the value of the primary key generated for the record inserted into the Product_Warehouse table.

// In WarehouseRepository.cs
    public async Task<int> AddProductToWarehouseUsingProcedure(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand("AddProductToWarehouse", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@IdProduct", idProduct));
        command.Parameters.Add(new SqlParameter("@IdWarehouse", idWarehouse));
        command.Parameters.Add(new SqlParameter("@Amount", amount));
        command.Parameters.Add(new SqlParameter("@CreatedAt", createdAt));

        await connection.OpenAsync();

        var newId = await command.ExecuteScalarAsync();

        return Convert.ToInt32(newId);
    }
}