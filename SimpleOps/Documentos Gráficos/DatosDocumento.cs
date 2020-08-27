using Dian.Factura;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps.DocumentosGráficos {



    public class DatosDocumento {


        #region Enumeraciones

        public enum Modo { CuerpoContinuo, PáginasIndependientes } // Se debe agregar aquí en vez de en DocumentosGráficos porque requiere el modificador public y no se debe hacer la clase estática DocumentosGráficos pública porque contiene referencias a otras clases no públicas.

        #endregion Enumeraciones>


        #region Propiedades Documento

        public string NombreDocumento { get; set; } = null!; // El nombre que va en el encabezado de la página. Nunca es vacío porque asigna en el constructor de cada documento heredado. Se usa el nombre redundante []Documento para darle más claridad cuando se escriba en la plantilla cshtml. 

        public string? LogoBase64 { get; set; } // El logo principal del documento. Se debe pasar como Base64 porque el iText no soporta imagenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? CertificadoBase64 { get; set; } // El logo del certificado de la empresa. Se debe pasar como Base64 porque el iText no soporta imagenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? CódigoDocumento { get; set; } // El número o código (cuando el número lleva prefijo) del documento. Puede ser nulo para documentos sin código. Se usa el nombre redundante []Documento para darle más claridad cuando se escriba en la plantilla cshtml. 

        public string? Observación { get; set; }

        public DatosEmpresa? Empresa { get; set; } // Los datos de la empresa usuaria de SimpleOps.

        /// <summary>
        /// En el <see cref="Modo.CuerpoContinuo"/> se debe pasar a 
        /// <see cref="CompilarPlantilla{T}(RazorEngineCore.RazorEngine, string, IDictionary{string, string})"/> 
        /// un diccionario con como mínimo un marco. El documento tendrá un encabezado, un cuerpo que podrá ocupar varias páginas y finalmente habrá un pie de 
        /// página. Solo habrá un encabezado y un pie de página en todo el documento.<br/><br/>
        /// En el <see cref="Modo.PáginasIndependientes"/> se debe pasar al diccionario un marco y un ítem con nombre Página[Número] 
        /// por cada página extra que tenga el documento, excepto la Página1 porque en esta se agrega el cuerpo. Cada página del 
        /// documento tendrá su propio encabezado y pie de página.
        /// </summary>
        public Modo ModoDocumento { get; set; } = Modo.CuerpoContinuo;

        public int TotalPáginas { get; set; } = 1; // Para tener un mayor control del diseño del documento y los posibles modos siempre se debe pasar el número de páginas del documento. Esto por lo general es fácil de conocer dependiendo del contenido, en particular de la cantidad de LíneasGráficas[Entidad].

        public DateTime? FechaHora { get; set; }

        #endregion Propiedades Documento>


        #region Propiedades Cliente

        public string? ClienteNombre { get; set; } // Puede ser nulo para documentos que no son de clientes.

        public string? ClienteTeléfono { get; set; }

        public string? ClienteMunicipioNombre { get; set; }

        public string? ClienteDirección { get; set; }

        public string? ClienteIdentificaciónCompleta { get; set; }

        public string? ClienteContactoFacturasEmail { get; set; }

        #endregion Propiedades Cliente>


        #region Propiedades Diseño

        public int AnchoHoja {get; set;} = 816; // 96 * 8.5.Tomado de https://stackoverflow.com/a/40032997/8330412.

        public int AltoHoja { get; set; } = 1056; // 96 * 11. Tomado de https://stackoverflow.com/a/40032997/8330412.

        public int AnchoObservación { get; set; } = 350; // 350 para facturas, notas crédito y facturas proforma.

        public int MargenHorizontal { get; set; } = 25;

        public int MargenVertical { get; set; } = 30; // Este valor puede causar que la cantidad de páginas difiera, pero esto solo se da en casos de muchas páginas y un margenCuerpo alto. Por ejemplo para un documento de 500 páginas se puede llevar este margen hasta 70 y se mantienen las 500 páginas, no es algo que pueda afectar en el uso más común.

        public string ColorBordes { get; set; } = "#E1E1E1";

        public string ColorLetra { get; set; } = "#232323";

        public string NombreFuente { get; set; } = "Calibri";

        public int TamañoLetra { get; set; } = 16;

        public double GrosorBordes { get; set; } = 3;

        public int MargenContenidos { get; set; } = 20;

        public int AltoCabeza { get; set; } = 120;

        public int AltoLogo { get; set; } = 65;

        public int RellenoSuperiorPie { get; set; } = 15;

        public int TamañoLetraPie { get; set; } = 14; // Solo puede ser entero y deben existir las clases t[tamañoLetraPie]b y t[tamañoLetraPie]n.

        public int LíneasTextoPie { get; set; } = 4; // Cantidad de divs dentro de div.pie. 
        
        public bool LíneaBajoCabeza { get; set; } = true;

        public int SeparaciónLíneaBajoCabeza { get; set; } = 5;

        public double AnchoLíneaBajoCabeza { get; set; } = 9.5;

        public bool ModoImpresión { get; set; } = false; // Un modo de diseño optimizado para la impresión que disminuye el uso de fondos de color.

        #endregion Propiedades Diseño>


        #region Propiedades Autocalculadas

        public int AnchoContenido => AnchoHoja - 2 * MargenHorizontal;

        public double AltoPie => RellenoSuperiorPie + LíneasTextoPie * 1.25 * TamañoLetraPie;

        #endregion Propiedades Autocalculadas>


    } // DatosDocumento>



} // SimpleOps.DocumentosGráficos>

