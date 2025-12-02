using Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs.Common;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObrasSocialesController : ControllerBase
{
    private readonly IRepositorioObraSocial _repositorio;
    private readonly ILogger<ObrasSocialesController> _logger;

    public ObrasSocialesController(IRepositorioObraSocial repositorio, ILogger<ObrasSocialesController> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult ObtenerTodas()
    {
        try
        {
            var obrasSociales = _repositorio.ObtenerTodas();
            return Ok(obrasSociales);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener obras sociales");
            return StatusCode(500, new ErrorResponse { Message = "Error interno", StatusCode = 500 });
        }
    }
}