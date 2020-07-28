using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.Integración {



    class DatosVenta : DatosDocumento {


        #region Propiedades Venta

        public string? OrdenCompraNúmero { get; set; }

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.
        #pragma warning restore CA2227

        #endregion Propiedades Venta>


    } // DatosVenta>



} // SimpleOps.Integración>
