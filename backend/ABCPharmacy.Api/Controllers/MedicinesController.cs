using ABCPharmacy.Api.Models;
using ABCPharmacy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCPharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly MedicineService _medicineService;

    public MedicinesController(MedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var medicines = await _medicineService.SearchAsync(search);
        return Ok(medicines);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var medicine = await _medicineService.GetByIdAsync(id);

        if (medicine is null)
            return NotFound(new { message = "Medicine not found." });

        return Ok(medicine);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMedicineRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new { message = "Medicine name is required." });

        if (string.IsNullOrWhiteSpace(request.Brand))
            return BadRequest(new { message = "Brand is required." });

        if (request.Quantity < 0)
            return BadRequest(new { message = "Quantity cannot be negative." });

        if (request.Price < 0)
            return BadRequest(new { message = "Price cannot be negative." });

        if (request.ExpiryDate.Date < DateTime.Today)
            return BadRequest(new { message = "Expiry date cannot be in the past." });

        var medicine = await _medicineService.AddAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = medicine.Id },
            medicine);
    }
}
