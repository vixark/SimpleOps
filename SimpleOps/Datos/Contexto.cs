// Copyright Notice:
//
// SimpleOps® is a free ERP software for small businesses and independents.
// Copyright© 2021 Vixark (vixark@outlook.com).
// For more information about SimpleOps®, see https://simpleops.net.
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero
// General Public License as published by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the
// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public
// License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not,
// see https://www.gnu.org/licenses.
//
// This License does not grant permission to use the trade names, trademarks, service marks, or product names
// of the Licensor, except as required for reasonable and customary use in describing the origin of the Work
// and reproducing the content of the NOTICE file.
//
// Removing or changing the above text is not allowed.
//

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SimpleOps.Modelo;
using static SimpleOps.Global;
using static Vixark.General;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Windows;
using static SimpleOps.Configuración;
using Vixark;



namespace SimpleOps.Datos {



    class Contexto : DbContext {


        #region DbSets
        // Para que la carga de datos inicial funcione correctamente deben ser ordenados del más independiente al más dependiente. Dicho de otra manera, de la tablas mayores a tablas que incluyen relaciones a estas. Por ejemplo, se debe poner primero Municipios y Contactos que Clientes porque Clientes tiene relaciones a esas dos tablas. 

        // Independientes
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Municipio> Municipios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<RolUsuario> RolesUsuarios { get; set; } = null!;
        public DbSet<Bloqueo> Bloqueos { get; set; } = null!;
        // Independientes>

