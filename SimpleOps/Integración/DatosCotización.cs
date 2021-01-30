using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Integración {



    class DatosCotización : DatosDocumento {


        #region Propiedades

        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.

        public int TamañoImagenes { get; set; }

        #endregion Propiedades>


    } // DatosCotización>



} // SimpleOps.Integración>
