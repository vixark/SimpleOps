using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Cliente o Proveedor.
    /// </summary>
    abstract class EntidadEconómica : Rastreable { // Es Rastreable porque las entidades económicas son actualizados frecuentemente y es de interés tener la información de su creación.


        #region Propiedades

        [Key]
        public int ID { get; set; }


        /// <summary>
        /// Razón social de la empresa o nombre de la persona.
        /// </summary>
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)]
        public string Nombre { get; set; } // Obligatorio. No es la clave principal porque podría ser cambiado y aumentaría mucho el tamaño de las tablas que lo relacionan.

        /// <MaxLength>100</MaxLength>
        [MaxLength(100)]
        public string? NombreComercial { get; set; } // Opcional. Se puede usar para ser impreso en la representación gráfica de las facturas y también se informa a la DIAN en la factura electrónica.

        /// <summary>
        /// Desconocido = 0, Empresa = 1, Persona = 2.
        /// </summary>
        public TipoEntidad TipoEntidad { get; set; } = TipoEntidad.Desconocido; // Empresa o Persona. Por lo general no se necesita realizar esta distinción porque una distinción más apropiada para fines de precios y condiciones comerciales se puede establecer en TipoCliente. No se usa el nombre más simple Tipo porque en el caso de clientes también existe TipoCliente entonces se prefiere hacer una distinción explicita de ambos tipos.

        /// <summary>
        /// Ordinario = 1, GranContribuyente = 2, Autorretenedor = 4, RetenedorIVA = 8, RégimenSimple = 16, ResponsableIVA = 32, NoResponsableIVA = 64. 
        /// Una entidad económica (cliente o proveedor) puede tener múltiples tipos de contribuyente, en estos casos se suman los valores de los tipos que 
        /// le apliquen. Por ejemplo, si es GranContribuyente = 2, Autorretenedor = 4 y ResponsableIVA = 32, se usa el valor 2 + 4 + 32 = 38. 
        /// Si la empresa usuaria de SimpleOps no es gran contribuyente ni retenedor de IVA no es necesario conocer si los clientes son responsables de 
        /// IVA o no pues la empresa usuaria nunca necesita aplicar retención de IVA y esta información solo se necesita para su cálculo.
        /// En el caso de los proveedores si es necesario establecer si es o no responsable de IVA porque se usa esta información no aplicar IVA a sus compras.
        /// </summary>
        public TipoContribuyente TipoContribuyente { get; set; } = TipoContribuyente.Ordinario | TipoContribuyente.ResponsableIVA;

        /// <summary>
        /// NIT (sin dígito de verificación) o cédula de ciudadanía.
        /// </summary>
        /// <MaxLength>20</MaxLength>
        [MaxLength(20)]
        public string? Identificación { get; set; } // Es opcional porque para cotizar no es necesario disponer de él.

        /// <summary>
        /// Principal. Usado en la factura.
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)] 
        public string? Teléfono { get; set; }

        /// <summary>
        /// General alternativo. Es útil para facilitar la adición de un telefono general alternativo de la entidad económica sin necesidad de crear un contacto asociado.
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? TeléfonoAlternativo { get; set; }

        public DirecciónCompleta? DirecciónCompleta { get; set; }


        public string? _Dirección;
        /// <summary>
        /// Principal. Usada en la factura.
        /// </summary>
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)] 
        public string? Dirección { get => _Dirección; 
            set {
                _Dirección = value;
                DirecciónCompleta = DirecciónCompleta.CrearDirecciónCompleta(_Municipio, _Dirección);
            }
        }


        public Municipio? _Municipio;
        /// <summary>
        /// Principal. Usado en la factura.
        /// </summary>
        [ForeignKey("MunicipioID")] 
        public Municipio? Municipio { get => _Municipio; 
            set {
                _Municipio = value;
                DirecciónCompleta = DirecciónCompleta.CrearDirecciónCompleta(_Municipio, _Dirección);
            }
        } // Municipio>


        public int? MunicipioID { get; set; }  // No es obligatorio porque las empresas que provean productos o servicios que no requieran representación o presencia física pueden realizar cotizaciones sin necesidad de conocer el municipio del cliente.

        /// <summary>
        /// Si es positivo es a favor de la entidad económica. Si es negativo es el valor que la entidad económica le debe a la empresa.
        /// </summary>
        public decimal Saldo { get; set; }

        /// <summary>
        /// Referencia identificadora de la entidad económica en los movimientos bancarios. Nombre basado en columna 'Referencia' en bancolombia.com.
        /// </summary>
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)] 
        public string? ReferenciaEnBanco { get; set; }

        /// <summary>
        /// Descripción identificadora de la entidad económica en los movimientos bancarios. Nombre basado en 'Descripción' en bancolombia.com.
        /// </summary>
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)] 
        public string? DescripciónEnBanco { get; set; }

        /// <summary>
        /// Cero si no tiene crédito aprobado. Aplica tanto para clientes (días de crédito dado a ellos), como para proveedores (días de crédito con ellos).
        /// </summary>
        public int DíasCrédito { get; set; }

        /// <summary>
        /// Cero si no tiene crédito aprobado, nulo si no tiene cupo definido, es decir tiene crédito ilimitado, por lo menos en SimpleOps. Aplica tanto para clientes (cupo de crédito dado a ellos), como para proveedores (cupo de crédito con ellos).
        /// </summary>
        public int? CupoCrédito { get; set; }

        #endregion Propiedades>


        #region Constructores

        public EntidadEconómica(string nombre) => (Nombre) = (nombre);

        public EntidadEconómica(string nombre, Municipio municipio) => (Nombre, MunicipioID, Municipio) = (nombre, municipio.ID, municipio);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public bool TieneCrédito => (DíasCrédito > 0) && (CupoCrédito != 0);

        public FormaPago FormaPago => TieneCrédito ? FormaPago.Crédito : FormaPago.Contado; // Útil para la factura electrónica de la DIAN.


        #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
        /// <summary>
        /// Si es persona natural o si no tiene identificación devuelve nulo.
        /// </summary>
        /// <returns></returns>
        public string? DígitoVerificaciónNit
            => TipoEntidad switch {
                TipoEntidad.Desconocido => null,
                TipoEntidad.Empresa => ObtenerDígitoVerificación(Identificación),
                TipoEntidad.Persona => null,
            };
        #pragma warning restore CS8524


        #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
        /// <summary>
        /// Si es empresa se incluye el número de verificación del NIT. Si es otro tipo de entidad no se agrega.
        /// </summary>
        public string? IdentificaciónCompleta 
            => TipoEntidad switch {
                TipoEntidad.Desconocido => Identificación,
                TipoEntidad.Empresa => $"{Identificación}-{ObtenerDígitoVerificación(Identificación)}",
                TipoEntidad.Persona => Identificación,
            };
        #pragma warning restore CS8524


        #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
        public string? NitLegalEfectivo
            => TipoEntidad switch {
                TipoEntidad.Desconocido => "2222222222", // Para efectos de la Dian se asumirá que un desconocido es una persona natural.
                TipoEntidad.Empresa => Identificación, 
                TipoEntidad.Persona => "2222222222", // Según el elemento FAK03-2 de la tabla Invoice del archivo de documentación de facturación electrónica de la DIAN cuando es pesona natural se usa un pseudo NIT con 10 números dos.
            };
        #pragma warning restore CS8524


        #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
        public string? NombreLegalEfectivo
            => TipoEntidad switch {
                TipoEntidad.Desconocido => "adquiriente final", // Para efectos de la Dian se asumirá que un desconocido es una persona natural.
                TipoEntidad.Empresa => Identificación, 
                TipoEntidad.Persona => "adquiriente final", // Según el elemento FAK20 de la tabla Invoice del archivo de documentación de facturación electrónica de la DIAN cuando es pesona natural se reporta "adquiriente final".
            };
        #pragma warning restore CS8524


        public string CódigoDocumentoIdentificación => ObtenerDocumentoIdentificación(TipoEntidad).AValor();

        #endregion Propiedades Autocalculadas>


    } // Entidad>



} // SimpleOps.Modelo>
