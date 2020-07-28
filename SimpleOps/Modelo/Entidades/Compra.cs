using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Recepción de productos a cambio de dinero.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class Compra : Factura<Proveedor, LíneaCompra> {


        #region Propiedades

        [NotMapped]
        public override Proveedor? EntidadEconómica => Proveedor;

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; }

        public override List<LíneaCompra> Líneas { get; set; } = new List<LíneaCompra>();

        public ComprobanteEgreso? ComprobanteEgreso { get; set; }
        public int? ComprobanteEgresoID { get; set; }

        public Pedido? Pedido { get; set; } 
        public int? PedidoID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private Compra() { } // Solo para que EF Core no saque error.

        public Compra(Proveedor proveedor, Pedido pedido) : this(proveedor) => (PedidoID, Pedido) = (pedido.ID, pedido);

        public Compra(Proveedor proveedor) { 
            (ProveedorID, Proveedor) = (proveedor.ID, proveedor);
            VerificarDatosEntidad();
        } // Compra>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public DateTime? FechaVencimiento => Proveedor == null ? (DateTime?)null : FechaHora.AddDays(Proveedor.DíasCrédito);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Proveedor, ProveedorID)}";

        public override void VerificarDatosEntidad() => Proveedor.VerificarDatosCompra(Proveedor);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => "?"; // No se conoce para el proveedor ni habría que conocerlo porque nunca se requiere calcular el CUDE para documentos del proveedor. Este método se agrega solo por compatibilidad con la clase Factura.

        #endregion Métodos y Funciones>


    } // Compra>



} // SimpleOps.Modelo>
