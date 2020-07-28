using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using System.Linq;



namespace SimpleOps.Modelo {



    class RetenciónFuente {


        #region Propiedades

        public ConceptoRetención Concepto { get; set; }

        public decimal MínimoUVT { get; set; }

        public double Porcentaje { get; set; }

        public TipoDeclarante TipoDeclarante { get; set; }

        #endregion Propiedades>


        #region Constructores

        private RetenciónFuente() { } // Necesaria para la deserialización.

        public RetenciónFuente(ConceptoRetención concepto, decimal mínimoUVT, double porcentajex100, TipoDeclarante tipoDeclarante) 
            => (Concepto, MínimoUVT, Porcentaje, TipoDeclarante) = (concepto, mínimoUVT, porcentajex100 / 100, tipoDeclarante);

        #endregion Constructores>


    } // RetenciónFuente>



} // SimpleOps.Modelo>
