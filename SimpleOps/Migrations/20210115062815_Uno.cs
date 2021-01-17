using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleOps.Migrations
{
    public partial class Uno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aplicaciones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aplicaciones", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Campañas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campañas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Categorías",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorías", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Contactos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Teléfono = table.Column<string>(maxLength: 30, nullable: true),
                    Nombre = table.Column<string>(maxLength: 50, nullable: true),
                    EmailActivo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LíneasNegocio",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasNegocio", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    EnDescripción = table.Column<bool>(nullable: false),
                    PriorizarEnBuscador = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Materiales",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiales", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    Departamento = table.Column<string>(maxLength: 60, nullable: false),
                    Código = table.Column<string>(maxLength: 10, nullable: true),
                    OtroPaís = table.Column<string>(maxLength: 50, nullable: true),
                    MensajeríaDisponible = table.Column<bool>(nullable: false),
                    PorcentajeIVAPropio = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.ID);
                    table.UniqueConstraint("AK_Municipios_Nombre_Departamento", x => new { x.Nombre, x.Departamento });
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    Permisos = table.Column<string>(maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    EsRepresentanteComercial = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Teléfono = table.Column<string>(maxLength: 30, nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subcategorías",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    CategoríaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategorías", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Subcategorías_Categorías_CategoríaID",
                        column: x => x.CategoríaID,
                        principalTable: "Categorías",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 100, nullable: false),
                    NombreComercial = table.Column<string>(maxLength: 100, nullable: true),
                    TipoEntidad = table.Column<byte>(nullable: false),
                    TipoContribuyente = table.Column<int>(nullable: false),
                    Identificación = table.Column<string>(maxLength: 20, nullable: true),
                    Teléfono = table.Column<string>(maxLength: 30, nullable: true),
                    TeléfonoAlternativo = table.Column<string>(maxLength: 30, nullable: true),
                    Dirección = table.Column<string>(maxLength: 100, nullable: true),
                    MunicipioID = table.Column<int>(nullable: true),
                    Saldo = table.Column<double>(nullable: false),
                    ReferenciaEnBanco = table.Column<string>(maxLength: 100, nullable: true),
                    DescripciónEnBanco = table.Column<string>(maxLength: 100, nullable: true),
                    DíasCrédito = table.Column<int>(nullable: false),
                    CupoCrédito = table.Column<int>(nullable: true),
                    PorcentajeCostoTransporte = table.Column<double>(nullable: true),
                    PorcentajeDescuento = table.Column<double>(nullable: false),
                    CompraMínima = table.Column<double>(nullable: false),
                    DíasEntrega = table.Column<int>(nullable: true),
                    PrioridadPropia = table.Column<byte>(nullable: false),
                    Banco = table.Column<int>(nullable: false),
                    TipoCuentaBancaria = table.Column<byte>(nullable: false),
                    NúmeroCuentaBancaria = table.Column<string>(maxLength: 30, nullable: true),
                    ContactoPedidosID = table.Column<int>(nullable: true),
                    ContactoInformesPagosID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Proveedores_Contactos_ContactoInformesPagosID",
                        column: x => x.ContactoInformesPagosID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Proveedores_Contactos_ContactoPedidosID",
                        column: x => x.ContactoPedidosID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Proveedores_Municipios_MunicipioID",
                        column: x => x.MunicipioID,
                        principalTable: "Municipios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bloqueos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreEntidad = table.Column<string>(maxLength: 50, nullable: false),
                    EntidadID = table.Column<int>(nullable: true),
                    Propiedad = table.Column<string>(nullable: true),
                    Tipo = table.Column<int>(nullable: false),
                    UsuarioID = table.Column<int>(nullable: false),
                    FechaHoraInicio = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloqueos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bloqueos_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 100, nullable: false),
                    NombreComercial = table.Column<string>(maxLength: 100, nullable: true),
                    TipoEntidad = table.Column<byte>(nullable: false),
                    TipoContribuyente = table.Column<int>(nullable: false),
                    Identificación = table.Column<string>(maxLength: 20, nullable: true),
                    Teléfono = table.Column<string>(maxLength: 30, nullable: true),
                    TeléfonoAlternativo = table.Column<string>(maxLength: 30, nullable: true),
                    Dirección = table.Column<string>(maxLength: 100, nullable: true),
                    MunicipioID = table.Column<int>(nullable: true),
                    Saldo = table.Column<double>(nullable: false),
                    ReferenciaEnBanco = table.Column<string>(maxLength: 100, nullable: true),
                    DescripciónEnBanco = table.Column<string>(maxLength: 100, nullable: true),
                    DíasCrédito = table.Column<int>(nullable: false),
                    CupoCrédito = table.Column<int>(nullable: true),
                    ContactoFacturasID = table.Column<int>(nullable: true),
                    ContactoCobrosID = table.Column<int>(nullable: true),
                    TipoCliente = table.Column<byte>(nullable: false),
                    SubtipoCliente = table.Column<string>(maxLength: 50, nullable: true),
                    PorcentajeRetenciónIVAPropio = table.Column<double>(nullable: true),
                    PorcentajeRetenciónFuentePropio = table.Column<double>(nullable: true),
                    PorcentajeRetenciónICAPropio = table.Column<double>(nullable: true),
                    PorcentajeRetencionesExtraPropio = table.Column<double>(nullable: true),
                    MínimoRetenciónIVAPropio = table.Column<double>(nullable: true),
                    MínimoRetenciónFuentePropio = table.Column<double>(nullable: true),
                    MínimoRetenciónICAPropio = table.Column<double>(nullable: true),
                    MínimoRetencionesExtraPropio = table.Column<double>(nullable: true),
                    PrioridadPropia = table.Column<byte>(nullable: false),
                    FormaEntregaPropia = table.Column<byte>(nullable: false),
                    MínimoTransporteGratisPropio = table.Column<double>(nullable: true),
                    CopiasFacturaPropia = table.Column<int>(nullable: true),
                    PorcentajeGananciaPropio = table.Column<double>(nullable: true),
                    PorcentajeIVAPropio = table.Column<double>(nullable: true),
                    ObservacionesFactura = table.Column<string>(maxLength: 500, nullable: true),
                    RepresentanteComercialID = table.Column<int>(nullable: true),
                    CampañaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Clientes_Campañas_CampañaID",
                        column: x => x.CampañaID,
                        principalTable: "Campañas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Contactos_ContactoCobrosID",
                        column: x => x.ContactoCobrosID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Contactos_ContactoFacturasID",
                        column: x => x.ContactoFacturasID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Municipios_MunicipioID",
                        column: x => x.MunicipioID,
                        principalTable: "Municipios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Usuarios_RepresentanteComercialID",
                        column: x => x.RepresentanteComercialID,
                        principalTable: "Usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolesUsuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(nullable: false),
                    RolID = table.Column<int>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesUsuarios", x => new { x.RolID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_RolesUsuarios_Roles_RolID",
                        column: x => x.RolID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolesUsuarios_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactosProveedores",
                columns: table => new
                {
                    ProveedorID = table.Column<int>(nullable: false),
                    ContactoID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Tipo = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactosProveedores", x => new { x.ContactoID, x.ProveedorID });
                    table.ForeignKey(
                        name: "FK_ContactosProveedores_Contactos_ContactoID",
                        column: x => x.ContactoID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactosProveedores_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ContactoID = table.Column<int>(nullable: true),
                    Estado = table.Column<byte>(nullable: false),
                    Observaciones = table.Column<string>(maxLength: 500, nullable: true),
                    ProveedorID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pedidos_Contactos_ContactoID",
                        column: x => x.ContactoID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Referencia = table.Column<string>(maxLength: 30, nullable: false),
                    Unidad = table.Column<int>(nullable: false),
                    UnidadEmpaque = table.Column<int>(nullable: false),
                    PesoUnidadEmpaque = table.Column<double>(nullable: true),
                    DimensiónUnidadEmpaque_Alto = table.Column<double>(nullable: true),
                    DimensiónUnidadEmpaque_Ancho = table.Column<double>(nullable: true),
                    DimensiónUnidadEmpaque_Largo = table.Column<double>(nullable: true),
                    Descripción = table.Column<string>(maxLength: 200, nullable: true),
                    Cantidad = table.Column<int>(nullable: false),
                    CantidadMínima = table.Column<int>(nullable: false),
                    CantidadMáxima = table.Column<int>(nullable: false),
                    CantidadReservada = table.Column<int>(nullable: false),
                    Físico = table.Column<bool>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: true),
                    SubcategoríaID = table.Column<int>(nullable: true),
                    LíneaNegocioID = table.Column<int>(nullable: true),
                    MarcaID = table.Column<int>(nullable: true),
                    MaterialID = table.Column<int>(nullable: true),
                    AplicaciónID = table.Column<int>(nullable: true),
                    PrioridadWebPropia = table.Column<byte>(nullable: false),
                    ProveedorPreferidoID = table.Column<int>(nullable: true),
                    UbicaciónAlmacén = table.Column<string>(maxLength: 30, nullable: true),
                    PorcentajeIVAPropio = table.Column<double>(nullable: true),
                    ExcluídoIVA = table.Column<bool>(nullable: false),
                    PorcentajeImpuestoConsumoPropio = table.Column<double>(nullable: true),
                    ImpuestoConsumoUnitarioPropio = table.Column<double>(nullable: true),
                    TipoImpuestoConsumoPropio = table.Column<byte>(nullable: false),
                    ProductosAsociados = table.Column<string>(maxLength: 1000, nullable: false),
                    PorcentajeAdicionalGananciaPropio = table.Column<double>(nullable: true),
                    ConceptoRetenciónPropio = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Productos_Aplicaciones_AplicaciónID",
                        column: x => x.AplicaciónID,
                        principalTable: "Aplicaciones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_LíneasNegocio_LíneaNegocioID",
                        column: x => x.LíneaNegocioID,
                        principalTable: "LíneasNegocio",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Marcas_MarcaID",
                        column: x => x.MarcaID,
                        principalTable: "Marcas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Materiales_MaterialID",
                        column: x => x.MaterialID,
                        principalTable: "Materiales",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorPreferidoID",
                        column: x => x.ProveedorPreferidoID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Subcategorías_SubcategoríaID",
                        column: x => x.SubcategoríaID,
                        principalTable: "Subcategorías",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cobros",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    ClienteID = table.Column<int>(nullable: false),
                    NúmerosFacturas = table.Column<string>(maxLength: 1000, nullable: false),
                    Total = table.Column<double>(nullable: false),
                    MáximoDíasVencimiento = table.Column<int>(nullable: false),
                    Respuesta = table.Column<string>(maxLength: 500, nullable: true),
                    Tipo = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cobros", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cobros_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesEgresos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHora = table.Column<string>(nullable: false),
                    ValorFacturas = table.Column<double>(nullable: false),
                    Abono = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    Lugar = table.Column<byte>(nullable: false),
                    ClienteID = table.Column<int>(nullable: true),
                    ProveedorID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesEgresos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ComprobantesEgresos_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobantesEgresos_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactosClientes",
                columns: table => new
                {
                    ClienteID = table.Column<int>(nullable: false),
                    ContactoID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ObservacionesFactura = table.Column<string>(maxLength: 500, nullable: true),
                    Tipo = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactosClientes", x => new { x.ContactoID, x.ClienteID });
                    table.ForeignKey(
                        name: "FK_ContactosClientes_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactosClientes_Contactos_ContactoID",
                        column: x => x.ContactoID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    ClienteID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InformesPagos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    ClienteID = table.Column<int>(nullable: false),
                    Valor = table.Column<double>(nullable: false),
                    FechaHoraPago = table.Column<DateTime>(nullable: false),
                    Banco = table.Column<int>(nullable: false),
                    OtroNúmeroCuenta = table.Column<string>(maxLength: 30, nullable: true),
                    Observaciones = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformesPagos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InformesPagos_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecibosCaja",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHora = table.Column<string>(nullable: false),
                    ValorFacturas = table.Column<double>(nullable: false),
                    Abono = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    Lugar = table.Column<byte>(nullable: false),
                    ClienteID = table.Column<int>(nullable: true),
                    ProveedorID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecibosCaja", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecibosCaja_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecibosCaja_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    ClienteID = table.Column<int>(nullable: false),
                    ContactoID = table.Column<int>(nullable: true),
                    ObservacionesEnvío = table.Column<string>(maxLength: 500, nullable: true),
                    MunicipioID = table.Column<int>(nullable: false),
                    Dirección = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.ID);
                    table.UniqueConstraint("AK_Sedes_Nombre_ClienteID", x => new { x.Nombre, x.ClienteID });
                    table.ForeignKey(
                        name: "FK_Sedes_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sedes_Contactos_ContactoID",
                        column: x => x.ContactoID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sedes_Municipios_MunicipioID",
                        column: x => x.MunicipioID,
                        principalTable: "Municipios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventariosConsignación",
                columns: table => new
                {
                    ClienteID = table.Column<int>(nullable: false),
                    ProductoID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: true),
                    Precio = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventariosConsignación", x => new { x.ProductoID, x.ClienteID });
                    table.ForeignKey(
                        name: "FK_InventariosConsignación_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventariosConsignación_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasPedidos",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    PedidoID = table.Column<int>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    CantidadEntregada = table.Column<int>(nullable: false),
                    FechaHoraCumplimiento = table.Column<string>(nullable: true),
                    Estado = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasPedidos", x => new { x.PedidoID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasPedidos_Pedidos_PedidoID",
                        column: x => x.PedidoID,
                        principalTable: "Pedidos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasPedidos_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListasPrecios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ProductoID = table.Column<int>(nullable: false),
                    Valor = table.Column<double>(nullable: false),
                    Protegido = table.Column<bool>(nullable: false),
                    TipoCliente = table.Column<byte>(nullable: false),
                    SubtipoCliente = table.Column<string>(maxLength: 50, nullable: true),
                    MáximoDíasCrédito = table.Column<int>(nullable: true),
                    TieneRepresentanteComercial = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPrecios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ListasPrecios_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreciosClientes",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    ClienteID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Valor = table.Column<double>(nullable: false),
                    Protegido = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosClientes", x => new { x.ProductoID, x.ClienteID });
                    table.ForeignKey(
                        name: "FK_PreciosClientes_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreciosClientes_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreciosProveedores",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    ProveedorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Valor = table.Column<double>(nullable: false),
                    Protegido = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosProveedores", x => new { x.ProductoID, x.ProveedorID });
                    table.ForeignKey(
                        name: "FK_PreciosProveedores_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreciosProveedores_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferenciasClientes",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    ClienteID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Valor = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenciasClientes", x => new { x.ProductoID, x.ClienteID });
                    table.ForeignKey(
                        name: "FK_ReferenciasClientes_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferenciasClientes_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferenciasProveedores",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    ProveedorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Valor = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenciasProveedores", x => new { x.ProductoID, x.ProveedorID });
                    table.ForeignKey(
                        name: "FK_ReferenciasProveedores_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferenciasProveedores_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Número = table.Column<int>(nullable: false),
                    Prefijo = table.Column<string>(maxLength: 10, nullable: true),
                    FechaHora = table.Column<string>(nullable: false),
                    DescuentoCondicionado = table.Column<double>(nullable: false),
                    DescuentoComercial = table.Column<double>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    IVA = table.Column<double>(nullable: false),
                    ImpuestoConsumo = table.Column<double>(nullable: false),
                    RetenciónFuente = table.Column<double>(nullable: false),
                    RetenciónIVA = table.Column<double>(nullable: false),
                    RetenciónICA = table.Column<double>(nullable: false),
                    RetencionesExtra = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    ConsecutivoDianAnual = table.Column<int>(nullable: true),
                    Cude = table.Column<string>(maxLength: 96, nullable: true),
                    ProveedorID = table.Column<int>(nullable: false),
                    ComprobanteEgresoID = table.Column<int>(nullable: true),
                    PedidoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Compras_ComprobantesEgresos_ComprobanteEgresoID",
                        column: x => x.ComprobanteEgresoID,
                        principalTable: "ComprobantesEgresos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Compras_Pedidos_PedidoID",
                        column: x => x.PedidoID,
                        principalTable: "Pedidos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Compras_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasCotizaciones",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    CotizaciónID = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasCotizaciones", x => new { x.CotizaciónID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasCotizaciones_Cotizaciones_CotizaciónID",
                        column: x => x.CotizaciónID,
                        principalTable: "Cotizaciones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasCotizaciones_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosBancarios",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHora = table.Column<string>(nullable: false),
                    Valor = table.Column<double>(nullable: false),
                    ReciboCajaID = table.Column<int>(nullable: true),
                    ComprobanteEgresoID = table.Column<int>(nullable: true),
                    Observaciones = table.Column<string>(maxLength: 500, nullable: true),
                    Estado = table.Column<byte>(nullable: false),
                    Banco = table.Column<int>(nullable: false),
                    OtroNúmeroCuenta = table.Column<string>(maxLength: 30, nullable: true),
                    Sucursal = table.Column<string>(maxLength: 50, nullable: true),
                    Descripción = table.Column<string>(maxLength: 100, nullable: true),
                    Referencia = table.Column<string>(maxLength: 100, nullable: true),
                    PadreID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosBancarios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MovimientosBancarios_ComprobantesEgresos_ComprobanteEgresoID",
                        column: x => x.ComprobanteEgresoID,
                        principalTable: "ComprobantesEgresos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosBancarios_MovimientosBancarios_PadreID",
                        column: x => x.PadreID,
                        principalTable: "MovimientosBancarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosBancarios_RecibosCaja_ReciboCajaID",
                        column: x => x.ReciboCajaID,
                        principalTable: "RecibosCaja",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosEfectivo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    Valor = table.Column<double>(nullable: false),
                    ReciboCajaID = table.Column<int>(nullable: true),
                    ComprobanteEgresoID = table.Column<int>(nullable: true),
                    Observaciones = table.Column<string>(maxLength: 500, nullable: true),
                    Estado = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosEfectivo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MovimientosEfectivo_ComprobantesEgresos_ComprobanteEgresoID",
                        column: x => x.ComprobanteEgresoID,
                        principalTable: "ComprobantesEgresos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosEfectivo_RecibosCaja_ReciboCajaID",
                        column: x => x.ReciboCajaID,
                        principalTable: "RecibosCaja",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesCompra",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ContactoID = table.Column<int>(nullable: true),
                    Estado = table.Column<byte>(nullable: false),
                    Observaciones = table.Column<string>(maxLength: 500, nullable: true),
                    ClienteID = table.Column<int>(nullable: false),
                    Número = table.Column<string>(maxLength: 30, nullable: false),
                    SedeID = table.Column<int>(nullable: true),
                    EnviadaProforma = table.Column<bool>(nullable: false),
                    Remisionar = table.Column<bool>(nullable: false),
                    SincronizadaWeb = table.Column<bool>(nullable: false),
                    Prioridad = table.Column<byte>(nullable: false),
                    InformePagoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompra", x => x.ID);
                    table.UniqueConstraint("AK_OrdenesCompra_Número_ClienteID", x => new { x.Número, x.ClienteID });
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Contactos_ContactoID",
                        column: x => x.ContactoID,
                        principalTable: "Contactos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_InformesPagos_InformePagoID",
                        column: x => x.InformePagoID,
                        principalTable: "InformesPagos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Sedes_SedeID",
                        column: x => x.SedeID,
                        principalTable: "Sedes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasCompras",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    CompraID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasCompras", x => new { x.CompraID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasCompras_Compras_CompraID",
                        column: x => x.CompraID,
                        principalTable: "Compras",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasCompras_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasCréditoCompra",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Número = table.Column<int>(nullable: false),
                    Prefijo = table.Column<string>(maxLength: 10, nullable: true),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    DescuentoCondicionado = table.Column<double>(nullable: false),
                    DescuentoComercial = table.Column<double>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    IVA = table.Column<double>(nullable: false),
                    ImpuestoConsumo = table.Column<double>(nullable: false),
                    RetenciónFuente = table.Column<double>(nullable: false),
                    RetenciónIVA = table.Column<double>(nullable: false),
                    RetenciónICA = table.Column<double>(nullable: false),
                    RetencionesExtra = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    ConsecutivoDianAnual = table.Column<int>(nullable: true),
                    Cude = table.Column<string>(maxLength: 96, nullable: true),
                    ProveedorID = table.Column<int>(nullable: false),
                    CompraID = table.Column<int>(nullable: false),
                    Razón = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCréditoCompra", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotasCréditoCompra_Compras_CompraID",
                        column: x => x.CompraID,
                        principalTable: "Compras",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCréditoCompra_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasDébitoCompra",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Número = table.Column<int>(nullable: false),
                    Prefijo = table.Column<string>(maxLength: 10, nullable: true),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    DescuentoCondicionado = table.Column<double>(nullable: false),
                    DescuentoComercial = table.Column<double>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    IVA = table.Column<double>(nullable: false),
                    ImpuestoConsumo = table.Column<double>(nullable: false),
                    RetenciónFuente = table.Column<double>(nullable: false),
                    RetenciónIVA = table.Column<double>(nullable: false),
                    RetenciónICA = table.Column<double>(nullable: false),
                    RetencionesExtra = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    ConsecutivoDianAnual = table.Column<int>(nullable: true),
                    Cude = table.Column<string>(maxLength: 96, nullable: true),
                    ProveedorID = table.Column<int>(nullable: false),
                    CompraID = table.Column<int>(nullable: false),
                    Razón = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasDébitoCompra", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotasDébitoCompra_Compras_CompraID",
                        column: x => x.CompraID,
                        principalTable: "Compras",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDébitoCompra_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasOrdenesCompra",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    OrdenCompraID = table.Column<int>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    CantidadEntregada = table.Column<int>(nullable: false),
                    FechaHoraCumplimiento = table.Column<string>(nullable: true),
                    Estado = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasOrdenesCompra", x => new { x.OrdenCompraID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasOrdenesCompra_OrdenesCompra_OrdenCompraID",
                        column: x => x.OrdenCompraID,
                        principalTable: "OrdenesCompra",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasOrdenesCompra_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Número = table.Column<int>(nullable: false),
                    Prefijo = table.Column<string>(maxLength: 10, nullable: true),
                    FechaHora = table.Column<string>(nullable: false),
                    DescuentoCondicionado = table.Column<double>(nullable: false),
                    DescuentoComercial = table.Column<double>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    IVA = table.Column<double>(nullable: false),
                    ImpuestoConsumo = table.Column<double>(nullable: false),
                    RetenciónFuente = table.Column<double>(nullable: false),
                    RetenciónIVA = table.Column<double>(nullable: false),
                    RetenciónICA = table.Column<double>(nullable: false),
                    RetencionesExtra = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    ConsecutivoDianAnual = table.Column<int>(nullable: true),
                    Cude = table.Column<string>(maxLength: 96, nullable: true),
                    ClienteID = table.Column<int>(nullable: false),
                    FechaPagoComisiónEnVenta = table.Column<DateTime>(nullable: true),
                    FechaPagoComisiónEnPago = table.Column<DateTime>(nullable: true),
                    DetalleEntrega = table.Column<string>(maxLength: 500, nullable: true),
                    DeInventarioConsignación = table.Column<bool>(nullable: false),
                    ReciboCajaID = table.Column<int>(nullable: true),
                    InformePagoID = table.Column<int>(nullable: true),
                    OrdenCompraID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_InformesPagos_InformePagoID",
                        column: x => x.InformePagoID,
                        principalTable: "InformesPagos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_OrdenesCompra_OrdenCompraID",
                        column: x => x.OrdenCompraID,
                        principalTable: "OrdenesCompra",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_RecibosCaja_ReciboCajaID",
                        column: x => x.ReciboCajaID,
                        principalTable: "RecibosCaja",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasNotasCréditoCompra",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    NotaCréditoCompraID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasNotasCréditoCompra", x => new { x.NotaCréditoCompraID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasNotasCréditoCompra_NotasCréditoCompra_NotaCréditoCompraID",
                        column: x => x.NotaCréditoCompraID,
                        principalTable: "NotasCréditoCompra",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasNotasCréditoCompra_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasNotasDébitoCompra",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    NotaDébitoCompraID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasNotasDébitoCompra", x => new { x.NotaDébitoCompraID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasNotasDébitoCompra_NotasDébitoCompra_NotaDébitoCompraID",
                        column: x => x.NotaDébitoCompraID,
                        principalTable: "NotasDébitoCompra",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasNotasDébitoCompra_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasVentas",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    VentaID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasVentas", x => new { x.VentaID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasVentas_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasVentas_Ventas_VentaID",
                        column: x => x.VentaID,
                        principalTable: "Ventas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasCréditoVenta",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Número = table.Column<int>(nullable: false),
                    Prefijo = table.Column<string>(maxLength: 10, nullable: true),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    DescuentoCondicionado = table.Column<double>(nullable: false),
                    DescuentoComercial = table.Column<double>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    IVA = table.Column<double>(nullable: false),
                    ImpuestoConsumo = table.Column<double>(nullable: false),
                    RetenciónFuente = table.Column<double>(nullable: false),
                    RetenciónIVA = table.Column<double>(nullable: false),
                    RetenciónICA = table.Column<double>(nullable: false),
                    RetencionesExtra = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    ConsecutivoDianAnual = table.Column<int>(nullable: true),
                    Cude = table.Column<string>(maxLength: 96, nullable: true),
                    ClienteID = table.Column<int>(nullable: false),
                    VentaID = table.Column<int>(nullable: false),
                    Razón = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasCréditoVenta", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotasCréditoVenta_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasCréditoVenta_Ventas_VentaID",
                        column: x => x.VentaID,
                        principalTable: "Ventas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasDébitoVenta",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    CreadorID = table.Column<int>(nullable: false),
                    Número = table.Column<int>(nullable: false),
                    Prefijo = table.Column<string>(maxLength: 10, nullable: true),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    DescuentoCondicionado = table.Column<double>(nullable: false),
                    DescuentoComercial = table.Column<double>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    IVA = table.Column<double>(nullable: false),
                    ImpuestoConsumo = table.Column<double>(nullable: false),
                    RetenciónFuente = table.Column<double>(nullable: false),
                    RetenciónIVA = table.Column<double>(nullable: false),
                    RetenciónICA = table.Column<double>(nullable: false),
                    RetencionesExtra = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    ConsecutivoDianAnual = table.Column<int>(nullable: true),
                    Cude = table.Column<string>(maxLength: 96, nullable: true),
                    ClienteID = table.Column<int>(nullable: false),
                    VentaID = table.Column<int>(nullable: false),
                    Razón = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasDébitoVenta", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotasDébitoVenta_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasDébitoVenta_Ventas_VentaID",
                        column: x => x.VentaID,
                        principalTable: "Ventas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Remisiones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreadorID = table.Column<int>(nullable: false),
                    ActualizadorID = table.Column<int>(nullable: false),
                    FechaHoraCreación = table.Column<string>(nullable: false),
                    FechaHoraActualización = table.Column<string>(nullable: false),
                    ClienteID = table.Column<int>(nullable: false),
                    Subtotal = table.Column<double>(nullable: false),
                    Estado = table.Column<byte>(nullable: false),
                    DetalleEntrega = table.Column<string>(maxLength: 500, nullable: true),
                    VentaID = table.Column<int>(nullable: true),
                    OrdenCompraID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remisiones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Remisiones_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Remisiones_OrdenesCompra_OrdenCompraID",
                        column: x => x.OrdenCompraID,
                        principalTable: "OrdenesCompra",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Remisiones_Ventas_VentaID",
                        column: x => x.VentaID,
                        principalTable: "Ventas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasNotasCréditoVenta",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    NotaCréditoVentaID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasNotasCréditoVenta", x => new { x.NotaCréditoVentaID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasNotasCréditoVenta_NotasCréditoVenta_NotaCréditoVentaID",
                        column: x => x.NotaCréditoVentaID,
                        principalTable: "NotasCréditoVenta",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasNotasCréditoVenta_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasNotasDébitoVenta",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    NotaDébitoVentaID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasNotasDébitoVenta", x => new { x.NotaDébitoVentaID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasNotasDébitoVenta_NotasDébitoVenta_NotaDébitoVentaID",
                        column: x => x.NotaDébitoVentaID,
                        principalTable: "NotasDébitoVenta",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasNotasDébitoVenta_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LíneasRemisiones",
                columns: table => new
                {
                    ProductoID = table.Column<int>(nullable: false),
                    RemisiónID = table.Column<int>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    CostoUnitario = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LíneasRemisiones", x => new { x.RemisiónID, x.ProductoID });
                    table.ForeignKey(
                        name: "FK_LíneasRemisiones_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LíneasRemisiones_Remisiones_RemisiónID",
                        column: x => x.RemisiónID,
                        principalTable: "Remisiones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aplicaciones_Nombre",
                table: "Aplicaciones",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bloqueos_UsuarioID",
                table: "Bloqueos",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Campañas_Nombre",
                table: "Campañas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorías_Nombre",
                table: "Categorías",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CampañaID",
                table: "Clientes",
                column: "CampañaID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ContactoCobrosID",
                table: "Clientes",
                column: "ContactoCobrosID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ContactoFacturasID",
                table: "Clientes",
                column: "ContactoFacturasID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_MunicipioID",
                table: "Clientes",
                column: "MunicipioID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Nombre",
                table: "Clientes",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_RepresentanteComercialID",
                table: "Clientes",
                column: "RepresentanteComercialID");

            migrationBuilder.CreateIndex(
                name: "IX_Cobros_ClienteID",
                table: "Cobros",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ComprobanteEgresoID",
                table: "Compras",
                column: "ComprobanteEgresoID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_PedidoID",
                table: "Compras",
                column: "PedidoID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ProveedorID",
                table: "Compras",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesEgresos_ClienteID",
                table: "ComprobantesEgresos",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesEgresos_ProveedorID",
                table: "ComprobantesEgresos",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_Email",
                table: "Contactos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactosClientes_ClienteID",
                table: "ContactosClientes",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosProveedores_ProveedorID",
                table: "ContactosProveedores",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteID",
                table: "Cotizaciones",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_InformesPagos_ClienteID",
                table: "InformesPagos",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_InventariosConsignación_ClienteID",
                table: "InventariosConsignación",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasCompras_ProductoID",
                table: "LíneasCompras",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasCotizaciones_ProductoID",
                table: "LíneasCotizaciones",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasNegocio_Nombre",
                table: "LíneasNegocio",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LíneasNotasCréditoCompra_ProductoID",
                table: "LíneasNotasCréditoCompra",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasNotasCréditoVenta_ProductoID",
                table: "LíneasNotasCréditoVenta",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasNotasDébitoCompra_ProductoID",
                table: "LíneasNotasDébitoCompra",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasNotasDébitoVenta_ProductoID",
                table: "LíneasNotasDébitoVenta",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasOrdenesCompra_ProductoID",
                table: "LíneasOrdenesCompra",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasPedidos_ProductoID",
                table: "LíneasPedidos",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasRemisiones_ProductoID",
                table: "LíneasRemisiones",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_LíneasVentas_ProductoID",
                table: "LíneasVentas",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_ListasPrecios_ProductoID",
                table: "ListasPrecios",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_Marcas_Nombre",
                table: "Marcas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_Nombre",
                table: "Materiales",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBancarios_ComprobanteEgresoID",
                table: "MovimientosBancarios",
                column: "ComprobanteEgresoID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBancarios_PadreID",
                table: "MovimientosBancarios",
                column: "PadreID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBancarios_ReciboCajaID",
                table: "MovimientosBancarios",
                column: "ReciboCajaID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosEfectivo_ComprobanteEgresoID",
                table: "MovimientosEfectivo",
                column: "ComprobanteEgresoID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosEfectivo_ReciboCajaID",
                table: "MovimientosEfectivo",
                column: "ReciboCajaID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCréditoCompra_CompraID",
                table: "NotasCréditoCompra",
                column: "CompraID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCréditoCompra_ProveedorID",
                table: "NotasCréditoCompra",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCréditoVenta_ClienteID",
                table: "NotasCréditoVenta",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasCréditoVenta_VentaID",
                table: "NotasCréditoVenta",
                column: "VentaID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDébitoCompra_CompraID",
                table: "NotasDébitoCompra",
                column: "CompraID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDébitoCompra_ProveedorID",
                table: "NotasDébitoCompra",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDébitoVenta_ClienteID",
                table: "NotasDébitoVenta",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_NotasDébitoVenta_VentaID",
                table: "NotasDébitoVenta",
                column: "VentaID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ClienteID",
                table: "OrdenesCompra",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ContactoID",
                table: "OrdenesCompra",
                column: "ContactoID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_InformePagoID",
                table: "OrdenesCompra",
                column: "InformePagoID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_SedeID",
                table: "OrdenesCompra",
                column: "SedeID");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ContactoID",
                table: "Pedidos",
                column: "ContactoID");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ProveedorID",
                table: "Pedidos",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosClientes_ClienteID",
                table: "PreciosClientes",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosProveedores_ProveedorID",
                table: "PreciosProveedores",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_AplicaciónID",
                table: "Productos",
                column: "AplicaciónID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_LíneaNegocioID",
                table: "Productos",
                column: "LíneaNegocioID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_MarcaID",
                table: "Productos",
                column: "MarcaID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_MaterialID",
                table: "Productos",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorPreferidoID",
                table: "Productos",
                column: "ProveedorPreferidoID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Referencia",
                table: "Productos",
                column: "Referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_SubcategoríaID",
                table: "Productos",
                column: "SubcategoríaID");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_ContactoInformesPagosID",
                table: "Proveedores",
                column: "ContactoInformesPagosID");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_ContactoPedidosID",
                table: "Proveedores",
                column: "ContactoPedidosID");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_MunicipioID",
                table: "Proveedores",
                column: "MunicipioID");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_Nombre",
                table: "Proveedores",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecibosCaja_ClienteID",
                table: "RecibosCaja",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosCaja_ProveedorID",
                table: "RecibosCaja",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenciasClientes_ClienteID",
                table: "ReferenciasClientes",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenciasProveedores_ProveedorID",
                table: "ReferenciasProveedores",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_Remisiones_ClienteID",
                table: "Remisiones",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Remisiones_OrdenCompraID",
                table: "Remisiones",
                column: "OrdenCompraID");

            migrationBuilder.CreateIndex(
                name: "IX_Remisiones_VentaID",
                table: "Remisiones",
                column: "VentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesUsuarios_UsuarioID",
                table: "RolesUsuarios",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_ClienteID",
                table: "Sedes",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_ContactoID",
                table: "Sedes",
                column: "ContactoID");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_MunicipioID",
                table: "Sedes",
                column: "MunicipioID");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategorías_CategoríaID",
                table: "Subcategorías",
                column: "CategoríaID");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategorías_Nombre",
                table: "Subcategorías",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Nombre",
                table: "Usuarios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteID",
                table: "Ventas",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_InformePagoID",
                table: "Ventas",
                column: "InformePagoID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_OrdenCompraID",
                table: "Ventas",
                column: "OrdenCompraID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ReciboCajaID",
                table: "Ventas",
                column: "ReciboCajaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bloqueos");

            migrationBuilder.DropTable(
                name: "Cobros");

            migrationBuilder.DropTable(
                name: "ContactosClientes");

            migrationBuilder.DropTable(
                name: "ContactosProveedores");

            migrationBuilder.DropTable(
                name: "InventariosConsignación");

            migrationBuilder.DropTable(
                name: "LíneasCompras");

            migrationBuilder.DropTable(
                name: "LíneasCotizaciones");

            migrationBuilder.DropTable(
                name: "LíneasNotasCréditoCompra");

            migrationBuilder.DropTable(
                name: "LíneasNotasCréditoVenta");

            migrationBuilder.DropTable(
                name: "LíneasNotasDébitoCompra");

            migrationBuilder.DropTable(
                name: "LíneasNotasDébitoVenta");

            migrationBuilder.DropTable(
                name: "LíneasOrdenesCompra");

            migrationBuilder.DropTable(
                name: "LíneasPedidos");

            migrationBuilder.DropTable(
                name: "LíneasRemisiones");

            migrationBuilder.DropTable(
                name: "LíneasVentas");

            migrationBuilder.DropTable(
                name: "ListasPrecios");

            migrationBuilder.DropTable(
                name: "MovimientosBancarios");

            migrationBuilder.DropTable(
                name: "MovimientosEfectivo");

            migrationBuilder.DropTable(
                name: "PreciosClientes");

            migrationBuilder.DropTable(
                name: "PreciosProveedores");

            migrationBuilder.DropTable(
                name: "ReferenciasClientes");

            migrationBuilder.DropTable(
                name: "ReferenciasProveedores");

            migrationBuilder.DropTable(
                name: "RolesUsuarios");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "NotasCréditoCompra");

            migrationBuilder.DropTable(
                name: "NotasCréditoVenta");

            migrationBuilder.DropTable(
                name: "NotasDébitoCompra");

            migrationBuilder.DropTable(
                name: "NotasDébitoVenta");

            migrationBuilder.DropTable(
                name: "Remisiones");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Aplicaciones");

            migrationBuilder.DropTable(
                name: "LíneasNegocio");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "Materiales");

            migrationBuilder.DropTable(
                name: "Subcategorías");

            migrationBuilder.DropTable(
                name: "ComprobantesEgresos");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "OrdenesCompra");

            migrationBuilder.DropTable(
                name: "RecibosCaja");

            migrationBuilder.DropTable(
                name: "Categorías");

            migrationBuilder.DropTable(
                name: "InformesPagos");

            migrationBuilder.DropTable(
                name: "Sedes");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Campañas");

            migrationBuilder.DropTable(
                name: "Contactos");

            migrationBuilder.DropTable(
                name: "Municipios");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
