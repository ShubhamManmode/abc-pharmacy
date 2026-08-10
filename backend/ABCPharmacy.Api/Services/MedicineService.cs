using ABCPharmacy.Api.Models;

namespace ABCPharmacy.Api.Services;

public class MedicineService
{
    private const string FileName = "medicines.json";
    private readonly JsonFileService _jsonFileService;

    public MedicineService(JsonFileService jsonFileService)
    {
        _jsonFileService = jsonFileService;
    }

    public Task<List<Medicine>> GetAllAsync()
    {
        return _jsonFileService.ReadAsync<Medicine>(FileName);
    }

    public async Task<List<Medicine>> SearchAsync(string? search)
    {
        var medicines = await GetAllAsync();

        if (string.IsNullOrWhiteSpace(search))
            return medicines;

        return medicines
            .Where(x => x.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<Medicine?> GetByIdAsync(int id)
    {
        var medicines = await GetAllAsync();
        return medicines.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Medicine> AddAsync(CreateMedicineRequest request)
    {
        var medicines = await GetAllAsync();

        var nextId = medicines.Count == 0
            ? 1
            : medicines.Max(x => x.Id) + 1;

        var medicine = new Medicine
        {
            Id = nextId,
            FullName = request.FullName.Trim(),
            Notes = request.Notes.Trim(),
            ExpiryDate = request.ExpiryDate.Date,
            Quantity = request.Quantity,
            Price = request.Price,
            Brand = request.Brand.Trim()
        };

        medicines.Add(medicine);
        await _jsonFileService.WriteAsync(FileName, medicines);

        return medicine;
    }

    public async Task<(bool Success, string? Error)> ReduceQuantityAsync(
        int medicineId,
        int quantity)
    {
        var medicines = await GetAllAsync();
        var medicine = medicines.FirstOrDefault(x => x.Id == medicineId);

        if (medicine is null)
            return (false, "Medicine not found.");

        if (quantity <= 0)
            return (false, "Quantity must be greater than zero.");

        if (medicine.Quantity < quantity)
            return (false, $"Only {medicine.Quantity} units are available.");

        medicine.Quantity -= quantity;

        await _jsonFileService.WriteAsync(FileName, medicines);

        return (true, null);
    }
}
