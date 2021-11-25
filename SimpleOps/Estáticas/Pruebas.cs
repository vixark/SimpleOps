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

using SimpleOps.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using static SimpleOps.Global;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using SimpleOps.Modelo;
using static Vixark.General;
using static SimpleOps.Legal.Dian;
using SimpleOps.Legal;
using System.IO;
using RazorEngineCore;
using SimpleOps.DocumentosGráficos;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;
using AutoMapper;
using Vixark;



namespace SimpleOps {



    static class Pruebas {


        /// <summary>
        /// Ejecuta algunas pruebas seleccionadas. Se pueden activar o desactivar con comentarios.
        /// </summary>
        public static void Ejecutar() {

            // LeerBaseDatosCompleta();
            // DocumentosElectrónicos(); // Prueba para ensayar todos los procedimientos relacionados con la facturación electrónica. No genera facturas electrónicas, las pruebas para generar facturas electrónicas se realizan con un botón.
            // ConflictosConcurrencia();
            // ConsultasVarias();
            // InserciónEntidadesRelacionadas();
            // RendimientoConsultasLectura();
            // GeneraciónCatálogo();
            // IntegraciónAplicacionesTerceros();
            // ImágenesEInformaciónProductos();
            // LeerYGuardarPersonalizacionesProductos();
            ProductosYProductosBase();
            AtributosProductos();
            ConsolidaciónAtributosProductoBase();

        } // Ejecutar>


        public static void LeerBaseDatosCompleta() {

            using var ctx = new Contexto(TipoContexto.Lectura);
            var aplicaciones = ctx.Aplicaciones.ToList();
            var campañas = ctx.Campañas.ToList();
            var categorías = ctx.Categorías.ToList();
            var clientes = ctx.Clientes.ToList();
            var cobros = ctx.Cobros.ToList();
            var compras = ctx.Compras.ToList();
            var comprobantesEgresos = ctx.ComprobantesEgresos.ToList();
            var contactos = ctx.Contactos.ToList();
            var contactosClientes = ctx.ContactosClientes.ToList();
            var contactosProveedores = ctx.ContactosProveedores.ToList();
            var líneasCotizaciones = ctx.LíneasCotizaciones.ToList();
            var líneasCompras = ctx.LíneasCompras.ToList();
            var líneasNotasCréditoCompra = ctx.LíneasNotasCréditoCompra.ToList();
            var líneasNotasCréditoVenta = ctx.LíneasNotasCréditoVenta.ToList();
            var líneasNotasDébitoCompra = ctx.LíneasNotasDébitoCompra.ToList();
            var líneasNotasDébitoVenta = ctx.LíneasNotasDébitoVenta.ToList();
            var líneasÓrdenesCompra = ctx.LíneasÓrdenesCompra.ToList();
            var líneasPedidos = ctx.LíneasPedidos.ToList();
            var líneasRemisiones = ctx.LíneasRemisiones.ToList();
            var líneasVentas = ctx.LíneasVentas.ToList();
            var informesPagos = ctx.InformesPagos.ToList();
            var inventariosConsignación = ctx.InventariosConsignación.ToList();
            var listasPrecios = ctx.ListasPrecios.ToList();
            var líneasNegocio = ctx.LíneasNegocio.ToList();
            var marcas = ctx.Marcas.ToList();
            var materiales = ctx.Materiales.ToList();
            var movimientosBancarios = ctx.MovimientosBancarios.ToList();
            var movimientosEfectivo = ctx.MovimientosEfectivo.ToList();
            var municipios = ctx.Municipios.ToList();
            var notasCréditoCompra = ctx.NotasCréditoCompra.ToList();
            var notasCréditoVenta = ctx.NotasCréditoVenta.ToList();
            var notasDébitoCompra = ctx.NotasDébitoCompra.ToList();
            var cotizaciones = ctx.Cotizaciones.ToList();
            var notasDébitoVenta = ctx.NotasDébitoVenta.ToList();
            var órdenesCompra = ctx.ÓrdenesCompra.ToList();
            var pedidos = ctx.Pedidos.ToList();
            var preciosClientes = ctx.PreciosClientes.ToList();
            var preciosProveedores = ctx.PreciosProveedores.ToList();
            var productosBase = ctx.ProductosBase.ToList();
            var productos = ctx.Productos.Include(p => p.Base).ToList();
            var proveedores = ctx.Proveedores.ToList();
            var recibosCaja = ctx.RecibosCaja.ToList();
            var referenciasClientes = ctx.ReferenciasClientes.ToList();
            var referenciasProveedores = ctx.ReferenciasProveedores.ToList();
            var remisiones = ctx.Remisiones.ToList();
            var roles = ctx.Roles.ToList();
            var rolesUsuarios = ctx.RolesUsuarios.ToList();
            var sedes = ctx.Sedes.ToList();
            var subcategorías = ctx.Subcategorías.ToList();
            var usuarios = ctx.Usuarios.ToList();
            var ventas = ctx.Ventas.ToList();

        } // CargaBaseDatosCompleta>


        public static void ConflictosConcurrencia() {

            // Conflicto de Actualización
            using var ctx = new Contexto(TipoContexto.Escritura);
            UsuarioActual.ID = 2; // Para generar conflicto de concurrencia por usuarios actualizadores diferentes. Comentar esta línea para probar el conflicto silenciado por ser un cambio del mismo usuario.
            var milisegundoActual = AhoraUtcAjustado.Millisecond;
            var primerUsuario = ctx.Usuarios.First();
            var primeraCompra = ctx.Compras.First();
            primerUsuario.Teléfono = $"3202010{milisegundoActual}";
            primeraCompra.Subtotal = milisegundoActual * 10000;

            using var ctx2 = new Contexto(TipoContexto.Escritura);
            var últimoUsuario = ctx2.Usuarios.OrderByDescending(u => u.ID).First();
            var primerUsuario2 = ctx2.Usuarios.First();
            var primeraCompra2 = ctx2.Compras.Include(c => c.Proveedor).First();
            ctx.GuardarCambios(); // Para generar conflicto de concurrencia. Mover antes de var ctx2 para simular caso sin conflicto de concurrencia.
            UsuarioActual.ID = 1; // Para generar conflicto de concurrencia por usuarios actualizadores diferentes.
            primeraCompra2.Subtotal = milisegundoActual * 10000 + 22;
            últimoUsuario.Teléfono = $"3000000{milisegundoActual} Ctx2";
            primerUsuario2.Teléfono = $"3202010{milisegundoActual} Ctx2";
            ctx2.GuardarCambios(); // Aquí se generan los mensajes de solución de conflicto de concurrencia.
            // Conflicto de Actualización>

            // Conflicto de Inserción para Entidad sin Control Inserción
            using var ctx3 = new Contexto(TipoContexto.Escritura);
            UsuarioActual.ID = 1;
            var últimoCliente3 = ctx3.Clientes.OrderByDescending(u => u.ID).First();
            ctx3.Cobros.Add(new Cobro(últimoCliente3) { Respuesta = "El cliente solicita que lo llamen el viernes a las 3 pm. (usuario 1)" });

            using var ctx4 = new Contexto(TipoContexto.Escritura);
            var últimoCliente4 = ctx4.Clientes.OrderByDescending(u => u.ID).First();
            ctx4.Cobros.Add(new Cobro(últimoCliente4) { Respuesta = "Llamar el viernes a las 3 (usuario 2)" });

            ctx3.GuardarCambios();
            UsuarioActual.ID = 2;
            ctx4.GuardarCambios();
            // Conflicto de Inserción para Entidad sin Control Inserción>

            // Conflicto de Inserción para Entidad con Control Inserción Optimista
            using var ctx5 = new Contexto(TipoContexto.Escritura);
            UsuarioActual.ID = 1;
            var primerCliente5 = ctx5.Clientes.First();
            var primerProducto5 = ctx5.Productos.Skip(milisegundoActual).First();
            ctx5.PreciosClientes.Add(new PrecioCliente(primerProducto5, primerCliente5, 5000));

            using var ctx6 = new Contexto(TipoContexto.Escritura);
            var primerCliente6 = ctx6.Clientes.First();
            var primerProducto6 = ctx6.Productos.Skip(milisegundoActual).First();
            var segundoProducto6 = ctx6.Productos.Skip(milisegundoActual + 1).First();
            ctx6.PreciosClientes.Add(new PrecioCliente(segundoProducto6, primerCliente6, 7000)); // Una operación que no tiene nada que ver con el conflicto.
            ctx6.ReferenciasClientes.Add(new ReferenciaCliente(primerProducto6, primerCliente6, "6-PP-" + milisegundoActual)); // Otra operación que no tiene nada que ver con el conflicto.
            ctx6.PreciosClientes.Add(new PrecioCliente(primerProducto6, primerCliente6, 6000)); // Operación que genera el conflicto de inserción.
            ctx6.ReferenciasClientes.Add(new ReferenciaCliente(segundoProducto6, primerCliente6, "6-SS-" + milisegundoActual)); // Otra operación que no tiene nada que ver con el conflicto.

            ctx5.GuardarCambios();
            UsuarioActual.ID = 2;
            ctx6.GuardarCambios();
            // Conflicto de Inserción para Entidad con ControlInserción = Optimista>

            // Combinación de Conflicto de Actualización con Conflicto de Inserción para Entidad con ControlInserción = Optimista
            using var ctx7 = new Contexto(TipoContexto.Escritura);
            UsuarioActual.ID = 1;
            var cliente7 = ctx7.Clientes.Skip(milisegundoActual).First();
            var producto7 = ctx7.Productos.Skip(milisegundoActual).First();
            var cliente72 = ctx7.Clientes.Skip(milisegundoActual + 2).First();
            cliente72.Teléfono = "55555";
            ctx7.PreciosClientes.Add(new PrecioCliente(producto7, cliente7, 7000 + 2));

            using var ctx8 = new Contexto(TipoContexto.Escritura);
            var cliente8 = ctx8.Clientes.Skip(milisegundoActual).First();
            var producto8 = ctx8.Productos.Skip(milisegundoActual).First();
            var cliente82 = ctx8.Clientes.Skip(milisegundoActual + 2).First();
            var producto82 = ctx8.Productos.Skip(milisegundoActual + 1).First();
            ctx8.PreciosClientes.Add(new PrecioCliente(producto8, cliente8, 8000)); // Operación que genera el conflicto de inserción.
            ctx8.ReferenciasClientes.Add(new ReferenciaCliente(producto82, cliente8, "8-SS-" + milisegundoActual)); // Una operación que no tiene nada que ver con el conflicto.
            cliente82.Teléfono = "555552"; // Operación que genera el conflicto de actualización. Se puede alternar esta línea con la anterior para probar el funcionamiento según el orden de ocurrencia de los conflictos aunque independiente del orden siempre se genera primero el conflicto de actualización con la excepción DbUpdateConcurrencyException.

            ctx7.GuardarCambios();
            UsuarioActual.ID = 2;
            ctx8.GuardarCambios();
            // Combinación de Conflicto de Actualización con Conflicto de Inserción para Entidad con ControlInserción = Optimista>

            // Bloqueos
            using var ctx9 = new Contexto(TipoContexto.Escritura);
            var listaDatosBloqueo = new List<BloqueoVarias> {
                new BloqueoVarias(nameof(Venta)) { IDs = new List<int> { 1, 2, 3, 4, 5, 6, 7 }, Propiedad = "Subtotal", Tipo = TipoPermiso.Inserción | TipoPermiso.Eliminación },
                new BloqueoVarias(nameof(LíneaVenta)) { Propiedad = "Cantidad" },
                new BloqueoVarias(nameof(LíneaVenta)) { Propiedad = "CostoUnitario" },
                new BloqueoVarias(nameof(Producto)) { IDs = new List<int> { 555 }}
            };

            var resultadoBloqueo5 = ctx9.VerificarYBloquear(listaDatosBloqueo);
            if (resultadoBloqueo5.Éxitoso) {
                // Se puede abrir la interface para ejecutar cierta acción y realizarla. Esto puede tardar entre 1 a 10 minutos de parte del usuario.
                // Al final de la acción se debe verificar si el bloqueo permanece y los mismos bloqueos solicitados aún permanecen de parte del usuario actual para asegurar que no ha sucedido nada extraño. Si se encuentra alguna inconsistencia aquí se puede devolver la operación.
            }
            // Bloqueos>

        } // ConflictosConcurrencia>


        public static void ConsultasVarias() {

            var milisegundoActual = AhoraUtcAjustado.Millisecond;
            using var ctx = new Contexto(TipoContexto.Lectura);
            var consulta = from contactoCliente in ctx.Set<ContactoCliente>()
                           join cliente in ctx.Set<Cliente>() on contactoCliente.ClienteID equals cliente.ID
                           select new { contactoCliente, cliente };
            var consultaLista = consulta.ToList();
            var venta = ctx.Ventas.Skip(milisegundoActual).First();
            var cantidadLíneasMásDe20 = ctx.Entry(venta).Collection(v => v.Líneas).Query().Where(dv => dv.Cantidad > 20).Count();
            var ventasPorSubtotal = ctx.Ventas.OrderByDescending(v => v.Subtotal).ToList(); // Se ejecuta en la base de datos si se ha establecido la conversión decimal-double en OnModelCreating().

        } // ConsultasVarias>


