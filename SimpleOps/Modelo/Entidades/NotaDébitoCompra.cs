using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Cargo al valor de una compra.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class NotaDébitoCompra : Factura<Proveedor, LíneaNotaDébitoCompra> {


        #region Propiedades

        [NotMapped]
        public override Proveedor? EntidadEconómica => Proveedor;

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; }

        public Compra? Compra { get; set; } // Obligatorio.
        public int CompraID { get; set; }

        public override List<LíneaNotaDébitoCompra> Líneas { get; set; } = new List<LíneaNotaDébitoCompra>();

        /// <summary>
        /// Intereses = 1, Gastos = 2, AjustePrecio = 3, Otra = 4.
        /// </summary>
        public RazónNotaDébito Razón { get; set; } = RazónNotaDébito.Otra;

        #endregion Propiedades>


        #region Constructores

        private NotaDébitoCompra() { } // Solo para que Entity Framework no saque error.

        public NotaDébitoCompra(Proveedor proveedor, Compra compra) {
            (ProveedorID, CompraID, Proveedor, Compra) = (proveedor.ID, compra.ID, proveedor, compra);
            VerificarDatosEntidad();
        } // NotaDébitoCompra>

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Proveedor, ProveedorID)}";

        public override void VerificarDatosEntidad() => Proveedor.VerificarDatosCompra(Proveedor);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => "?"; // No se conoce para el proveedor ni habría que conocerlo porque nunca se requiere calcular el CUDE para documentos del proveedor. Este método se agrega solo por compatibilidad con la clase Factura.

        #endregion Métodos y Funciones>


    } // NotaDébitoCompra>



} // SimpleOps.Modelo>
