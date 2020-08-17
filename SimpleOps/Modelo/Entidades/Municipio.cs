using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;
using static SimpleOps.Legal.Dian;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Ciudad o pueblo.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Municipio : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio. No es la clave principal porque podría ser cambiado y podrían haber varios con el mismo nombre en diferentes departamentos.

        /// <MaxLength>60</MaxLength>
        [MaxLength(60)]
        public string Departamento { get; set; } = null!; // Obligatorio en el uso normal. Cuando se carga desde un DTO de integración podría llegar a ser nulo y generar problemas, pero como la integración no es el foco de la aplicación se dejará sin controlar este caso. Aunque se podría pensar en establecer una clave. Es de 60 de largo para cumplir con el caso de 'Archipiélago De San Andrés, Providencia Y Santa Catalina'.

        /// <summary>
        /// Código único del municipio asignado por el gobierno.
        /// </summary>
        /// <MaxLength>10</MaxLength>
        [MaxLength(10)]
        public string? Código { get; set; } // Aunque siempre debería tener un valor para los municipios colombianos porque es obligatorio para la facturación electrónica también puede ser un municipio de otro país y para estos no se requiere proporcionar este código a la DIAN.

        /// <summary>
        /// Si es nulo se usa el país en opciones.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? OtroPaís { get; set; }

        public bool MensajeríaDisponible { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas en Global.ObtenerPorcentajeIVA(). 0 si es exento de IVA, por ejemplo: San Andrés y Providencia.
        /// </summary>
        public double? PorcentajeIVAPropio { get; set; }

        #endregion Propiedades>


        #region Constructores

        public Municipio() { } // Solo para que EF Core no saque error. Se permite público para poderlo pasar nulo al método genérico Vixark.General.CopiarA() y que dentro de este se cree la nueva instancia de Municipio.

        public Municipio(string nombre, string departamento) => (Nombre, Departamento) = (nombre, departamento);

        public double PorcentajeIVA => PorcentajeIVAPropio ?? Empresa.PorcentajeIVAPredeterminadoEfectivo; // Es solo para fines informativos porque al facturar se debe usar la función Global.ObtenerPorcentajeIVA().

        public bool ExentoIVA => PorcentajeIVA == 0; // Es solo para fines informativos porque al facturar se debe usar la función Global.ObtenerPorcentajeIVA().

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public string País => OtroPaís ?? Generales.PaísPredeterminado;

        public string CódigoDepartamento => ObtenerCódigoDepartamento(Departamento);

        public string? CódigoPaís => País == "Colombia" ? "CO" : null;

        public string? CódigoLenguajePaís => País == "Colombia" ? "es" : null; // Identificador del lenguaje utilizado en el nombre del país. Para español, utilizar el literal "es". Ver lista de valores posibles en el numeral 0, columna ISO639-1. Debe ser "es" para que funcione la facturación electrónica.

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Municipio>



} // SimpleOps.Modelo>