        // Entidades Económicas
        public DbSet<Contacto> Contactos { get; set; } = null!;
        public DbSet<Campaña> Campañas { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Proveedor> Proveedores { get; set; } = null!;
        public DbSet<Sede> Sedes { get; set; } = null!;
        public DbSet<ContactoCliente> ContactosClientes { get; set; } = null!;
        public DbSet<ContactoProveedor> ContactosProveedores { get; set; } = null!;
        public DbSet<InformePago> InformesPagos { get; set; } = null!;
        public DbSet<Cobro> Cobros { get; set; } = null!;
        // Entidades Económicas>

        // Productos
        public DbSet<TipoAtributoProducto> TiposAtributosProductos { get; set; } = null!;
        public DbSet<AtributoProducto> AtributosProductos { get; set; } = null!;
        public DbSet<Categoría> Categorías { get; set; } = null!;
        public DbSet<Subcategoría> Subcategorías { get; set; } = null!;
        public DbSet<LíneaNegocio> LíneasNegocio { get; set; } = null!;
        public DbSet<Aplicación> Aplicaciones { get; set; } = null!;
        public DbSet<Material> Materiales { get; set; } = null!;
        public DbSet<Marca> Marcas { get; set; } = null!;
        public DbSet<ProductoBase> ProductosBase { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<PrecioLista> ListasPrecios { get; set; } = null!;
        // Productos>

        // Documentos
        public DbSet<ReciboCaja> RecibosCaja { get; set; } = null!;
        public DbSet<ComprobanteEgreso> ComprobantesEgresos { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<OrdenCompra> ÓrdenesCompra { get; set; } = null!;
        public DbSet<Venta> Ventas { get; set; } = null!;
        public DbSet<Compra> Compras { get; set; } = null!;
        public DbSet<Remisión> Remisiones { get; set; } = null!;
        public DbSet<NotaCréditoVenta> NotasCréditoVenta { get; set; } = null!;
        public DbSet<NotaCréditoCompra> NotasCréditoCompra { get; set; } = null!;
        public DbSet<NotaDébitoVenta> NotasDébitoVenta { get; set; } = null!;
        public DbSet<NotaDébitoCompra> NotasDébitoCompra { get; set; } = null!;
        public DbSet<Cotización> Cotizaciones { get; set; } = null!;
        // Documentos>

        // Líneas de Documentos
        public DbSet<LíneaVenta> LíneasVentas { get; set; } = null!;
        public DbSet<LíneaCompra> LíneasCompras { get; set; } = null!;
        public DbSet<LíneaRemisión> LíneasRemisiones { get; set; } = null!;
        public DbSet<LíneaNotaCréditoVenta> LíneasNotasCréditoVenta { get; set; } = null!;
        public DbSet<LíneaNotaCréditoCompra> LíneasNotasCréditoCompra { get; set; } = null!;
        public DbSet<LíneaNotaDébitoVenta> LíneasNotasDébitoVenta { get; set; } = null!;
        public DbSet<LíneaNotaDébitoCompra> LíneasNotasDébitoCompra { get; set; } = null!;
        public DbSet<LíneaPedido> LíneasPedidos { get; set; } = null!;
        public DbSet<LíneaOrdenCompra> LíneasÓrdenesCompra { get; set; } = null!;
        public DbSet<LíneaCotización> LíneasCotizaciones { get; set; } = null!;
        // Líneas de Documentos>

        // Entidades Económicas y Productos
        public DbSet<PrecioCliente> PreciosClientes { get; set; } = null!;
        public DbSet<PrecioProveedor> PreciosProveedores { get; set; } = null!;
        public DbSet<ReferenciaCliente> ReferenciasClientes { get; set; } = null!;
        public DbSet<ReferenciaProveedor> ReferenciasProveedores { get; set; } = null!;
        public DbSet<InventarioConsignación> InventariosConsignación { get; set; } = null!;
        // Entidades Económicas y Productos>

        // Movimientos de Dinero
        public DbSet<MovimientoBancario> MovimientosBancarios { get; set; } = null!;
        public DbSet<MovimientoEfectivo> MovimientosEfectivo { get; set; } = null!;
        // Movimientos de Dinero>

        #endregion DbSets>


        #region Variables y Constantes

        private static readonly Marca MarcaGato = new Marca("Gato"); // Variable auxiliar únicamente para tomar el nombre de la propiedad FechaHoraCreación y FechaHoraActualización y evitar tener que usar textos escritos manualmente al acceder al nombre de estas propiedades.

        private readonly string NombreFechaHoraCreación = nameof(MarcaGato.FechaHoraCreación);

        private readonly string NombreFechaHoraActualización = nameof(MarcaGato.FechaHoraActualización);

        private readonly string NombreActualizadorID = nameof(MarcaGato.ActualizadorID);

        private readonly string NombreCreadorID = nameof(MarcaGato.CreadorID);

        private TipoContexto TipoContexto { get; set; }

        #endregion Variables y Constantes>


        #region Constructores


        /// <summary>
        /// Siempre se debe usar este constructor para hacer explícita la intención del contexto desde su creación.
        /// El TipoContexto.Lectura se crea con la configuración .NoTracking que es equivalente a usar .AsNoTracking()
        /// en todas sus consultas. El TipoContexto.Escritura es un contexto normal que admite escritura y lectura.
        /// El TipoContexto.LecturaConRastreo actual igual que escritura pero se usa para hacer explícito que la intención
        /// es de solo lectura pero que se necesita aplicar el rastreo (tracking) para algunas tareas específicas como
        /// la necesidad de usar la caché de consultas del contexto o cuando por alguna razón el rendimiento resulta mejor
        /// que con NoTracking activado, ver https://github.com/dotnet/efcore/issues/14366.
        /// </summary>
        /// <param name="tipoContexto"></param>
        public Contexto(TipoContexto tipoContexto) { // Para .Net 7 en adelante, poner private solo para la migración y después volver a poner public.

            ChangeTracker.QueryTrackingBehavior = tipoContexto switch {
                TipoContexto.Lectura => QueryTrackingBehavior.NoTracking,
                TipoContexto.Escritura => QueryTrackingBehavior.TrackAll,
                TipoContexto.LecturaConRastreo => QueryTrackingBehavior.TrackAll,
            }; // Si saca excepción en este punto, se puede deber a que se actualizó el paquete Microsoft.Extensions.Configuration.Abstractions a una versión no soportada. Se debe reversar el paquete a la versión anterior y se corrige el problema. Ver https://stackoverflow.com/questions/64809716/could-not-load-file-or-assembly-microsoft-extensions-configuration-abstractions y https://github.com/dotnet/aspnetcore/issues/21033.
            TipoContexto = tipoContexto;

        } // Contexto>


        private Contexto() { } // Necesario para la migración de Entity Framework. No debería ser usado en ninguna parte del código. Poner public solo para la migración y después volver a poner private.

        #endregion Constructores>


        #region Métodos


        protected override void OnConfiguring(DbContextOptionsBuilder opciones) {

            #pragma warning disable CS0162 // Se detectó código inaccesible. Se omite la advertencia porque HabilitarRastreoDeDatosSensibles puede ser modificado por el usuario del código.
            if (HabilitarRastreoDeDatosSensibles) {
                opciones.UseSqlite(@$"Data Source={RutaBaseDatosSQLite}").UseLoggerFactory(FábricaRastreadores).EnableSensitiveDataLogging();
            } else {
                opciones.UseSqlite(@$"Data Source={RutaBaseDatosSQLite}").UseLoggerFactory(FábricaRastreadores);
            }
            #pragma warning restore CS0162

        } // OnConfiguring>


        protected override void OnModelCreating(ModelBuilder constructor) {

            var tiposEntidad = constructor.Model.GetEntityTypes().ToList();

            // Conversión de Datos
            constructor.Entity<Rol>().Property(r => r.Permisos).HasConversion(ConvertidorJSON<List<Permiso>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<Permiso>>()); // Es necesario establecer el comparador para permitir la actualización ver https://github.com/aspnet/EntityFramework.Docs/blob/master/entity-framework/core/modeling/value-conversions.md, https://github.com/aspnet/EntityFramework.Docs/issues/1986 y https://github.com/dotnet/efcore/issues/17471.
            constructor.Entity<Producto>().Property(p => p.ProductosAsociados).HasConversion(ConvertidorJSON<List<string>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<string>>());
            constructor.Entity<Producto>().Property(p => p.Atributos).HasConversion(ConvertidorJSON<List<string>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<string>>());
            constructor.Entity<ProductoBase>().Property(p => p.ProductosBaseAsociados).HasConversion(ConvertidorJSON<List<string>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<string>>());
            constructor.Entity<ProductoBase>().Property(p => p.Características).HasConversion(ConvertidorJSON<List<string>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<string>>());
            constructor.Entity<Producto>().Property(p => p.CaracterísticasEspecíficas).HasConversion(ConvertidorJSON<List<string>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<string>>());
            constructor.Entity<Cobro>().Property(c => c.NúmerosFacturas).HasConversion(ConvertidorJSON<List<string>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<string>>());
            constructor.Entity<LíneaVenta>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaCompra>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaNotaCréditoCompra>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaNotaCréditoVenta>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaNotaDébitoVenta>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaNotaDébitoCompra>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaOrdenCompra>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaPedido>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<LíneaRemisión>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<Dictionary<string, string>>())
                .Metadata.SetValueComparer(ComparadorJSON<Dictionary<string, string>>());
            constructor.Entity<Producto>().Property(c => c.Personalizaciones).HasConversion(ConvertidorJSON<List<TuplaSerializable<string, List<string>>>>())
                .Metadata.SetValueComparer(ComparadorJSON<List<TuplaSerializable<string, List<string>>>>());

            if (UsarSQLite && GuardarFechaReducidaSQLite) {

                var convertidorYYMMDD = new ValueConverter<DateTime, string>(f => f.ATextoYYMMDD(), s => s.AFechaYYMMDD());
                var convertidorYYMMDDHHMMSSF = new ValueConverter<DateTime, string>(f => f.ATextoYYMMDDHHMMSSF(), s => s.AFechaYYMMDDHHMMSSF());
                var convertidorYYMMDDNulable = new ValueConverter<DateTime?, string?>(f => f.ATextoYYMMDDNulable(), s => s.AFechaYYMMDDNulable());
                foreach (var tipoEntidad in tiposEntidad.Where(t => typeof(IRegistro).IsAssignableFrom(t.ClrType.BaseType))) {
                    constructor.Entity(tipoEntidad.Name).Property(NombreFechaHoraCreación).HasConversion(convertidorYYMMDD);
                }
                foreach (var tipoEntidad in tiposEntidad.Where(t => typeof(IActualizable).IsAssignableFrom(t.ClrType.BaseType))) {
                    constructor.Entity(tipoEntidad.Name).Property(NombreFechaHoraActualización).HasConversion(convertidorYYMMDDHHMMSSF); // Las fechaHoras de actualización no se deben resumir a YYMMDD porque se usan para el control de cambios. Se usa hasta la décima de segundo para agregar un nivel más de seguridad en el control de conflictos de concurrencia. Usar resolución hasta 1 segundo sería casí suficiente porque para que hubiera conflicto tendría que suceder que dos usuarios simultaneamente cargaran datos y los guardarán en el mismo segundo. Al agregar la décima de segundo, que es lo que típicamente tarda una consulta, se asegura casi al completamente que no habrá problemas de este tipo. No se agrega más resolución para no incrementar innecesariamente el tamaño de la base de datos.
                }
                constructor.Entity<Bloqueo>().Property(c => c.FechaHoraInicio).HasConversion(convertidorYYMMDDHHMMSSF);
                constructor.Entity<LíneaOrdenCompra>().Property(c => c.FechaHoraCumplimiento).HasConversion(convertidorYYMMDDNulable);
                constructor.Entity<LíneaPedido>().Property(c => c.FechaHoraCumplimiento).HasConversion(convertidorYYMMDDNulable);
                constructor.Entity<Compra>().Property(c => c.FechaHora).HasConversion(convertidorYYMMDD);
                constructor.Entity<Venta>().Property(c => c.FechaHora).HasConversion(convertidorYYMMDD);
                constructor.Entity<ComprobanteEgreso>().Property(c => c.FechaHora).HasConversion(convertidorYYMMDD);
                constructor.Entity<ReciboCaja>().Property(c => c.FechaHora).HasConversion(convertidorYYMMDD);
                constructor.Entity<MovimientoBancario>().Property(c => c.FechaHora).HasConversion(convertidorYYMMDD); // No se aplica para MovimientoEfectivo porque para estos se podría requerir la hora para hacer la revisión de caja.       

            }

            #pragma warning disable CS0162 // Se detectó código inaccesible. Se omite la advertencia porque UsarSQLite y GuardarDecimalComoDoubleSQLite pueden ser modificado por el usuario del código.
            if (UsarSQLite && GuardarDecimalComoDoubleSQLite) {
                constructor.UsarConvertidorGlobal<decimal?, double?>();
                constructor.UsarConvertidorGlobal<decimal, double>(); // Conversión global intermedia entre double y decimal para forzar que se guarden todos los decimales del modelo como reales en SQLite y permitir OrderBy() y otras operaciones de comparación directamente en la base de datos. Tiene el inconveniente de producir pérdida de precisión en los datos guardados (en comparación con guardarlos como decimal) pues quedan con la precisión de double. Aún así, después de leer los valores de la base de datos se almacenan en las propiedades de las entidades como decimal entonces las operaciones matemáticas se siguen haciendo en decimal y se disminuye la posible pérdida precisión en estas operaciones entonces el efecto no es tan grave. Además, 15 dígitos de precisión de double son más que suficientes para los usos actuales porque podría representar hasta 1 000 000 000 000.00 (1 billón de pesos colombianos) con 2 cifras decimales, cualquier aplicación que requiera manejar valores de dinero superior debería estar usando un servidor de SQL (No SQLite) que no tiene este inconveniente. Ver recomendación en https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations.
            } else { _ = 0; } // Solo se usa esta línea para que no saque advertencia de supresión de CS0162 innecesaria.
            #pragma warning restore CS0162

            // Conversión de Datos>

            // Propiedades Ignoradas - Todas se pueden establecer con el atributo [NotMapped] excepto las que son creadas en las clases abstractas que se deben configurar manualmente en cada entidad.

            constructor.Entity<Cliente>().Ignore(c => c.DirecciónCompleta);
            constructor.Entity<Proveedor>().Ignore(p => p.DirecciónCompleta);

            constructor.Entity<Venta>().Ignore(v => v.SubtotalBaseReal).Ignore(v => v.Margen).Ignore(v => v.Costo).Ignore(v => v.APagar)
                .Ignore(v => v.QR).Ignore(v => v.SubtotalFinalConImpuestos).Ignore(v => v.SubtotalBaseIVA).Ignore(v => v.SubtotalBase)
                .Ignore(v => v.SubtotalFinal).Ignore(v => v.SubtotalBaseIVADian);
            constructor.Entity<Compra>().Ignore(c => c.SubtotalBaseReal).Ignore(c => c.Margen).Ignore(c => c.Costo).Ignore(c => c.APagar)
                .Ignore(c => c.QR).Ignore(c => c.SubtotalFinalConImpuestos).Ignore(c => c.SubtotalBaseIVA).Ignore(c => c.SubtotalBase)
                .Ignore(c => c.SubtotalFinal).Ignore(c => c.SubtotalBaseIVADian);
            constructor.Entity<NotaCréditoCompra>().Ignore(ncc => ncc.SubtotalBaseReal).Ignore(ncc => ncc.Margen).Ignore(ncc => ncc.Costo)
                .Ignore(ncc => ncc.APagar).Ignore(ncc => ncc.SubtotalFinalConImpuestos).Ignore(ncc => ncc.SubtotalBaseIVA).Ignore(ncc => ncc.QR)
                .Ignore(ncc => ncc.SubtotalBase).Ignore(ncc => ncc.SubtotalFinal).Ignore(ncc => ncc.SubtotalBaseIVADian);
            constructor.Entity<NotaCréditoVenta>().Ignore(ncv => ncv.SubtotalBaseReal).Ignore(ncv => ncv.Margen).Ignore(ncv => ncv.Costo)
                .Ignore(ncv => ncv.APagar).Ignore(ncv => ncv.SubtotalFinalConImpuestos).Ignore(ncv => ncv.SubtotalBaseIVA).Ignore(ncv => ncv.QR)
                .Ignore(ncv => ncv.SubtotalBase).Ignore(ncv => ncv.SubtotalFinal).Ignore(ncv => ncv.SubtotalBaseIVADian);
            constructor.Entity<NotaDébitoCompra>().Ignore(ndc => ndc.SubtotalBaseReal).Ignore(ndc => ndc.Margen).Ignore(ndc => ndc.Costo)
                .Ignore(ndc => ndc.APagar).Ignore(ndc => ndc.SubtotalFinalConImpuestos).Ignore(ndc => ndc.SubtotalBaseIVA).Ignore(ndc => ndc.QR)
                .Ignore(ndc => ndc.SubtotalBase).Ignore(ndc => ndc.SubtotalFinal).Ignore(ndc => ndc.SubtotalBaseIVADian);
            constructor.Entity<NotaDébitoVenta>().Ignore(ndv => ndv.SubtotalBaseReal).Ignore(ndv => ndv.Margen).Ignore(ndv => ndv.Costo)
                .Ignore(ndv => ndv.APagar).Ignore(ndv => ndv.SubtotalFinalConImpuestos).Ignore(ndv => ndv.SubtotalBaseIVA).Ignore(ndv => ndv.QR)
                .Ignore(ndv => ndv.SubtotalBase).Ignore(ndv => ndv.SubtotalFinal).Ignore(ndv => ndv.SubtotalBaseIVADian);

            constructor.Entity<LíneaVenta>().Ignore(dv => dv.PorcentajeDescuentoComercial).Ignore(dv => dv.MuestraGratis)
                .Ignore(dv => dv.PorcentajeDescuentoCondicionado);
            constructor.Entity<LíneaCompra>().Ignore(dc => dc.PorcentajeDescuentoComercial).Ignore(dc => dc.MuestraGratis)
                .Ignore(dc => dc.PorcentajeDescuentoCondicionado);
            constructor.Entity<LíneaNotaCréditoCompra>().Ignore(dncc => dncc.PorcentajeDescuentoComercial).Ignore(dncc => dncc.MuestraGratis)
                .Ignore(dncc => dncc.PorcentajeDescuentoCondicionado);
            constructor.Entity<LíneaNotaCréditoVenta>().Ignore(dncv => dncv.PorcentajeDescuentoComercial).Ignore(dncv => dncv.MuestraGratis)
                .Ignore(dncv => dncv.PorcentajeDescuentoCondicionado);
            constructor.Entity<LíneaNotaDébitoCompra>().Ignore(dndc => dndc.PorcentajeDescuentoComercial).Ignore(dndc => dndc.MuestraGratis)
                .Ignore(dndc => dndc.PorcentajeDescuentoCondicionado);
            constructor.Entity<LíneaNotaDébitoVenta>().Ignore(dndv => dndv.PorcentajeDescuentoComercial).Ignore(dndv => dndv.MuestraGratis)
                .Ignore(dndv => dndv.PorcentajeDescuentoCondicionado);
            constructor.Entity<LíneaRemisión>().Ignore(dr => dr.PorcentajeDescuentoComercial).Ignore(dr => dr.MuestraGratis)
                .Ignore(dr => dr.PorcentajeDescuentoCondicionado);

            constructor.Entity<Producto>().Ignore(p => p.ImpuestoConsumoUnitarioPropio); // Por alguna razón no funciona el atributo [NotMapped] en esta propiedad.

            // Propiedades Ignoradas>

            // Única No Clave Principal
            constructor.Entity<Producto>().HasIndex(p => p.Referencia).IsUnique();
            constructor.Entity<ProductoBase>().HasIndex(pb => pb.Referencia).IsUnique();
            constructor.Entity<Contacto>().HasIndex(c => c.Email).IsUnique();
            constructor.Entity<Cliente>().HasIndex(c => c.Nombre).IsUnique(); // No se crea índice para Identificación porque esta puede ser nula.
            constructor.Entity<Proveedor>().HasIndex(p => p.Nombre).IsUnique(); // No se crea índice para Identificación porque esta puede ser nula.
            constructor.Entity<Categoría>().HasIndex(c => c.Nombre).IsUnique();
            constructor.Entity<Subcategoría>().HasIndex(s => s.Nombre).IsUnique();
            constructor.Entity<LíneaNegocio>().HasIndex(ln => ln.Nombre).IsUnique();
            constructor.Entity<Aplicación>().HasIndex(a => a.Nombre).IsUnique();
            constructor.Entity<Material>().HasIndex(m => m.Nombre).IsUnique();
            constructor.Entity<Marca>().HasIndex(m => m.Nombre).IsUnique();
            constructor.Entity<Rol>().HasIndex(r => r.Nombre).IsUnique();
            constructor.Entity<Campaña>().HasIndex(c => c.Nombre).IsUnique();
            constructor.Entity<Usuario>().HasIndex(u => u.Nombre).IsUnique();
            constructor.Entity<TipoAtributoProducto>().HasIndex(tap => tap.Nombre).IsUnique();
            constructor.Entity<AtributoProducto>().HasIndex(ap => ap.Nombre).IsUnique();
            // Única No Clave Principal>

            // Clave Doble Principal - Ver https://entityframeworkcore.com/knowledge-base/50398457/2-foreign-keys-as-primary-key-using-ef-core-2-0-code-first.
            constructor.Entity<PrecioCliente>().HasKey(pe => new { pe.ProductoID, pe.ClienteID });
            constructor.Entity<PrecioProveedor>().HasKey(pe => new { pe.ProductoID, pe.ProveedorID });
            constructor.Entity<ContactoCliente>().HasKey(cc => new { cc.ContactoID, cc.ClienteID });
            constructor.Entity<ContactoProveedor>().HasKey(cp => new { cp.ContactoID, cp.ProveedorID });
            constructor.Entity<RolUsuario>().HasKey(ur => new { ur.RolID, ur.UsuarioID });
            constructor.Entity<LíneaCotización>().HasKey(dc => new { dc.CotizaciónID, dc.ProductoID });
            constructor.Entity<LíneaVenta>().HasKey(dv => new { dv.VentaID, dv.ProductoID });
            constructor.Entity<LíneaCompra>().HasKey(dc => new { dc.CompraID, dc.ProductoID });
            constructor.Entity<LíneaRemisión>().HasKey(dr => new { dr.RemisiónID, dr.ProductoID });
            constructor.Entity<LíneaOrdenCompra>().HasKey(doc => new { doc.OrdenCompraID, doc.ProductoID });
            constructor.Entity<LíneaPedido>().HasKey(doc => new { doc.PedidoID, doc.ProductoID });
            constructor.Entity<LíneaNotaCréditoVenta>().HasKey(dncv => new { dncv.NotaCréditoVentaID, dncv.ProductoID });
            constructor.Entity<LíneaNotaCréditoCompra>().HasKey(dncc => new { dncc.NotaCréditoCompraID, dncc.ProductoID });
            constructor.Entity<LíneaNotaDébitoVenta>().HasKey(dndv => new { dndv.NotaDébitoVentaID, dndv.ProductoID });
            constructor.Entity<LíneaNotaDébitoCompra>().HasKey(dndc => new { dndc.NotaDébitoCompraID, dndc.ProductoID });
            constructor.Entity<ReferenciaCliente>().HasKey(rc => new { rc.ProductoID, rc.ClienteID });
            constructor.Entity<ReferenciaProveedor>().HasKey(pe => new { pe.ProductoID, pe.ProveedorID });
            constructor.Entity<InventarioConsignación>().HasKey(ic => new { ic.ProductoID, ic.ClienteID });
            // Clave Doble Principal>

            // Unicidad de Pareja No Clave Principal -  No se usan como dobles claves principales porque no es conveniente una clave doble relacionada en otras tablas (LíneasÓrdenesCompra, OrdenCompra, etc). 
            constructor.Entity<OrdenCompra>().HasAlternateKey(oc => new { oc.Número, oc.ClienteID });
            constructor.Entity<Sede>().HasAlternateKey(s => new { s.Nombre, s.ClienteID });
            constructor.Entity<Municipio>().HasAlternateKey(s => new { s.Nombre, s.Departamento });
            // Unicidad de Pareja No Clave Principal>

            // Convenciones
            foreach (var tipoEntidad in tiposEntidad) { // Cambia el comportamiento de cascada en eliminación a restringir. Es una medida adicional de protección porque de todas maneras no se va a permitir eliminación de filas. Ver https://stackoverflow.com/questions/46837617/where-are-entity-framework-core-conventions.
                tipoEntidad.GetForeignKeys().Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade).ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict); // Equivalente en EF a modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>() y modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>().
            }
            // Convenciones>

            // Verificaciones
            foreach (var tipoEntidad in tiposEntidad) {

                var tipoClr = tipoEntidad.ClrType;
                var atributo = tipoClr.GetCustomAttribute<ControlInserciónAttribute>();
                if (atributo == null) {
                    if (tipoClr.Name != "TuplaSerializable`2") // Se omite este tipo porque es un tipo de una propiedad (Producto.Personalizaciones) que se serializa a texto y no es una entidad que tenga su propia tabla en la base de datos.
                        throw new Exception($"No se ha establecido el atributo obligatorio ControlInserción para la entidad {tipoClr.Name}");
                } else {
                    if (atributo.Tipo == ControlConcurrencia.Optimista && !typeof(IActualizable).IsAssignableFrom(tipoClr.BaseType))
                        throw new Exception($"La entidad {tipoClr.Name} no implementa IActualizable y tiene atributo de control de inserción optimista. " +
                            $"Esta inconsistencia producirá excepciones cuando se presenten conflictos de concurrencia de inserción.");
                }

            }
            // Verificaciones>


        } // OnModelCreating>


