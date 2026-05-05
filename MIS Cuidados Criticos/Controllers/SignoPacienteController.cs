using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;
namespace MIS_Cuidados_Criticos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignoPacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SignoPacienteController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Listar signo por paciente
        [HttpGet("listar-signo-paciente")]
        public async Task<IActionResult> ListarSignoPaciente()
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                select new
                {
                    Paciente = b.Codigo,
                    SignoVital = c.Codigo,
                    a.Fecha_hora
                }).ToListAsync();
            return Ok(dato);
        }
        [HttpPost]
        public async Task<IActionResult> AsociarPacienteSigno(string codigopaciente, string codigosigno)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(a => a.Codigo == codigopaciente && a.Estado == "Activo");
            if (paciente == null)
                return BadRequest("El paciente no existe o está inactivo");
            var signo = await _context.SignosVitales
                .FirstOrDefaultAsync(a => a.Codigo == codigosigno && a.Estado == "Activo");
            if (signo == null)
                return BadRequest("El signo vital no existe o está inactivo");
            var rela = new SignoPaciente
            {
                Id_paciente = paciente.Id,
                id_signo = signo.Id,
                Fecha_hora = DateTime.UtcNow
            };
            _context.SignoPacientes.Add(rela);
            await _context.SaveChangesAsync();
            return Ok("La relacion fue creada corectamente");
        }

    }
}
