using ABCPharmacy.Api.Models;

namespace ABCPharmacy.Api.Services;

public class SaleService
{
    private const string FileName = "sales.json";

    private readonly JsonFileService _jsonFileService;
    private readonly MedicineService _medicineService;

    public SaleService(
        JsonFileService jsonFileService,
        MedicineService medicineService)
    {
        _jsonFileService = jsonFileService;
        _medicineService = medicineService;
    }

    public Task<List<Sale>> GetAllAsync()
    {
        return _jsonFileService.ReadAsync<Sale>(FileName);
    }

    public async Task<(bool Success, Sale? Sale, string? Error)> CreateAsync(
        CreateSaleRequest request)
    {
        if (request.QuantitySold <= 0)
            return (false, null, "Quantity sold must be greater than zero.");

        var medicine = await _medicineService.GetByIdAsync(request.MedicineId);

        if (medicine is null)
            return (false, null, "Medicine not found.");

        if (medicine.Quantity < request.QuantitySold)
        {
            return (
                false,
                null,
                $"Insufficient stock. Available quantity: {medicine.Quantity}.");
        }

        var sales = await GetAllAsync();

        var sale = new Sale
        {
            Id = sales.Count == 0 ? 1 : sales.Max(x => x.Id) + 1,
            MedicineId = medicine.Id,
            MedicineName = medicine.FullName,
            QuantitySold = request.QuantitySold,
            UnitPrice = medicine.Price,
            TotalAmount = medicine.Price * request.QuantitySold,
            SaleDate = DateTime.UtcNow
        };

        // The assignment uses JSON storage, so there is no database transaction.
        // We update inventory first and then persist the sale.
        var reduceResult = await _medicineService.ReduceQuantityAsync(
            request.MedicineId,
            request.QuantitySold);

        if (!reduceResult.Success)
            return (false, null, reduceResult.Error);

        sales.Add(sale);
        await _jsonFileService.WriteAsync(FileName, sales);

        return (true, sale, null);
    }
}