        public override int SaveChanges() {

            if (TipoContexto != TipoContexto.Escritura) throw new InvalidOperationException("El contexto actual no permite guardar cambios.");

            var actualizarMunicipiosConMensajería = false;

            // Acciones Antes de Guardar
            foreach (var entrada in ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)) {

                var entidad = entrada.Entity;
                switch (entrada.State) {

                    case EntityState.Modified:

                        if (entidad is IActualizable eIActualizable) {
                            eIActualizable.FechaHoraActualización = AhoraUtcAjustado;
                            eIActualizable.ActualizadorID = UsuarioActual.ID;
                        } else {
                            if (!(entidad is Bloqueo)) 
                                throw new Exception($"No se permite modificar los valores de las entidades de tipo {entidad.GetType().Name} pues no implementan " + 
                                    "IActualizable. Solo se permite crear nuevas entidades (filas en la base de datos) desde cero."); // Las entidades que no implementen IActualizable, excepto Bloqueos, no podrán ser modificadas.
                        }
                        break;

                    case EntityState.Added:

                        if (entidad is IRegistro eIRegistro) {
                            eIRegistro.FechaHoraCreación = AhoraUtcAjustado;
                            eIRegistro.CreadorID = UsuarioActual.ID;
                        }

                        if (entidad is IActualizable eIActualizable2) {
                            eIActualizable2.FechaHoraActualización = AhoraUtcAjustado;
                            eIActualizable2.ActualizadorID = UsuarioActual.ID;
                            eIActualizable2.CreadorID = UsuarioActual.ID;
                        }

                        var atributo = entidad.GetType().GetCustomAttribute<ControlInserciónAttribute>();
                        if (atributo != null && atributo.Tipo == ControlConcurrencia.NoPermitido) {
                            throw new Exception($"No se permite la inserción de filas en la tabla {entidad.GetType().Name}.");
                        }

                        break;

                    case EntityState.Deleted:
                        if (!(entidad is Bloqueo)) throw new Exception($"No se permite la eliminación de las entidades de tipo {entidad.GetType().Name}."); // Por el momento no se permite la eliminación de ninguna entidad excepto de Bloqueos.
                        break;
                    default:
                        throw new Exception(CasoNoConsiderado(entrada.State.ToString()));
                }

                if (entidad is Municipio) actualizarMunicipiosConMensajería = true; // Si al menos un municipio se modificó, agregó o eliminó se debe actualizar la lista de los municipios con mensajería.

            }
            // Acciones Antes de Guardar>

