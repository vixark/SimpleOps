using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class DatosUsuario {



        #region Propiedades

        public string Nombre { get; set; } = null!; // Obligatorio.

        public string Email { get; set; } = null!; // Obligatorio.

        public string? Teléfono { get; set; }

        #endregion Propiedades>



    } // DatosUsuario>



} // SimpleOps.DocumentosGráficos>