        public static void PersonalizacionesProductos() {

            var productoBase = new ProductoBase("654") { Descripción = "Silla Tapizada" };
            var producto1 = new Producto("654-1", productoBase,
                new List<string> { "Sin Apoyabrazos", "Altura 43 cm", "Recubierta", "Tela AAA", "Patas Plásticas" });
            producto1.AgregarPersonalización("Nombre en Pata", new List<string> { "Jesús", "Dora" });
            producto1.AgregarPersonalización("Nombre en Pata", new List<string> { "Josué", "Diana" }, reemplazarExistente: false);
            producto1.AgregarPersonalización("Nombre en Placa", new List<string> { "Pedro", "María", "Natalia" });
            producto1.AgregarPersonalización("Nombre en Placa", new List<string>()); // Reempaza por personalización sin valores predeterminados.
            try {
                producto1.EliminarPersonalización("Nombre en Pata");
                MostrarError("No se esperaba que no se lanzara excepción.");
            } catch (Exception) {
                throw;
            }
            producto1.EliminarPersonalización("Nombre en Pata", eliminarVarias: true);

        } // PersonalizacionesProductos>


        public static void AtributosProductos() {

            var productoBase = new ProductoBase("CMS") { Descripción = "Camiseta manga corta con cuello", Físico = false };
            var producto = new Producto("CMSARM", productoBase,
                new List<string> { "Amarilla", "Rojo Oscuro", "Talla M", "Cuello Reforzado" });
            if (!producto.AgregarAtributo("Azul")) MostrarError("No se esperaba que no se pudiera eliminar el atributo Azul.");
            if (producto.EliminarAtributo("Amarillito")) MostrarError("No se esperaba que se pudiera eliminar el atributo amarillito.");
            if (!producto.AgregarAtributo("talla s")) MostrarError("No se esperaba que no se pudiera agregar el atributo Talla S.");
            if (producto.AgregarAtributo("Talla s")) MostrarError("No se esperaba que se pudiera agregar el atributo Talla S.");
            if (!producto.EliminarAtributo("Talla M")) MostrarError("No se esperaba que no se pudiera eliminar el atributo Talla M.");
            if (!producto.EliminarAtributo("Rojo Oscuro")) MostrarError("No se esperaba que no se pudiera eliminar el atributo Rojo Oscuro.");
            if (!producto.TieneAtributo("Talla S")) MostrarError("No se esperaba que no tuviera el atributo Talla S.");
            if (producto.AgregarAtributo("Cuello reforzado")) MostrarError("No se esperaba que se pudiera agregar el atributo Cuello Reforzado.");
            if (!producto.AgregarAtributo("Cosida a Mano")) MostrarError("No se esperaba que no se pudiera agregar el atributo Cosido a Mano.");
            if (Producto.ObtenerTipoAtributo("talla 50") != "Talla Numérica")
                MostrarError("No se esperaba que el tipo atributo de Talla 50 no fuera Talla Numérica.");
            if (Producto.ObtenerTipoAtributo("talle 50") == "Talla Numérica")
                MostrarError("No se esperaba que el tipo atributo de Talle 50 fuera Talla Numérica.");
            producto.Atributos.Add("cuello reforzado"); // Se agrega de esta manera para generar un duplicado de atributo libre.
            producto.Atributos.Add("Talla S"); // Se agrega de esta manera para generar un duplicado de atributo.
            if (producto.ObtenerAtributosNoRepetidos().Count == producto.Atributos.Count)
                MostrarError("No se esperaba que ObtenerAtributos() tuviera la misma cantidad de elementos que Atributos.");
            if (producto.AgregarAtributo("Cuello de Tortuga", permitirAtributosLibres: false))
                MostrarError("No se esperaba poder agregar el atributo Cuello de Tortuga no permitiendo los atributos libres.");

            var clasificaciónAtributosRepetidos = producto.ClasificarAtributos(permitirRepetidos: true);
            var clasificaciónAtributosNoRepetidos = producto.ClasificarAtributos(permitirRepetidos: false);
            if (clasificaciónAtributosNoRepetidos[TipoAtributoProductoLibre].Count == clasificaciónAtributosRepetidos[TipoAtributoProductoLibre].Count)
                MostrarError("No se esperaba que clasificaciónAtributosNoRepetidos y clasificaciónAtributosRepetidos tuvieran la misma cantidad " +
                    "de elementos.");
            if (clasificaciónAtributosNoRepetidos["Talla Alfabética"].Count == clasificaciónAtributosRepetidos["Talla Alfabética"].Count)
                MostrarError("No se esperaba que clasificaciónANLRepetidos y clasificaciónANLNoRepetidos tuvieran la misma cantidad de elementos.");

            var diccPrueba = new Dictionary<string, int>() { { "a", 1 }, { "b", 2 }, { "c", 3 } };
            var valorA = diccPrueba.ObtenerValor("A");
            if (valorA == null) MostrarError("No se esperaba no encontrar el valor de A.");
            var valorG = diccPrueba.ObtenerValor("G");
            if (valorG != null) MostrarError("No se esperaba encontrar el valor de G.");
            var valorA2 = diccPrueba.ObtenerValor("A", ignorarCapitalización: false);
            if (valorA2 != null) MostrarError("No se esperaba encontrar el valor de A sin ignorar la capitalización.");
            if (!diccPrueba.ContieneClave("a", ignorarCapitalización: true)) MostrarError("No se esperaba no encontrar el valor de a.");
            if (diccPrueba.ContieneClave("A", ignorarCapitalización: false))
                MostrarError("No se esperaba encontrar el valor de A sin ignorar la capitalización.");
            if (diccPrueba.ContieneClave("G", ignorarCapitalización: false)) MostrarError("No se esperaba encontrar el valor de G.");

            var claves = diccPrueba.Keys.ToList();
            var claveA = claves.ObtenerValorCapitalizaciónCorrecta("A");
            if (claveA != "a") MostrarError("No se esperaba no encontrar la capitalización a correcta de la clave.");

        } // AtributosProductos>