            // Guardado
            var respuesta = base.SaveChanges();
            // Guardado>

            // Acciones Después de Guardar
            if (actualizarMunicipiosConMensajería) LeerMunicipiosDeInterés(this);
            // Acciones Después de Guardar>

            return respuesta;

        } // SaveChanges>


        /// <summary>
        /// Función que pregunta al usuario que valor desea conservar de un conflicto de concurrencia.
        /// </summary>
        /// <param name="valorPropuesto"></param>
        /// <param name="valorBaseDatos"></param>
        /// <param name="entidad"></param>
        /// <param name="nombrePropiedad"></param>
        /// <param name="otroUsuarioID"></param>
        /// <returns></returns>
        public object? SolucionadorConflictos(object? valorPropuesto, object? valorBaseDatos, object entidad, string nombrePropiedad, int otroUsuarioID) {

            var nombreOtroUsuario = Usuarios.Single(u => u.ID == otroUsuarioID).Nombre;
            var valorPropuestoTxt = valorPropuesto?.ATextoDinero();
            var valorBaseDatosTxt = valorBaseDatos?.ATextoDinero();
            var nombreEntidad = entidad.GetType().Name;
            var nombrePropiedadMinúscula = nombrePropiedad.AMinúscula();
            var nombreEntidadMinúscula = nombreEntidad.AMinúscula();
            var respuesta = MostrarDiálogo(
                $"Se ha presentado un conflicto al guardar los datos porque hace poco {nombreOtroUsuario} también modificó " +
                $"{ObtenerPalabraNúmeroYGénero(nombrePropiedadMinúscula, "el")} {nombrePropiedad} " +
                $"{ObtenerPalabraNúmeroYGénero(nombreEntidadMinúscula, "del")} {nombreEntidad} {entidad}.{DobleLínea}" +
                $"El valor que intentaste guardar fue {valorPropuestoTxt}.{NuevaLínea}" +
                $"El valor guardado por {nombreOtroUsuario} fue {valorBaseDatosTxt}.{DobleLínea}" +
                $"¿Deseas sobreescribir el valor de {nombreOtroUsuario} con tu valor {valorPropuestoTxt}?", "Conflicto al Guardar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (respuesta == MessageBoxResult.Yes) {
                return valorPropuesto;
            } else {
                return valorBaseDatos;
            }

        } // SolucionadorConflictos>


        /// <summary>
        /// Siempre se debe usar este método en vez de SaveChanges porque implementa el control de concurrencia.
        /// </summary>
        /// <returns></returns>
        public int GuardarCambios() => GuardarCambios((vp, vbd, e, np, ou) => SolucionadorConflictos(vp, vbd, e, np, ou));


        /// <summary>
        /// Función para guardar los cambios realizados que permite pasar una función para solucionar los conflictos de concurrencia.
        /// </summary>
        /// <param name="solucionadorConflictos"></param>
        /// <returns></returns>
        public int GuardarCambios(Func<object?, object?, object, string, int, object?> solucionadorConflictos) {

            var cantidadModificados = 0;

            void controlarDbUpdateConcurrencyException(DbUpdateException ex) {

                foreach (var entrada in ex.Entries) {

                    var entidad = entrada.Entity;
                    var nombreEntidad = entidad.GetType().Name;
                    if (!(entidad is IActualizable)) 
                        throw new Exception($"No se esperaban errores de concurrencia para una entidad de tipo {nombreEntidad} que no implementa IActualizable.");

                    var valoresActuales = entrada.CurrentValues;
                    var valoresBD = entrada.GetDatabaseValues();
                    var propiedadActualizadorID = valoresActuales.Properties.Where(p => p.Name == NombreActualizadorID).Single();
                    int actualizadorIDActual = (int)valoresActuales[propiedadActualizadorID];
                    int actualizadorIDBD = (int)valoresBD[propiedadActualizadorID];

                    if (actualizadorIDActual != actualizadorIDBD) { // Si el cambio anterior lo realizó un usuario distinto, se verificarán las columnas cambiadas para mostrar el resolvedor de conflictos. Si el cambio anterior lo realizó el usuario actual no se genera conflicto, no es necesario realizar ninguna acción y se aceptan transparentemente todos los valores actuales. Esto se puede dar cuando el usuario está accediendo desde otro dispositivo al mismo tiempo. Es equivalente a aplicar el patrón 'cliente gana' de https://docs.microsoft.com/en-us/ef/ef6/saving/concurrency.

                        foreach (var propiedad in valoresActuales.Properties) {

                            var valorActual = valoresActuales[propiedad];
                            var valorBaseDatos = valoresBD[propiedad];
                            var nombrePropiedad = propiedad.Name;
                            var sonIguales = (valorActual == null && valorBaseDatos == null) || valorBaseDatos.Equals(valorActual);
                            if (!sonIguales && nombrePropiedad != NombreFechaHoraActualización && nombrePropiedad != NombreActualizadorID) { // Si el valor actual de la propiedad es igual al que está en la base de datos no se requiere hacer nada. Si la propiedad cambiada es FechaHoraActualización o ActualizadorID tampoco se hace nada y se aceptan transparentemente los valores actuales sin preguntarle al usuario. Esto es porque aún en el caso que el usuario deje los valores anteriores tomó una decisión conciente de guardarlos y por lo tanto se convierte en el último actualizador de esa fila. Esto es importante para controlar el caso que el otro usuario también realize una tercera modificación y al acceder a la fila encuentre una FechaHoraActualización distinta y se pueda dar nuevamente un conflicto de concurrencia.

                                if (nombrePropiedad == NombreCreadorID) {
                                    valoresActuales[propiedad] = valorBaseDatos; // Sucede en el caso de solución de conflictos de concurrencia por inserción (que son direccionados aquí por el código de control de inserción). El CreadorID siempre será el primero que insertó la fila, nunca será actualizado, se usa por lo tanto el valor en la base de datos.
                                } else {
                                    valoresActuales[propiedad] = solucionadorConflictos(valorActual, valorBaseDatos, entidad, nombrePropiedad, actualizadorIDBD); // El usuario decide que valor se guardará en la base de datos.
                                }

                            }

                        }

                    }

                    entrada.OriginalValues.SetValues(valoresBD); // Se actualizan los valores 'originales' de la operación actual con los valores realmente existentes en la base de datos (actualizando así la FechaHoraActualización) para que al realizar la nueva actualización (usando la FechaHoraActualización esperada por la operación actual igual a la existente en la base de datos) no se genere nuevamente un conflicto de concurrencia. Según lo recomendado en https://docs.microsoft.com/en-us/ef/core/saving/concurrency.

                }

            } // controlarDbUpdateConcurrencyException>

            var enConflictoInserción = false; // Si es verdadero las inserciones se realizan una a una para capturar y controlar la o las inserciones que están produciendo las excepciones. Este código para identificar las entidades con conflicto es necesario porque Entity Framework aún no las devuelve en ex.Entries como si sucede en DbUpdateConcurrencyException. Ver https://github.com/dotnet/efcore/issues/7829.
            List<EntityEntry>? ePendientesInsertar = null; // Lista de entradas pendientes. Se llena cuando se da un conflicto de inserción.      
            EntityEntry? eInsertar = null; // Entrada que se insertará individualmente.
            bool? hayInserciones = null; 
                    
            var éxitoInserción = false;
            while (!éxitoInserción) { // El ciclo externo captura y controla las excepciones DbUpdateException de fallo de restricción de claves únicas que se dan principalmente en conflictos de concurrencia en inserciones.

                if (enConflictoInserción) { // Entra aquí después de que se ha presentado una excepción DbUpdateException de fallo de restricción de claves únicas al intentar guardar todas las entradas pendientes.

                    if (ePendientesInsertar == null) {
                        ePendientesInsertar = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList(); // Se hace la consulta aquí para evitar reducir el rendimiento de todas las otras solicitudes a GuardarCambios que no lo necesitan.
                        ePendientesInsertar.ForEach(e => e.State = EntityState.Unchanged); // Todas las entradas pendientes por insertar se marcan Unchanged y se va marcando una a una Added para insertar individualmente las entidades y detectar y controlar las generadoras del conflicto.
                        hayInserciones = ePendientesInsertar.Any();
                    }

                    if (hayInserciones == true) {
                        eInsertar = ePendientesInsertar.First();
                        if (eInsertar.State != EntityState.Modified) eInsertar.State = EntityState.Added; // Se evita cambiar el estado a insertada cuando está modificada porque cuando está intentando solucionar el conflicto de inserción pasa por aquí con estado modificada porque está haciendo pasar la entrada como una actualización.
                    }
                          
                }

                try {

                    var éxitoActualización = false;
                    while (!éxitoActualización) { // El ciclo interno captura y controla las excepciones DbUpdateConcurrencyException que se dan en conflictos de concurrencia en actualizaciones.

                        try {
                            cantidadModificados += SaveChanges();
                            éxitoActualización = true;
                        } catch (DbUpdateConcurrencyException ex) { // Independiente del orden de las entradas el conflicto de concurrencia en actualización es el primero que se genera. Esto hace que en el caso que ambos sucedan en la misma transacción la correción de un conflicto de concurrencia de actualización se aplica pero solo se guarda en la base de datos una vez se da el proceso de solución del conflicto de concurrencia en inserciones.
                            controlarDbUpdateConcurrencyException(ex);
                        }

                    }

                    if (enConflictoInserción && hayInserciones == true) {
                        ePendientesInsertar!.Remove(eInsertar!); // Si hayInserciones = true, se garantiza que ePendientesInsertar y eInsertar no son nulos.
                        éxitoInserción = !ePendientesInsertar.Any(); // Se establece como exitosa la inserción cuando ya no quedan más pendientes por insertar.
                    } else {
                        éxitoInserción = true;
                    }

                } catch (DbUpdateException ex) {

                    if (ex.InnerException != null && ex.InnerException.Message.EmpiezaPor(ExcepciónSQLiteFallóRestricciónÚnica)) {
                               
                        if (enConflictoInserción && hayInserciones == true) { // Si se está insertando una a una y se obtiene excepción DbUpdateException es porque se encontró una de las entidades que generan el conflicto de inserción.

                            var entidad = eInsertar!.Entity; // Si hayInserciones = true, se garantiza que eInsertar no es nulo.     
                            var atributo = entidad.GetType().GetCustomAttribute<ControlInserciónAttribute>();
                            if (atributo != null && atributo.Tipo == ControlConcurrencia.Optimista) { // Solo se procesa si la entidad explicitamente se ha marcado que implementa el control de concurrencia en inserciones optimista.
                                eInsertar.State = EntityState.Modified; // Se cambia el estado de la entidad de insertada a modificada porque como ya existe la fila en la base de datos (de la inserción del usuario anterior) entonces para que ya no intente insertarla si no actualizarla. Si en la actualización se presentan conflictos estos serán resueltos en controlarDbUpdateConcurrencyException().
                            } else {
                                throw; // Si la entidad que se está insertando no implementa el control de concurrencia de inserción optimista relanza la excepción.
                            }

                        } else {
                            enConflictoInserción = hayInserciones == null ? true : throw ex; // Se activa enConflictoInserción si se verifica que se trata de una excepción de fallo de restricción de claves únicas y aún no se ha verificado si hay inserciones por realizar. El relanzamiento de la excepción se produce posiblemente en un caso especial en el se entra en conflicto por la modificación de las claves de una entidad a las mismas claves que otro usuario hace poco insertó, aunque a la versión actual Entity Framework impide estas operaciones se deja el lanzamiento de la excepción por si se filtra algún caso que lo genere.  
                        }
                                
                    } else {
                        throw; // Si la excepción no es de fallo de restricción de claves únicas relanza la excepción. Si se agrega soporte a otras bases de datos se puede presentar este error que se soluciona agregando una nueva condicion || adaptada a la base de datos en el condicional anterior.
                    }

                }

            }

            return cantidadModificados;

        } // GuardarCambios>


