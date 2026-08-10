namespace ABCPharmacy.Api.Models;

public class CreateSaleRequest
{
    public int MedicineId { get; set; }
    public int QuantitySold { get; set; }
}
