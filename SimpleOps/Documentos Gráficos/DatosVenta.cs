using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;



namespace SimpleOps.DocumentosGráficos {



    public class DatosVenta : DatosDocumento, IConLíneasProductos { // Común para ventas y para notas crédito de ventas.


        #region Propiedades

        public string? Cude { get; set; } // Puede ser nulo para las facturas proforma.

        public string? QR { get; set; } // Puede ser nulo para las facturas proforma.

        public DateTime? FechaVencimiento { get; set; } // Puede ser nulo para las facturas proforma.

        public string? OrdenCompraNúmero { get; set; }

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.
        #pragma warning restore CA2227

        public OpcionesColumnas Columnas { get; set; } = new OpcionesColumnas();

        public string? SubtotalBaseTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? IVATexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? SubtotalFinalConImpuestosTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? ImpuestoConsumoTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? DescuentoCondicionadoTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? RazónNotaCréditoTexto { get; set; } // Puede ser nulo para facturas proforma y para facturas de venta. Solo aplica para las notas crédito de venta.

        public string? NombreCude { get; set; } // Puede ser nulo para las facturas proforma.

        public string? CódigoVenta { get; set; } // Puede ser nulo para las facturas proforma y para ventas.

        #endregion Propiedades>


    } // DatosVenta>



} // SimpleOps.DocumentosGráficos>
