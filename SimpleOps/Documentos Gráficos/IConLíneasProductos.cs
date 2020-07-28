using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public interface IConLíneasProductos {


        #region Propiedades

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; }
        #pragma warning restore CA2227

        public OpcionesColumnas Columnas { get; set; }

        public bool ModoImpresión { get; set; }

        #endregion Propiedades>


    } // IConLíneasProductos>



} // SimpleOps.DocumentosGráficos>

