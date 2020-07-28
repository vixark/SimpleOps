using System;
using System.Collections.Generic;
using System.Text;
using static Vixark.General;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Clase auxiliar que sirve como intermediaria cuando se necesita hacer el mismo bloqueo para varias entidades del mismo tipo.
    /// El ID heredado no tiene significado en esta clase.
    /// </summary>
    class BloqueoVarias : Bloqueo {


        #region Propiedades

        public List<int>? IDs { get; set; }

        public BloqueoVarias(string nombreEntidad) : base(nombreEntidad, 0) { } // Al ser una entidad auxiliar no se necesita obligar el usuarioID en el constructor para que cumpla su función. Cuando se copian los datos de BloqueoVarias a Bloqueo se escribe el usuarioID desde Global.UsuarioActual.

        public BloqueoVarias(Bloqueo bloqueo) : this(bloqueo.NombreEntidad) => bloqueo.CopiarA(this);

        #endregion Propiedades>


    } // BloqueoVarias>



} // SimpleOps.Modelo>
