using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad que admite cambios después de creada. Se rastrea la fecha de actualización, el actualizador y el creador, pero no deja un rastro 
    /// independiente de fecha de creación. Es útil para entidades que ya disponen de propiedades asociadas a su creación (como Factura) o entidades 
    /// donde no es tan necesario disponer de esta información porque basta con el rastro de actualización (que se también se escribe en la creación).
    /// </summary>
    abstract class Actualizable : IActualizable {


        #region Propiedades
  
        public int ActualizadorID { get; set; } // Para evitar complejizar innecesariamente las relaciones de cada tabla en la base de datos no se hará una propiedad de navegación al usuario actualizador. Estos registros son informativos y además es fácil obtener el usuario a partir del ID cuando se requiera.

        [ConcurrencyCheck]
        public DateTime FechaHoraActualización { get; set; }

        public int CreadorID { get; set; } // Se almacena el CreadorID en Actualizable porque es información útil que ocupa muy poco espacio en la base de datos (En un ensayo dio 23 752 KB vs 23 540 KB sin CreadorID). La propiedad de Registrable que requiere mucho espacio es FechaActualización. Para evitar complejizar innecesariamente las relaciones de cada tabla en la base de datos no se hará una propiedad de navegación al usuario actualizador. Estos registros son informativos y además es fácil obtener el usuario a partir del ID cuando se requiera.

        #endregion Propiedades


        #region Métodos

        public abstract override string ToString(); // Obliga a las entidades derivadas a implementar un método ToString() propio para describirse de una manera entendible para el usuario. También se podría hacer a nivel de interface (ver https://stackoverflow.com/a/50930292/8330412) pero habría que crear otro método entonces se prefiere hacer por clase abstracta.

        #endregion Métodos>


    } // Actualizable>



} // SimpleOps.Modelo>
