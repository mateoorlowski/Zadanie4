namespace APBD_Zadanie_6.Models;

public class ProductWarehouse
{
    public int IdProduct { get; init; }
    public int IdWarehouse { get; init; }
    public int IdOrder { get; init; }
    public int Amount { get; init; }
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}