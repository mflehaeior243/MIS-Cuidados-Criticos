using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;

namespace MIS_Cuidados_Criticos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PacienteController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ConsiguirPaciente()
        {
            var dato = await _context.Pacientes
                .Where(a => a.Estado == "Activo")
                .Select(a => new
                {
                    a.Codigo,
                    a.Nomre
                }).ToListAsync();
            return Ok(dato);
        }
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConseguirPacienteporCod(string codigo)
        {
            var dato = await _context.Pacientes
                .Where(a => a.Codigo == codigo && a.Estado == "Activo")
                .Select(a => new
                {
                    a.Codigo,
                    a.Nomre
                })
                .FirstOrDefaultAsync();

            if (dato == null) return NotFound();
            return Ok(dato);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> AñadirPaciente(string codigo, string nombre)
        {
            var dato = new Paciente
            {
                Estado = "Activo",
                Codigo = codigo.ToLower(),
                Nomre = nombre.ToLower()
            };
            _context.Pacientes.Add(dato);
            await _context.SaveChangesAsync();
            return Ok(dato);
        }

        // PUT POR CÓDIGO
        [HttpPut("{codigo}")]
        public async Task<IActionResult> AñadirAlertaporCod(string codigo,string nombre)
        {
            var dato = await _context.Pacientes
                .FirstOrDefaultAsync(a => a.Codigo == codigo);

            if (dato == null) return NotFound();
            dato.Nomre = nombre.ToLower();
            await _context.SaveChangesAsync();

            return Ok($"El paciente {codigo}, fue actualizado");
        }

        // DELETE 
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarPaciente(string codigo)
        {
            var dato = await _context.Pacientes
                .FirstOrDefaultAsync(a => a.Codigo == codigo);

            if (dato == null) return NotFound();

            dato.Estado = "Inactivo";

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
