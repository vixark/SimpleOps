using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    [Owned] [ControlInserción(ControlConcurrencia.Ninguno)]
    class Dimensión {


        #region Campos

        public double Alto { get; set; }

        public double Ancho { get; set; }

        public double Largo { get; set; }

        #endregion Campos>


        #region Constructores

        public Dimensión() { }

        public Dimensión(double alto, double ancho, double largo) => (Alto, Ancho, Largo) = (alto, ancho, largo);

        #endregion


        #region Propiedades Autocalculadas

        public double Volumen => Alto * Ancho * Largo;

        #endregion Propiedades Autocalculadas>


    } // Dimensión>



} // SimpleOps.Modelo>
