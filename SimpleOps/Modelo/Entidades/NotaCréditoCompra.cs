using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Descuento al valor de una compra por devolución de productos.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class NotaCréditoCompra : Factura<Proveedor, LíneaNotaCréditoCompra> {


        #region Propiedades

        [NotMapped]
        public override Proveedor? EntidadEconómica => Proveedor;

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; }

        public Compra? Compra { get; set; } // Obligatorio.
        public int CompraID { get; set; }

        public override List<LíneaNotaCréditoCompra> Líneas { get; set; } = new List<LíneaNotaCréditoCompra>();

        /// <summary>
        /// DevoluciónParcial = 1, AnulaciónFactura = 2, Descuento = 3, AjustePrecio = 4, Otra = 5.
        /// </summary>
        public RazónNotaCrédito Razón { get; set; } = RazónNotaCrédito.Otra;

        #endregion Propiedades>


        #region Constructores

        private NotaCréditoCompra() { } // Solo para que EF Core no saque error.

        public NotaCréditoCompra(Proveedor proveedor, Compra compra) {
            (ProveedorID, CompraID, Proveedor, Compra) = (proveedor.ID, compra.ID, proveedor, compra);
            VerificarDatosEntidad();
        } // NotaCréditoCompra>

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Proveedor, ProveedorID)}";

        public override void VerificarDatosEntidad() => Proveedor.VerificarDatosCompra(Proveedor);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => "?"; // No se conoce para el proveedor ni habría que conocerlo porque nunca se requiere calcular el CUDE para documentos del proveedor. Este método se agrega solo por compatibilidad con la clase Factura.

        #endregion Métodos y Funciones>


    } // NotaCréditoCompra>



} // SimpleOps.Modelo>
