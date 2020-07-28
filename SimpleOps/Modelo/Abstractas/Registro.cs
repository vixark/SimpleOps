using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad que representa un registro histórico. Estas entidades solo se crean no se actualizan, por lo tanto no tienen propiedades para rastrear la fecha de su actualización ni su actualizador. Tienen ID como clave única y no tienen restricciones adicionales sobre las otras columnas.
    /// </summary>
    abstract class Registro : IRegistro {


        #region Propiedades

        [Key]
        public int ID { get; set; }

        public int CreadorID { get; set; } // Para evitar complejizar innecesariamente las relaciones de cada tabla en la base de datos no se hará una propiedad de navegación al usuario creador. Estos registros son informativos y además es fácil obtener el usuario a partir del ID cuando se requiera.

        public DateTime FechaHoraCreación { get; set; }

        #endregion Propiedades


        #region Métodos

        public abstract override string ToString(); // Obliga a las entidades derivadas a implementar un método ToString() propio para describirse de una manera entendible para el usuario. También se podría hacer a nivel de interface (ver https://stackoverflow.com/a/50930292/8330412) pero habría que crear otro método entonces se prefiere hacer por clase abstracta.

        #endregion Métodos>


    } // Registro>



} // SimpleOps.Modelo>
