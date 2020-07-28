using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class DatosLíneaProducto {


        #region Propiedades

        public int Cantidad { get; set; }

        public string? PrecioBaseTexto { get; set; }

        public string? IVATexto { get; set; }

        public string? ImpuestoConsumoTexto { get; set; }

        public string? SubtotalBaseTexto { get; set; }

        public string? SubtotalBaseConImpuestosTexto { get; set; }

        public string ProductoReferencia { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia.

        public string? ProductoDescripción { get; set; }

        public string? ProductoUnidadTexto { get; set; }

        #endregion Propiedades>


    } // DatosLíneaProducto>



} // SimpleOps.DocumentosGráficos>
