using RazorEngineCore;
using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class DatosCotización : DatosDocumento, IConLíneasProductos {


        #region Propiedades

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.
        #pragma warning restore CA2227

        public OpcionesColumnas Columnas { get; set; } = new OpcionesColumnas();

        #endregion Propiedades>


        #region Constructores

        public DatosCotización() => (NombreDocumento) = ("Cotización");

        #endregion Constructores>


    } // DatosCotización>



} // SimpleOps.DocumentosGráficos>
