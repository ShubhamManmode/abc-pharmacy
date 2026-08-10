using ABCPharmacy.Api.Models;

namespace ABCPharmacy.Api.Services;

public class SaleService
{
    private static readonly List<Sale> Sales = [];
    private readonly MedicineService _medicineService;

    public SaleService(MedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    public Task<List<Sale>> GetAllAsync()
    {
        return Task.FromResult(Sales);
    }

    public async Task<(bool Success, Sale? Sale, string? Error)> CreateAsync(CreateSaleRequest request)
    {
        if (request.QuantitySold <= 0)
            return (false, null, "Quantity sold must be greater than zero.");

        var medicine = await _medicineService.GetByIdAsync(request.MedicineId);

        if (medicine is null)
            return (false, null, "Medicine not found.");

        if (medicine.Quantity < request.QuantitySold)
        {
            return (false,null,$"Insufficient stock. Available quantity: {medicine.Quantity}.");
        }

        var sale = new Sale
        {
            Id = Sales.Count == 0 ? 1 : Sales.Max(x => x.Id) + 1,
            MedicineId = medicine.Id,
            MedicineName = medicine.FullName,
            QuantitySold = request.QuantitySold,
            UnitPrice = medicine.Price,
            TotalAmount = medicine.Price * request.QuantitySold,
            SaleDate = DateTime.UtcNow
        };

        var reduceResult = await _medicineService.ReduceQuantityAsync(
            request.MedicineId,
            request.QuantitySold);

        if (!reduceResult.Success)
            return (false, null, reduceResult.Error);

        Sales.Add(sale);

        return (true, sale, null);
    }
}
