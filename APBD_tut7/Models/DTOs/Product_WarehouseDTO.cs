namespace APBD_tut7.Models.DTOs;

public class Product_WarehouseDTO
{
    public int IdProductWarehouse { get; set; }
    public int IdWarehouse { get; set; }
    public int IdOrder { get; set; }
    public int IdProduct { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public float Price { get; set; }
}



public record WarehouseDTO
{
    public int IdWarehouse { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

