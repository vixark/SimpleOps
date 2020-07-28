using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Integración {



    class DatosLíneaProducto {


        #region Propiedades

        public int Cantidad { get; set; }

        public decimal? Precio { get; set; }

        public string ProductoReferencia { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia.

        public string? ProductoDescripción { get; set; }

        public Unidad ProductoUnidad { get; set; }

        public double? ProductoPorcentajeIVAPropio { get; set; }

        public bool ProductoExcluídoIVA { get; set; }

        #endregion Propiedades>


    } // DatosLíneaProducto>



} // SimpleOps.Integración>
