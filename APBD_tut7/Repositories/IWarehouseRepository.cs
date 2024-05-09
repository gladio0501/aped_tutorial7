namespace APBD_tut7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExist(int id);
    Task<bool> DoesWarehouseExist(int id);
    Task<bool> DoesOrderExist(int productId, int amount, DateTime createdAt);
    Task<int> getOrderId(int productId, int amount, DateTime createdAt);
    Task<bool> DoesOrderProductExist(int productId);
    Task UpdateOrder(int orderId);
    Task<float> GetProductPrice(int idProduct);
    Task AddProductToWarehouse(int idProduct, int idWarehouse, int amount, DateTime createdAt);
    Task<int> GetProductWarehouseID(int productId, int warehouseId, int orderId);
    Task<int> AddProductToWarehouseUsingProcedure(int idProduct, int idWarehouse, int amount, DateTime createdAt);
}