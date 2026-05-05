using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;
using System.Security.Principal;
namespace MIS_Cuidados_Criticos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET 
        [HttpGet]
        public async Task<IActionResult> ConseguirAlerta()
        {
            var dato = await _context.Alertas
                .Where(a => a.Estado == "Activo")
                .Select(a => new
                {
                    a.Codigo,
                    a.Tipo,
                    a.Nivel_criticidad
                })
                .ToListAsync();

            return Ok(dato);
        }

        // GET POR CÓDIGO
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConseguirAlertaporCod(string codigo)
        {
            var dato = await _context.Alertas
                .Where(a => a.Codigo == codigo && a.Estado == "Activo")
                .Select(a => new
                {
                    a.Codigo,
                    a.Tipo,
                    a.Nivel_criticidad
                })
                .FirstOrDefaultAsync();

            if (dato == null) return NotFound();
            return Ok(dato);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> AñadirAlerta(string codigo,string tipo, string nvcriticidad)
        {
            var dato = new Alerta
            {
                Estado = "Activo",
                Codigo = codigo.ToLower(),
                Tipo = tipo.ToLower(),
                Nivel_criticidad = nvcriticidad
            };
            _context.Alertas.Add(dato);
            await _context.SaveChangesAsync();
            return Ok(dato);
        }

        // PUT POR CÓDIGO
        [HttpPut("{codigo}")]
        public async Task<IActionResult> AñadirAlertaporCod(string codigo, string tipo, string nvcriticidad)
        {
            var dato = await _context.Alertas
                .FirstOrDefaultAsync(a => a.Codigo == codigo);

            if (dato == null) return NotFound();

            dato.Tipo = tipo.ToLower();
            dato.Nivel_criticidad = nvcriticidad.ToLower();

            await _context.SaveChangesAsync();

            return Ok($"La alerta {codigo}, fue actualizada");
        }

        // DELETE 
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarAlerta(string codigo)
        {
            var dato = await _context.Alertas
                .FirstOrDefaultAsync(a => a.Codigo == codigo);

            if (dato == null) return NotFound();

            dato.Estado = "Inactivo";

            await _context.SaveChangesAsync();

            return Ok("La alerta fue eliminada");
        }
    }
}
