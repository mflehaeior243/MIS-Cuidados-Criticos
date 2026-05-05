using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;
using System.Security.Principal;
namespace MIS_Cuidados_Criticos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConexionesController : ControllerBase
    {
        private readonly EnfermeriaService _enfermeriaService;
        private readonly ApplicationDbContext _context;

        public ConexionesController(EnfermeriaService enfermeriaService , ApplicationDbContext context)
        {
            _enfermeriaService = enfermeriaService;
            _context = context;
        }
        [HttpGet("enfermeras-disponibles")]
        public async Task<IActionResult> EnfermerasDisponibles()
        {
            var data = await _enfermeriaService.ObtenerEnfermerasDisponibles();

            if (data == null)
                return BadRequest("No se pudo obtener datos del microservicio de enfermería");

            return Ok(data);
        }
        [HttpPost("recibir-paciente-logistica")]
        public async Task<IActionResult> RecibirPaciente(string codigo, string nombre)
        {
            var paciente = new Paciente
            {
                Estado = "Activo",
                Codigo = codigo.ToLower(),
                Nomre = nombre.ToLower()
            };

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Paciente recibido desde logística correctamente",
                paciente.Codigo,
                paciente.Nomre
            });
        }
    }
}
