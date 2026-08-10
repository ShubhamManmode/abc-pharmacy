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
        var medicine = await _medicineService.AddAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = medicine.Id },
            medicine);
    }
}
