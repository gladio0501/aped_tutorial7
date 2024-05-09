using APBD_tut7.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APBD_tut7.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    // GET
    [HttpGet]
    [Route("product/{productID}")]
    public async Task<IActionResult> GetProduct(int productID)
    {
        if (!await _warehouseRepository.DoesProductExist(productID))
            return NotFound();

        return Ok();
    }
    // GET
    [HttpGet]
    [Route("warehouse/{warehouseID}")]
    public async Task<IActionResult> GetWarehouse(int warehouseID)
    {
        if (!await _warehouseRepository.DoesWarehouseExist(warehouseID))
            return NotFound();

        return Ok();
    }
    [HttpPost]
    [Route("addproducttowarehouse")]
    public async Task<IActionResult> Addpr_wr(int productID, int warehouseID,int amount,DateTime CreatedAt)
    {
        
        //We can add a product to the warehouse only if there is a product purchase order in the Order table. Therefore, we check if there is arecord in the Order table with IdProduct and Amount that matches ourrequest. The CreatedAt of the order should be lower than the CreatedAtin the request.
        if (amount <= 0) return NotFound();
        if (!await _warehouseRepository.DoesProductExist(productID))
            return NotFound("product does not exist");
        if (!await _warehouseRepository.DoesWarehouseExist(warehouseID))
            return NotFound("warehouse does not exist");
        if (!await _warehouseRepository.DoesOrderExist(productID, amount, CreatedAt))
            return NotFound("order does not exist");
         
        //We check whether this order has been completed by any chance. We check if there is no row with the given IdOrder in the Product_Warehousetable.
        int orderId = await _warehouseRepository.getOrderId(productID, amount, CreatedAt);
        if (await _warehouseRepository.DoesOrderProductExist(orderId))
            return NotFound("The order has already been completed.");
        //We update the FullfilledAt column of the order with the current date and time. (UPDATE)
        await _warehouseRepository.UpdateOrder(orderId);
        //As a result of the operation, we return the value of the primary key generated for the record inserted into the Product_Warehouse table.
        await _warehouseRepository.AddProductToWarehouse(productID, warehouseID, amount, CreatedAt);
        int ID = await _warehouseRepository.GetProductWarehouseID(productID, warehouseID, orderId);
        return Ok(ID);
    }
    
    // In WarehouseController.cs
    [HttpPost]
    [Route("addproducttowarehouse/procedure")]
    public async Task<IActionResult> AddProductToWarehouseUsingProcedure(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        try
        {
            int newId = await _warehouseRepository.AddProductToWarehouseUsingProcedure(idProduct, idWarehouse, amount, createdAt);
            return Ok(newId);
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    
    
}