// Copyright Notice:
//
// SimpleOps® is a free ERP software for small businesses and independents.
// Copyright© 2021 Vixark (vixark@outlook.com).
// For more information about SimpleOps®, see https://simpleops.net.
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero
// General Public License as published by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the
// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public
// License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not,
// see https://www.gnu.org/licenses.
//
// This License does not grant permission to use the trade names, trademarks, service marks, or product names
// of the Licensor, except as required for reasonable and customary use in describing the origin of the Work
// and reproducing the content of the NOTICE file.
//
// Removing or changing the above text is not allowed.
//

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

        public string NombreDocumento { get; set; } = null!; // El nombre que va en el encabezado de la página. Nunca es vacío porque asigna en el constructor de cada documento heredado. Se usa el nombre redundante []Documento para darle más claridad cuando se escriba en la plantilla CSHTML. 

        public string PrefijoNombreArchivo { get; set; } = ""; // Algunos documentos usan un prefijo adicional al nombre de archivo autogenerado con el código para evitar colisiones con otros almacenados en la misma carpeta, por ejemplo se usa NC para las notas crédito de ventas y texto vacío para las ventas. El código de documento de las ventas ya puede traer su propio prefijo de facturación.

        public string? LogoBase64 { get; set; } // El logo principal del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? Logo2Base64 { get; set; } // Un logo secundario del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? Logo3Base64 { get; set; } // Un logo secundario del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? Logo4Base64 { get; set; } // Un logo secundario del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? Logo5Base64 { get; set; } // Un logo secundario del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? Logo6Base64 { get; set; } // Un logo secundario del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? Logo7Base64 { get; set; } // Un logo secundario del documento. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public string? LogoExcelBase64 { get; set; } 

        public string? CertificadoBase64 { get; set; } // El logo del certificado de la empresa. Se debe pasar como Base64 porque el iText no soporta imágenes que estén relacionadas en el atributo src con rutas locales que contengan espacios en su nombre.

        public Dictionary<string, string> ImágenesProductosBase64 { get; } = new Dictionary<string, string>(); // Las imágenes de los productos que están en la lista. Solo se usa get; por esta recomendación https://docs.microsoft.com/es-es/dotnet/fundamentals/code-analysis/quality-rules/ca2227?view=vs-2019 y aunque esta propiedad pertenece a un DTO no es una propiedad que se lea desde el otro objeto (Venta, Cotización, etc) si no que se construye con la información leída de él.

        public string? CódigoDocumento { get; set; } // El número o código (cuando el número lleva prefijo) del documento. Puede ser nulo para documentos sin código. Se usa el nombre redundante []Documento para darle más claridad cuando se escriba en la plantilla CSHTML. 

        public string? NombreArchivoPropio { get; set; } // Si no se establece se obtiene automáticamente con el PrefijoNombreArchivo + CódigoDocumento.

        public string? Observación { get; set; }

        /// <summary>
        /// En el <see cref="Modo.CuerpoContinuo"/> se debe pasar a 
        /// <see cref="CompilarPlantilla{T}(RazorEngineCore.RazorEngine, string, IDictionary{string, string})"/> 
        /// un diccionario con como mínimo un marco. El documento tendrá un encabezado, un cuerpo que podrá ocupar varias páginas y finalmente habrá un pie de 
        /// página. Solo habrá un encabezado y un pie de página en todo el documento.<br/><br/>
        /// En el <see cref="Modo.PáginasIndependientes"/> se debe pasar al diccionario un marco y un ítem con nombre Página[Número] 
        /// por cada página extra que tenga el documento, excepto la Página1 porque en esta se agrega el cuerpo (VentaPdf.cshtml, CatálogoPdf.cshtml, etc).
        /// Cada página del documento tendrá su propio encabezado y pie de página.
        /// </summary>
        public Modo ModoDocumento { get; set; } = Modo.CuerpoContinuo;

        public int TotalPáginas { get; set; } = 1; // Para tener un mayor control del diseño del documento y los posibles modos siempre se debe pasar el número de páginas del documento. Esto por lo general es fácil de conocer dependiendo del contenido, en particular de la cantidad de LíneasGráficas[Entidad].

        public DateTime? FechaHora { get; set; }

        public DatosEmpresa? Empresa { get; set; } // Los datos de la empresa usuaria de SimpleOps. Estos datos normalmente se leen del de opciones Empresa.json.

        public DatosUsuario? Usuario { get; set; } // Los datos del usuario de SimpleOps que está realizando el documento. Este usuario se puede obtener del objeto Global.UsuarioActual o se puede construir desde datos pasados por un archivo de integración.

        public DatosCliente? Cliente { get; set; } // Puede ser nulo para documentos que no son de clientes.

        #endregion Propiedades Documento>



        #region Propiedades Diseño

        public int AnchoHoja {get; set;} = 816; // 96 * 8.5. Ver https://stackoverflow.com/a/40032997/8330412.

        public int AltoHoja { get; set; } = 1056; // 96 * 11. Ver https://stackoverflow.com/a/40032997/8330412.

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

        public string TextoPruebaColor { get; set; } = Global.TextoPruebas;

        public int RellenoSuperiorPie { get; set; } = 15;

        public int TamañoLetraPie { get; set; } = 14; // Solo puede ser entero y deben existir las clases t[tamañoLetraPie]b y t[tamañoLetraPie]n.

        public int LíneasTextoPie { get; set; } = 4; // Cantidad de divs dentro de div.pie. 
        
        public bool LíneaBajoCabeza { get; set; } = true;

        public int SeparaciónLíneaBajoCabeza { get; set; } = 5;

        public double AnchoLíneaBajoCabeza { get; set; } = 9.5;

        public bool ModoImpresión { get; set; } = false; // Un modo de diseño optimizado para la impresión que disminuye el uso de fondos de color.

        public bool MostrarInformaciónAdicional { get; set; } = false; // Permite la adición de una sección extra que no es visible en el modo normal. Sirve para agregar una sección de manera temporal o de manera selectiva según el destinatario del documento. El uso más común es la adición de información adicional legal a las facturas requerida por algunos clientes.

        #endregion Propiedades Diseño>



        #region Propiedades Autocalculadas

        public int AnchoContenido => AnchoHoja - 2 * MargenHorizontal;

        public double AltoPie => RellenoSuperiorPie + LíneasTextoPie * 1.25 * TamañoLetraPie;

        public string NombreArchivo => NombreArchivoPropio ?? $"{PrefijoNombreArchivo}{CódigoDocumento}";

        #endregion Propiedades Autocalculadas>



    } // DatosDocumento>



} // SimpleOps.DocumentosGráficos>

