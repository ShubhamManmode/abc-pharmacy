using ABCPharmacy.Api.Models;

namespace ABCPharmacy.Api.Services;

public class MedicineService
{
    private static readonly List<Medicine> Medicines = new()
    {
        new()
        {
            Id = 1,
            FullName = "Paracetamol 500mg",
            Notes = "Pain and fever relief",
            ExpiryDate = new DateTime(2027, 5, 20),
            Quantity = 25,
            Price = 25.50m,
            Brand = "ABC Pharma"
        },
        new()
        {
            Id = 2,
            FullName = "Azithromycin 500mg",
            Notes = "Antibiotic",
            ExpiryDate = new DateTime(2026, 9, 1),
            Quantity = 8,
            Price = 85.00m,
            Brand = "HealthCare"
        },
        new()
        {
            Id = 3,
            FullName = "Cetirizine 10mg",
            Notes = "Allergy relief",
            ExpiryDate = new DateTime(2026, 8, 25),
            Quantity = 15,
            Price = 40.00m,
            Brand = "MediLife"
        }
    };

    public Task<List<Medicine>> GetAllAsync()
    {
        return Task.FromResult(Medicines);
    }

    public Task<List<Medicine>> SearchAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Task.FromResult(Medicines);

        var results = Medicines
            .Where(x => x.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(results);
    }

    public Task<Medicine?> GetByIdAsync(int id)
    {
        return Task.FromResult(Medicines.FirstOrDefault(x => x.Id == id));
    }

    public Task<Medicine> AddAsync(CreateMedicineRequest request)
    {
        var nextId = Medicines.Count == 0
            ? 1
            : Medicines.Max(x => x.Id) + 1;

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

        Medicines.Add(medicine);

        return Task.FromResult(medicine);
    }

    public Task<(bool Success, string? Error)> ReduceQuantityAsync(
        int medicineId,
        int quantity)
    {
        var medicine = Medicines.FirstOrDefault(x => x.Id == medicineId);

        if (medicine is null)
            return Task.FromResult((false, (string?)"Medicine not found."));

        if (quantity <= 0)
            return Task.FromResult((false, (string?)"Quantity must be greater than zero."));

        if (medicine.Quantity < quantity)
            return Task.FromResult((false, (string?)$"Only {medicine.Quantity} units are available."));

        medicine.Quantity -= quantity;

        return Task.FromResult((true, (string?)null));
    }
}
