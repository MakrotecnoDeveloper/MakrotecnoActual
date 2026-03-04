using Microsoft.AspNetCore.Mvc;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;

namespace Plataforma.Controllers
{
    public class TercerosController : Controller
    {
        private readonly ITercerosService _terceroService;
        public TercerosController(ITercerosService terceroService)
        {
            _terceroService = terceroService;
        }
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] Clientes cliente)
        {
                var clienteCreado = await _terceroService.CrearClienteAsync(cliente);
                return Ok(new { 
                    success = true,
                    cliente = new {
                        idCliente = clienteCreado.IdCliente,
                        nombreCliente = clienteCreado.NombreCliente,
                        telefonoCliente = clienteCreado.TelefonoCliente
                    }
                
                });
        }
    }
}