using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using SimpleOps.Datos;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad auxiliar que describe un bloqueo necesario para implementar control de concurrencia pesimista. Cuando se estén realizando operaciones 
    /// que lo requieran permite realizar un bloqueo a todas las entidades de un tipo o a unas puntuales (toda la tabla o algunas filas). 
    /// Aunque en realidad no es un sistema estricto de concurrencia pesimista porque la conexión a la base de datos no permanece abierta y 
    /// no se implementa a nivel de la base de datos. Es más un sistema de 'ingreso' y 'egreso' de los usuarios a la posibilidad o no de modificar 
    /// las tablas y sus filas.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)]
    class Bloqueo { // No se necesita información de su creación ni de su actualización porque es una entidad auxiliar. Se hace de esta manera personalizada porque no se encontró algo propio del EF Core que permitiera implementar este control ni se encontró artículos completos que dieran lineamientos de la forma de implementarlo. En parte se inspiró de estas dos respuestas https://forums.asp.net/t/2127954.aspx?implementing+pessimistic+concurrency+using+entity+framework+ y https://stackoverflow.com/questions/32335266/pessimistic-concurrency-for-entity-framework.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string NombreEntidad { get; set; } = null!; // Obligatorio.

        /// <summary>
        /// ID de la entidad que está bloqueada. Si es nulo todas las entidades están bloqueadas, es decir todas las filas. Si se necesita bloquear solo algunas filas se debe agregar una fila a la tabla Bloqueos por cada ID de entidad (fila) a bloquear.
        /// </summary>
        public int? EntidadID { get; set; } // Actualmente solo es válido el bloqueo por filas para tablas que tengan ID.

        /// <summary>
        /// Nombre de la propiedad que está bloqueada. Si es nula todas las propiedades están bloqueadas, es de decir todas las columnas. Si se necesita bloquear solo algunas columnas se deben agregar una fila a la tabla Bloqueos por cada propiedad de entidad (columna) a bloquear.
        /// </summary>
        public string? Propiedad { get; set; }

        /// <summary>
        /// Describe el tipo de bloqueo. Ninguno = -1, Lectura = 1, Modificación = 2, Inserción = 4, Eliminación = 8. Los valores son sumables, es decir, para un bloquear la eliminación e inserción pero no la lectura ni actualización, el valor es: 4 + 8 = 12. El bloqueo de lectura no es implementado, todos los usuarios tendrán siempre permisos de lectura según sus roles.
        /// </summary>
        public TipoPermiso Tipo { get; set; } = TipoPermiso.Eliminación | TipoPermiso.Inserción | TipoPermiso.Modificación; // El tipo de bloqueo inicial de los bloqueos. Bloquea todo excepto la lectura.

        /// <summary>
        /// Usuario generador del bloqueo.
        /// </summary>
        public Usuario? Usuario { get; set; } // Obligatorio.
        public int UsuarioID { get; set; } // Obligatorio.

        /// <summary>
        /// FechaHora de inicio del bloqueo. Si el bloqueo se tarda mucho en ser liberado puede ser desbloqueado por otro usuario y el usuario bloqueador perdería la posibilidad de realizar la operación.
        /// </summary>
        public DateTime FechaHoraInicio { get; set; }

        #endregion Propiedades>


        #region Constructores

        private Bloqueo() { } // Solo para que EF Core no saque error.

        public Bloqueo(string nombreEntidad, int usuarioBloqueadorID)
            => (NombreEntidad, UsuarioID, FechaHoraInicio) = (nombreEntidad, usuarioBloqueadorID, AhoraUtcAjustado);

        public Bloqueo(BloqueoVarias bloqueoVarias) : this(bloqueoVarias.NombreEntidad, bloqueoVarias.UsuarioID) => bloqueoVarias.CopiarA(this);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => NombreEntidad;


        public static List<Bloqueo> ObtenerLista(BloqueoVarias bloqueoVarias, Usuario usuarioBloqueador) {

            var bloqueos = new List<Bloqueo>();
            if (bloqueoVarias.IDs != null) {
                foreach (var id in bloqueoVarias.IDs) {
                    bloqueos.Add(new Bloqueo(bloqueoVarias) { EntidadID = id, UsuarioID = usuarioBloqueador.ID }); // Se establece el UsuarioID aquí para evitar tener que establecerlo en el constructor de BloqueoVarias.
                }
            } else {
                bloqueos.Add(new Bloqueo(bloqueoVarias) { UsuarioID = usuarioBloqueador.ID }); // Se establece el UsuarioID aquí para evitar tener que establecerlo en el constructor de BloqueoVarias.
            }
            return bloqueos;

        } // ObtenerLista>


        #endregion Métodos y Funciones>


    } // Bloqueo>



} // SimpleOps.Modelo>
