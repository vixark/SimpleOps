using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Integración {



    class DatosDocumento {


        #region Propiedades Documento

        public int? Número { get; set; }

        public string? Prefijo { get; set; }

        public string? Observación { get; set; }

        public bool MostrarInformaciónAdicional { get; set; }

        public DateTime? FechaHora { get; set; } // Puede ser nulo para documentos que no la requieren como fichas técnicas.

        #endregion Propiedades Documento


        #region Propiedades Cliente

        public string? ClienteNombre { get; set; } // Puede ser nulo para documentos que no son específicos para cierto cliente como los catálogos.

        public string? ClienteTeléfono { get; set; }

        public string? ClienteMunicipioNombre { get; set; }

        public string? ClienteMunicipioDepartamento { get; set; }

        public string? ClienteDirección { get; set; }

        public string? ClienteContactoFacturasEmail { get; set; }

        public int? ClienteDíasCrédito { get; set; }

        public double? ClienteMunicipioPorcentajeIVAPropio { get; set; }

        public double? ClientePorcentajeIVAPropio { get; set; }

        public TipoEntidad ClienteTipoEntidad { get; set; }

        public TipoCliente ClienteTipoCliente { get; set; }

        public string? ClienteIdentificación { get; set; }

        #endregion Propiedades Cliente>


    } // DatosDocumento>



} // SimpleOps.Integración>
