using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.Modelo {



    class ResultadoBloqueo {


        public bool Éxitoso { get; set; } // Es falso si el bloqueo no se pudo realizar porque ya existía otro bloqueo que lo impedía.

        public List<Bloqueo> Bloqueos { get; set; }

        public ResultadoBloqueo(bool éxitoso, List<Bloqueo> bloqueos) => (Éxitoso, Bloqueos) = (éxitoso, bloqueos);


    } // ResultadoBloqueo>



} // SimpleOps.Modelo>
