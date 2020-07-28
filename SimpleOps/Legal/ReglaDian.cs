using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Legal {



    class ReglaDian {


        #region Propiedades

        public TipoReglaDian Tipo { get; set; }

        public string Código { get; set; }

        public string Mensaje { get; set; }

        #endregion Propiedades>


        #region Constructores

        public ReglaDian(string código, TipoReglaDian tipo, string mensaje) => (Código, Tipo, Mensaje) = (código, tipo, mensaje);

        #endregion Constructores>


    } // ReglaDian>



} // SimpleOps.Legal>
