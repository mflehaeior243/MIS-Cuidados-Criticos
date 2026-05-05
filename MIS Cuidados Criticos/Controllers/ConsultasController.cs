using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Data;
using MIS_Cuidados_Criticos.Dominio;
using System.Formats.Asn1;
namespace MIS_Cuidados_Criticos.Controllers
{
    public class ConsultasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }
        //JOIN de 3 tablas
        [HttpGet("listado-general")]
        public async Task<IActionResult> ConseguiListado()
        {
            var dato = await (from a in _context.SignosVitales
                              join b in _context.SignoAlertas
                              on a.Id equals b.Id_signo_vital
                              join c in _context.Alertas
                              on b.Id_alerta equals c.Id
                              where a.Estado == "Activo" && c.Estado == "Activo"
                              select new
                              {
                                  CodigoSigno = a.Codigo,
                                  FrecuenciaCardiaca = a.Frecuencia_cardiaca,
                                  Precion = a.Presion_arterial,
                                  Saturacion = a.Saturacion_oxigeno,
                                  CodigoAlerta = c.Codigo,
                                  TipoALerta = c.Tipo,
                                  NivelCritico = c.Nivel_criticidad
                              }).ToListAsync();
            return Ok(dato);
        }
        //group by + count
        [HttpGet("alertas-por-nivel")]
        public async Task<IActionResult> ConseguirAlertasNivel()
        {
            var dato = await _context.Alertas
                .Where(a => a.Estado == "Activo")
                .GroupBy(a => a.Nivel_criticidad)
                .Select(a => new
                {
                    Nivel = a.Key,
                    Cantidad = a.Count()
                }).OrderBy(a =>
                a.Nivel.ToLower() == "alta" ? 1 :
                a.Nivel.ToLower() == "media" ? 2 :
                a.Nivel.ToLower() == "baja" ? 3 : 4
                ).ToListAsync();
           return Ok(dato);
        }

        //Group by + sum
        [HttpGet("frecuencia-cardiaca-por-paciente")]
        public async Task<IActionResult> ConseguirSumaFC(string codigo)
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.SignosVitales on a.id_signo equals b.Id
                join c in _context.Pacientes on a.Id_paciente equals c.Id
                where b.Estado == "Activo" && c.Estado == "Activo" && c.Codigo == codigo
                group b by c.Codigo into grupo
                select new
                {
                    PacienteCodigo = grupo.Key,
                    SumaFC = grupo.Sum(x => x.Frecuencia_cardiaca)
                }).ToListAsync();
            return Ok(dato);
        }
        //Busqueda por codigo
        [HttpGet("Alerta-por-codigo/{codigo}")]
        public async Task<IActionResult> ConseguirBusqueda(string codigo)
        {
            var dato = await _context.Alertas
                .Where(a => a.Codigo == codigo && a.Estado == "Activo")
                .Select(a => new
                {
                    CodigoAlerta = a.Codigo,
                    TipoAlerta = a.Tipo,
                    Nivel = a.Nivel_criticidad
                }).FirstOrDefaultAsync();
            if (dato == null) return NotFound();
            return Ok(dato);
        }
        //Registro sin relacion (ejemplo: signos vitales)
        [HttpGet("Signo-sin-alerta")]
        public async Task<IActionResult> SignoSinAlerta()
        {
            var signo = await _context.SignosVitales
                .Where(a => a.Estado == "Activo" && !_context.SignoAlertas.Any(b => b.Id_signo_vital == a.Id))
                .Select(a => new
                {
                    CodigoSigno = a.Codigo,
                    FrecuenciaCardiaca = a.Frecuencia_cardiaca,
                    Precion = a.Presion_arterial,
                    Saturacion = a.Saturacion_oxigeno
                }).ToListAsync();
            return Ok(signo);
        }
        //4. CONSULTAS MIS
        [HttpGet("estado-actual-paciente/{codigo}")]
        public async Task<IActionResult> EstadoActualPaciente()
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                where b.Estado == "Activo" && c.Estado == "Activo"
                orderby a.Fecha_hora descending
                select new
                {
                    b.Codigo,
                    c.Presion_arterial,
                    c.Saturacion_oxigeno,
                    c.Frecuencia_cardiaca,
                    a.Fecha_hora
                }).FirstOrDefaultAsync();
            return Ok(dato);
        }
        [HttpGet("historial-signos/{codigo}")]
        public async Task<IActionResult> Historial(string codigo)
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                where b.Codigo == codigo && b.Estado == "Activo"
                orderby a.Fecha_hora descending
                select new
                {
                    SignoCodigo = c.Codigo,
                    c.Frecuencia_cardiaca,
                    c.Saturacion_oxigeno,
                    a.Fecha_hora
                }).ToListAsync();
            if (!dato.Any()) return NotFound("No hay historial");
            return Ok(dato);
        }

        [HttpGet("Paciente-con-mas-alertas")]
        public async Task<IActionResult> PacienteMasAlerta()
        {
            var dato = await (
                from a in _context.AlertaPacientes
                join b in _context.Pacientes on a.Id_Paciente equals b.Id
                group a by b.Codigo into c
                select new
                {
                    PacienteMasCritico = c.Key,
                    AlertaTotales = c.Count()
                }).OrderByDescending(a => a.AlertaTotales).FirstOrDefaultAsync();
            return Ok(dato);
        }
        [HttpGet("Paciente-sin-alerta")]
        public async Task<IActionResult> PacienteSinAlerta()
        {
            var dato = await (
                from a in _context.Pacientes
                where a.Estado == "Activo"
                where !(from b in _context.AlertaPacientes
                        select b.Id_Paciente).Contains(a.Id)
                select new
                {
                    Pacientesinalerta = a.Estado,
                    Codigo = a.Codigo
                }).ToListAsync();
            return Ok(dato);
        }
        [HttpGet("Oxigeno bajo")]
        public async Task<IActionResult> OxiBajo()
        {
            var data = await (
                from a in _context.SignoPacientes
                join b in _context.SignosVitales on a.id_signo equals b.Id
                where b.Saturacion_oxigeno < 85
                select new
                {
                    b.Codigo,
                    b.Saturacion_oxigeno,
                    a.Fecha_hora
                }).ToListAsync();
            return Ok(data);
        }
        [HttpGet("evolucion-clinica/{codigoPaciente}")]
        public async Task<IActionResult> EvolucionClinica(string codigoPaciente)
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.SignosVitales on a.id_signo equals b.Id
                join c in _context.Pacientes on a.Id_paciente equals c.Id
                where c.Codigo == codigoPaciente && c.Estado == "Activo"
                orderby a.Fecha_hora
                select new
                {
                    Fecha = a.Fecha_hora,
                    FrecuenciaCardiaca = b.Frecuencia_cardiaca,
                    SaturacionOxigeno = b.Saturacion_oxigeno
                }).ToListAsync();

            if (!dato.Any()) return NotFound("No hay datos de evolución clínica");

            return Ok(dato);
        }
        [HttpGet("Cantidad-signos")]
        public async Task<IActionResult> CantidadSignos()
        {
            var dato = await
                (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                group a by b.Codigo into c
                select new
                {
                    PacienteElegido = c.Key,
                    TotalSigno = c.Count()
                }).ToListAsync();
            return Ok(dato);
        }
        [HttpGet("Mostrar-ultimas-alertas")]
        public async Task<IActionResult> MostrarUltiAler()
        {
            var dato = await (
                from a in _context.Alertas
                orderby a.Id descending
                select new
                {
                    a.Codigo,
                    a.Tipo,
                    a.Nivel_criticidad
                }).Take(5).ToListAsync();
            return Ok(dato);
        }
        [HttpGet("promedio-fc/{codigo}")]
        public async Task<IActionResult> PromedioFCporPaciente(string codigo)
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                where b.Estado == "Activo"
                      && c.Estado == "Activo"
                      && b.Codigo == codigo
                group c by b.Codigo into d
                select new
                {
                    Paciente = d.Key,
                    PromedioFC = d.Average(x => x.Frecuencia_cardiaca)
                }).FirstOrDefaultAsync();

            if (dato == null) return NotFound("Paciente sin datos");

            return Ok(dato);
        }

        [HttpGet("Pacientes-criticos-con-alerta")]
        public async Task<IActionResult> PacienteCriticoAlerta()
        {
            var dato = await
                (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                join d in _context.SignoAlertas on c.Id equals d.Id_signo_vital
                join e in _context.Alertas on d.Id_alerta equals e.Id
                where c.Saturacion_oxigeno < 95
                select new
                {
                    PacienteCritico = b.Codigo,
                    FC = c.Frecuencia_cardiaca,
                    SaturacionOxigeno = c.Saturacion_oxigeno,
                    TipoAlerta = e.Tipo,
                    Nivel = e.Nivel_criticidad,
                    FechaHora = a.Fecha_hora
                }).ToListAsync();
            return Ok(dato);
        }
        [HttpGet("filtrar-pacientes-rango")]
        public async Task<IActionResult> FiltrarPacientes(int fcMin, int fcMax, float satMin)
        {
            var dato = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                where b.Estado == "Activo"
                      && c.Estado == "Activo"
                      && c.Frecuencia_cardiaca >= fcMin
                      && c.Frecuencia_cardiaca <= fcMax
                      && c.Saturacion_oxigeno <= satMin
                select new
                {
                    Paciente = b.Codigo,
                    c.Frecuencia_cardiaca,
                    c.Saturacion_oxigeno,
                    a.Fecha_hora
                }).ToListAsync();

            return Ok(dato);
        }
        [HttpGet("comparar-pacientes")]
        public async Task<IActionResult> CompararPacientes(string cod1, string cod2)
        {
            var p1 = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                where b.Codigo == cod1 && b.Estado == "Activo"
                orderby a.Fecha_hora descending
                select new
                {
                    b.Codigo,
                    c.Frecuencia_cardiaca,
                    c.Saturacion_oxigeno,
                    c.Presion_arterial
                }).FirstOrDefaultAsync();

            var p2 = await (
                from a in _context.SignoPacientes
                join b in _context.Pacientes on a.Id_paciente equals b.Id
                join c in _context.SignosVitales on a.id_signo equals c.Id
                where b.Codigo == cod2 && b.Estado == "Activo"
                orderby a.Fecha_hora descending
                select new
                {
                    b.Codigo,
                    c.Frecuencia_cardiaca,
                    c.Saturacion_oxigeno,
                    c.Presion_arterial
                }).FirstOrDefaultAsync();

            if (p1 == null || p2 == null)
                return BadRequest("No se pudo hacer la comparativa");

            return Ok(new
            {
                Paciente1 = p1,
                Paciente2 = p2
            });
        }
        [HttpGet("frecuencia-alertas")]
        public async Task<IActionResult> FrecuenciaAlertas(string codigo, DateTime fechaInicio, DateTime fechaFin)
        {
            var dato = await (
                from sp in _context.SignoPacientes
                join p in _context.Pacientes on sp.Id_paciente equals p.Id
                join sv in _context.SignosVitales on sp.id_signo equals sv.Id
                join sa in _context.SignoAlertas on sv.Id equals sa.Id_signo_vital
                join a in _context.Alertas on sa.Id_alerta equals a.Id
                where p.Codigo == codigo
                      && sp.Fecha_hora >= fechaInicio
                      && sp.Fecha_hora <= fechaFin
                select a.Id
            ).CountAsync();

            return Ok(new
            {
                Paciente = codigo,
                TotalAlertas = dato
            });
        }
        [HttpGet("variabilidad-signos")]
        public async Task<IActionResult> VariabilidadSignos(string fechaInicio, string fechaFin)
        {
            // Validación básica
            if (string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin))
                return BadRequest("Debe ingresar ambas fechas en formato yyyy-MM-dd");

            // Convertir a DateTime UTC (inicio del día y fin del día)
            DateTime inicio = DateTime.SpecifyKind(
                DateTime.Parse(fechaInicio).Date,
                DateTimeKind.Utc
            );

            DateTime fin = DateTime.SpecifyKind(
                DateTime.Parse(fechaFin).Date.AddDays(1).AddSeconds(-1),
                DateTimeKind.Utc
            );

            if (inicio > fin)
                return BadRequest("La fecha inicio no puede ser mayor que la fecha fin");

            var dato = await (
                from sp in _context.SignoPacientes
                join p in _context.Pacientes on sp.Id_paciente equals p.Id
                join sv in _context.SignosVitales on sp.id_signo equals sv.Id
                where sp.Fecha_hora >= inicio
                      && sp.Fecha_hora <= fin
                      && p.Estado == "Activo"
                group sv by p.Codigo into g
                select new
                {
                    Paciente = g.Key,
                    VariacionFC = g.Max(x => x.Frecuencia_cardiaca) - g.Min(x => x.Frecuencia_cardiaca),
                    VariacionOxigeno = g.Max(x => x.Saturacion_oxigeno) - g.Min(x => x.Saturacion_oxigeno),
                    Estado = (g.Max(x => x.Frecuencia_cardiaca) - g.Min(x => x.Frecuencia_cardiaca)) > 20
                                ? "Inestable"
                                : "Estable"
                }
            ).ToListAsync();

            if (!dato.Any())
                return NotFound("No hay datos en ese rango de fechas");

            return Ok(dato);
        }
    }
}
