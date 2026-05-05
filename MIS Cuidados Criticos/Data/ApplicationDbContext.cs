using Microsoft.EntityFrameworkCore;
using MIS_Cuidados_Criticos.Dominio;
namespace MIS_Cuidados_Criticos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SignoVital> SignosVitales { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<SignoAlerta> SignoAlertas { get; set; }
        public DbSet<AlertaPaciente> AlertaPacientes { get; set; }
        public DbSet<SignoPaciente> SignoPacientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SignoAlerta>()
                .HasOne(sa => sa.SignoVital)
                .WithMany(s => s.signoAlertas)
                .HasForeignKey(sa => sa.Id_signo_vital);
            modelBuilder.Entity<SignoAlerta>()
                .HasOne(sa => sa.Alerta)
                .WithMany(a => a.SignoAlertas)
                .HasForeignKey(sa => sa.Id_alerta);
            modelBuilder.Entity<AlertaPaciente>()
                .HasOne(ap => ap.alerta)
                .WithMany()
                .HasForeignKey(ap => ap.Id_alerta);
            modelBuilder.Entity<AlertaPaciente>()
                .HasOne(ap => ap.paciente)
                .WithMany()
                .HasForeignKey(ap => ap.Id_Paciente);
            modelBuilder.Entity<SignoAlerta>()
                .HasOne(a => a.SignoVital)
                .WithMany()
                .HasForeignKey(b => b.Id_signo_vital);
            modelBuilder.Entity<SignoAlerta>()
                .HasOne(a => a.Alerta)
                .WithMany()
                .HasForeignKey(b => b.Id_alerta);
            modelBuilder.Entity<SignoPaciente>()
                .HasOne(a => a.signoVital)
                .WithMany()
                .HasForeignKey(b => b.id_signo);
            modelBuilder.Entity<SignoPaciente>()
                .HasOne(a => a.paciente)
                .WithMany()
                .HasForeignKey(b => b.Id_paciente);
        }
    }
}
