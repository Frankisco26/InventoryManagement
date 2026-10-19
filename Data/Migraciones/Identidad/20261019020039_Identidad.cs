using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migraciones.Identidad
{
    /// <inheritdoc />
    public partial class Identidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "db_identidad");

            migrationBuilder.CreateTable(
                name: "Pantallas",
                schema: "db_identidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pantallas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "db_identidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentificacionEmpresaPadre = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Empresa = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    PermiteCrear = table.Column<bool>(type: "bit", nullable: false),
                    PermiteEditar = table.Column<bool>(type: "bit", nullable: false),
                    PermiteAnular = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "db_identidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentificacionEmpresaPadre = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Empresa = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesPantallas",
                schema: "db_identidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    PantallaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesPantallas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesPantallas_Pantallas_PantallaId",
                        column: x => x.PantallaId,
                        principalSchema: "db_identidad",
                        principalTable: "Pantallas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesPantallas_Roles_RolId",
                        column: x => x.RolId,
                        principalSchema: "db_identidad",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolesReclamos",
                schema: "db_identidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesReclamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesReclamos_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "db_identidad",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosAccesos",
                schema: "db_identidad",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosAccesos", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UsuariosAccesos_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_identidad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosReclamos",
                schema: "db_identidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosReclamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosReclamos_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_identidad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                schema: "db_identidad",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "db_identidad",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_identidad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosTokens",
                schema: "db_identidad",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UsuariosTokens_Usuarios_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_identidad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "db_identidad",
                table: "Pantallas",
                columns: new[] { "Id", "Action", "Active", "Area", "Controller", "Module", "Name", "Order" },
                values: new object[,]
                {
                    { 1, "Inicio", true, "Configuracion", "Empresas", "M000", "Empresas", (short)1 },
                    { 2, "Inicio", true, "Configuracion", "Usuarios", "M000", "Usuarios", (short)2 },
                    { 3, "Inicio", true, "Configuracion", "Rol", "M000", "Permisos", (short)3 },
                    { 4, "Inicio", true, "Stock", "Productos", "M003", "Productos", (short)1 },
                    { 5, "Inicio", true, "Stock", "Movimientos", "M003", "Movimientos", (short)2 }
                });

            migrationBuilder.InsertData(
                schema: "db_identidad",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Empresa", "FechaCreacion", "FechaModificacion", "IdentificacionEmpresaPadre", "Name", "NormalizedName", "PermiteAnular", "PermiteCrear", "PermiteEditar", "UsuarioCreacionId", "UsuarioModificacionId" },
                values: new object[] { 1, "5d1e6c1a-6f43-4c1c-9a39-2c1f0f6a3b11", "1250130620001", null, null, null, "Administrador", "ADMINISTRADOR", true, true, true, null, null });

            migrationBuilder.InsertData(
                schema: "db_identidad",
                table: "Usuarios",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Empresa", "FechaCreacion", "FechaModificacion", "IdentificacionEmpresaPadre", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UsuarioCreacionId", "UsuarioModificacionId" },
                values: new object[] { 1, 0, "48ab553d-012b-4098-844a-560d62b08bff", "1727495366001", true, "1250130620001", null, new DateTime(2026, 10, 13, 17, 44, 40, 312, DateTimeKind.Unspecified).AddTicks(8231), null, false, null, "MANTENIMIENTO", "1727495366001", "1727495366001", "AQAAAAIAAYagAAAAEHlHGRmIMuQlN/XPKn0GUK7WPf+5vtvh8ZWm+Q8lTtX4nQizBE44uFjL2q5+1WVIrQ==", null, false, "SP3KWAFXBUEQNJWYNX3ZXH5PNQ3FVKN5", false, "1727495366001", null, 1 });

            migrationBuilder.InsertData(
                schema: "db_identidad",
                table: "RolesPantallas",
                columns: new[] { "Id", "PantallaId", "RolId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 5, 1 }
                });

            migrationBuilder.InsertData(
                schema: "db_identidad",
                table: "UsuariosReclamos",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "MANTENIMIENTO", 1 },
                    { 2, "http://schemas.xmlsoap.org/ws/2019/05/identity/claims/businessidentifier", "1250130620001", 1 }
                });

            migrationBuilder.InsertData(
                schema: "db_identidad",
                table: "UsuariosRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Pantallas_Area_Controller_Action",
                schema: "db_identidad",
                table: "Pantallas",
                columns: new[] { "Area", "Controller", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_Pantallas_Module",
                schema: "db_identidad",
                table: "Pantallas",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_Pantallas_Order",
                schema: "db_identidad",
                table: "Pantallas",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Empresa",
                schema: "db_identidad",
                table: "Roles",
                column: "Empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name_Empresa",
                schema: "db_identidad",
                table: "Roles",
                columns: new[] { "Name", "Empresa" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName",
                schema: "db_identidad",
                table: "Roles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_RolesPantallas_PantallaId",
                schema: "db_identidad",
                table: "RolesPantallas",
                column: "PantallaId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesPantallas_RolId",
                schema: "db_identidad",
                table: "RolesPantallas",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesReclamos_RoleId",
                schema: "db_identidad",
                table: "RolesReclamos",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "db_identidad",
                table: "Usuarios",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Empresa",
                schema: "db_identidad",
                table: "Usuarios",
                column: "Empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Name_Empresa",
                schema: "db_identidad",
                table: "Usuarios",
                columns: new[] { "Name", "Empresa" },
                unique: true,
                filter: "[Empresa] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "db_identidad",
                table: "Usuarios",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosAccesos_UserId",
                schema: "db_identidad",
                table: "UsuariosAccesos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosReclamos_UserId",
                schema: "db_identidad",
                table: "UsuariosReclamos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_RoleId",
                schema: "db_identidad",
                table: "UsuariosRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolesPantallas",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "RolesReclamos",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "UsuariosAccesos",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "UsuariosReclamos",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "UsuariosRoles",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "UsuariosTokens",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "Pantallas",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "db_identidad");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "db_identidad");
        }
    }
}
