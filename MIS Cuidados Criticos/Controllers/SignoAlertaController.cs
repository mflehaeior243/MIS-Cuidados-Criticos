using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;
namespace MIS_Cuidados_Criticos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignoAlertaController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public SignoAlertaController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ConseguirSignoAlerta()
        {
            var dato = await
                (
                    from a in _context.SignoAlertas
                    join b in _context.SignosVitales on a.Id_signo_vital equals b.Id
                    join c in _context.Alertas on a.Id_alerta equals c.Id
                    where b.Estado == "Activo" && c.Estado == "Activo"
                    select new
                    {
                        SignosVitales = b.Codigo,
                        Alerta = c.Codigo,
                    }).ToListAsync();
            return Ok(dato);
        }
        [HttpPost]
        public async Task<IActionResult> CrearRelacion(string codigoSigno, string codigoAlerta)
        {
            var signo = await _context.SignosVitales.FirstOrDefaultAsync
                (a => a.Codigo == codigoSigno && a.Estado == "Activo");

            if (signo == null)
                return BadRequest("El signo vital no existe o está inactivo");

            var alerta = await _context.Alertas.FirstOrDefaultAsync
                (a => a.Codigo == codigoAlerta && a.Estado == "Activo");

            if (alerta == null)
                return BadRequest("La alerta no existe o está inactiva");

            var relacion = new SignoAlerta
            {
                Id_signo_vital = signo.Id,
                Id_alerta = alerta.Id
            };
            _context.SignoAlertas.Add(relacion);
            await _context.SaveChangesAsync();
            return Ok("Relacion fue realizada con exito");
        }
        
    } 
}
