using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    /// <summary>
    /// Clase auxiliar para encapsular algunos parámetros externos al documento en sí del documento gráfico. Por lo general cambian
    /// los tamaños, colores o cantidad de elementos en el documento y pueden ser personalizables por el usuario al generar
    /// el documento.
    /// </summary>
    public class OpcionesDocumento {


        #region Propiedades

        public bool ModoImpresión { get; set; } = false;

        public bool MostrarInformaciónAdicional { get; set; } = false;

        #endregion Propiedades>


    } // OpcionesDocumento>



} // SimpleOps.DocumentosGráficos>
