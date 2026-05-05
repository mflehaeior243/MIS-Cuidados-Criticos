using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;

namespace MIS_Cuidados_Criticos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignoVitalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SignoVitalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET 
        [HttpGet]
        public async Task<IActionResult> ConseguiSignos()
        {
            var dato = await _context.SignosVitales
                .Where(a => a.Estado == "Activo")
                .Select(a => new
                {
                    a.Codigo,
                    a.Frecuencia_cardiaca,
                    a.Presion_arterial,
                    a.Saturacion_oxigeno
                })
                .ToListAsync();

            return Ok(dato);
        }

        // GET cod
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConseguiSignosporCod(string codigo)
        {
            var dato = await _context.SignosVitales
                .Where(a => a.Codigo == codigo && a.Estado == "Activo")
                .Select(a => new
                {
                    a.Codigo,
                    a.Frecuencia_cardiaca,
                    a.Presion_arterial,
                    a.Saturacion_oxigeno
                })
                .FirstOrDefaultAsync();

            if (dato == null) return NotFound();
            return Ok(dato);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> AgregarSignos(string codigo,int fecuenciacardiaca, float saturacionoxigeno,string precionarterial)
        {
            var dato = new SignoVital
            {
                Estado = "Activo",
                Codigo = codigo.ToLower(),
                Frecuencia_cardiaca = fecuenciacardiaca,
                Presion_arterial = precionarterial,
                Saturacion_oxigeno = saturacionoxigeno
            };
            _context.SignosVitales.Add(dato);
            await _context.SaveChangesAsync();
            return Ok(dato);
        }

        // PUT por cod
        [HttpPut("{codigo}")]
        public async Task<IActionResult> AgregarSignosporCod(string codigo, int fecuenciacardiaca, float saturacionoxigeno, string precionarterial)
        {
            var dato = await _context.SignosVitales
                .FirstOrDefaultAsync(a => a.Codigo == codigo);

            if (dato == null) return NotFound();

            dato.Frecuencia_cardiaca = fecuenciacardiaca;
            dato.Presion_arterial = precionarterial;
            dato.Saturacion_oxigeno = saturacionoxigeno;

            await _context.SaveChangesAsync();

            return Ok($"El signo vital {codigo}, fue actualizado");
        }

        // DELETE 
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarSignos(string codigo)
        {
            var dato = await _context.SignosVitales
                .FirstOrDefaultAsync(a => a.Codigo == codigo);

            if (dato == null) return NotFound();

            dato.Estado = "Inactivo";

            await _context.SaveChangesAsync();

            return Ok();
        }
        //JOIN con los 3 controllers
        
        [HttpGet("CC-resumido")]
        public async Task<IActionResult> ObtenerTodo()
        {
            var dato = await
                (
                    from a in _context.SignosVitales
                    join b in _context.SignoAlertas
                    on a.Id equals b.Id_signo_vital
                    join c in _context.Alertas
                    on b.Id_alerta equals c.Id
                    where a.Estado == "Activo" && c.Estado == "Activo"
                    select new
                    {
                        CodigoSigno = a.Codigo,
                        FrecuenciaCardiaca = a.Frecuencia_cardiaca,
                        Presion = a.Presion_arterial,
                        Saturacion = a.Saturacion_oxigeno < 85 ? "Critico" : a.Saturacion_oxigeno > 92 ? "Riesgo" : "Estable",
                        CodigoAlerta = c.Codigo,
                        TipoAle = c.Tipo,
                        Nivel = c.Nivel_criticidad
                    }
                ).ToListAsync();
            return Ok(dato);
        }
    }
}
