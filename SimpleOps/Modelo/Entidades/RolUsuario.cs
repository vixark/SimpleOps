using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Relaciona los roles que tiene cada usuario.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class RolUsuario : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        public Usuario? Usuario { get; set; } // Obligatorio.
        public int UsuarioID { get; set; } // Clave foránea que con RolID forman la clave principal.

        public Rol? Rol { get; set; } // Obligatorio.
        public int RolID { get; set; } // Clave foránea que con UsuarioID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private RolUsuario() { } // Solo para que EF Core no saque error.

        public RolUsuario(Usuario usuario, Rol rol) => (UsuarioID, RolID, Usuario, Rol) = (usuario.ID, rol.ID, usuario, rol);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Usuario, UsuarioID)} - {ATexto(Rol, RolID)}";

        #endregion Métodos y Funciones>


    } // RolUsuario>



} // SimpleOps.Modelo>
