using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.Modelo {



    class ReglasImpuesto {


        #region Propiedades

        /// <summary>
        /// Si la suma de los <see cref="MovimientoProducto.SubtotalBase"/> es mayor a este mínimo, se aplica la retención.
        /// </summary>
        public decimal Mínimo { get; set; }

        /// <summary>
        /// Guardado como fracción. Puede ser un porcentaje con respecto a cualquier base, como SubtotalBase o IVA, porque la base se puede proveer 
        /// a <see cref="Factura{E, M}.ObtenerRetención{C}(Func{C, ReglasImpuesto}, Func{M, C}, Func{M, decimal})"/>
        /// </summary>
        public decimal Porcentaje { get; set; }

        #endregion Propiedades>


        #region Constructores

        public ReglasImpuesto(decimal porcentaje, decimal mínimo) => (Mínimo, Porcentaje) = (mínimo, porcentaje);

        #endregion Constructores>


    } // ReglasImpuesto>



} // SimpleOps.Modelo>
