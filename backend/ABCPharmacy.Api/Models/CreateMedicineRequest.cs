using System.ComponentModel.DataAnnotations;

namespace ABCPharmacy.Api.Models;

public class CreateMedicineRequest
{
    [Required(ErrorMessage = "Medicine name is required.")]
    public string FullName { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    [CustomValidation(typeof(CreateMedicineRequest), nameof(ValidateExpiryDate))]
    public DateTime ExpiryDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Brand is required.")]
    public string Brand { get; set; } = string.Empty;

    public static ValidationResult? ValidateExpiryDate(DateTime expiryDate)
    {
        return expiryDate.Date < DateTime.Today
            ? new ValidationResult("Expiry date cannot be in the past.")
            : ValidationResult.Success;
    }
}
