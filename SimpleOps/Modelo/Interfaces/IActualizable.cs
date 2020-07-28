using System;



namespace SimpleOps.Modelo {



    interface IActualizable {


        #region Propiedades

        public int ActualizadorID { get; set; } 

        public DateTime FechaHoraActualización { get; set; }

        public int CreadorID { get; set; }

        #endregion Propiedades


    } // IActualizable>



} // SimpleOps.Modelo>
