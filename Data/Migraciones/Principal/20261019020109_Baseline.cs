using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migraciones.Principal
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "db_principal");

            migrationBuilder.CreateTable(
                name: "Empresas",
                schema: "db_principal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ActividadEcononica = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    Tipo = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    IdentificacionRepresentanteLegal = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    NombreRepresentanteLegal = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Estatus = table.Column<short>(type: "smallint", nullable: false),
                    LlevaContable = table.Column<bool>(type: "bit", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PersonaContacto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Principal = table.Column<bool>(type: "bit", nullable: false),
                    IdentificacionEmpresaPadre = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                schema: "db_principal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modulos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "db_principal",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parametros",
                schema: "db_principal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parametros_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "db_principal",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                schema: "db_principal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QuantityInStock = table.Column<int>(type: "int", nullable: false),
                    MinimumStock = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "db_principal",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                schema: "db_principal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Productos_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "db_principal",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "db_principal",
                table: "Empresas",
                columns: new[] { "Id", "ActividadEcononica", "Codigo", "Correo", "Direccion", "Estatus", "FechaCreacion", "FechaModificacion", "Identificacion", "IdentificacionEmpresaPadre", "IdentificacionRepresentanteLegal", "LlevaContable", "Nombre", "NombreRepresentanteLegal", "PersonaContacto", "Principal", "RazonSocial", "Telefono", "Tipo", "UsuarioCreacionId", "UsuarioModificacionId" },
                values: new object[] { 1, null, "0000001", null, null, (short)1, null, null, "1250130620001", null, null, false, "EMPRESA PRINCIPAL", null, null, true, "EMPRESA PRINCIPAL", null, (short)1, null, null });

            migrationBuilder.InsertData(
                schema: "db_principal",
                table: "Modulos",
                columns: new[] { "Id", "Activo", "Codigo", "EmpresaId", "FechaCreacion", "FechaModificacion", "UsuarioCreacionId", "UsuarioModificacionId" },
                values: new object[,]
                {
                    { 1, true, "M000", 1, null, null, null, null },
                    { 2, true, "M003", 1, null, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Identificacion",
                schema: "db_principal",
                table: "Empresas",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_EmpresaId",
                schema: "db_principal",
                table: "Modulos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProductId",
                schema: "db_principal",
                table: "MovimientosStock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_EmpresaId",
                schema: "db_principal",
                table: "Parametros",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_EmpresaId_SKU",
                schema: "db_principal",
                table: "Productos",
                columns: new[] { "EmpresaId", "SKU" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modulos",
                schema: "db_principal");

            migrationBuilder.DropTable(
                name: "MovimientosStock",
                schema: "db_principal");

            migrationBuilder.DropTable(
                name: "Parametros",
                schema: "db_principal");

            migrationBuilder.DropTable(
                name: "Productos",
                schema: "db_principal");

            migrationBuilder.DropTable(
                name: "Empresas",
                schema: "db_principal");
        }
    }
}