        #region Bloqueos


        /// <summary>
        /// Obtiene la lista de todos los bloqueos actuales de cierto tipo de entidad. Se obtienen los que su ID esté contenido en bloqueoVarias.IDs.
        /// </summary>
        /// <returns></returns>
        public List<Bloqueo> ObtenerBloqueos(BloqueoVarias bloqueoVarias) 
            => Bloqueos.Where(ObtenerPredicadoBloqueos(bloqueoVarias), ConectorLógico.Y).Include(b => b.Usuario).ToList();

        /// <summary>
        /// Igual que ObtenerBloqueos(bloqueoVarias) pero usa un predicado y permite establecer un conector entre las expresiones en él.
        /// </summary>
        /// <param name="predicado"></param>
        /// <param name="conector"></param>
        /// <returns></returns>
        public List<Bloqueo> ObtenerBloqueos(Predicado<Bloqueo> predicado, ConectorLógico conector)
            => Bloqueos.Where(predicado, conector).Include(b => b.Usuario).ToList();

        /// <summary>
        /// Obtiene una expresión que puede ser usada en la consulta para obtener los bloqueos de una entidad o para ser combinada con expresiones 
        /// de otras entidades para obtener los bloqueos aplicables a al menos una de ellas (con operador O) y obtener el estado de bloqueo de varias 
        /// entidades en una sola consulta a la base de datos.
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<Bloqueo, bool>> ObtenerExpresiónBloqueos(BloqueoVarias bloqueoVarias)
            => ObtenerExpresión(ObtenerPredicadoBloqueos(bloqueoVarias), ConectorLógico.Y);


