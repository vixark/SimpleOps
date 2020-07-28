using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad que admite cambios después de creada. Se hace rastreo completo del creador, actualizador, fecha de creación y fecha de actualización. Es útil para entidades que sus datos cambian frecuentemente y que se necesita disponer de la información del creador y la fecha de creación.
    /// </summary>
    abstract class Rastreable : IRegistro, IActualizable {


        #region Propiedades
  
        public int CreadorID { get; set; } // Para evitar complejizar innecesariamente las relaciones de cada tabla en la base de datos no se hará una propiedad de navegación al usuario creador. Estos registros son informativos y además es fácil obtener el usuario a partir del ID cuando se requiera.
 
        public int ActualizadorID { get; set; } // Para evitar complejizar innecesariamente las relaciones de cada tabla en la base de datos no se hará una propiedad de navegación al usuario actualizador. Estos registros son informativos y además es fácil obtener el usuario a partir del ID cuando se requiera.
    
        public DateTime FechaHoraCreación { get; set; }

        [ConcurrencyCheck]
        public DateTime FechaHoraActualización { get; set; }

        #endregion Propiedades>


        #region Métodos

        public abstract override string ToString(); // Obliga a las entidades derivadas a implementar un método ToString() propio para describirse de una manera entendible para el usuario. También se podría hacer a nivel de interface (ver https://stackoverflow.com/a/50930292/8330412) pero habría que crear otro método entonces se prefiere hacer por clase abstracta.

        #endregion Métodos>


    } // Rastreable>



} // SimpleOps.Modelo>