        public static void ConsolidaciónAtributosProductoBase() {

            var b = new ProductoBase("CM");
            var c = new Cotización(new Cliente("Distribuciones ABC", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor));
            c.Líneas.Add(new LíneaCotización(c, new Producto("1", b, new List<string> { "Azul", "Roja", "Talla L", "Extra Suave" }), 82800) { }); // Azul, Roja, Talla L, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("2", b, new List<string> { "Azul", "Talla XL", "Extra Suave" }), 82800) { }); // Azul, Talla XL, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("3", b, new List<string> { "Verde", "Talla M", "Extra Suave" }), 82800) { }); // Verde, Talla M, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("4", b, new List<string> { "Verde", "Talla L", "Extra Suave" }), 82800) { }); // Verde, Talla L, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("5", b, new List<string> { "Azul", "Talla M", "Extra Suave" }), 82800) { }); // Azul, Talla M, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("6", b, new List<string> { "Azul", "Talla XXL", "Extra Suave" }), 93500) { }); // Azul, Talla XXL, Extra Suave 93 500.
            c.Líneas.Add(new LíneaCotización(c, new Producto("7", b, new List<string> { "Azul", "Talla 3XL", "Extra Suave" }), 82800) { }); // Azul, Talla 3XL, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("8", b, new List<string> { "Azul", "Talla XS", "Extra Suave" }), 82800) { }); // Azul, Talla XS, Extra Suave 82 800.
            c.Líneas.Add(new LíneaCotización(c, new Producto("9", b, new List<string> { "Azul", "Talla XXL", "Estándar" }), 65500) { }); // Azul, Talla XXL, Estándar 65 500.
            c.Líneas.Add(new LíneaCotización(c, new Producto("10", b, new List<string> { "Azul", "Talla L", "Estándar" }), 60000) { }); // Azul, Talla L, Estándar 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("11", b, new List<string> { "Azul", "Talla M", "Estándar" }), 60000) { }); // Azul, Talla M, Estándar 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("12", b, new List<string> { "Verde", "Talla M", "Estándar" }), 60000) { }); // Verde, Talla M, Estándar 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("13", b, new List<string> { "Roja", "Talla M", "Estándar" }), 60000) { }); // Roja, Talla M, Estándar 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("14", b, new List<string> { "Roja", "Talla M", "Estándar", "Copa C" }), 60000) { }); // Roja, Talla M, Estándar, Copa C 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("15", b, new List<string> { "Roja", "Talla M", "Estándar", "Copa B" }), 60000) { }); // Roja, Talla M, Estándar, Copa B 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("16", b, new List<string> { "Roja", "Talla M", "Estándar", "Copa A" }), 60000) { }); // Roja, Talla M, Estándar, Copa A 60 000.
            c.Líneas.Add(new LíneaCotización(c, new Producto("17", b, new List<string> { "Roja", "Verde", "Talla M", "Estándar", "Copa DD" }), 60000) { }); // Roja, Verde, Talla M, Estándar, Copa DD 60 000.

            var dp = c.ObtenerDatos(new OpcionesDocumento(), PlantillaDocumento.CatálogoPdf).DatosProductos["CM"];
            if (dp.Atributos[0] != "Azul, Roja, Verde, Azul+Roja y Roja+Verde")
                MostrarError("No coincidió el atributo 0 en DatosProductoBase().");
            if (dp.Atributos[1] != "Copa A a C y DD") MostrarError("No coincidió el atributo 1 en DatosProductoBase().");

            var cuenta = 0;
            foreach (var kv in dp.Precios) {

                var atributos = kv.Key;
                var precio = kv.Value;
                switch (cuenta) {
                    case 0:
                        if (precio != 60000) MostrarError($"No coincidió el precio id {cuenta} en DatosProductoBase().");
                        if (atributos[0] != "Talla M y L") MostrarError($"No coincidió el atributo 0 precio id {cuenta} en DatosProductoBase().");
                        if (atributos[1] != "Estándar") MostrarError($"No coincidió el atributo 1 precio id {cuenta} en DatosProductoBase().");
                        break;
                    case 1:
                        if (precio != 82800) MostrarError($"No coincidió el precio id {cuenta} en DatosProductoBase().");
                        if (atributos[0] != "Talla XS, M a XL y 3XL") MostrarError($"No coincidió el atributo 0 precio id {cuenta} en DatosProductoBase().");
                        if (atributos[1] != "Extra Suave") MostrarError($"No coincidió el atributo 1 precio id {cuenta} en DatosProductoBase().");
                        break;
                    case 2:
                        if (precio != 65500) MostrarError($"No coincidió el precio id {cuenta} en DatosProductoBase()");
                        if (atributos[0] != "Talla XXL") MostrarError($"No coincidió el atributo 0 precio id {cuenta} en DatosProductoBase().");
                        if (atributos[1] != "Estándar") MostrarError($"No coincidió el atributo 1 precio id {cuenta} en DatosProductoBase().");
                        break;
                    case 3:
                        if (precio != 93500) MostrarError($"No coincidió el precio id {cuenta} en DatosProductoBase().");
                        if (atributos[0] != "Talla XXL") MostrarError($"No coincidió el atributo 0 precio id {cuenta} en DatosProductoBase().");
                        if (atributos[1] != "Extra Suave") MostrarError($"No coincidió el atributo 1 precio id {cuenta} en DatosProductoBase().");
                        break;
                    default:
                        MostrarError($"No se esperaba un producto id {cuenta} en DatosProductoBase().");
                        break;
                }
                cuenta++;

            }

            var atributos2 = new List<string> { "talla especial   de 40", "tallA especial 40   de 50", "Talla especial de 55" };
            var atributos2Resumidos = atributos2.ATextoConComas(resumir: true);
            if (atributos2Resumidos != "talla especial de 40, 40 de 50 y de 55")
                MostrarError($"Falló el procedimiento de ATextoConComas(resumir: true) para atributos2Resumidos.");

            var atributos3 = new List<string> { "talla 8", "talla 9", "" };
            var atributos3Resumidos = atributos3.ATextoConComas(resumir: true);
            if (atributos3Resumidos != "talla 8, talla 9 y ")
                MostrarError($"Falló el procedimiento de ATextoConComas(resumir: true) para atributos3Resumidos.");

            var b2 = new ProductoBase("CM");
            var c2 = new Cotización(new Cliente("Distribuciones ABC", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor));
            c2.Líneas.Add(new LíneaCotización(c, new Producto("1", b2, new List<string> { "Azul", "Talla 12" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("2", b2, new List<string> { "Azul", "Talla 12.5" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("3", b2, new List<string> { "Azul", "Talla 13" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("4", b2, new List<string> { "Azul", "Talla 13.5" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("5", b2, new List<string> { "Azul", "Talla 14" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("6", b2, new List<string> { "Azul", "Talla 16" }), 82800) { }); // Justo para ser considerado secuencia.
            c2.Líneas.Add(new LíneaCotización(c, new Producto("7", b2, new List<string> { "Verde", "Talla 17" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("8", b2, new List<string> { "Azul", "Talla 20" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("9", b2, new List<string> { "Azul", "Talla 21" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("10", b2, new List<string> { "Azul", "Talla 22" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("11", b2, new List<string> { "Azul", "Talla 29" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("12", b2, new List<string> { "Azul", "Talla 30" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("13", b2, new List<string> { "Azul", "Talla 32" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("14", b2, new List<string> { "Azul", "Talla 34" }), 82800) { }); // Justo para ser considerado secuencia.
            c2.Líneas.Add(new LíneaCotización(c, new Producto("15", b2, new List<string> { "Azul", "Talla 34.5" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("16", b2, new List<string> { "Azul", "Talla 36.5" }), 82800) { }); // Justo para ser considerado secuencia.
            c2.Líneas.Add(new LíneaCotización(c, new Producto("17", b2, new List<string> { "Azul", "Talla 42" }), 82800) { }); // Justo para no ser considerado secuencia.
            c2.Líneas.Add(new LíneaCotización(c, new Producto("18", b2, new List<string> { "Azul", "Talla 44" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("19", b2, new List<string> { "Azul", "Talla 44.5" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("20", b2, new List<string> { "Azul", "Talla 47" }), 82800) { }); // Justo para no ser considerado secuencia.
            c2.Líneas.Add(new LíneaCotización(c, new Producto("21", b2, new List<string> { "Azul", "Talla 50" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("22", b2, new List<string> { "Azul", "Talla 51" }), 82800) { });
            c2.Líneas.Add(new LíneaCotización(c, new Producto("23", b2, new List<string> { "Azul", "Talla 53" }), 82800) { });
            var dp2 = c2.ObtenerDatos(new OpcionesDocumento(), PlantillaDocumento.CatálogoPdf).DatosProductos["CM"];
            if (dp2.Atributos[1] != "Talla 12 a 17, 20 a 22, 29 a 36.5, 42 a 44.5, 47 y 50 a 53")
                MostrarError($"Falló el procedimiento de ObtenerDatos() para dp2.");

            var b3 = new ProductoBase("CM");
            var c3 = new Cotización(new Cliente("Distribuciones ABC", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor));
            c3.Líneas.Add(new LíneaCotización(c, new Producto("1", b3, new List<string> { "Azul", "Talla 13.5" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("2", b3, new List<string> { "Azul", "Talla 16" }), 82800) { }); // Justo para no ser considerado secuencia.
            c3.Líneas.Add(new LíneaCotización(c, new Producto("3", b3, new List<string> { "Verde", "Talla 17" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("4", b3, new List<string> { "Verde", "Talla 18" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("5", b3, new List<string> { "Azul", "Talla 29" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("6", b3, new List<string> { "Azul", "Talla 30" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("7", b3, new List<string> { "Azul", "Talla 31" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("8", b3, new List<string> { "Azul", "Talla 34" }), 82800) { }); // Justo para no ser considerado secuencia.
            c3.Líneas.Add(new LíneaCotización(c, new Producto("9", b3, new List<string> { "Azul", "Talla 34.5" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("10", b3, new List<string> { "Azul", "Talla 37" }), 82800) { }); // Justo para no ser considerado secuencia.
            c3.Líneas.Add(new LíneaCotización(c, new Producto("11", b3, new List<string> { "Azul", "Talla 38" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("12", b3, new List<string> { "Azul", "Talla 39" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("13", b3, new List<string> { "Azul", "Talla 42" }), 82800) { }); // Justo para no ser considerado secuencia.
            c3.Líneas.Add(new LíneaCotización(c, new Producto("14", b3, new List<string> { "Azul", "Talla 43.5" }), 82800) { });
            c3.Líneas.Add(new LíneaCotización(c, new Producto("15", b3, new List<string> { "Azul", "Talla 45" }), 82800) { }); // Justo para ser considerado secuencia.
            c3.Líneas.Add(new LíneaCotización(c, new Producto("16", b3, new List<string> { "Azul", "Talla 46" }), 82800) { });
            var dp3 = c3.ObtenerDatos(new OpcionesDocumento(), PlantillaDocumento.CatálogoPdf).DatosProductos["CM"];
            if (dp3.Atributos[1] != "Talla 13.5, 16 a 18, 29 a 31, 34, 34.5, 37 a 39 y 42 a 46")
                MostrarError($"Falló el procedimiento de ObtenerDatos() para dp3.");

            var b4 = new ProductoBase("CM");
            var c4 = new Cotización(new Cliente("Distribuciones ABC", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor));
            c4.Líneas.Add(new LíneaCotización(c, new Producto("1", b4, new List<string> { "Azul", "Talla 8" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("2", b4, new List<string> { "Azul", "Talla 10" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("3", b4, new List<string> { "Verde", "Talla 11" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("4", b4, new List<string> { "Verde", "Talla 13" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("5", b4, new List<string> { "Azul", "Talla 14" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("6", b4, new List<string> { "Azul", "Talla 15" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("7", b4, new List<string> { "Azul", "Talla 31" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("8", b4, new List<string> { "Azul", "Talla 34" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("9", b4, new List<string> { "Azul", "Talla 36" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("10", b4, new List<string> { "Azul", "Talla 38" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("11", b4, new List<string> { "Azul", "Talla 42" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("12", b4, new List<string> { "Azul", "Talla 43" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("13", b4, new List<string> { "Azul", "Talla 44" }), 82800) { });
            c4.Líneas.Add(new LíneaCotización(c, new Producto("14", b4, new List<string> { "Azul", "Talla 46" }), 82800) { });

            Empresa.RangosDoblePasoEnSecuenciaTallaNumérica = new Dictionary<string, string> { { "Talla 30", "Talla 50" } };
            ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica.Clear();
            foreach (var rango in Empresa.RangosDoblePasoEnSecuenciaTallaNumérica) {
                ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica.Add(Producto.ObtenerIDAtributo(rango.Key), Producto.ObtenerIDAtributo(rango.Value));
            }
            var dp4 = c4.ObtenerDatos(new OpcionesDocumento(), PlantillaDocumento.CatálogoPdf).DatosProductos["CM"];
            Empresa.RangosDoblePasoEnSecuenciaTallaNumérica = new Dictionary<string, string> { { "Talla 0", "Talla 80" } }; // Se devuelve al valor predeterminado.
            ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica.Clear();
            foreach (var rango in Empresa.RangosDoblePasoEnSecuenciaTallaNumérica) {
                ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica.Add(Producto.ObtenerIDAtributo(rango.Key), Producto.ObtenerIDAtributo(rango.Value));
            }
            if (dp4.Atributos[1] != "Talla 8, 10, 11, 13 a 15, 31, 34 a 38 y 42 a 46")
                MostrarError($"Falló el procedimiento de ObtenerDatos() para dp4.");

        } // ConsolidaciónAtributosProductoBase>


        public static void ImágenesEInformaciónProductos() {

            var ZZM14CEJSO = new ProductoBase("ZZM14CEJSO") { Descripción = "Monitor de Computador Usado" };
            var ZZM14CEJSOT15 = new Producto("ZZM14CEJSOT15", ZZM14CEJSO, new List<string> { "Talla 15" });
            _ = ZZM14CEJSOT15.ObtenerInformaciónHtml(codificarImágenes: false, rutaCarpetaImágenes: "https://www.w3schools.com"); // Se puede usar https://www.w3schools.com y https://www.w3schools.com/. Para visualizar el resultado, se puede dar suspender la ejecución, poner el mouse encima de la variable y en la lupa darle clic en Visualizador HTML.
            _ = ZZM14CEJSOT15.ObtenerInformaciónHtml(codificarImágenes: true); // Las imágenes que se usan en los archivos de información normalmente no son las imágenes de los productos por que estas se agregan a las fichas técnicas y a las páginas web de producto desde otro lugar. Desde la plantilla CSHTML para la ficha técnica y desde el código de la web para la página de la web. Las imágenes a incluir en los archivos de información son auxiliares y se guardan predeterminadamente en SimpleOps/Productos/Información/Imágenes/.

            var TV30EJSO = new Producto("TV30EJSO") { DescripciónBase = "Televisor de Vanguardia", ArchivoInformaciónEspecífica = "zzTV30EJSO" }; // Se establece manualmente el archivo de información solo para probar la funcionalidad de poder establecerlo sin extensión.
            _ = TV30EJSO.ObtenerInformaciónHtml(codificarImágenes: false);

            var muebleBase = new ProductoBase("MBASE") { ArchivoImagen = "zzEM2EJSO" };
            var EM2EJSO = new Producto("EM2EJSO", muebleBase, new List<string> { "Café" });
            var rutaImagenEM2EJSO = EM2EJSO.ObtenerRutaImagen();

            AbrirArchivo(rutaImagenEM2EJSO);
            AbrirArchivo(Path.Combine(ObtenerRutaInformaciónCompiladosProductos(), @$"{ZZM14CEJSOT15.Referencia}{".html"}"));
            AbrirArchivo(Path.Combine(ObtenerRutaInformaciónCompiladosProductos(), @$"{TV30EJSO.ArchivoInformaciónEspecífica}{".html"}"));

        } // ImágenesEInformaciónProductos>


        public static void InserciónEntidadesRelacionadas() {

            var milisegundoActual = AhoraUtcAjustado.Millisecond;
            using var ctx = new Contexto(TipoContexto.Escritura);

            var contactoNuevo = new Contacto($"[EmailNuevoContacto {milisegundoActual}]");
            var clienteNuevo = new Cliente($"[NombreNuevoCliente {milisegundoActual}]");
            var proveedorNuevo = new Proveedor($"[NombreNuevoProveedor {milisegundoActual}]");
            var productoNuevo = new Producto($"[ReferenciaNuevoProducto {milisegundoActual}]");
            var precioClienteNuevo = new PrecioCliente(productoNuevo, clienteNuevo, 1000 + milisegundoActual);
            var precioProveedorNuevo = new PrecioProveedor(productoNuevo, proveedorNuevo, 500 + milisegundoActual);
            var contactoClienteNuevo = new ContactoCliente(clienteNuevo, contactoNuevo);
            var contactoProveedorNuevo = new ContactoProveedor(proveedorNuevo, contactoNuevo);
            var referenciaClienteNueva = new ReferenciaCliente(productoNuevo, clienteNuevo, $"[ReferenciaClienteNueva {milisegundoActual}]");
            var referenciaProveedorNueva = new ReferenciaProveedor(productoNuevo, proveedorNuevo, $"[ReferenciaProveedorNueva {milisegundoActual}]");
            var inventarioConsignaciónNuevo = new InventarioConsignación(productoNuevo, clienteNuevo, 100 + milisegundoActual, 1000 + milisegundoActual);
            var comprobanteEgresoNuevo = new ComprobanteEgreso(proveedorNuevo, LugarMovimientoDinero.Caja, 10000 + milisegundoActual);
            var reciboCajaNuevo = new ReciboCaja(clienteNuevo, LugarMovimientoDinero.Caja, 1000 + milisegundoActual);
            ctx.Contactos.Add(contactoNuevo);
            ctx.Clientes.Add(clienteNuevo);
            ctx.Proveedores.Add(proveedorNuevo);
            ctx.Productos.Add(productoNuevo);
            ctx.PreciosClientes.Add(precioClienteNuevo);
            ctx.PreciosProveedores.Add(precioProveedorNuevo);
            ctx.ContactosClientes.Add(contactoClienteNuevo); // No es necesario agregar clienteNuevo y contactoNuevo pues se agregan al agregar contactoClienteNuevo. Pero se hace por consistencia del código y para facilitar su revisión.
            ctx.ContactosProveedores.Add(contactoProveedorNuevo);
            ctx.ReferenciasClientes.Add(referenciaClienteNueva);
            ctx.ReferenciasProveedores.Add(referenciaProveedorNueva);
            ctx.InventariosConsignación.Add(inventarioConsignaciónNuevo);
            ctx.ComprobantesEgresos.Add(comprobanteEgresoNuevo);
            ctx.RecibosCaja.Add(reciboCajaNuevo);
            ctx.GuardarCambios();

        } // InserciónEntidadesRelacionadas>


        public static void RendimientoConsultasLectura() {

            var milisegundoActual = AhoraUtcAjustado.Millisecond;
            using var ctx = new Contexto(TipoContexto.LecturaConRastreo); // Se inicia como contexto de LecturaConRastreo para poder realizar las pruebas con y sin AsNoTracking().
            var ventasPorSubtotal = ctx.Ventas.OrderByDescending(v => v.Subtotal).ToList(); // Una consulta cualquiera para que el contexto se inicie.

#if true

            // Rendimiento 1 Entidad - Probar cada línea en una ejecución de SimpleOps independiente.
            var clienteCompleto = ctx.Clientes.Skip(milisegundoActual).First(); // 76 milisegundos en promedio.
            var clienteCompletoAsNoTracking = ctx.Clientes.AsNoTracking().Skip(milisegundoActual).First(); // 26 milisegundos en promedio. Esta es la opción recomendada si los datos se necesitan solo para lectura.
            var clienteTipoAnónimo = ctx.Clientes.Skip(milisegundoActual).Select(c => new { c.Nombre, id = c.ID, c.Teléfono }).First(); // 31 milisegundos en promedio.  
            var clienteTipoAnónimoAsNoTracking = ctx.Clientes.AsNoTracking().Skip(milisegundoActual)
                .Select(c => new { c.Nombre, id = c.ID, c.Teléfono }).First(); // 34 milisegundos en promedio.
            // Rendimiento 1 Entidad> 

            // Rendimiento 1 Entidad con Hijos - Probar cada línea en una ejecución de SimpleOp independiente.
            var clienteCompleto2 = ctx.Clientes.Include(c => c.ContactosClientes).Single(c => c.ID == 7408); // 132 milisegundos en promedio.
            var clienteCompletoAsNoTracking2 = ctx.Clientes.AsNoTracking().Include(c => c.ContactosClientes).Single(c => c.ID == 7408); // 63 milisegundos en promedio.

            var clienteCompletoCargarLista2 = ctx.Clientes.Find(7408);
            ctx.CargarLista(clienteCompletoCargarLista2, c => c.ContactosClientes); // 146 milisegundos en promedio.

            var clienteTipoAnónimo2 = ctx.Clientes.Select(c => new { c.Nombre, c.ID, c.Teléfono, c.ContactosClientes }).Single(c => c.ID == 7408); // 75 milisegundos en promedio.
            var clienteTipoAnónimoAsNoTracking2 = ctx.Clientes.AsNoTracking()
                .Select(c => new { c.Nombre, c.ID, c.Teléfono, c.ContactosClientes }).Single(c => c.ID == 7408); // 62 milisegundos en promedio.
            // Rendimiento 1 Entidad con Hijos>

            // Rendimiento 1 Entidad con Hijos y Nietos
            var clienteCompleto3 = ctx.Clientes.Include(c => c.ContactosClientes).ThenInclude(ct => ct.Contacto).Single(c => c.ID == 7408); // 162 milisegundos en promedio.
            var clienteCompletoAsNoTracking3 = ctx.Clientes.AsNoTracking().Include(c => c.ContactosClientes)
                .ThenInclude(ct => ct.Contacto).Single(c => c.ID == 7408); // 81 milisegundos en promedio.

            var clienteCompletoCargarLista3 = ctx.Clientes.Find(7408);
            ctx.CargarLista(clienteCompletoCargarLista3, c => c.ContactosClientes); 
            ctx.CargarPropiedad(clienteCompletoCargarLista3.ContactosClientes.First(), ct => ct.Contacto); // 170 milisegundos en promedio.

            var clienteTipoAnónimo3 = ctx.Clientes.Select(c => new { c.Nombre, c.ID, c.Teléfono, 
                Contactos = c.ContactosClientes.Select(a => new { a.Contacto }) }).Single(c => c.ID == 7408); // 93 milisegundos en promedio.
            var clienteTipoAnónimoAsNoTracking3 = ctx.Clientes.AsNoTracking().Select(c => new { c.Nombre, c.ID, c.Teléfono, 
                Contactos = c.ContactosClientes.Select(a => new { a.Contacto }) }).Single(c => c.ID == 7408); // 71 milisegundos en promedio.
            // Rendimiento 1 Entidad con Hijos y Nietos>

            // Rendimiento 1 Entidad con 1 Hijo
            var clienteCompleto4 = ctx.Clientes.Include(c => c.Municipio).Skip(milisegundoActual).First(); // 148 milisegundos en promedio.
            var clienteCompletoAsNoTracking4 = ctx.Clientes.AsNoTracking().Include(c => c.Municipio).Skip(milisegundoActual).First(); // 77 milisegundos en promedio.

            var clienteCompletoCargarLista4 = ctx.Clientes.Skip(milisegundoActual).First();
            ctx.CargarPropiedad(clienteCompletoCargarLista4, ct => ct.Municipio); // 146 milisegundos en promedio.

            var clienteTipoAnónimo4 = ctx.Clientes.Skip(milisegundoActual).Select(c => new { c.Nombre, id = c.ID, c.Teléfono, c.Municipio }).First(); // 86 milisegundos en promedio.  
            var clienteTipoAnónimoAsNoTracking4 = ctx.Clientes.AsNoTracking().Skip(milisegundoActual)
                .Select(c => new { c.Nombre, id = c.ID, c.Teléfono, c.Municipio }).First(); // 71 milisegundos en promedio.
            // Rendimiento 1 Entidad con 1 Hijo>

            // Conclusiones
            // 1. Si se necesita la entidad y sus hijos para ser modificados, se debe usar el método normal (Tracking) con Include.
            // 2. Si se necesita la entidad para ser modificada y a veces sus hijos, se puede cargar la entidad sin Include y usar los métodos CargarLista 
            //    y CargarPropiedad dentro de un condicional.
            // 3. Si se necesita la entidad o la entidad y sus hijos para solo lectura,  se debe usar Include + AsNoTracking.
            // 4. Si se tienen muchos hijos (más de 3 niveles) se deberá comparar el rendimiento de Include + AsNoTracking con Tipo Anónimo + AsNoTracking 
            //    y consultas por separado similar a como funciona CargarPropiedad y CargarLista pero adaptado a la estructura más compleja.	
            // 5. Agregar segundos AsNoTracking después de Include no cambia el rendimiento. Esto pasa porque AsNoTracking es una configuración 
            //    global de la consulta. Ver https://stackoverflow.com/questions/44140413/ef-include-with-asnotracking.
            // 6. Si se necesita mucho más rendimiento se puede considerar procedimientos almacenados, Dapper o ADO.Net. 
            //    Ver https://dotnetcultist.com/maximizing-entity-framework-core-query-performance/.
            // Conclusiones>

#endif

            var productos = ctx.ObtenerProductos();
            var producto1 = ctx.ObtenerProducto("+");
            var producto2 = ctx.ObtenerProducto("0..");

            // Conclusiones Producto
            // 1. Si se necesita el producto y su base para ser modificados, se debe usar el método normal (Tracking) con Include si más del 20% de los productos tienen base. Si no se debe usar el método normal sin include + un condicional para CargarPropiedad.
            // 2. Si se necesita el producto o el producto y su base para solo lectura, se debe usar Include + AsNoTracking.
            // Conclusiones Producto>

        } // RendimientoConsultasLectura>


        public static void ProductosYProductosBase() {

            var milisegundoActual = AhoraUtcAjustado.Millisecond + 3;
            using var ctx = new Contexto(TipoContexto.LecturaConRastreo); // Se inicia como contexto de LecturaConRastreo para poder realizar las pruebas con y sin AsNoTracking().
            var ventasPorSubtotal = ctx.Ventas.OrderByDescending(v => v.Subtotal).ToList(); // Una consulta cualquiera para que el contexto se inicie.

            var producto0 = ctx.Productos.Skip(0).First();
            var productoN = ctx.Productos.Skip(milisegundoActual).First();

            var producto0ConBase = ctx.Productos.Include(p => p.Base).Skip(0).First();
            var producto1ConBase = ctx.Productos.Include(p => p.Base).Skip(1).First();
            var producto2ConBase = ctx.Productos.Include(p => p.Base).Skip(2).First();
            if (producto0ConBase.BaseID == producto1ConBase.BaseID && producto0ConBase.BaseID == producto2ConBase.BaseID && producto0ConBase.Base != null
                && producto1ConBase.Base != null && producto2ConBase.Base != null) {

                producto1ConBase.Base.Descripción = "pju";
                if (!producto0ConBase.Descripción.EmpiezaPor("pju")) MostrarError("Error prueba.");

                if (producto2ConBase.DescripciónEspecífica != null) {
                    if (producto2ConBase.Descripción.EmpiezaPor("pju")) MostrarError("Error prueba.");
                }

                producto1ConBase.UnidadEspecífica = Unidad.Desconocida;
                if (producto2ConBase.UnidadEspecífica == Unidad.Desconocida) {
                    if (producto1ConBase.Unidad != producto2ConBase.Unidad) MostrarError("Error prueba.");
                }

                producto2ConBase.PorcentajeIVAPropioEspecífico = 0.05;
                producto1ConBase.Base.PorcentajeIVAPropio = 0.15;
                if (producto2ConBase.PorcentajeIVA != 0.05) MostrarError("Error prueba.");
                if (producto0ConBase.PorcentajeIVA != 0.15) MostrarError("Error prueba.");

            }

            var productoNConBase = ctx.Productos.Include(p => p.Base).Skip(milisegundoActual).First();
            if (productoNConBase.Base != null) {
                productoNConBase.Base.Descripción = "Descripción del Producto N";
            } else {
                productoNConBase.DescripciónEspecífica = "Descripción del Producto N";
            }
            if (!productoNConBase.TieneBase && productoNConBase.DescripciónEspecífica != "Descripción del Producto N") MostrarError("Error prueba.");

            #region Modificación Propiedades Base y Específicas
            var camisetaBase = new ProductoBase("CMS") { ID = 1, Descripción = "Camiseta Manga Corta", PorcentajeIVAPropio = 0.05 };
            var camisetaXXL = new Producto("CMSAXXL", camisetaBase, new List<string> { "Azul", "Talla XXL" });
            var sacóExcepción = false;
            try {
                camisetaXXL!.PesoUnidadEmpaque = 50;
            } catch (Exception) {
                sacóExcepción = true;
            }
            if (!sacóExcepción) MostrarError("No se esperaba que la línea camisetaXXL!.PesoUnidadEmpaque = 50; no sacara excepción.");
            if (camisetaXXL.PesoUnidadEmpaque != null) MostrarError("No se esperaba que camisetaXXL.PesoUnidadEmpaque no fuera nula.");
            camisetaXXL!.Base!.PesoUnidadEmpaque = 50;
            if (camisetaXXL.PesoUnidadEmpaque != 50) MostrarError("No se esperaba que camisetaXXL.PesoUnidadEmpaque != 50.");
            camisetaXXL!.PesoUnidadEmpaqueEspecífica = 55;
            if (camisetaXXL.PesoUnidadEmpaque != 55) MostrarError("No se esperaba que camisetaXXL.PesoUnidadEmpaque != 55.");
            #endregion Modificación Propiedades Base y Específicas>

        } // ProductosYProductosBase>


        /// <summary>
        /// Esta prueba se usa cuando se quiere simular el comportamiento de un programa tercero que genera archivos de comunicación .json con SimpleOps
        /// para el modo de integración de facturación electrónica.
        /// </summary>
        public static void IntegraciónAplicacionesTerceros() {

            var número = Empresa.PróximoNúmeroDocumentoElectrónicoPruebas; // Se almacena en esta variable temporal y después se guardan las opciones para evitar posibles problemas de concurrencia.
            Empresa.PróximoNúmeroDocumentoElectrónicoPruebas++;
            GuardarOpciones(Empresa);
            VentaSimple(out _, número, 10, 5500, "Empresa Ensayo", "900900900", "Calle 29 47 45", "2889896",
                false, out Venta? venta, out DocumentoElectrónico<Factura<Cliente, LíneaVenta>, LíneaVenta>? _, pruebaIntegración: true);

            if (venta != null) {

                número = Empresa.PróximoNúmeroDocumentoElectrónicoPruebas; // Se almacena en esta variable temporal y después se guardan las opciones para evitar posibles problemas de concurrencia.
                Empresa.PróximoNúmeroDocumentoElectrónicoPruebas++;
                GuardarOpciones(Empresa);
                NotaCréditoEjemploXml(out _, número, pruebaHabilitación: false, venta, out NotaCréditoVenta? _,
                    out DocumentoElectrónico<Factura<Cliente, LíneaNotaCréditoVenta>, LíneaNotaCréditoVenta>? _, pruebaIntegración: true);

            }

        } // IntegraciónAplicacionesTerceros>


        public static void DocumentosElectrónicos() {

            var decimales = new List<decimal> { 2.4M, 2.6M, 2.5M, 3.5M, 1.65M, -1.5M, -12.5M };
            var valoresEsperados = new List<decimal> { 2, 3, 2, 4, 2, -2, -12 };
            for (int i = 0; i < decimales.Count; i++) {
                if (valoresEsperados[i] != Redondear(decimales[i])) MostrarError("Falló Redondear().");
            }

            if ("2018-09-01/2018-09-30" != IntervaloATexto(new DateTime(2018, 9, 1), new DateTime(2018, 9, 30)))
                MostrarError("Falló FormatoIntervalo().");

            if (!EsVálido("ABC", "3", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("551.1", "3", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("5.1", "3", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("ABCD", "3..5", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("ABCDE", "3..5", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("A", "3..5", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("ABCDEF", "3..5", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("123456.1234", "11 p 4", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("123456.123", "11 p 4", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("12345.1234", "11 p 4", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("ABCDEFGHIJK", "11 p 4", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("123456.1234", "11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("12345678901", "11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("12345678.12", "11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("123456712", "11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("1234567890123", "11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("123.12", "1..11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("123", "(1..11) p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("123.123456", "1..11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("123.1234567", "1..11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("12345.123456", "1..11 p (0..6)", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("ABCDE,123", "5,3", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("ABCDE,1234", "5,3", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("ABCDE,1234,987", "5,3", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("ABCDE", "5,3", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido("", "0..5 p 0..3", out _, out _)) MostrarError("Falló EsVálido().");
            if (EsVálido("", "", out _, out _)) MostrarError("Falló EsVálido().");
            if (!EsVálido(null, "0..5", out _, out _)) MostrarError("Falló EsVálido().");

            if (Validar(100M, "4..15 p (2..6)", 3).ATexto() == "100.00") MostrarError("Falló Validar(decimal)");
            if (Validar(100M, "4..15 p (2..6)", 2).ATexto() != "100.00") MostrarError("Falló Validar(decimal)");

            if (Validar("Desc", "5..20", forzarCumplimiento: true) != "Desc ") MostrarError("Falló Validar(texto)");
            if (Validar("Larga muy, larga descripción que no lo debería ser", "5..20", forzarCumplimiento: true) != "Larga muy, larga des")
                MostrarError("Falló Validar(texto)");
            if (Validar("Descripción Correcta", "5..20", forzarCumplimiento: true) != "Descripción Correcta") MostrarError("Falló Validar(texto)");
            if (Validar("Desc.", "5..20", forzarCumplimiento: true) != "Desc.") MostrarError("Falló Validar(texto)");
            if (Validar(null, "5..20", forzarCumplimiento: true) != "     ") MostrarError("Falló Validar(texto)");

            if (ObtenerDígitoVerificación("890879814") != "0") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("860324218") != "1") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("19897562152") != "2") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("806324218") != "3") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("860234218") != "3") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("1001990892") != "4") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("860324128") != "5") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("660589659") != "6") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("77777774") != "7") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("890568469") != "8") MostrarError("Falló ObtenerDígitoVerificación()");
            if (ObtenerDígitoVerificación("990854369") != "9") MostrarError("Falló ObtenerDígitoVerificación()");

            // Facturación(pruebaHabilitación: false); // Las pruebas de habilitación solo se harán con un botón especial, son peligrosas porque activan el modo de facturación electrónica obligatoria para la empresa. Las pruebas normales de facturación también se harán con un botón para mayor facilidad.
            VentaGenérica1Excel();
            VentaGenérica2Excel();
            VentaServiciosExcel();
            VentaExentosYExcluídosIVAExcel();
            CompraGenérica();
            Cudes();
            GeneraciónPdf();

        } // DocumentosElectrónicos>


        public static Cotización ObtenerCotizaciónConPersonalizaciones(TipoCotización tipo) {

            var contacto = new Contacto("pedro@distribucionesabc123.com") { Nombre = "Pedro Martínez", Teléfono = "313 330 33 33" };

            var cliente = new Cliente("Distribuciones ABC 123", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Distribuidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100 68", Teléfono = "4589843", Identificación = "990986892"
            };

            var productoBase1 = new ProductoBase("654") { Descripción = "Silla Tapizada" };
            var cotización = new Cotización(cliente, contacto, tipo);

            // var personalizaciones
            cotización.Líneas = new List<LíneaCotización> {
                new LíneaCotización(cotización, new Producto("654-1", productoBase1,
                    new List<string>() {"Sin Apoyabrazos", "Altura 43 cm", "Recubierta", "Tela AAA", "Patas Plásticas"})
                    { Personalizaciones = new List<TuplaSerializable<string, List<string>>>() {
                        new TuplaSerializable<string, List<string>> ("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", new List<string>()) , } },
                    1200000),
            };

            return cotización;

        } // ObtenerCotizaciónConPersonalizaciones>


        public static Cotización ObtenerCotización(TipoCotización tipo) {

            var contacto = new Contacto("pedro@distribucionesabc123.com") { Nombre = "Pedro Martínez", Teléfono = "313 330 33 33" };

            var cliente = new Cliente("Distribuciones ABC 123", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Distribuidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100 68", Teléfono = "4589843", Identificación = "990986892"
            };

            var CM = new ProductoBase("ZZCM") { Descripción = "Camiseta de Algodón Suave" };
            var cotización = new Cotización(cliente, contacto, tipo);
            cotización.Líneas = new List<LíneaCotización> {
                new LíneaCotización(cotización, new Producto("ZZTV30EJSO") { DescripciónBase = "Televisor Pantalla Plana 30 pulgadas" }, 1200000),
                new LíneaCotización(cotización, new Producto("ZZVP50EJSO") { DescripciónBase = "Videoproyector Alto Brillo 50 pulgadas" }, 1400000),
                new LíneaCotización(cotización, new Producto("ZZMP3EJSO") { DescripciónBase = "Mueble para 3 Personas" }, 2000000),
                new LíneaCotización(cotización, new Producto("ZZM14CEJSO") { DescripciónBase = "Monitor de 14 pulgadas" }, 600000),
                new LíneaCotización(cotización, new Producto("ZZEM2EJSO") { DescripciónBase = "Escritorio de Madera de 2 m" }, 500000),
                new LíneaCotización(cotización, new Producto("ZZCMAM", CM, new List<string> { "Azul", "Talla M" })
                    { ArchivoImagenEspecífica = "ZZCMA" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMAL", CM, new List<string> { "Azul", "Talla L" })
                    { ArchivoImagenEspecífica = "ZZCMA" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMAXL", CM, new List<string> { "Azul", "Talla XL" })
                    { ArchivoImagenEspecífica = "ZZCMA" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMAXXL", CM, new List<string> { "Azul", "Talla XXL" })
                    { ArchivoImagenEspecífica = "ZZCMA" }, 64000),
                new LíneaCotización(cotización, new Producto("ZZCMVS", CM, new List<string> { "Verde", "Talla S" })
                    { ArchivoImagenEspecífica = "ZZCMV" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMVM", CM, new List<string> { "Verde", "Talla M" })
                    { ArchivoImagenEspecífica = "ZZCMV" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMVXL", CM, new List<string> { "Verde", "Talla XL" })
                    { ArchivoImagenEspecífica = "ZZCMV" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMRL", CM, new List<string> { "Roja", "Talla L" })
                    { ArchivoImagenEspecífica = "ZZCMR" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMRXL", CM, new List<string> { "Roja", "Talla XL" })
                    { ArchivoImagenEspecífica = "ZZCMR" }, 54000),
                new LíneaCotización(cotización, new Producto("ZZCMRXXL", CM, new List<string> { "Roja", "Talla XXL" })
                    { ArchivoImagenEspecífica = "ZZCMR" }, 64000),
            };

            return cotización;

        } // ObtenerCotización>


        public static Cotización ObtenerCotización(string rutaJson, TipoCotización tipo) {

            OperacionesEspecialesDatos = true; // Necesario para evitar el mensaje de error al escribir Cliente.ContactoFacturas.Email en el mapeo inverso hacia el objeto venta.
            try {

                var datosCotización = Deserializar<Integración.DatosCotización>(File.ReadAllText(rutaJson), Serialización.EnumeraciónEnTexto);
                if (datosCotización == null) throw new ArgumentNullException(nameof(datosCotización), "El objeto datosCotización está vacío.");
                var mapeador = new Mapper(ConfiguraciónMapeadorCotizaciónIntegraciónInverso);
                var cotización = mapeador.Map<Cotización>(datosCotización);
                cotización.EstablecerTipo(tipo); // Siempre se debe establecer el tipo después del mapeo para agregue los valores predeterminados de algunas propiedades si no fueron mapeadas y quedaron nulas.
                foreach (var l in cotización.Líneas) {
                    l.Cotización = cotización; // Necesario porque después de ser leídas por el Automapper no quedan automáticamente enlazadas.
                    if (l.Producto?.TieneBase == false) l.Producto.Base = null; // Necesario porque el Automaper siempre crea el objeto Base.
                }
                return cotización;

            } catch (Exception) {
                throw;
            } finally {
                OperacionesEspecialesDatos = false;
            }

        } // ObtenerCotización>


        public static bool ExisteArchivoPruebaIntegración(string nombreArchivo, out string rutaArchivoPrueba) {

            var rutaIntegración = "";
            if (!string.IsNullOrEmpty(Equipo.RutaIntegración)) rutaIntegración = ObtenerRutaCarpeta(Equipo.RutaIntegración, "", crearSiNoExiste: true);
            rutaArchivoPrueba = Path.Combine(rutaIntegración, nombreArchivo);
            return File.Exists(rutaArchivoPrueba);

        } // ExisteArchivoPruebaIntegración>


        public static void GeneraciónCatálogo(out string ruta) {

            var cotización = ObtenerCotización(TipoCotización.Catálogo);
            foreach (var línea in cotización.Líneas) {

                var producto = línea.Producto;
                if (producto == null) continue;
                if (producto.TieneBase && producto.Base != null) {
                    cotización.ReferenciasProductosPáginasExtra.Agregar(producto.Base.Referencia, permitirRepetidos: false);
                }
                cotización.ReferenciasProductosPáginasExtra.Add(producto.Referencia);

            }

            cotización.ÍndiceInversoInserciónPáginasExtra = 1; // Para que deje una página al final para la contraportada.

            CrearPdfCatálogo(cotización, out ruta);

        } // GeneraciónCatálogo>


        public static void GeneraciónCotización(out string ruta) {

            var cotización = ObtenerCotización(TipoCotización.Cotización);
            CrearPdfCotización(cotización, out ruta);

        } // GeneraciónCotización>


        public static void GeneraciónCatálogoIntegración() {

            if (ExisteArchivoPruebaIntegración("CT-Prueba.json", out string rutaJsonCatálogoPrueba)) {

                try {
                    CrearPdfCatálogo(ObtenerCotización(rutaJsonCatálogoPrueba, TipoCotización.Catálogo), out string ruta);
                    MostrarÉxito($"Catálogo creado: {ruta}.");
                } catch (Exception ex) { // Antes se estaba suprimiendo la alerta CA1031. No capture tipos de excepción generales. Se acepta porque el error se informa al usuario.
                    MostrarError(@$"Ocurrió un error creando el catálogo desde el archivo de prueba '{Equipo.RutaIntegración}\CT-Prueba.json'." +
                                 @$"{DobleLínea}{ex.Message}");
                }

            } else { // Si no existe el archivo de prueba, se generará un catálogo con una prueba genérica.

                MostrarInformación(@$"Para crear un catálogo a partir de un archivo JSON de prueba, créalo en '{Equipo.RutaIntegración}\CT-Prueba.json'." +
                                   @$"{DobleLínea}Se generará un catálogo con datos de prueba.");
                GeneraciónCatálogo(out string ruta);
                MostrarÉxito($"Catálogo creado: {ruta}.");

            }

        } // GeneraciónCatálogoIntegración>


        public static void LeerYGuardarPersonalizacionesProductos() {

            using var ctx = new Contexto(TipoContexto.Escritura);
            var productoMás = ctx.ObtenerProducto("+");
            if (productoMás != null && productoMás.Personalizaciones.Count == 0) {
                productoMás.Personalizaciones.Add(new TuplaSerializable<string, List<string>>("", new List<string> { "Amarillo", "Rojo", "Verde" }));
                productoMás.Personalizaciones.Add(new TuplaSerializable<string, List<string>>("", new List<string> { "Marco Dorado", "Marco Plateado" }));
                productoMás.Personalizaciones.Add(new TuplaSerializable<string, List<string>>("Nombre", new List<string> { "Pedro", "José" }));
                productoMás.Personalizaciones.Add(new TuplaSerializable<string, List<string>>("Lema", new List<string>()));
            }

            var primeraLíneaOC = ctx.LíneasÓrdenesCompra.FirstOrDefault();
            if (primeraLíneaOC != null && primeraLíneaOC.Personalizaciones.Count == 0) {
                primeraLíneaOC.Personalizaciones.Add("Nombre", "David");
                primeraLíneaOC.Personalizaciones.Add("Color", "Amarillo");
                primeraLíneaOC.Personalizaciones.Add("Color Marco", "Marco Dorado");
            }

            ctx.GuardarCambios();

        } // GuardarPersonalizacionesProductos>


        public static void GenerarCotizaciónIntegración() {

            if (ExisteArchivoPruebaIntegración("CZ-Prueba.json", out string rutaJsonCotizaciónPrueba)) {

                try {
                    CrearPdfCotización(ObtenerCotización(rutaJsonCotizaciónPrueba, TipoCotización.Cotización), out string ruta);
                    MostrarÉxito($"Cotización creada: {ruta}.");
                } catch (Exception ex) { // Antes se estaba suprimiendo la alerta CA1031. No capture tipos de excepción generales. Se acepta porque el error se informa al usuario.
                    MostrarError(@$"Ocurrió un error creando la cotización desde el archivo de prueba '{Equipo.RutaIntegración}\CZ-Prueba.json'." +
                                 @$"{DobleLínea}{ex.Message}");
                }

            } else { // Si no existe el archivo de prueba, se generará una cotización con una prueba genérica.

                MostrarInformación(@$"Para crear una cotización a partir de un archivo JSON de prueba, créalo en '{Equipo.RutaIntegración}\CZ-Prueba.json'." +
                                   @$"{DobleLínea}Se generará una cotización con datos de prueba.");
                GeneraciónCotización(out string ruta);
                MostrarÉxito($"Cotización creada: {ruta}.");

            }

        } // GenerarCotizaciónIntegración>


        public static void GeneraciónFichasInformativasIntegración() {



        } // GeneraciónFichasInformativasIntegración>


        public static void GeneraciónPdf() {

            // Prueba del Motor de Razor para la Generación de HTML
            var motorRazor = new RazorEngine();
            var cuerpo = @"@{ ClaveMarco = ""Marco""; } <C <h1>@Model.CódigoDocumento</h1> @Incluir(""Lista"", Model) C>";
            var partes = new Dictionary<string, string> {
                {"Marco", @"<M Encab. @CrearCuerpo() CUFE: @Model.Cude PieP. @Incluir(""Firma"", Model) M>"},
                {"Lista", @"@{ ClaveMarco = ""MarcoLista""; } <L  1. 2. 3. 4 L>"},
                {"MarcoLista", "<ML Cliente: <h2>@Model.ClienteNombre</h2> @CrearCuerpo() ML>"},
                {"Firma", @"<F Atentamente, Vixark F>"},
            };
            var plantillaCompilada = CompilarPlantilla<DatosVenta>(motorRazor, cuerpo, partes);
            var html = plantillaCompilada.ObtenerHtml(new DatosVenta {
                Cude = "AFJ451MN", CódigoDocumento = "123", Cliente = new DatosCliente() { Nombre = "Ópticas" }, Observación = ""
            });
            if (html != "<M Encab.  <C <h1>123</h1> <ML Cliente: <h2>Ópticas</h2>  <L  1. 2. 3. 4 L> ML> C> CUFE: AFJ451MN PieP. <F Atentamente, Vixark F> M>")
                MostrarError("Falló la generación del HTML para la generación del PDF.");
            // Prueba del Motor de Razor para la Generación de HTML>

        } // GeneraciónPdf>


        /// <summary>
        /// Se envían los documentos como pruebas de habilitación a la DIAN y automáticamente se activa la facturación electrónica para la empresa y 
        /// esta ya no podrá facturar por el método tradicional. Solo se debe hacer cuando la empresa esté completamente segura que procederá a la migración
        /// y seguirá facturando electrónicamente.
        /// </summary>
        public static void Habilitación() {

            if (MostrarDiálogo($"¿Deseas realizar las pruebas para habilitar a tu empresa como facturador electrónico ante la DIAN?{DobleLínea}" +
                               "¡Cuidado! Si las pruebas resultan exitosas tu empresa estará obligada a seguir facturando electrónicamente y no " +
                               "podrá seguir facturando de la manera tradicional física. Hazlo solo cuando tengas todo listo para operar facturando " +
                               "electrónicamente. Si actualmente facturas con la solución gratuita de la DIAN o con un operador tecnológico, aún podrás " +
                               "seguir facturando de esa forma.", "¿Hacer Pruebas de Habilitación?", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                if (Facturación(pruebaHabilitación: true)) {
                    MostrarInformación("Las pruebas para la habilitación de tu empresa como facturador electrónico han sido exitosas.");
                } else {
                    MostrarError("Sucedió un error en una de las pruebas. Aún así es posible que se hayan completado las requeridas por la DIAN. " +
                                 "Verifica el estado de las pruebas en el portal de habilitación de la DIAN.");
                }

            }

        } // Habilitación>


        public static bool Facturación(bool pruebaHabilitación) {

            if (Empresa.AmbienteFacturaciónElectrónica == AmbienteFacturaciónElectrónica.Producción) {
                MostrarError("No se permite la generación de facturas de prueba en modo de producción de facturación electrónica porque todas las " +
                             "facturas realizadas en este modo son legalmente válidas ante la DIAN.");
                return false;
            }

            if (!pruebaHabilitación)
                MostrarInformación("Se enviarán varios documentos electrónicos a la DIAN. Con esto se verificará que la conexión y configuración esté " +
                                   "correcta. Estos documentos se pueden consultar en el portal de habilitación de la DIAN, pero no suman a los necesarios " +
                                   "para la habilitación de la facturación electrónica. Para habilitar a tu empresa como facturador " +
                                   "electrónico usa el botón de habilitación.");

            if (!EnviarSolicitud("<wcf:GetStatus><wcf:trackId>123456666</wcf:trackId></wcf:GetStatus>", Operación.GetStatus, out string? mensajeEnvío, out _)) {
                MostrarError($"Error en solicitud GetStatus a la DIAN.{DobleLínea}{mensajeEnvío}");
                return false;
            }

            if (VentaEjemploXml(out string? mensaje, Empresa.PróximoNúmeroDocumentoElectrónicoPruebas, pruebaHabilitación: pruebaHabilitación,
                out Venta? venta, out DocumentoElectrónico<Factura<Cliente, LíneaVenta>, LíneaVenta>? ventaElectrónica)) {

                Empresa.PróximoNúmeroDocumentoElectrónicoPruebas++;
                GuardarOpciones(Empresa);
                MostrarÉxito("¡Éxito del envío de la factura electrónica completa a la DIAN!"); // Éxito. Se puede continuar con los procedimientos posteriores como grabar en la base de datos, hacer cambios en la interfaz y demás. La factura se considera realizada así puedan fallar los siguientes procedimientos de representación gráfica y email al cliente.

                if (venta != null && CrearPdfVenta(venta, ventaElectrónica, out _)) {
                    // Si se creó la representación gráfica exitosamente, se puede enviar el email al cliente.
                } else {
                    MostrarError("No se pudo crear la representación gráfica de la factura electrónica.");
                }

            } else {
                MostrarError($"Error en factura electrónica.{DobleLínea}{mensaje}"); // Error. Se aborta la operación, no se debe grabar en la base de datos y se debe mantener la interfaz inalterada para que se realicen las correcciones necesarias.
                return false;
            }

            if (venta != null) {

                if (NotaCréditoEjemploXml(out string? mensajeNotaCrédito, Empresa.PróximoNúmeroDocumentoElectrónicoPruebas, pruebaHabilitación, venta,
                    out NotaCréditoVenta? notaCrédito,
                    out DocumentoElectrónico<Factura<Cliente, LíneaNotaCréditoVenta>, LíneaNotaCréditoVenta>? notaCréditoElectrónica)) {

                    Empresa.PróximoNúmeroDocumentoElectrónicoPruebas++;
                    GuardarOpciones(Empresa);
                    MostrarÉxito("¡Éxito del envío de la nota crédito electrónica a la DIAN!");
                    if (notaCrédito != null && CrearPdfVenta(notaCrédito, notaCréditoElectrónica, out _)) {
                        // Si se creó la representación gráfica exitosamente, se puede enviar el email al cliente.
                    } else {
                        MostrarError("No se pudo crear la representación gráfica de la nota crédito electrónica.");
                    }

                } else {
                    MostrarError($"Error en nota crédito electrónica.{DobleLínea}{mensajeNotaCrédito}");
                    return false;
                }

            } else {
                MostrarError("No se puede hacer una nota crédito sin una venta asociada.");
                return false;
            }

            if (venta != null) {

                if (NotaDébitoEjemploXml(out string? mensajeNotaDébito, Empresa.PróximoNúmeroDocumentoElectrónicoPruebas, pruebaHabilitación, venta)) {

                    Empresa.PróximoNúmeroDocumentoElectrónicoPruebas++;
                    GuardarOpciones(Empresa);
                    MostrarÉxito("¡Éxito del envío de la nota débito electrónica a la DIAN!");

                } else {
                    MostrarError($"Error en nota débito electrónica.{DobleLínea}{mensajeNotaDébito}");
                    return false;
                }

            } else {
                MostrarError("No se puede hacer una nota débito sin una venta asociada.");
                return false;
            }

            for (int i = 1; i <= 10; i++) {

                if (VentaSimple(out string? mensajeSimple, Empresa.PróximoNúmeroDocumentoElectrónicoPruebas, i, i * 1000, $"Empresa {i}", $"90090090{i}",
                    $"Calle {i} # {i} - {i}", $"{i}{i}{i}{i}{i}{i}{i}", pruebaHabilitación, out Venta? ventaSimple,
                    out DocumentoElectrónico<Factura<Cliente, LíneaVenta>, LíneaVenta>? ventaSimpleElectrónica, unoExcluídoIVA: i == 1,
                    todosExcluídosIVA: i == 2, unoConINC: i == 3, unoConINCyExcluídoIVA: i == 4, unoExentoIVA: i == 5, clienteIVACero: i == 6,
                    todosSoloINC: i == 10)) {

                    Empresa.PróximoNúmeroDocumentoElectrónicoPruebas++;
                    GuardarOpciones(Empresa);
                    MostrarÉxito($"¡Éxito del envío de la factura electrónica simple #{i} a la DIAN!");

                    if (ventaSimple != null && CrearPdfVenta(ventaSimple, ventaSimpleElectrónica, out _)) {
                        // Si se creó la representación gráfica exitosamente, se puede enviar el email al cliente.
                    } else {
                        MostrarError("No se pudo crear la representación gráfica de la factura electrónica.");
                    }

                } else {
                    MostrarError($"Error en factura electrónica.{DobleLínea}{mensajeSimple}");
                    return false;
                }

            }

            return true;

        } // Facturación>


        public static void Cudes() {

            var nit = Empresa.Nit; Empresa.Nit = "700085371";
            var claveTécnicaAplicación = Empresa.ClaveTécnicaAplicación;
            Empresa.ClaveTécnicaAplicación = "693ff6f2a553c3646a063436fd4dd9ded0311471";
            var ambiente = Empresa.AmbienteFacturaciónElectrónica; Empresa.AmbienteFacturaciónElectrónica = AmbienteFacturaciónElectrónica.Producción;
            var pinAplicación = Empresa.PinAplicación; Empresa.PinAplicación = "12345";

            var cliente = new Cliente("cliente", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100", Teléfono = "4589843", Identificación = "990986892"
            };

            // Prueba CUFE
            cliente.Identificación = "800199436";
            var venta = new Venta(cliente) {
                FechaHora = new DateTime(2019, 01, 16, 10, 53, 10), SubtotalBase = 1500000, IVA = 285000, ImpuestoConsumo = 0, Número = 129,
                Prefijo = "323200000", SubtotalFinalConImpuestos = 1785000,
            };
            venta.Líneas.Add(new LíneaVenta(new Producto("Ref"), venta, 1, 1500000, 1500000));
            venta.CalcularTodo();

            if (venta.Cude != "8bb918b19ba22a694f1da11c643b5e9de39adf60311cf179179e9b33381030bcd4c3c3f156c506ed5908f9276f5bd9b4")
                MostrarError("Falló ObtenerCufe()");
            // Prueba CUFE>

            // Prueba CUDE 1
            Empresa.Nit = "900373076";
            cliente.Identificación = "8355990";
            Empresa.AmbienteFacturaciónElectrónica = AmbienteFacturaciónElectrónica.Pruebas;
            var venta2 = new Venta(cliente) {
                FechaHora = new DateTime(2019, 02, 20, 16, 46, 55), SubtotalBase = 235.28M, IVA = 19M, ImpuestoConsumo = 0, Número = 7871,
                Prefijo = "811000", SubtotalFinalConImpuestos = 262.56M
            };
            venta2.Líneas.Add(new LíneaVenta(new Producto("Ref"), venta2, 1, 235.28M, 235.28M));
            venta2.CalcularTodo();

            if (venta2.Cude != "85f5cc105dd09b338086491b16007b9d0a90bf07a7fee7f61a9044be7a837833591a718060f319ce2fe0fb98bc2a45f5")
                MostrarError("Falló ObtenerCude()");
            // Prueba CUDE 1>

            // Prueba CUDE 2
            Empresa.PinAplicación = "12301";
            Empresa.AmbienteFacturaciónElectrónica = AmbienteFacturaciónElectrónica.Producción;
            var notaCrédito = new NotaCréditoVenta(cliente, venta) {
                FechaHora = new DateTime(2019, 01, 12, 7, 0, 0), SubtotalBase = 5000, IVA = 950, ImpuestoConsumo = 0, Número = 7871,
                Prefijo = "811000", SubtotalFinalConImpuestos = 5950
            };
            notaCrédito.Líneas.Add(new LíneaNotaCréditoVenta(new Producto("Ref"), notaCrédito, 1, 5000, 5000));
            notaCrédito.CalcularTodo();

            if (notaCrédito.Cude != "907e4444decc9e59c160a2fb3b6659b33dc5b632a5008922b9a62f83f757b1c448e47f5867f2b50dbdb96f48c7681168")
                MostrarError("Falló ObtenerCude()");
            // Prueba CUDE 2>

            // Prueba CUDE 3
            Empresa.Nit = "900197264";
            cliente.Identificación = "10254102";
            Empresa.PinAplicación = "10201";
            Empresa.AmbienteFacturaciónElectrónica = AmbienteFacturaciónElectrónica.Pruebas;
            var notaDébito = new NotaDébitoVenta(cliente, venta) {
                FechaHora = new DateTime(2019, 01, 18, 10, 58, 0), SubtotalBase = 30000, IVA = 0, ImpuestoConsumo = 2400, Número = 1001,
                Prefijo = "ND", SubtotalFinalConImpuestos = 32400
            };
            notaDébito.Líneas.Add(new LíneaNotaDébitoVenta(new Producto("Ref"), notaDébito, 1, 30000, 30000));
            notaDébito.CalcularTodo();

            if (notaDébito.Cude != "540f266793e6a299a4ee68f9fd558d9754aa9e443bac6a4e2e2b45969be43d2ef5101aa23ae6b1376331f903c695b6b0") // Aunque el ejemplo de la DIAN dice que debería dar b9483dc2a17167feedf37b6bd67c4204e7b601933e0e389cffbd545e4d0ec370b403cbb41ff656776cb6cb5d8348ecd4, se revisó la función cuidadosamente y teniendo en cuenta que los anteriores dieron bien, se supone que es un error en la documentación de la DIAN.
                MostrarError("Falló ObtenerCude()");
            // Prueba CUDE 3>

            Empresa.PinAplicación = pinAplicación;
            Empresa.ClaveTécnicaAplicación = claveTécnicaAplicación;
            Empresa.Nit = nit;
            Empresa.AmbienteFacturaciónElectrónica = ambiente;

        } // Cudes>


        public static void CompraGenérica() {

            var proveedor = new Proveedor("proveedor", new Municipio("Bogótá", "Bogotá", "Bogotá, D.C.")) {
                TipoContribuyente = TipoContribuyente.Autorretenedor, TipoEntidad = TipoEntidad.Empresa, Identificación = "9898989"
            };
            var compra = new Compra(proveedor);
            compra.Líneas = new List<LíneaCompra> {
                { new LíneaCompra(new Producto("Mouse"), compra, 2, 20000, 0) },
                { new LíneaCompra(new Producto("Monitor"), compra, 1, 300000, 0) }
            };

            if (compra.CalcularTodo()) {

                var mostrarError = !(Iguales(compra.Subtotal, 340000) && Iguales(compra.SubtotalBase, 340000) && Iguales(compra.SubtotalBaseReal, 340000) &&
                    Iguales(compra.SubtotalBaseIVA, 340000) && Iguales(compra.IVA, 64600) && Iguales(compra.ImpuestoConsumo, 0) &&
                    Iguales(compra.RetenciónFuente, 0) && Iguales(compra.RetenciónIVA, 0) && Iguales(compra.RetenciónICA, 0)
                    && Iguales(compra.SubtotalFinalConImpuestos, 404600) && Iguales(compra.APagar, 404600) && Iguales(compra.SubtotalFinal, 340000));
                if (mostrarError) MostrarError("Error en Validación Cálculo FacturaGenéricaEjemploExcel.CalcularTodo().");

            } else {
                MostrarError("Error en FacturaGenéricaEjemploExcel.CalcularTodo().");
            }

        } // CompraGenérica>


        public static void VentaExentosYExcluídosIVAExcel() {

            var cliente = new Cliente("cliente", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100", Teléfono = "4589843", Identificación = "990986892"
            };
            var venta = new Venta(cliente) { DescuentoComercial = 0, DescuentoCondicionado = 40000 };
            venta.Líneas = new List<LíneaVenta> {
                { new LíneaVenta(new Producto("Bien Excluído") { ExcluídoIVA = true }, venta, 1, 300000, 0) },
                { new LíneaVenta(new Producto("Bien Exento") { PorcentajeIVAPropio = 0 }, venta, 1, 100000, 0) },
            };

            if (venta.CalcularTodo()) {

                var mostrarError = !(Iguales(venta.Subtotal, 400000) && Iguales(venta.SubtotalBase, 400000) && Iguales(venta.SubtotalBaseReal, 400000) &&
                    Iguales(venta.SubtotalBaseIVA, 100000) && Iguales(venta.IVA, 0) && Iguales(venta.ImpuestoConsumo, 0) &&
                    Iguales(venta.RetenciónFuente, 0) && Iguales(venta.RetenciónIVA, 0) && Iguales(venta.RetenciónICA, 0)
                    && Iguales(venta.SubtotalFinalConImpuestos, 360000) && Iguales(venta.APagar, 360000) && Iguales(venta.SubtotalFinal, 360000));
                if (mostrarError) MostrarError("Error en Validación Cálculo FacturaGenéricaEjemploExcel.CalcularTodo().");

            } else {
                MostrarError("Error en FacturaGenéricaEjemploExcel.CalcularTodo().");
            }

        } // VentaExentosYExcluídosIVAEjemploExcel>


        public static void VentaServiciosExcel() {

            var cliente = new Cliente("cliente", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100", Teléfono = "4589843", Identificación = "990986892"
            };
            var venta = new Venta(cliente) { DescuentoComercial = 0, DescuentoCondicionado = 40000 };
            venta.Líneas = new List<LíneaVenta> {
                { new LíneaVenta(new Producto("Administración") { ExcluídoIVA = true }, venta, 1, 600000, 0) },
                { new LíneaVenta(new Producto("Imprevisto") { ExcluídoIVA = true }, venta, 1, 60000, 0) },
                { new LíneaVenta(new Producto("Utilidad"), venta, 1, 60000, 0) },
            };

            if (venta.CalcularTodo()) {

                var mostrarError = !(Iguales(venta.Subtotal, 720000) && Iguales(venta.SubtotalBase, 720000) && Iguales(venta.SubtotalBaseReal, 720000) &&
                    Iguales(venta.SubtotalBaseIVA, 60000) && Iguales(venta.IVA, 11400) && Iguales(venta.ImpuestoConsumo, 0) &&
                    Iguales(venta.RetenciónFuente, 0) && Iguales(venta.RetenciónIVA, 0) && Iguales(venta.RetenciónICA, 0)
                    && Iguales(venta.SubtotalFinalConImpuestos, 691400) && Iguales(venta.APagar, 691400) && Iguales(venta.SubtotalFinal, 680000));
                if (mostrarError) MostrarError("Error en Validación Cálculo FacturaGenéricaEjemploExcel.CalcularTodo().");

            } else {
                MostrarError("Error en FacturaGenéricaEjemploExcel.CalcularTodo().");
            }

        } // VentaServiciosEjemploExcel>


        public static void VentaGenérica2Excel() {

            var tipoContribuyente = Empresa.TipoContribuyente;
            Empresa.TipoContribuyente = TipoContribuyente.Ordinario | TipoContribuyente.ResponsableIVA;

            var cliente = new Cliente("cliente", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100", Teléfono = "4589843", Identificación = "990986892",
                TipoContribuyente = TipoContribuyente.RetenedorIVA
            };

            var venta = new Venta(cliente) { DescuentoComercial = 0, DescuentoCondicionado = 469000 };
            venta.Líneas = new List<LíneaVenta> {
                { new LíneaVenta(new Producto("Base para TV"), venta, 2, 300000, 0) },
                { new LíneaVenta(new Producto("Antena (regalo)"), venta, 10, 10000, 0) { MuestraGratis = true } },
                { new LíneaVenta(new Producto("TV"), venta, 10, 1400000, 0) },
                { new LíneaVenta(new Producto("Papa") { ExcluídoIVA = true,
                    ConceptoRetenciónPropio = ConceptoRetención.AgrícolasOPecuariosSinProcesamiento }, venta, 100, 20000, 0) },
                { new LíneaVenta(new Producto("Carne") { ExcluídoIVA = true,
                    ConceptoRetenciónPropio = ConceptoRetención.AgrícolasOPecuariosSinProcesamiento }, venta, 1000, 7000, 0) },
                { new LíneaVenta(new Producto("Acarreo") { ConceptoRetenciónPropio = ConceptoRetención.TransporteCarga }, venta, 1, 40000, 0) },
                { new LíneaVenta(new Producto("Instalación") { ConceptoRetenciónPropio = ConceptoRetención.ServiciosGenerales }, venta, 1, 1300000, 0) },
                { new LíneaVenta(new Producto("Bolsas") { ExcluídoIVA = true, TipoImpuestoConsumoPropio = TipoImpuestoConsumo.BolsasPlásticas },
                    venta, 100, 100, 0) { MuestraGratis = true } }
            };

            if (venta.CalcularTodo()) {

                var mostrarError = !(Iguales(venta.Subtotal, 24940000) && Iguales(venta.SubtotalBase, 24940000) && Iguales(venta.SubtotalBaseReal, 25050000) &&
                    Iguales(venta.SubtotalBaseIVA, 16040000) && Iguales(venta.IVA, 3047600) && Iguales(venta.ImpuestoConsumo, 5000) &&
                    Iguales(venta.RetenciónFuente, 552000) && Iguales(venta.RetenciónIVA, 457140) && Iguales(venta.RetenciónICA, 0)
                    && Iguales(venta.SubtotalFinalConImpuestos, 27523600) && Iguales(venta.APagar, 26514460) && Iguales(venta.SubtotalFinal, 24471000));
                if (mostrarError) MostrarError("Error en Validación Cálculo FacturaGenéricaEjemploExcel.CalcularTodo().");

            } else {
                MostrarError("Error en FacturaGenéricaEjemploExcel.CalcularTodo().");
            }

            Empresa.TipoContribuyente = tipoContribuyente;

        } // FacturaGenérica2EjemploExcel>


        public static void VentaGenérica1Excel() {

            var tipoEntidadEmpresa = Empresa.TipoEntidad;
            var tipoContribuyente = Empresa.TipoContribuyente;
            Empresa.TipoEntidad = TipoEntidad.Empresa; // Para simular retenciones en la fuente como declarante si están activadas como no declarante en opciones.
            Empresa.TipoContribuyente = TipoContribuyente.Ordinario | TipoContribuyente.ResponsableIVA;

            var cliente = new Cliente("cliente", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C."), TipoCliente.Consumidor) {
                TipoEntidad = TipoEntidad.Empresa, Dirección = "Calle 80-100", Teléfono = "4589843", Identificación = "990986892",
                TipoContribuyente = TipoContribuyente.RégimenSimple | TipoContribuyente.GranContribuyente, PorcentajeRetenciónICAPropio = 0.01  // Régimen simple es solo para que el TipoContribuyente contenga dos elementos.       
            };
            var venta = new Venta(cliente) { DescuentoComercial = 70800, DescuentoCondicionado = 59000 };
            var fracción = 0.8M;

            venta.Líneas = new List<LíneaVenta> {
                { new LíneaVenta(new Producto("Base para TV"), venta, 3, 300000, 300000M * fracción) },
                { new LíneaVenta(new Producto("Antena (regalo)"), venta, 1, 100000, 100000M * fracción) { MuestraGratis = true } },
                { new LíneaVenta(new Producto("TV"), venta, 1, 1400000, 1400000 * fracción) },
                { new LíneaVenta(
                    new Producto("Servicio Salud (exluido)") { ConceptoRetenciónPropio = ConceptoRetención.SaludPorIPS, ExcluídoIVA = true },
                    venta, 1, 20000, 20000 * fracción) },
                { new LíneaVenta(new Producto("acarreo"), venta, 1, 40000, 40000 * fracción) },
                { new LíneaVenta(
                    new Producto("bolsas") { TipoImpuestoConsumoPropio = TipoImpuestoConsumo.BolsasPlásticas, ExcluídoIVA = true },
                    venta, 2, 100, 100 * fracción) { MuestraGratis = true }
                },
                { new LíneaVenta(
                    new Producto("almuerzo") { ExcluídoIVA = true, TipoImpuestoConsumoPropio = TipoImpuestoConsumo.ServiciosRestaurante,
                        ConceptoRetenciónPropio = ConceptoRetención.HotelesYRestaurantes},
                    venta, 5, 35000, 35000 * fracción)
                },
            };

            if (venta.CalcularTodo()) {

                var mostrarError = !(Iguales(venta.Subtotal, 2535000) && Iguales(venta.SubtotalBase, 2464200) && Iguales(venta.SubtotalBaseReal, 2564400) &&
                    Iguales(venta.SubtotalBaseIVA, 2374646) && Iguales(venta.IVA, 451183) && Iguales(venta.ImpuestoConsumo, 13709) &&
                    Iguales(venta.RetenciónFuente, 62820) && Iguales(venta.RetenciónIVA, 67677) && Iguales(venta.RetenciónICA, 24642)
                    && Iguales(venta.SubtotalFinalConImpuestos, 2870092) && Iguales(venta.APagar, 2714952) && Iguales(venta.Margen, 297040) &&
                    Iguales(venta.SubtotalFinal, 2405200) && Iguales(venta.PorcentajeMargen, 0.1235M, 0.01M)
                    && Iguales(venta.PorcentajeGanancia, 0.1409M, 0.01M));
                if (mostrarError) MostrarError("Error en Validación Cálculo FacturaGenéricaEjemploExcel.CalcularTodo().");

            } else {
                MostrarError("Error en FacturaGenéricaEjemploExcel.CalcularTodo().");
            }

            Empresa.TipoEntidad = tipoEntidadEmpresa;
            Empresa.TipoContribuyente = tipoContribuyente;

        } // FacturaGenérica1EjemploExcel>


        public static bool VentaEjemploXml(out string? mensaje, int númeroFactura, bool pruebaHabilitación, out Venta? venta,
            out DocumentoElectrónico<Factura<Cliente, LíneaVenta>, LíneaVenta>? ventaElectrónica) {

            venta = null;
            ventaElectrónica = null;

            if (Empresa.PrimerNúmeroFacturaAutorizada == null) return Falso(out mensaje, "No se esperaba Empresa.PrimerNúmeroFacturaAutorizada nulo.");

            var fracciónCosto = 0.8M;
            var cliente = new Cliente("OPTICAS GMO COLOMBIA S A S", new Municipio("Bogotá", "Bogotá", "Bogotá, D.C.") { ID = 1, Código = "11001" },
                TipoCliente.Consumidor) {
                Identificación = "900108281", TipoEntidad = TipoEntidad.Empresa, Dirección = "CR 9 A N0 99 - 07 OF 802",
                DíasCrédito = 92, Teléfono = "5555555"
            };

            cliente.ContactoFacturas = new Contacto("dcruz@empresa.org") { Nombre = "Diana Cruz", Teléfono = "31031031089" };
            cliente.Sedes.Add(new Sede("Bodega", cliente, "CARRERA 8 No 20-14/40", cliente.Municipio!));
            var sede = cliente.Sedes.First();
            var informePago = new InformePago(1000, new DateTime(2018, 09, 29), cliente);
            var ordenCompra = new OrdenCompra(cliente, "AFR6591") { FechaHoraCreación = new DateTime(2019, 06, 10), Sede = sede }; // Fecha de emisión: Fecha de emisión de la orden. Número: Prefijo y número del documento orden de compra referenciado. Los campos orden de compra son para información y describen una orden de pedido para esta factura. Referencias no tributarias pero si de interés mercantil. Se utiliza cuando se requiera referenciar una sola orden de pedido a la factura realizada.
            venta = new Venta(cliente, ordenCompra) {
                FechaHora = AhoraUtcAjustado, Prefijo = Empresa.PrefijoFacturas, Número = (int)Empresa.PrimerNúmeroFacturaAutorizada + númeroFactura,
                InformePago = informePago, DescuentoCondicionado = 2000, DescuentoComercial = 5000, ConsecutivoDianAnual = 50 + númeroFactura, // Número de documento: Número de factura o factura cambiaria. Incluye prefijo + número consecutivo de factura autorizados por la DIAN. No se permiten caracteres adicionales como espacios o guiones. El número consecutivo de factura debe ser igual o superior al valor inicial del rango de numeración otorgado.
                Observación = "Una nota informativa."
            };

            var camisetaBase = new ProductoBase("CMS") { ID = 1, Descripción = "Camiseta Manga Corta", PorcentajeIVAPropio = 0.05 };

            venta.Líneas = new List<LíneaVenta> {
                new LíneaVenta(new Producto("AOHV84-225") { DescripciónBase = "Lente de Contacto HV8400 (Indicado para personas zurdas)",
                    Unidad = Unidad.Par }, venta, cantidad: 5, precio: 12600.06M, costo: 12600 * fracciónCosto),
                new LíneaVenta(new Producto("BOIVA16") { PorcentajeIVAPropio = 0.16,  DescripciónBase = "16% IVA de Bolsa" }, venta,
                    cantidad: 1, precio: 187.50M, costo: 187.50M * fracciónCosto ) { MuestraGratis = true },
                new LíneaVenta(new Producto("PAPA") { DescripciónBase = "Papa Campesina Calidad AAA", PorcentajeIVAPropio = 0.05 }, venta,
                    cantidad: 20, precio: 1500, costo: 1500 * fracciónCosto),
                new LíneaVenta(new Producto("YUCA") { DescripciónBase = "Yuca Alta Calidad", PorcentajeIVAPropio = 0.05 }, venta,
                    cantidad: 10, precio: 2000, costo: 2000 * fracciónCosto),
                //new LíneaVenta(new Producto("datos celular") { TipoImpuestoConsumoPropio = TipoImpuestoConsumo.TelefoníaCelularYDatos,
                //    Descripción = "D. datos" }, venta, cantidad: 2, precio: 60000, costo: 60000 * fracciónCosto), // Desactivado porque saca un error atípico que no tienen otros impuestos al consumo. Posiblemente es un error del servidor de la DIAN.
                new LíneaVenta(new Producto("ALM") { TipoImpuestoConsumoPropio = TipoImpuestoConsumo.ServiciosRestaurante, ExcluídoIVA = true,
                    DescripciónBase = "Almuerzo Completo con Carne y Ensalada con Yuca Papa" }, venta, cantidad: 3, precio: 25000,
                    costo: 25000 * fracciónCosto),
                new LíneaVenta(new Producto("CENA") { TipoImpuestoConsumoPropio = TipoImpuestoConsumo.ServiciosRestaurante, ExcluídoIVA = true,
                    DescripciónBase = "Cena Completa con Jugo y Postre" }, venta, cantidad: 3, precio: 15000, costo: 15000 * fracciónCosto),
                //new LíneaVenta(new Producto("Bolsa IC Bolsas") { TipoImpuestoConsumoPropio = TipoImpuestoConsumo.BolsasPlásticas,
                //    Descripción = "Bolsa Empaque", ExcluídoIVA = true }, venta, cantidad: 3, precio: 150, costo: 50 * fracciónCosto )
                //    { MuestraGratis = true }, // Desactivado porque saca un error relacionado con el CUFE que solo aparece cuando se establece TipoImpuestoConsumo.BolsasPlásticas: Rechazo FAD06: Valor del CUFE no está calculado correctamente.
                new LíneaVenta(new Producto("CMSRS", camisetaBase, new List<string> { "Roja", "Talla S" }),
                    venta, 2, 50990, 40000),
                new LíneaVenta(new Producto("CMSVL", camisetaBase, new List<string> { "Verde", "Talla L" }) {
                    UnidadEspecífica = Unidad.Docena }, venta, 3, 51990, 42000),
                new LíneaVenta(new Producto("CMSAXXL", camisetaBase, new List<string> { "Azul", "Talla XXL" }) {
                    PorcentajeIVAPropioEspecífico = 0.16 }, venta, 1, 51990, 42000),
            };

            venta.Remisiones = new List<Remisión> { new Remisión(1000, cliente) { ID = 123456, FechaHoraCreación = new DateTime(2019, 06, 19) } };

            return CrearYEnviarDocumentoElectrónico(venta, out mensaje, out ventaElectrónica, pruebaHabilitación);

        } // VentaEjemploXml>


        public static bool VentaSimple(out string? mensaje, int númeroFactura, int cantidad, decimal precio, string empresa, string nit, string dirección,
            string teléfono, bool pruebaHabilitación, out Venta? venta, out DocumentoElectrónico<Factura<Cliente, LíneaVenta>, LíneaVenta>? ventaElectrónica,
            bool pruebaIntegración = false, bool unoExcluídoIVA = false, bool todosExcluídosIVA = false, bool unoConINC = false,
            bool unoConINCyExcluídoIVA = false, bool unoExentoIVA = false, bool clienteIVACero = false, bool todosSoloINC = false) {

            ventaElectrónica = null;
            venta = null;
            if (Empresa.PrimerNúmeroFacturaAutorizada == null) return Falso(out mensaje, "No se esperaba Empresa.PrimerNúmeroFacturaAutorizada nulo.");

            var cliente = new Cliente(empresa, new Municipio("Bogotá", "Bogotá", "Bogotá, D.C.") { ID = 1, Código = "11001" }, TipoCliente.Consumidor) {
                Identificación = nit, TipoEntidad = TipoEntidad.Empresa, Dirección = dirección, DíasCrédito = 7, Teléfono = teléfono,
                ContactoFacturas = new Contacto($"facturación@{empresa.Reemplazar(" ", "")}.com"), PorcentajeIVAPropio = clienteIVACero ? 0 : (double?)null
            };

            venta = new Venta(cliente, new OrdenCompra(cliente, AhoraUtcAjustado.ATextoYYMMDD())) {
                FechaHora = AhoraUtcAjustado, Prefijo = Empresa.PrefijoFacturas, Número = (int)Empresa.PrimerNúmeroFacturaAutorizada + númeroFactura,
                ConsecutivoDianAnual = 50 + númeroFactura, Observación = "Alguna observación sobre la venta"
            };

            var camisetaBase = new ProductoBase("CMS") { ID = 1, Descripción = "Camiseta Manga Corta", PorcentajeIVAPropio = 0.05 };

            if (todosSoloINC) {

                venta.Líneas = new List<LíneaVenta> {
                    new LíneaVenta(new Producto("R3") { DescripciónBase = "Producto 3", ExcluídoIVA = true,
                        TipoImpuestoConsumoPropio = TipoImpuestoConsumo.ServiciosRestaurante },
                        venta, cantidad: cantidad + 10, precio: precio + 3000, costo: precio * 0.8M),
                    new LíneaVenta(new Producto("R4") { DescripciónBase = "Producto 4", ExcluídoIVA = true,
                        TipoImpuestoConsumoPropio =  TipoImpuestoConsumo.ServiciosRestaurante },
                        venta, cantidad: cantidad + 15, precio: precio + 4000, costo: precio * 0.8M),
                };

            } else {

                venta.Líneas = new List<LíneaVenta> {
                    new LíneaVenta(new Producto("R1") { DescripciónBase = "Producto 1", ExcluídoIVA = todosExcluídosIVA || unoExcluídoIVA },
                        venta, cantidad: cantidad, precio: precio, costo: precio * 0.8M),
                    new LíneaVenta(new Producto("R2") { DescripciónBase = "Producto 2", ExcluídoIVA = todosExcluídosIVA },
                        venta, cantidad: cantidad + 5, precio: precio + 2000, costo: precio * 0.8M),
                    new LíneaVenta(new Producto("R3") { DescripciónBase = "Producto 3", ExcluídoIVA = todosExcluídosIVA,
                        TipoImpuestoConsumoPropio = unoConINC ? TipoImpuestoConsumo.ServiciosRestaurante : TipoImpuestoConsumo.Desconocido },
                        venta, cantidad: cantidad + 10, precio: precio + 3000, costo: precio * 0.8M),
                    new LíneaVenta(new Producto("R4") { DescripciónBase = "Producto 4", ExcluídoIVA = todosExcluídosIVA || unoConINCyExcluídoIVA,
                        TipoImpuestoConsumoPropio = unoConINCyExcluídoIVA ? TipoImpuestoConsumo.ServiciosRestaurante : TipoImpuestoConsumo.Desconocido },
                        venta, cantidad: cantidad + 15, precio: precio + 4000, costo: precio * 0.8M),
                    new LíneaVenta(new Producto("R5") { DescripciónBase = "Producto 5", PorcentajeIVAPropio = unoExentoIVA ? 0 : (double?)null },
                        venta, cantidad: cantidad + 5, precio: precio + 2000, costo: precio * 0.8M),
                    new LíneaVenta(new Producto("CMSRS", camisetaBase, new List<string> { "Roja", "Talla S" }) {
                        DescripciónEspecífica = "Camiseta Roja Talla S en Promoción ¡Extra Económica!" }, venta, cantidad + 2, precio + 1000, precio * 0.8M),
                    new LíneaVenta(new Producto("CMSVL", camisetaBase, new List<string> { "Verde", "Talla L", "Extra Suave" })
                        { UnidadEspecífica = Unidad.Docena}, venta, 1, 12 * (precio + 1400), 12 * (precio * 0.8M)),
                    new LíneaVenta(new Producto("CMSAXXL", camisetaBase, new List<string> { "Azul", "Talla XXL" }) {
                        PorcentajeIVAPropioEspecífico = 0.16 }, venta, cantidad, precio + 1300, precio * 0.8M),
                };

            }

            if (pruebaIntegración) {

                var datos = venta.ObtenerDatosIntegración();
                if (!ExisteRuta(TipoElementoRuta.Carpeta, Equipo.RutaIntegración, "integración con programas terceros", out string? mensajeExiste))
                    throw new DirectoryNotFoundException(mensajeExiste); // No es un código que normalmente vaya a ejecutar un usuario entonces con la excepción basta.
                File.WriteAllText(Path.Combine(Equipo.RutaIntegración!, $"{DocumentoIntegración.Venta.ATexto()}{AhoraNombresArchivos}.json"), // Se usa ! porque en la línea anterior saca exepción si RutaIntegración es nula.
                    Serializar(datos, Serialización.EnumeraciónEnTexto));
                mensaje = "";
                return true;

            } else {
                return CrearYEnviarDocumentoElectrónico(venta, out mensaje, out ventaElectrónica, pruebaHabilitación);
            }

        } // VentaSimple>


        public static bool NotaCréditoEjemploXml(out string? mensaje, int númeroNotaCrédito, bool pruebaHabilitación, Venta venta,
            out NotaCréditoVenta? notaCrédito,
            out DocumentoElectrónico<Factura<Cliente, LíneaNotaCréditoVenta>, LíneaNotaCréditoVenta>? notaCréditoElectrónica,
            bool pruebaIntegración = false) {

            notaCréditoElectrónica = null;
            notaCrédito = null;
            if (Empresa.PrimerNúmeroFacturaAutorizada == null) return Falso(out mensaje, "No se esperaba Empresa.PrimerNúmeroFacturaAutorizada nulo.");
            if (venta.Cliente == null) return Falso(out mensaje, "No se esperaba que el cliente de la venta fuera nulo.");

            notaCrédito = new NotaCréditoVenta(venta.Cliente, venta) {
                FechaHora = AhoraUtcAjustado, Número = númeroNotaCrédito, ConsecutivoDianAnual = 50 + númeroNotaCrédito,
                Razón = RazónNotaCrédito.DevoluciónParcial, Observación = "Devolución por mala calidad."
            };
            notaCrédito.Líneas = new List<LíneaNotaCréditoVenta> {
                new LíneaNotaCréditoVenta(new Producto("AOHV84-225") { DescripciónBase = "Articulo 1 Prueba" }, notaCrédito, 1, 12600.06M, 10000),
            };

            if (pruebaIntegración) {

                var datos = notaCrédito.ObtenerDatosIntegración();
                if (!ExisteRuta(TipoElementoRuta.Carpeta, Equipo.RutaIntegración, "integración con programas terceros", out string? mensajeExiste))
                    throw new DirectoryNotFoundException(mensajeExiste); // No es un código que normalmente vaya a ejecutar un usuario entonces con la excepción basta.
                File.WriteAllText(Path.Combine(Equipo.RutaIntegración!, $"{DocumentoIntegración.NotaCrédito.ATexto()}{AhoraNombresArchivos}.json"), // Se usa ! porque en la línea anterior saca exepción si RutaIntegración es nula.
                    Serializar(datos, Serialización.EnumeraciónEnTexto));
                mensaje = "";
                return true;

            } else {
                return CrearYEnviarDocumentoElectrónico(notaCrédito, out mensaje, out notaCréditoElectrónica, pruebaHabilitación);
            }

        } // NotaCréditoEjemploXml>


        public static bool NotaDébitoEjemploXml(out string? mensaje, int númeroNotaDébito, bool pruebaHabilitación, Venta venta) {

            if (Empresa.PrimerNúmeroFacturaAutorizada == null) return Falso(out mensaje, "No se esperaba Empresa.PrimerNúmeroFacturaAutorizada nulo.");
            if (venta.Cliente == null) return Falso(out mensaje, "No se esperaba que el cliente de la venta fuera nulo.");

            var notaDébito = new NotaDébitoVenta(venta.Cliente, venta) {
                FechaHora = AhoraUtcAjustado, Número = númeroNotaDébito, ConsecutivoDianAnual = 50 + númeroNotaDébito,
                Razón = RazónNotaDébito.Intereses, Observación = "Intereses de factura F568798."
            };
            notaDébito.Líneas = new List<LíneaNotaDébitoVenta> {
                new LíneaNotaDébitoVenta(new Producto("INT6") { DescripciónBase = "Interés de 6 meses de mora" }, notaDébito, 1, 77600, 0),
            };

            return CrearYEnviarDocumentoElectrónico(notaDébito, out mensaje, out _, pruebaHabilitación);

        } // NotaDébitoEjemploXml>


    } // Pruebas>



} // SimpleOps>

