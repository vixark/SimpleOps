using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Integración {



    class DatosVenta : DatosDocumento {


        #region Propiedades

        public string? OrdenCompraNúmero { get; set; }

        public RazónNotaCrédito Razón { get; set; } = RazónNotaCrédito.Otra; // Siempre es Otra para facturas proforma y para facturas de venta. Solo aplica para las notas crédito de venta.

        public string? VentaPrefijo { get; set; } // Puede ser nulo para las facturas proforma y para ventas.

        public int? VentaNúmero { get; set; } // Puede ser nulo para las facturas proforma y para ventas.

        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.

        #endregion Propiedades>


    } // DatosVenta>



} // SimpleOps.Integración>
