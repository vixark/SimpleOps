using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    [ControlInserción(ControlConcurrencia.Ninguno)]
    class Permiso {


        #region Propiedades

        /// <summary>
        /// Ninguno = -1, Lectura = 1, Modificación = 2, Inserción = 4, Eliminación = 8. Los valores son sumables, es decir, para un permiso de Lectura, Modificación y Eliminación pero no de Inserción, el valor es: 1 + 2 + 8 = 11.
        /// </summary>
        public TipoPermiso Tipo { get; set; } = TipoPermiso.Ninguno;

        /// <summary>
        /// Puede ser Todas para aplicar permisos iniciales.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Tabla { get; set; } = null!; // Obligatorio.

        /// <summary>
        /// Si es nulo el permiso aplica para toda la tabla. Si no es nulo el permiso aplica para estas columnas.
        /// </summary>
        public List<string>? Columnas { get; set; }

        #endregion Propiedades>


        #region Constructores

        public Permiso(TipoPermiso tipo, string tabla, List<string>? columnas) => (Tipo, Tabla, Columnas) = (tipo, tabla, columnas); // Requiere Global.Permiso para que no entre en conflicto con Modelo.Permiso.

        private Permiso() { } // Necesario para que no saque error la serialización.

        #endregion Constructores>


    } // Permiso>



} // SimpleOps.Modelo>
