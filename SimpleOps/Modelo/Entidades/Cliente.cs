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
using System.Text;
using static SimpleOps.Global;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vixark;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad a la que se le venden productos.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)] // Aunque es una entidad muy importante no se puede hacer nada si dos usuarios concurrentemente insertan el mismo cliente con nombre ligeramente diferente. Solo se logra interceptar y aplicar el control de concurrencia en inserciones en el caso en el que usen exactamente el mismo nombre. En caso de inserciones no concurrentes si se puede controlar la similitud de nombre actual con los existentes en la base de datos.
    class Cliente : EntidadEconómica {


        #region Propiedades

        /// <summary>
        /// Al que se enviarán las facturas electrónicas.
        /// </summary>
        [ForeignKey("ContactoFacturasID")] 
        public Contacto? ContactoFacturas { get; set; }
        public int? ContactoFacturasID { get; set; }

        /// <summary>
        /// Al que se enviarán las comunicaciones de cobros y estados de cuenta.
        /// </summary>
        [ForeignKey("ContactoCobrosID")] 
        public Contacto? ContactoCobros { get; set; }
        public int? ContactoCobrosID { get; set; }

        /// <summary>
        /// Desconocido = 0, Consumidor = 1, Distribuidor = 2, GrandesContratos = 3, Otro = 255. Desconocido solo es aceptado si se establece Global.PermitirTipoClienteDesconocido = true.
        /// </summary>
        public TipoCliente TipoCliente { get; set; } = TipoCliente.Desconocido; // No se usa el nombre más simple Tipo porque en en la entidad económica también existe TipoEntidad entonces se prefiere hacer una distinción explicita de ambos tipos.

        /// <summary>
        /// Subtipo adicional del cliente. Especialmente útil cuando TipoCliente = Otro o TipoCliente = Grandes Contratos para agrupar las razones sociales (clientes) de cada contrato.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)] 
        public string? SubtipoCliente { get; set; } // Se mantiene Cliente en el nombre de la propiedad para hacerlo consistente con TipoCliente.

        /// <summary>
        /// Si es nulo se usan las reglas legales. Cero si no aplica retención de IVA.
        /// </summary>
        public double? PorcentajeRetenciónIVAPropio { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas legales. Cero si no aplica retención en la fuente.
        /// </summary>
        public double? PorcentajeRetenciónFuentePropio { get; set; }

        /// <summary>
        /// Si es nulo se usa el porcentaje en opciones. Cero si no aplica retención del ICA.
        /// </summary>
        public double? PorcentajeRetenciónICAPropio { get; set; }

        /// <summary>
        /// Si es nulo se usa el porcentaje en opciones. Retenciones varias que algunas empresas aplican al pago de todas sus facturas. Lo suelen hacer las públicas. Cero si no aplican. 
        /// </summary>
        public double? PorcentajeRetencionesExtraPropio { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas legales. Si no, realiza retención de IVA para facturas con subtotal superior o igual a este mínimo. Puede ser cero para aplicarla a todas.
        /// </summary>
        public decimal? MínimoRetenciónIVAPropio { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas legales. Si no, realiza retención en la fuente de renta para facturas con subtotal superior o igual a este mínimo. Puede ser cero para aplicarla a todas.
        /// </summary>
        public decimal? MínimoRetenciónFuentePropio { get; set; }

        /// <summary>
        /// Si es nulo se usa el mínimo en opciones. Si no, realiza retención del ICA para facturas con subtotal superior o igual a este mínimo. Puede ser cero para aplicarla a todas.
        /// </summary>
        public decimal? MínimoRetenciónICAPropio { get; set; }

        /// <summary>
        /// Si es nulo, como no hay mínimo en opciones, se aplica a todas. Si no, realiza retenciones extra para facturas con subtotal superior o igual a este mínimo. Puede ser cero para aplicarla a todas.
        /// </summary>
        public decimal? MínimoRetencionesExtraPropio { get; set; }

        /// <summary>
        /// Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50. Si es desconocida se usan las reglas en opciones. Al programar la asignación de los productos en inventario a las órdenes de compra pendientes se prefieren los clientes de mayor prioridad.
        /// </summary>
        public Prioridad PrioridadPropia { get; set; } = Prioridad.Desconocida;

        /// <summary>
        /// Desconocida = 0, Virtual = 1, PuntoDeVenta = 2, Mensajería = 3, Transportadora = 4, TransportadoraInternacional = 5, Otra = 255. Si es desconocida se usan las reglas en opciones. Forma de entrega predeterminada para pedidos que cumplen con las condiciones comerciales y que no se ha recibido comunicación del cliente para hacerlo de otra forma.
        /// </summary>
        public FormaEntrega FormaEntregaPropia { get; set; } = FormaEntrega.Desconocida;

        /// <summary>
        /// Si es nulo se usan las reglas en opciones. El subtotal mínimo de la orden de compra para ser envíada con transporte gratis.
        /// </summary>
        public decimal? MínimoTransporteGratisPropio { get; set; }

        /// <summary>
        /// Si es nula se usa el valor en opciones. La cantidad de copias de la factura que se imprime. Legalmente puede ser cero pues la factura electrónica es legalmente suficiente.
        /// </summary>
        public int? CopiasFacturaPropia { get; set; } // Se refiere a Propia porque el significado viene de CantidadCopiasFacturaPropia.

        /// <summary>
        /// Si es nulo se usan las reglas en opciones. El porcentaje de ganancia que se le aplica a los costos de los productos para obtener sus precios de venta. Es útil cuando no se quieren usar las listas de precios o cuando el producto no está en ellas.
        /// </summary>
        public double? PorcentajeGananciaPropio { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas en Global.ObtenerPorcentajeIVA(). Algunas entidades pueden estar exentas de IVA.
        /// </summary>
        public double? PorcentajeIVAPropio { get; set; }

        /// <summary>
        /// Se escriben en el campo observaciones de las facturas y se muestran en un cuadro de mensaje en la interfaz al hacer una factura.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? ObservacionesFactura { get; set; }

        [ForeignKey("RepresentanteComercialID")]
        public Usuario? RepresentanteComercial { get; set; }
        public int? RepresentanteComercialID { get; set; }

        /// <summary>
        /// Campaña de ventas que generó el primer contacto o recontacto efectivo con el cliente.
        /// </summary>
        public Campaña? Campaña { get; set; }
        public int? CampañaID { get; set; }

        public List<ContactoCliente> ContactosClientes { get; set; } = new List<ContactoCliente>();

        public List<Sede> Sedes { get; set; } = new List<Sede>();

        #endregion Propiedades>


        #region Constructores

        private Cliente() : base(null!) { } // Solo para que Entity Framework no saque error.

        public Cliente(string nombre) : this(nombre, TipoCliente.Desconocido) { }

        public Cliente(string nombre, Municipio municipio) : this(nombre, municipio, TipoCliente.Desconocido) { }

        public Cliente(string nombre, TipoCliente tipoCliente) : base(nombre) {
            (TipoCliente) = (tipoCliente);
            VerificarNecesidadMunicipioYTipoCliente(null, tipoCliente);
        } // Cliente>

        public Cliente(string nombre, Municipio municipio, TipoCliente tipoCliente) : base(nombre, municipio) {
            (TipoCliente) = (tipoCliente);
            VerificarNecesidadMunicipioYTipoCliente(municipio, tipoCliente);
        } // Cliente>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public double PorcentajeRetenciónICA => PorcentajeRetenciónICAPropio ?? Generales.PorcentajeRetenciónICAPredeterminado;

        public double PorcentajeRetencionesExtra => PorcentajeRetencionesExtraPropio ?? Generales.PorcentajeRetencionesExtraPredeterminado;

        public decimal MínimoRetenciónICA => MínimoRetenciónICAPropio ?? Generales.MínimoRetenciónICAPredeterminado;

        public decimal MínimoRetencionesExtra => MínimoRetencionesExtraPropio ?? Generales.MínimoRetencionesExtraPredeterminado;

        public int CopiasFactura => CopiasFacturaPropia ?? Empresa.CopiasFacturaPredeterminada;

        public Prioridad Prioridad => PrioridadPropia != Prioridad.Desconocida ? PrioridadPropia : ObtenerPrioridadCliente(TipoCliente);

        public FormaEntrega FormaEntrega => FormaEntregaPropia != FormaEntrega.Desconocida ? FormaEntregaPropia : ObtenerFormaEntrega(Municipio);

        public decimal MínimoTransporteGratis => MínimoTransporteGratisPropio ?? ObtenerMínimoTransporteGratis(TipoCliente, Municipio);

        public double PorcentajeGanancia => PorcentajeGananciaPropio ?? ObtenerPorcentajeGananciaCliente(TipoCliente);

        public double PorcentajeIVA => PorcentajeIVAPropio ?? Empresa.PorcentajeIVAPredeterminadoEfectivo; // Es solo para fines informativos porque al facturar se debe usar la función Global.ObtenerPorcentajeIVA().

        public bool ExentoIVA => PorcentajeIVA == 0; // Es solo para fines informativos porque al facturar se debe usar la función Global.ObtenerPorcentajeIVA().

        public string TipoClienteTexto => TipoCliente.ATexto(); // Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos.

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => Nombre;


        public static void VerificarNecesidadMunicipioYTipoCliente(Municipio? municipio, TipoCliente tipoCliente) {

            if (municipio == null && !Empresa.HabilitarProductosVirtuales && !OperacionesEspecialesDatos) 
                throw new Exception("Si no están habilitados los productos virtuales se debe establecer el municipio del cliente en el constructor.");

            if (tipoCliente == TipoCliente.Desconocido && !Empresa.PermitirTipoClienteDesconocido && !OperacionesEspecialesDatos)
                throw new Exception("Si no se permite clientes de tipo desconocido se debe establecer el tipo del cliente en el constructor.");

        } // VerificarNecesidadMunicipioYTipoCliente>


        /// <summary>
        /// Cumple una función de segunda verificación que la venta no se genere con datos incorrectos porque en la 
        /// interfaz de usuario se hace el control principal.
        /// </summary>
        public static void VerificarDatosVenta(Cliente? cliente) {

            var éxito = true;
            if (cliente != null) {
                if (cliente.TipoEntidad == TipoEntidad.Desconocido) éxito = false;
                if (cliente.Identificación == null) éxito = false;
                if (cliente.Municipio == null) éxito = false;
                if (cliente.Teléfono == null) éxito = false;
                if (cliente.Dirección == null) éxito = false;
            } else {
                éxito = false;
            }
            if (!éxito) throw new Exception("Faltan datos de cliente para poder generar una venta.");

        } // VerificarDatosVenta>


        #endregion Métodos y Funciones>


    } // Cliente>



} // SimpleOps.Modelo>
