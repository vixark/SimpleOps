using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class DatosCliente {



        #region Propiedades

        public string Nombre { get; set; } = null!; // Obligatorio.

        public string? Teléfono { get; set; }

        public string? MunicipioNombre { get; set; }

        public string? Dirección { get; set; }

        public string? IdentificaciónCompleta { get; set; }

        public string? ContactoFacturasEmail { get; set; }

        public string TipoClienteTexto { get; set; } = null!; // Se asegura que no es nulo.

        #endregion Propiedades>



    } // DatosCliente>



} // SimpleOps.DocumentosGráficos>
