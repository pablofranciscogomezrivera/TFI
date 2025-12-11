using Aplicacion.Intefaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObrasSocialesController : ControllerBase
{
    private readonly IServicioObraSocial _servicioObraSocial;
    private readonly ILogger<ObrasSocialesController> _logger;

    public ObrasSocialesController(IServicioObraSocial servicioObraSocial, ILogger<ObrasSocialesController> logger)
    {
        _servicioObraSocial = servicioObraSocial;
        _logger = logger;
    }


    [HttpGet]
    [ProducesResponseType(typeof(List<Dominio.Entidades.ObraSocial>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult ObtenerTodas()
    {
        try
        {
            var obrasSociales = _servicioObraSocial.ObtenerTodasLasObrasSociales();
            return Ok(obrasSociales);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener obras sociales");
            return StatusCode(500, new ErrorResponse { Message = "Error interno del servidor", StatusCode = 500 });
        }
    }
}