        private static Predicado<Bloqueo> ObtenerPredicadoBloqueos(BloqueoVarias bloqueoVarias) {

            var predicado = new Predicado<Bloqueo>();
            if (bloqueoVarias.IDs != null) predicado.Agregar(b => b.EntidadID != null && bloqueoVarias.IDs.Contains((int)b.EntidadID));
            if (bloqueoVarias.Propiedad != null) predicado.Agregar(b => b.Propiedad == bloqueoVarias.Propiedad);
            predicado.Agregar(b => b.NombreEntidad == bloqueoVarias.NombreEntidad);

            var tipo = bloqueoVarias.Tipo; 
            if (tipo.HasFlag(TipoPermiso.Modificación | TipoPermiso.Inserción | TipoPermiso.Eliminación)) { // Se deben generar expresiones con O y no con Y por que la aparición de un solo tipo de bloqueo del nuevo bloqueo en la base de datos impide la creación del nuevo bloqueo. Por ejemplo, si quiere hacer un bloqueo de tipo Inserción|Eliminación se debe buscar todos los bloqueos coincidentes que incluyan Inserción o Eliminación. En esta búsqueda concidiría Modificación|Inserción y también con Eliminación|Lectura. La presencia de al menos uno de ellos (Inserción o Eliminación) hace que que el bloqueo que se quiere hacer entre en conflicto con el bloqueo actual y no se pueda realizar. No se tienen en cuenta los casos con lectura porque no son casos que interesen para fines de bloqueos temporales, los permisos de lectura se establecen a nivel de permisos generales por roles.
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Modificación) || b.Tipo.HasFlag(TipoPermiso.Inserción) || b.Tipo.HasFlag(TipoPermiso.Eliminación));
            } else if (tipo.HasFlag(TipoPermiso.Modificación | TipoPermiso.Inserción)) {
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Modificación) || b.Tipo.HasFlag(TipoPermiso.Inserción));
            } else if (tipo.HasFlag(TipoPermiso.Modificación | TipoPermiso.Eliminación)) {
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Modificación) || b.Tipo.HasFlag(TipoPermiso.Eliminación));
            } else if (tipo.HasFlag(TipoPermiso.Eliminación | TipoPermiso.Inserción)) {
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Eliminación) || b.Tipo.HasFlag(TipoPermiso.Inserción));
            } else if (tipo.HasFlag(TipoPermiso.Eliminación)) {
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Eliminación));
            } else if (tipo.HasFlag(TipoPermiso.Modificación)) {
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Modificación));
            } else if (tipo.HasFlag(TipoPermiso.Inserción)) {
                predicado.Agregar(b => b.Tipo.HasFlag(TipoPermiso.Inserción));
            }

            return predicado;

        } // ObtenerPredicadoBloqueos>


        /// <summary>
        /// Bloquea las entidades de tipo bloqueoVarias.NombreEntidad e IDs en bloqueoVarias.IDs, o todas las entidades de su tipo 
        /// si no se especifica bloqueoVarias.IDs, para la bloqueoVarias.Propiedad y bloqueoVarias.Tipo
        /// especificados.<br/>
        /// Devuelve un ResultadoBloqueo que contiene la lista de los bloqueos realizados. Si no es exitoso se devuelven los bloqueos
        /// existentes.
        /// </summary>
        /// <returns></returns>
        public ResultadoBloqueo VerificarYBloquear(BloqueoVarias bloqueoVarias, bool mostrarError = true) {

            var bloqueosExistentes = ObtenerBloqueos(bloqueoVarias);
            if (bloqueosExistentes.Any()) {
                if (mostrarError) MostrarErrorBloqueosExistentes(bloqueosExistentes, ObtenerNombreTabla(bloqueoVarias.NombreEntidad, this));
                return new ResultadoBloqueo(false, bloqueosExistentes);
            } else {
                return new ResultadoBloqueo(true, GuardarBloqueos(Bloqueo.ObtenerLista(bloqueoVarias, UsuarioActual)));
            }

        } // VerificarYBloquear>


        /// <summary>
        /// Igual que VerificarYBloquear(bloqueoVarias) pero actua sobre una lista de BloqueoVarias ejecutando solo dos consultas a la base de datos.
        /// </summary>
        /// <param name="bloqueosVarias"></param>
        /// <param name="mostrarError"></param>
        /// <returns></returns>
        public ResultadoBloqueo VerificarYBloquear(List<BloqueoVarias> bloqueosVarias, bool mostrarError = true) {

            var predicado = new Predicado<Bloqueo>();
            foreach (var bloqueoVarias in bloqueosVarias) {
                predicado.Agregar(ObtenerExpresiónBloqueos(bloqueoVarias));
            }
            var bloqueosExistentes = ObtenerBloqueos(predicado, ConectorLógico.O).ToList();

            if (bloqueosExistentes.Any()) {

                if (mostrarError) {
                    MostrarErrorBloqueosExistentes(bloqueosExistentes, bloqueosVarias.Select(db => db.NombreEntidad).Distinct()
                        .Select(ne => ObtenerNombreTabla(ne, this)).ToList().ATextoConComas());
                }
                return new ResultadoBloqueo(false, bloqueosExistentes);

            } else {

                var bloqueos = new List<Bloqueo>();
                foreach (var bloqueoVarias in bloqueosVarias) {
                    bloqueos.AddRange(Bloqueo.ObtenerLista(bloqueoVarias, UsuarioActual));
                }
                return new ResultadoBloqueo(true, GuardarBloqueos(bloqueos));

            }

        } // VerificarYBloquear>


        private static void MostrarErrorBloqueosExistentes(List<Bloqueo> bloqueosExistentes, string? nombreTablas) {

            var textoTabla = nombreTablas != null && nombreTablas.Contiene(",") ? "tablas" : "tabla";
            MostrarAdvertencia($"No se pudo realizar la acción porque {ObtenerPalabraNúmeroYGénero(textoTabla, "el")} {textoTabla} {nombreTablas} " + 
                $"{ObtenerPalabraNúmeroYGénero(textoTabla, "está")} {ObtenerPalabraNúmeroYGénero(textoTabla, "bloqueado")} por " +
                $"{bloqueosExistentes.Select(b => b.Usuario?.Nombre).Distinct().ToList().ATextoConComas()}."); // Se refiere a acción y no a bloqueo porque normalmente el bloqueo se intenta hacer antes de realizar una acción.

        } // MostrarErrorBloqueosExistentes>


        public List<Bloqueo> GuardarBloqueos(List<Bloqueo> bloqueos) {

            Bloqueos.AddRange(bloqueos);
            GuardarCambios();
            return bloqueos;

        } // GuardarBloqueos>


        #endregion Bloqueos>


        /// <summary>
        /// Carga a la base de datos los datos en archivos JSON en la carpeta <paramref name="rutaJsons"/>. Solo sirve para la carga inicial. Se deben cargar todos juntos en una sola tanda y con la base de datos vacía para asegurar que los IDs quedarán iguales que en los JSON y que las relaciones queden correctas.
        /// </summary>
        /// <param name="rutaJsons"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool CargarDatosIniciales(string rutaJsons, out string error) {

            OperacionesEspecialesDatos = true; // Esto evita múltiples errores de verificación de datos que se pueden presentar al cargar el CSV. Por ejemplo, la excepción al asignar MunicipioID nulo a una entidad económica.
            error = "";
            using var ctx = new Contexto(TipoContexto.Escritura);

            string? cargarJson<E>(string nombreTabla, bool individualmente = false) where E : class { // Devuelve nulo si fue exitoso o el error si los hubo. Si individualmente = true el proceso es más lento pero puede detectar errores en filas puntuales.

                var rutaJson = Path.Combine(rutaJsons, nombreTabla + ".json");
                if (!File.Exists(rutaJson)) return null; // No es error, solo no se carga la tabla.
                var entidades = Deserializar<List<E>>(File.ReadAllText(rutaJson));
                if (entidades == null) return null;

                if (individualmente) {

                    var cuenta = 0;
                    foreach (var entidad in entidades) {
 
                        try {
                            ctx.Add(entidad);
                            cuenta += ctx.GuardarCambios();
                        } catch (Exception ex) { // Antes se estaba deshabilitando la advertencia CA1031. No capture tipos de excepción generales. Se desactiva porque se necesita el texto de la excepción. No hay mayor problema porque esta excepción es controlada pues el proceso finaliza y se le notifica al usuario el error encontrado.
                            return $"Error en carga individual en tabla {nombreTabla}, fila {cuenta + 1}.{DobleLínea}{ex.Message}";
                        }
                        
                    }

                } else {
                    ctx.AddRange(entidades);
                    ctx.GuardarCambios();      
                }

                return null;

            } // cargarJSON>

            foreach (var propiedad in ctx.GetType().GetProperties()) {

                var tipo = propiedad.PropertyType;
                if (!(tipo.IsGenericType && (typeof(DbSet<>).IsAssignableFrom(tipo.GetGenericTypeDefinition())))) continue; // Omite los que no son DbSet.

                var nombreTabla = propiedad.Name;

                #if true
                try {
                #endif

                    var tieneDatosClientes = ctx.Clientes.Any();
                    var tieneDatos = nombreTabla switch { 
                        "Aplicaciones" => ctx.Aplicaciones.Any(),
                        "Campañas" => ctx.Campañas.Any(),
                        "Categorías" => ctx.Categorías.Any(),
                        "Clientes" => ctx.Clientes.Any(),
                        "Cobros" => ctx.Cobros.Any(),
                        "Compras" => ctx.Compras.Any(),
                        "ComprobantesEgresos" => ctx.ComprobantesEgresos.Any(),
                        "Contactos" => ctx.Contactos.Any(),
                        "ContactosClientes" => ctx.ContactosClientes.Any(),
                        "ContactosProveedores" => ctx.ContactosProveedores.Any(),
                        "LíneasCotizaciones" => ctx.LíneasCotizaciones.Any(),
                        "LíneasCompras" => ctx.LíneasCompras.Any(),
                        "LíneasNotasCréditoCompra" => ctx.LíneasNotasCréditoCompra.Any(),
                        "LíneasNotasCréditoVenta" => ctx.LíneasNotasCréditoVenta.Any(),
                        "LíneasNotasDébitoCompra" => ctx.LíneasNotasDébitoCompra.Any(),
                        "LíneasNotasDébitoVenta" => ctx.LíneasNotasDébitoVenta.Any(),
                        "LíneasÓrdenesCompra" => ctx.LíneasÓrdenesCompra.Any(),
                        "LíneasPedidos" => ctx.LíneasPedidos.Any(),
                        "LíneasRemisiones" => ctx.LíneasRemisiones.Any(),
                        "LíneasVentas" => ctx.LíneasVentas.Any(),
                        "InformesPagos" => ctx.InformesPagos.Any(),
                        "InventariosConsignación" => ctx.InventariosConsignación.Any(),
                        "LíneasNegocio" => ctx.LíneasNegocio.Any(),
                        "ListasPrecios" => ctx.ListasPrecios.Any(),
                        "Marcas" => ctx.Marcas.Any(),
                        "Materiales" => ctx.Materiales.Any(),
                        "MovimientosBancarios" => ctx.MovimientosBancarios.Any(),
                        "MovimientosEfectivo" => ctx.MovimientosEfectivo.Any(),
                        "Municipios" => ctx.Municipios.Any(),
                        "NotasCréditoCompra" => ctx.NotasCréditoCompra.Any(),
                        "NotasCréditoVenta" => ctx.NotasCréditoVenta.Any(),
                        "NotasDébitoCompra" => ctx.NotasDébitoCompra.Any(),
                        "Cotizaciones" =>  ctx.Cotizaciones.Any(),
                        "NotasDébitoVenta" => ctx.NotasDébitoVenta.Any(),
                        "ÓrdenesCompra" => ctx.ÓrdenesCompra.Any(),
                        "Pedidos" => ctx.Pedidos.Any(),
                        "PreciosClientes" => ctx.PreciosClientes.Any(),
                        "PreciosProveedores" => ctx.PreciosProveedores.Any(),
                        "Productos" => ctx.Productos.Any(),
                        "ProductosBase" => ctx.ProductosBase.Any(),
                        "Proveedores" => ctx.Proveedores.Any(),
                        "RecibosCaja" => ctx.RecibosCaja.Any(),
                        "ReferenciasClientes" => ctx.ReferenciasClientes.Any(),
                        "ReferenciasProveedores" => ctx.ReferenciasProveedores.Any(),
                        "Remisiones" => ctx.Remisiones.Any(),
                        "Roles" => ctx.Roles.Any(),
                        "RolesUsuarios" => ctx.RolesUsuarios.Any(),
                        "Sedes" => ctx.Sedes.Any(),
                        "Subcategorías" => ctx.Subcategorías.Any(),
                        "Usuarios" => ctx.Usuarios.Any(),
                        "Ventas" => ctx.Ventas.Any(),
                        "Bloqueos" => ctx.Bloqueos.Any(),
                        "AtributosProductos" => ctx.AtributosProductos.Any(),
                        "TiposAtributosProductos" => ctx.TiposAtributosProductos.Any(),
                        _ => throw new Exception(CasoNoConsiderado(nombreTabla)) // Pendiente agregar entidad al switch.
                    };

                    if (tieneDatos) {
                        error = $"La tabla {nombreTabla} tiene datos. No es posible cargar los datos con CargarDatosIniciales() sobre datos existentes. " + 
                            "Reinicia la base de datos e intenta nuevamente.";
                        OperacionesEspecialesDatos = false;
                        return false;
                    }

                    var errorCarga = nombreTabla switch { // El orden de las tablas no importa aquí pero si importa el orden de los DbSet<> porque este es el orden en el que se leen las propiedades con GetProperties(). Deben ser ordenados de las menos dependientes a las más dependientes.
                        "Aplicaciones" => cargarJson<Aplicación>(nombreTabla),
                        "Campañas" => cargarJson<Campaña>(nombreTabla),
                        "Categorías" => cargarJson<Categoría>(nombreTabla),
                        "Clientes" => cargarJson<Cliente>(nombreTabla),
                        "Cobros" => cargarJson<Cobro>(nombreTabla),
                        "Compras" => cargarJson<Compra>(nombreTabla),
                        "ComprobantesEgresos" => cargarJson<ComprobanteEgreso>(nombreTabla),
                        "Contactos" => cargarJson<Contacto>(nombreTabla),
                        "ContactosClientes" => cargarJson<ContactoCliente>(nombreTabla),
                        "ContactosProveedores" => cargarJson<ContactoProveedor>(nombreTabla),
                        "LíneasCotizaciones" => cargarJson<LíneaCotización>(nombreTabla),
                        "LíneasCompras" => cargarJson<LíneaCompra>(nombreTabla),
                        "LíneasNotasCréditoCompra" => cargarJson<LíneaNotaCréditoCompra>(nombreTabla),
                        "LíneasNotasCréditoVenta" => cargarJson<LíneaNotaCréditoVenta>(nombreTabla),
                        "LíneasNotasDébitoCompra" => cargarJson<LíneaNotaDébitoCompra>(nombreTabla),
                        "LíneasNotasDébitoVenta" => cargarJson<LíneaNotaDébitoVenta>(nombreTabla),
                        "LíneasÓrdenesCompra" => cargarJson<LíneaOrdenCompra>(nombreTabla),
                        "LíneasPedidos" => cargarJson<LíneaPedido>(nombreTabla),
                        "LíneasRemisiones" => cargarJson<LíneaRemisión>(nombreTabla),
                        "LíneasVentas" => cargarJson<LíneaVenta>(nombreTabla),
                        "InformesPagos" => cargarJson<InformePago>(nombreTabla),
                        "InventariosConsignación" => cargarJson<InventarioConsignación>(nombreTabla),
                        "LíneasNegocio" => cargarJson<LíneaNegocio>(nombreTabla),
                        "ListasPrecios" => cargarJson<PrecioLista>(nombreTabla),
                        "Marcas" => cargarJson<Marca>(nombreTabla),
                        "Materiales" => cargarJson<Material>(nombreTabla),
                        "MovimientosBancarios" => cargarJson<MovimientoBancario>(nombreTabla),
                        "MovimientosEfectivo" => cargarJson<MovimientoEfectivo>(nombreTabla),
                        "Municipios" => cargarJson<Municipio>(nombreTabla),
                        "NotasCréditoCompra" => cargarJson<NotaCréditoCompra>(nombreTabla),
                        "NotasCréditoVenta" => cargarJson<NotaCréditoVenta>(nombreTabla),
                        "NotasDébitoCompra" => cargarJson<NotaDébitoCompra>(nombreTabla),
                        "NotasDébitoVenta" => cargarJson<NotaDébitoVenta>(nombreTabla),
                        "Cotizaciones" => cargarJson<Cotización>(nombreTabla),
                        "ÓrdenesCompra" => cargarJson<OrdenCompra>(nombreTabla),
                        "Pedidos" => cargarJson<Pedido>(nombreTabla),
                        "PreciosClientes" => cargarJson<PrecioCliente>(nombreTabla),
                        "PreciosProveedores" => cargarJson<PrecioProveedor>(nombreTabla),
                        "Productos" => cargarJson<Producto>(nombreTabla),
                        "ProductosBase" => cargarJson<ProductoBase>(nombreTabla),
                        "Proveedores" => cargarJson<Proveedor>(nombreTabla),
                        "RecibosCaja" => cargarJson<ReciboCaja>(nombreTabla),
                        "ReferenciasClientes" => cargarJson<ReferenciaCliente>(nombreTabla),
                        "ReferenciasProveedores" => cargarJson<ReferenciaProveedor>(nombreTabla),
                        "Remisiones" => cargarJson<Remisión>(nombreTabla),
                        "Roles" => cargarJson<Rol>(nombreTabla),
                        "RolesUsuarios" => cargarJson<RolUsuario>(nombreTabla),
                        "Sedes" => cargarJson<Sede>(nombreTabla),
                        "Subcategorías" => cargarJson<Subcategoría>(nombreTabla),
                        "Usuarios" => cargarJson<Usuario>(nombreTabla),
                        "Ventas" => cargarJson<Venta>(nombreTabla),
                        "Bloqueos" => cargarJson<Bloqueo>(nombreTabla),
                        "AtributosProductos" => cargarJson<AtributoProducto>(nombreTabla),
                        "TiposAtributosProductos" => cargarJson<TipoAtributoProducto>(nombreTabla),
                        _ => throw new Exception(CasoNoConsiderado(nombreTabla)) // Pendiente agregar entidad al switch.
                    };

                    if (!string.IsNullOrEmpty(errorCarga)) {
                        error = errorCarga;
                        OperacionesEspecialesDatos = false;
                        return false;
                    }

                #if true
                } catch (Exception ex) {

                    OperacionesEspecialesDatos = false; // Se debe restaurar el valor porque siempre se lanzará excepción y se finalizará la función.
                    var mensajeExcepciónInterna = ex.InnerException?.Message;
                    if (mensajeExcepciónInterna == "Cannot get the value of a token type 'String' as a number.") {

                        var nombrePropiedad = ExtraerConPatrónObsoleta(ex.Message, $@"({PatrónNombreVariable}+) \| LineNumber:", 1, out _);
                        error = $"No se pudo convertir un texto en número. Usualmente este problema sucede en la conversión de texto a decimal porque " +
                                $"una propiedad de tipo decimal se guardó como texto en el JSON. Si este es el caso agrega {nombreTabla}.{nombrePropiedad}" +
                                $" a la tabla 'Decimales Forzados' en 'Cargador Datos.xlsm > Procedimiento', genera el JSON actualizado de " +
                                $"la tabla {nombreTabla} e intenta nuevamente.{DobleLínea}{mensajeExcepciónInterna}";
                        return false;

                    } else if (mensajeExcepciónInterna == "Cannot get the value of a token type 'Number' as a boolean.") {

                        var nombrePropiedad = ExtraerConPatrónObsoleta(ex.Message, $@"({PatrónNombreVariable}+) \| LineNumber:", 1, out _);
                        error = $"No se pudo convertir un número en un valor booleano. Usualmente este problema sucede porque una propiedad de tipo " +
                                $"booleano se guardó como número en el JSON. Si este es el caso agrega {nombreTabla}.{nombrePropiedad} a la tabla " +
                                $"'Booleanos Forzados' en 'Cargador Datos.xlsm > Procedimiento', genera el JSON actualizado de la tabla {nombreTabla} " +
                                $"e intenta nuevamente.{DobleLínea}{mensajeExcepciónInterna}";
                        return false;

                    } else if (mensajeExcepciónInterna != null && mensajeExcepciónInterna.EmpiezaPor(ExcepciónSQLiteFallóRestricciónÚnica)) {

                        var nombreEntidadPropiedad
                            = ExtraerConPatrónObsoleta(mensajeExcepciónInterna, $@"constraint failed: ({PatrónNombreVariable}+\.{PatrónNombreVariable}+)", 1, out _);
                        error = $"No se ha podido agregar una fila porque tiene valores repetidos. Para intentar solucionar este problema:{DobleLínea}" +
                                $"1. Verifica que la tabla {nombreTabla} no tenga datos preñadidos. Si los tiene, elimínalos e intenta nuevamente." +
                                $"{NuevaLínea}2. Revisa los datos de la columna {nombreEntidadPropiedad} en el JSON, corrige los valores " +
                                $"repetidos e intenta nuevamente.{DobleLínea}{mensajeExcepciónInterna}";
                        return false;

                    } else if (mensajeExcepciónInterna == "SQLite Error 19: 'FOREIGN KEY constraint failed'.") {

                        error = $"El ID de una tabla relacionada en la tabla {nombreTabla} no existe. Para intentar solucionar este problema:{DobleLínea}" +
                                $"1. Verifica que existan archivos JSON en {rutaJsons} para todas las tablas relacionadas en la tabla " +
                                $"{nombreTabla}.{NuevaLínea}2. Verifica que las tablas relacionadas se esten cargando antes de la " +
                                $"tabla {nombreTabla}. Esto lo puedes hacer reordenando los DbSets de las tablas relacionadas para que estén antes del " +
                                $"DbSet de la tabla {nombreTabla}.{NuevaLínea}3. Para encontrar la fila problemática establece " +
                                $"HabilitarRastreoDeDatosSensibles en true y analiza los datos en la ventana 'Inmediato'.{DobleLínea}{mensajeExcepciónInterna}";
                        return false;

                    } else if (ex.Message.EmpiezaPor("SQLite Error 1: 'no such table:")) {

                        error = $"La tabla {nombreTabla} no existe. Verifica que el nombre de la tabla es correcto y que {RutaBaseDatosSQLite} " +
                                $"exista. Si no existe, haz una copia de {RutaBaseDatosVacíaSQLite}, renómbrala a {RutaBaseDatosSQLite} e " +
                                $"intenta nuevamente.{DobleLínea}{ex.Message}";
                        return false;

                    } else if (ex.Message.EmpiezaPor("is invalid after a value. Expected either")) {

                        error = $"Los datos de la tabla {nombreTabla} pueden contener carácteres problemáticos o corruptos. Este error puede suceder " +
                                @$"si uno de los datos de tipo texto tiene dentro de su valor comillas dobles: "", si es el caso elimina todas las " +
                                $"comillas dobles, genera el JSON actualizado de la tabla {nombreTabla} e intenta nuevamente.{DobleLínea}{ex.Message}";
                        return false;

                    } else if (ex.Message.Contiene("The JSON value could not be converted to System.String")) {

                        var nombrePropiedad = ExtraerConPatrónObsoleta(ex.Message, @$"({PatrónNombreVariable}+)[0-9\[\]]+ \| LineNumber:", 1, out _);
                        error = $"Un dato de la columna {nombreTabla}.{nombrePropiedad} no puede ser convertido a texto. Este error puede suceder si se " +
                                $"trata de un dato de tipo JSON que se va a guardar en una columna de texto y no ha sido correctamente " +
                                $"identificado como tal. Si este es el caso, agrega {nombreTabla}.{nombrePropiedad} a la tabla 'JSONs Forzados' en " +
                                $"'Cargador Datos.xlsm > Procedimiento', genera el JSON actualizado de la tabla {nombreTabla} " +
                                $"e intenta nuevamente.{DobleLínea}{ex.Message}";
                        return false;

                    } else if (ex.Message.Contiene("cannot be tracked because another instance with the key value")) {

                        var parejaClave = ExtraerConPatrónObsoleta(ex.Message, "key value '{(.+?)}' is", 1, out _);
                        error = $"Una fila en la tabla {nombreTabla} contiene valores que forman una clave que ya se encuentra en la tabla. " +
                                $"Elimina los valores {parejaClave} repetidos e intenta nuevamente.{DobleLínea}{ex.Message}";
                        return false;

                    } else if (ex.Message.Contiene("Nullable object must have a value")) {

                        error = $"Es posible que una columna de la tabla {nombreTabla} no se encuentre en el archivo 'Cargador Datos.xlsm'. " + 
                                $"Realiza el procedimiento de 'Consistencia de Cargador Datos.xlsm con Datos.db' e intenta nuevamente." + 
                                $"{DobleLínea}{ex.Message}";
                        return false;

                    } else {
                        throw; // Otro error. Pendiente por clasificar y devolver mensaje adecuado.
                    } 

                }
                #endif

            }

            OperacionesEspecialesDatos = false;
            return true;  

        } // CargarDatosIniciales>


        #endregion Métodos>


        #region Métodos de Lectura de Datos


        public static void CalcularVariablesEstáticas(Contexto ctx) {

            LeerMunicipiosDeInterés(ctx);
            CalcularAtributosProductosYTipos(ctx);

        } // CalcularVariablesIniciales>


        public static void CalcularAtributosProductosYTipos(Contexto ctx) {

            var atributos = ctx.AtributosProductos.Include(ap => ap.Tipo).ToList();
            foreach (var atributo in atributos) {
                if (atributo.Tipo == null) throw new Exception("No se esperaba que el tipo del atributo fuera nulo.");
                AtributosProductosYTipos.Add(atributo.Nombre, atributo.Tipo.Nombre);
                ÍndicesYAtributos.Add(atributo.ID, atributo.Nombre);
            }

            foreach (var rango in Empresa.RangosTallasMediasNuméricas) {
                ÍndicesRangosTallasMediasNuméricas.Add(Producto.ObtenerIDAtributo(rango.Key), Producto.ObtenerIDAtributo(rango.Value));
            }

            foreach (var rango in Empresa.RangosDoblePasoEnSecuenciaTallaNumérica) {
                ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica.Add(Producto.ObtenerIDAtributo(rango.Key), Producto.ObtenerIDAtributo(rango.Value));
            }

        } // CalcularAtributosProductosYTipos>


        /// <summary>
        /// Actualización de los municipios con mensajería y de los datos de los municipios de la empresa.
        /// </summary>
        public static void LeerMunicipiosDeInterés(Contexto ctx) {

            var municipios = ctx.Municipios.Where(m => m.MensajeríaDisponible || m.ID == Empresa.MunicipioFacturaciónID
                || m.ID == Empresa.MunicipioUbicaciónEfectivoID).ToList(); // En una sola consulta a la base de datos obtiene los dos grupos de municipios de interés.

            MunicipiosConMensajería = municipios.Where(m => m.MensajeríaDisponible).Select(m => m.ID).ToList();
            if (municipios.Count > 0) { // Solo debe suceder si la base de datos no tiene municipios. 
                municipios.Single(m => m.ID == Empresa.MunicipioFacturaciónID).CopiarA(ref Empresa.MunicipioFacturación!); // Siempre debe existir.
                municipios.Single(m => m.ID == Empresa.MunicipioUbicaciónEfectivoID).CopiarA(ref Empresa.MunicipioUbicación!); // Siempre debe existir porque incluso en el caso que no haya MunicipioUbicaciónID se usa MunicipioFacturaciónID en su reemplazo.
            } else {

                if (ctx.Municipios.Any()) {
                    throw new Exception($"No se esperaba que la tabla municipios tenga filas y en ellas no se encuentre alguno de los ids de " +
                                        $"municipios de la empresa. MunicipioFacturaciónID: {Empresa.MunicipioFacturaciónID}. " +
                                        $"MunicipioUbicaciónID: {Empresa.MunicipioUbicaciónEfectivoID}");
                } else {
                    throw new Exception($"No se esperaba que la tabla municipios no tenga filas."); // Se puede deber a un error en la creación automática de la base de datos SQLite (ver Global.ConfigurarCarpetasYArchivos()) o a un problema con la base de datos SQL. Para SQLite usualmente se soluciona eliminando el archivo Datos.db en la carpeta de producción para que sea copiado desde la ruta de desarrollo y regenerado automáticamente.
                }

            }

        } // LeerMunicipiosDeInterés>


        /// <summary>
        /// Función equivalente sin pasar contexto para cuando no se tenga.
        /// </summary>
        public static void LeerMunicipiosDeInterés() {
            using var ctx = new Contexto();
            LeerMunicipiosDeInterés(ctx);
        } // LeerMunicipiosDeInterés>


        /// <summary>
        /// Consulta las órdenes de compra pendientes y actualiza la FechaHoraCreación usando las líneas.
        /// </summary>
        public List<OrdenCompra> ObtenerÓrdenesCompraPendientes() {

            var órdenesCompra
                = ÓrdenesCompra.Where(oc => oc.Estado == EstadoSolicitudProducto.Pendiente).Include(oc => oc.Cliente).Include(oc => oc.Líneas).ToList();
            foreach (var ordenCompra in órdenesCompra) {
                if (ordenCompra.Líneas != null && ordenCompra.Líneas.Count > 0)
                    ordenCompra.FechaHoraCreación = ordenCompra.Líneas.Min(ocd => ocd.FechaHoraCreación);
            }
            return órdenesCompra;

        } // ObtenerÓrdenesCompraPendientes>


        /// <summary>
        /// Consulta los pedidos pendientes y actualiza la FechaHoraCreación usando las líneas.
        /// </summary>
        public List<Pedido> ObtenerPedidosPendientes() {

            var pedidos = Pedidos.Where(oc => oc.Estado == EstadoSolicitudProducto.Pendiente).Include(oc => oc.Proveedor).Include(oc => oc.Líneas).ToList();
            foreach (var pedido in pedidos) {
                if (pedido.Líneas != null && pedido.Líneas.Count > 0) pedido.FechaHoraCreación = pedido.Líneas.Min(ocd => ocd.FechaHoraCreación);
            }
            return pedidos;

        } // ObtenerPedidosPendientes>


        public List<Producto> ObtenerProductos() {

            List<Producto> productos;
            if (Empresa.HabilitarProductosBase) {

                productos = Productos.Include(p => p.Base).ToList(); // Alrededor de 235 ms en modo lectura (100 ms más que !HabilitarProductosBase) y 450 ms en modo escritura (130 ms más que !HabilitarProductosBase).
                var porcentajeConBase = Math.Round(productos.Sum(p => p.TieneBase ? 1D : 0D) / productos.Count, 1); // Solo se redondea a una pocisión decimal porque no se requiere mucha exactitud en este cálculo y para evitar escribir innecesariamente el archivo Empresa.json.
                if (porcentajeConBase != Empresa.PorcentajeProductosConBase) {
                    Empresa.PorcentajeProductosConBase = porcentajeConBase;
                    GuardarOpciones(Empresa);
                }

            } else {
                productos = Productos.ToList(); // Alrededor de 135 ms en modo lectura y 320 ms en modo escritura.
            }
            return productos;

        } // ObtenerProductos>


        public Producto? ObtenerProducto(string referencia) {

            Producto? producto;
            Producto? obtenerProductoConInclude() => Productos.Include(p => p.Base).Where(p => p.Referencia == referencia).FirstOrDefault();
            Producto? obtenerProductoSinInclude() => Productos.Where(p => p.Referencia == referencia).FirstOrDefault();

            if (Empresa.HabilitarProductosBase) {
        
                if (TipoContexto == TipoContexto.Lectura) {
                    producto = obtenerProductoConInclude(); // Alrededor de 35 ms en modo lectura (para productos con base y productos sin base) y 110 ms en modo escritura para productos con base y 90 ms en modo escritura para productos sin base.
                } else { // Modo escritura.

                    var duraciónPromedioConInclude = 110 * Empresa.PorcentajeProductosConBase + 90 * (1 - Empresa.PorcentajeProductosConBase); // Es la duración promedio si se cargaran todos los productos uno a uno. Ver valores en la línea anterior.
                    var duraciónPromedioSinInclude = 145 * Empresa.PorcentajeProductosConBase + 80 * (1 - Empresa.PorcentajeProductosConBase); // Ver valores en la línea después del else del siguiente condicional.

                    if (duraciónPromedioConInclude < duraciónPromedioSinInclude) { // Alrededor de Empresa.PorcentajeProductosConBase en 20% se empieza a hacer más rápido en promedio el método sin include. En realidad no es una optimización muy grande, pero ya que el código está desarrollado se dejará porque no tiene ninguna desventaja dejarlo.
                        producto = obtenerProductoConInclude();
                    } else {
                        producto = obtenerProductoSinInclude(); // Igual que !HabilitarProductosBase en modo escritura: 80 ms (para productos con base y productos sin base).
                        if (producto?.TieneBase == true) this.CargarPropiedad(producto, p => p.Base); // Alrededor de 65 ms en modo escritura, 145 ms en total. CargarPropiedad solo funciona con rastreo: TipoContexto.Escritura y TipoContexto.LecturaConRastreo.
                    }

                }
  
            } else {
                producto = obtenerProductoSinInclude(); // Alrededor de 30 ms en modo lectura y 80 ms en modo escritura.
            }

            return producto;

        } // ObtenerProducto>


        #endregion Métodos de Lectura de Datos>


    } // Contexto>



} // SimpleOps.Datos>
