using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MIS_Cuidados_Criticos.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Nivel_criticidad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Nomre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SignosVitales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Frecuencia_cardiaca = table.Column<int>(type: "integer", nullable: false),
                    Presion_arterial = table.Column<string>(type: "text", nullable: false),
                    Saturacion_oxigeno = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignosVitales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertaPacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_alerta = table.Column<int>(type: "integer", nullable: false),
                    Id_Paciente = table.Column<int>(type: "integer", nullable: false),
                    AlertaId = table.Column<int>(type: "integer", nullable: true),
                    PacienteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertaPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertaPacientes_Alertas_AlertaId",
                        column: x => x.AlertaId,
                        principalTable: "Alertas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlertaPacientes_Alertas_Id_alerta",
                        column: x => x.Id_alerta,
                        principalTable: "Alertas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertaPacientes_Pacientes_Id_Paciente",
                        column: x => x.Id_Paciente,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertaPacientes_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SignoAlertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_signo_vital = table.Column<int>(type: "integer", nullable: false),
                    Id_alerta = table.Column<int>(type: "integer", nullable: false),
                    AlertaId = table.Column<int>(type: "integer", nullable: true),
                    SignoVitalId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignoAlertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignoAlertas_Alertas_AlertaId",
                        column: x => x.AlertaId,
                        principalTable: "Alertas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SignoAlertas_Alertas_Id_alerta",
                        column: x => x.Id_alerta,
                        principalTable: "Alertas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignoAlertas_SignosVitales_Id_signo_vital",
                        column: x => x.Id_signo_vital,
                        principalTable: "SignosVitales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignoAlertas_SignosVitales_SignoVitalId",
                        column: x => x.SignoVitalId,
                        principalTable: "SignosVitales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SignoPacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_paciente = table.Column<int>(type: "integer", nullable: false),
                    id_signo = table.Column<int>(type: "integer", nullable: false),
                    Fecha_hora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PacienteId = table.Column<int>(type: "integer", nullable: true),
                    SignoVitalId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignoPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignoPacientes_Pacientes_Id_paciente",
                        column: x => x.Id_paciente,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignoPacientes_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SignoPacientes_SignosVitales_SignoVitalId",
                        column: x => x.SignoVitalId,
                        principalTable: "SignosVitales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SignoPacientes_SignosVitales_id_signo",
                        column: x => x.id_signo,
                        principalTable: "SignosVitales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertaPacientes_AlertaId",
                table: "AlertaPacientes",
                column: "AlertaId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertaPacientes_Id_alerta",
                table: "AlertaPacientes",
                column: "Id_alerta");

            migrationBuilder.CreateIndex(
                name: "IX_AlertaPacientes_Id_Paciente",
                table: "AlertaPacientes",
                column: "Id_Paciente");

            migrationBuilder.CreateIndex(
                name: "IX_AlertaPacientes_PacienteId",
                table: "AlertaPacientes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_SignoAlertas_AlertaId",
                table: "SignoAlertas",
                column: "AlertaId");

            migrationBuilder.CreateIndex(
                name: "IX_SignoAlertas_Id_alerta",
                table: "SignoAlertas",
                column: "Id_alerta");

            migrationBuilder.CreateIndex(
                name: "IX_SignoAlertas_Id_signo_vital",
                table: "SignoAlertas",
                column: "Id_signo_vital");

            migrationBuilder.CreateIndex(
                name: "IX_SignoAlertas_SignoVitalId",
                table: "SignoAlertas",
                column: "SignoVitalId");

            migrationBuilder.CreateIndex(
                name: "IX_SignoPacientes_Id_paciente",
                table: "SignoPacientes",
                column: "Id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_SignoPacientes_id_signo",
                table: "SignoPacientes",
                column: "id_signo");

            migrationBuilder.CreateIndex(
                name: "IX_SignoPacientes_PacienteId",
                table: "SignoPacientes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_SignoPacientes_SignoVitalId",
                table: "SignoPacientes",
                column: "SignoVitalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertaPacientes");

            migrationBuilder.DropTable(
                name: "SignoAlertas");

            migrationBuilder.DropTable(
                name: "SignoPacientes");

            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "SignosVitales");
        }
    }
}
