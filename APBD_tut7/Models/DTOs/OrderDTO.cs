namespace APBD_tut7.Models.DTOs;

public class OrderDTO
{
    public int IdOrder { get; set; }
    public int IdProduct { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime FulfilledAt { get; set; }
}

public record ProductDTO
{
    public int IdProduct { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public string Description { get; set; }
}
