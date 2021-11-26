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
using static Vixark.General;
using static SimpleOps.Global;
using SimpleOps.Modelo;
using System.Security.Cryptography;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.IO;
using System.Xml;
using System.Linq;



namespace SimpleOps.Legal {



    static class Dian {


        #region Enumeraciones
        // Enumeraciones que solo se usan en el contexto de esta clase Dian o de DocumentoElectrónico.

        public enum AgrupaciónTaxTotales { Línea, Tarifa };

        public enum Operación { 
            GetExchangeEmails, GetNumberingRange, GetStatus, GetStatusZip, GetXmlByDocumentKey, SendBillAsync, SendBillAttachmentAsync, SendBillSync, 
            SendEventUpdateStatus, SendTestSetAsync 
        }

        #endregion Enumeraciones>


        #region Constantes

        public const string BaseUrlPruebas = "vpfe-hab.dian.gov.co";

        public const string BaseUrlProducción = "vpfe.dian.gov.co";

        public const string AgenciaID195 = "195";

        public const string NombreAgenciaDian = "CO, DIAN (Dirección de Impuestos y Aduanas Nacionales)";

        public const string NitDian = "800197268";

        public const string DígitoVerificaciónNitDian = "4";

        public const string VersiónUBL = "UBL 2.1"; // Versión base de UBL usada para crear este perfil

        public const string CódigoFacturaEstándar = "10"; // Indicador del tipo de operación. Rechazo: Si contiene un valor distinto a los definidos en el grupo en el numeral 13.1.5.1: 10 Estandar (Valor predeterminado), 09 AIU, 11 Mandatos. 12 Transporte, 13 Cambiario. SimpleOps no implementa aún ninguno los tipos de operación diferente de estándar. 

        public const string CódigoNotaCréditoConFactura = "20";

        public const string CódigoNotaCréditoSinFactura = "22";

        public const string CódigoNotaDébitoConFactura = "30";

        public const string CódigoNotaDébitoSinFactura = "32";

        public const string CódigoDepartamentoNulo = "00";

        public const string AgenciaIdentificaciónPaís = "United Nations Economic Commission for Europe";

        public const string UriEsquemaIdentificaciónPaís = "urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.1";

        public const string AgenciaIdentificaciónPaísID = "6";

        public const string NombreYVersiónFacturaElectrónica = "DIAN 2.1: Factura Electrónica de Venta"; // Versión del Formato: Indicar versión del documento.

        public const string NombreYVersiónNotaCréditoElectrónica = "DIAN 2.1: Nota Crédito de Factura Electrónica de Venta"; 

        public const string NombreYVersiónNotaDébitoElectrónica = "DIAN 2.1: Nota Débito de Factura Electrónica de Venta"; 

        public const string AlgoritmoCufe = "CUFE-SHA384"; // Identificador del esquema de identificación. Algoritmo utilizado para el cáculo del CUFE. Ver lista de valores posibles en el numeral 13.1.2.1.

        public const string AlgoritmoCude = "CUDE-SHA384"; // Identificador del esquema de identificación. Algoritmo utilizado para el cáculo del CUDE. Ver lista de valores posibles en el numeral 13.1.2.2.

        public const string CódigoFacturaContingencia = "FTC"; // 13.1.4. Referencia a otros documentos. Otros: FTP Factura Talonario Papel, FTPC Factura Talonario Por computador.

        public const string CódigoRemisión = "AAJ"; // 13.1.4. Referencia a otros documentos.

        public const string NombreUsuarioFinal = "usuario";

        public const string ApellidoUsuarioFinal = "final";

        public const string CódigoMedioPagoPorDefinir = "ZZZ"; // Ver la tabla 13.3.4.2.

        public const string CódigoPrecioReferencia = "01"; // Ver la tabla 13.3.8.

        public const string CódigoEstándarAdopciónContribuyente = "999"; // Ver la tabla 6.3.5.


        public static Dictionary<string, string> CódigosDepartamentos = new Dictionary<string, string> { // Tomados del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN. Departamentos (ISO 3166-2:CO). Se manejan en un diccionario por facilidad y para no tener que crear una nueva tabla en la base de datos o agregarlo a cada municipio. Es inncesario para estos datos tan estáticos. Se agregan en minúscula para facilitar su uso al buscar un departamento que tenga cualquier capitalización.
            {"amazonas","91"},
            {"antioquia","05"},
            {"arauca","81"},
            {"atlántico","08"},
            {"bogotá","11"},
            {"bolívar","13"},
            {"boyacá","15"},
            {"caldas","17"},
            {"caquetá","18"},
            {"casanare","85"},
            {"cauca","19"},
            {"cesar","20"},
            {"chocó","27"},
            {"córdoba","23"},
            {"cundinamarca","25"},
            {"guainía","94"},
            {"guaviare","95"},
            {"huila","41"},
            {"la guajira","44"},
            {"magdalena","47"},
            {"meta","50"},
            {"nariño","52"},
            {"norte de santander","54"},
            {"putumayo","86"},
            {"quindío","63"},
            {"risaralda","66"},
            {"san andrés y providencia","88"},
            {"santander","68"},
            {"sucre","70"},
            {"tolima","73"},
            {"valle del cauca","76"},
            {"vaupés","97"},
            {"vichada","99"},
        };


        public static Dictionary<TipoContribuyente, string> CódigosTiposContribuyentes = new Dictionary<TipoContribuyente, string> { // Tomados de la tabla 13.2.6.1. del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN.
            {TipoContribuyente.Ordinario, "R-99-PN" }, // En agosto de 2021 se cambió por R-99-PN según la versión 1.8 de la facturación electrónica. Equivalente a Consumidor Final/No Aplica/Otros. Código aplicable para el elemento FAJ26. En agosto de 2020 se cambió por O-99 porque en el entorno de pruebas estaba sacando error con el anterior que era ZZ.
            {TipoContribuyente.GranContribuyente, "O-13" }, // Código aplicable para el elemento FAJ26.
            {TipoContribuyente.Autorretenedor, "O-15" }, // Código aplicable para el elemento FAJ26.
            {TipoContribuyente.RetenedorIVA, "O-23" }, // Código aplicable para el elemento FAJ26.
            {TipoContribuyente.RégimenSimple, "O-47" }, // Código aplicable para el elemento FAJ26.
            {TipoContribuyente.ResponsableIVA, "48" }, // En agosto de 2021 la DIAN eliminó la necesidad de la regla FAJ27, entonces este valor no sería necesario, pero se deja porque no sería extraño que lo volvieran a requerir. Código aplicable para el elemento FAJ27. Se obtiene de la tabla 16.1.6. Modificación del anexo técnico (06-09-2019).
            {TipoContribuyente.NoResponsableIVA, "49" }, // En agosto de 2021 la DIAN eliminó la necesidad de la regla FAJ27, entonces este valor no sería necesario, pero se deja porque no sería extraño que lo volvieran a requerir. Código aplicable para el elemento FAJ27. Se obtiene de la tabla 16.1.6. Modificación del anexo técnico (06-09-2019).
        };


        public static Dictionary<Unidad, string> CódigosUnidades = new Dictionary<Unidad, string> { // Tomados de la tabla 13.3.5.1. del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN.
            {Unidad.Unidad, "94" },
            {Unidad.Par, "PR" },
            {Unidad.Trío, "P3" }, // También QD: Cuarto de docena.
            {Unidad.Cuarteto, "P4" },
            {Unidad.Quinteto, "P5" },
            {Unidad.MediaCentena, "P6" }, // También HD: Media docena.
            {Unidad.Septeto, "P7" },
            {Unidad.Octeto, "P8" },
            {Unidad.Decena, "TP" }, // También TPR: Diez pares.
            {Unidad.Docena, "DZN" }, // También DPC: Docena pieza, DPR: Docena par, DZP: Paquete de doce.
            {Unidad.Veintena, "4E" },
            {Unidad.Centena, "CEN" }, // También CNP: Paquete de cien.
            {Unidad.Millar, "MIL" }, // También T3, T5: Caja de mil, T4: Bolsa de mil y T3: Mil piezas.
            {Unidad.Millón, "MIO" },
            {Unidad.Millardo, "MLD" }, // Mil millones.
        }; // No agregadas: P9: Paquete de nueve, EP: Paquete de once.


        public static Dictionary<TipoImpuesto, string> CódigosTiposImpuestos = new Dictionary<TipoImpuesto, string> { // Tomados de la tabla 13.2.6.2. del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN.
            {TipoImpuesto.IVA, "01" },
            {TipoImpuesto.INC, "04" },
            {TipoImpuesto.IVAeINC, "ZA" },
            {TipoImpuesto.NoAplica, "ZZ" },
        };


        #endregion Constantes>


        #region Variables Autocalculadas

        public static string BaseUrl 
            => Empresa.AmbienteFacturaciónElectrónica == AmbienteFacturaciónElectrónica.Producción ? BaseUrlProducción : BaseUrlPruebas;

        #endregion Variables Autocalculadas>


        #region Métodos y Funciones

        /// <summary>
        /// Redondeo del banquero requerido por la DIAN.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Redondear(decimal value) => decimal.Round(value, MidpointRounding.ToEven); // https://stackoverflow.com/questions/311696/why-does-net-use-bankers-rounding-as-default

        public static string ObtenerRutaQR(string Cude) => $"https://catalogo-{BaseUrl}/document/searchqr?documentkey={Cude}";


        public static string ObtenerCódigoTipoImpuesto(TipoImpuesto tipoImpuesto, bool forzar01 = false) {

            var código = CódigosTiposImpuestos.ObtenerValorObjeto(tipoImpuesto) ?? "ZZ";
            if (código == "ZA" && forzar01) código = "01";
            if (código == "ZZ" && forzar01) código = "01";
            return código;

        } // ObtenerCódigoTipoImpuesto>


        public static string ObtenerNombreImpuesto(TipoImpuesto tipoImpuesto, bool forzarIVA = false) {

            var nombre = tipoImpuesto.ATexto();
            if (nombre == "IVA e INC" && forzarIVA) nombre = "IVA";
            if (nombre == "No aplica" && forzarIVA) nombre = "IVA";
            return nombre;

        } // ObtenerNombreImpuesto>


        /// <summary>
        /// Solo funciona en el ambiente de producción.
        /// </summary>
        /// <returns></returns>
        public static string? ObtenerClaveTécnicaAmbienteProducción() {
    
            string? claveTécnica = null;

            var mensajeInicial = $"Error en obtención de la clave técnica de facturación electrónica.{DobleLínea}";
            if (Empresa.AmbienteFacturaciónElectrónica != AmbienteFacturaciónElectrónica.Producción) {
                MostrarError($"{mensajeInicial}La clave técnica solo se puede obtener en el ambiente de producción.");
            } else {

                if (!EnviarSolicitud($"<wcf:GetNumberingRange>" +
                                         $"<wcf:accountCode>{Empresa.Nit}</wcf:accountCode>" +
                                         $"<wcf:accountCodeT>{Empresa.Nit}</wcf:accountCodeT>" +
                                         $"<wcf:softwareCode>{Empresa.IdentificadorAplicación}</wcf:softwareCode>" +
                                     $"</wcf:GetNumberingRange>", Operación.GetNumberingRange, out string? mensajeEnvío, out XmlDocument? respuestaXml)) {

                    MostrarError(mensajeInicial + mensajeEnvío);

                } else {

                    var nodoGetNumberingRangeResult = respuestaXml?["s:Envelope"]?["s:Body"]?["GetNumberingRangeResponse"]?["GetNumberingRangeResult"];
                    if (nodoGetNumberingRangeResult?["b:OperationCode"]?.InnerText == "100") {
                        claveTécnica = nodoGetNumberingRangeResult?["b:ResponseList"]?["c:NumberRangeResponse"]?["c:TechnicalKey"].InnerText;
                    } else {
                        MostrarError(mensajeInicial + nodoGetNumberingRangeResult?["b:OperationDescription"]?.InnerText);
                    }

                }

            }

            return claveTécnica;

        } // ObtenerClaveTécnicaAmbienteProducción>


        public static string ObtenerCódigoTipoTributo(TipoTributo tipoTributo) => tipoTributo == TipoTributo.Otro ? "ZZ" : tipoTributo.AValor(2);

        public static string ObtenerCódigoUnidad(Unidad unidad) => CódigosUnidades.ObtenerValorObjeto(unidad) ?? "PA"; // Ver la documentación de la DIAN. También se podría usar PK: Paquete, BX: Caja, CA: Caja o CR: Caja.

        public static string FechaATexto(DateTime fecha) => fecha.ATexto(FormatoFecha);

        public static string IntervaloATexto(DateTime fecha1, DateTime fecha2) => $"{FechaATexto(fecha1)}/{FechaATexto(fecha2)}";

        public static string ObtenerCódigoDepartamento(string nombreDepartamento) 
            => CódigosDepartamentos.ObtenerValorObjeto(nombreDepartamento.AMinúscula()!) ?? CódigoDepartamentoNulo; // Como nombreDepartamento no es nulo se asegura que nombreDepartamento.AMinúscula() tampoco lo es.


        public static string ObtenerCódigoTipoOperación(Venta? venta, NotaCréditoVenta? notaCrédito, NotaDébitoVenta? notaDébito, Venta? ventaNota) 
            => venta != null ? CódigoFacturaEstándar :
                   (notaCrédito != null ? (ventaNota != null && ventaNota.Cude != null ? CódigoNotaCréditoConFactura : CódigoNotaCréditoSinFactura) :
                        (notaDébito != null ? (ventaNota != null && ventaNota.Cude != null ? CódigoNotaDébitoConFactura : CódigoNotaDébitoSinFactura) : 
                            throw new ArgumentNullException($"{nameof(venta)},{nameof(notaCrédito)},{nameof(notaDébito)},{nameof(ventaNota)}", 
                                "Todos los documentos son nulos")));


        /// <summary>
        /// Valida un elemento de acuerdo a las restricciones de la 'Tabla 4: Tamaños de Elementos' del archivo 'Anexo Técnico de Factura Electrónica'.
        /// </summary>
        /// <param name="elemento">Texto del elemento a validar puede ser un texto o un número con punto como separador decimal. En general pueden ser varios elementos separados por coma.</param>
        /// <param name="restricción">De formato: (x1..y1) p (n1..m1), (x2..y2) p (n2..m2), ..., (xn..yn) p (nn..mn) donde se permiten n elementos
        /// separados por coma (o un solo elemento si no hay coma), donde (xi..yi) p (ni..mi) indica un elemento de mínimo xi de largo total y 
        /// máximo yi de largo total (incluyendo el punto de decimal y la parte decimal) y que es un decimal con mínimo ni posiciones decimales 
        /// y máximo mi posiciones decimales. Si se omite p (...) aplica para solo textos y números enteros. Si se omiten los dos puntos, así: 
        /// zi p li implica que el elemento debe ser exactamente zi de largo y tener li posiciones decimales si es un número.</param>
        /// <param name="largoMáximo">Solo válido para los que no tienen decimales ni comas.</param>
        /// <param name="largoMínimo">Solo válido para los que no tienen decimales ni comas.</param>
        /// <returns></returns>
        public static bool EsVálido(string? elemento, string? restricción, out int largoMáximo, out int largoMínimo) {

            largoMáximo = 0;
            largoMínimo = 0;

            if (string.IsNullOrEmpty(restricción)) return false;
            var restricciones = restricción.Split(",");
            string[] elementos;
            if (restricciones.Length > 1) {
                elementos = elemento == null ? new string[1] { "" } : elemento.Split(",");
                if (restricciones.Length != elementos.Length) return false; // Si la restricción tiene comas el elemento necesariamente tiene que tener la misma cantidad de comas y los elementos individuales no podrían llevar comas propias porque se confundiría el algorítmo.
            } else { // Si la restricción no tiene comas se permiten normalmente comas dentro del elemento.
                elementos = elemento == null ? new string[1] { "" } : new string[1] { elemento };
            }

            if (restricciones.Length == 1) {

                var restricción0 = restricciones[0];
                var elemento0 = elementos[0];

                if (restricción0.NoContiene(" p ")) {

                    if (elemento0.Contiene(".") && Coincide(elemento0, PatrónNúmeroPuntoDecimal)) return false; // Si el elemento si tiene punto y además coincide con patrón de únicamente números, asumirá que es un decimal y por lo tanto es incorrecto porque la restricción no especifica ningún decimal.         
                    if (restricción0.Contiene("..")) {

                        var partesRestricción = restricción0.Split("..");
                        largoMínimo = partesRestricción[0].AEntero();
                        largoMáximo = partesRestricción[1].AEntero();
                        if (elemento0.Length < largoMínimo || elemento0.Length > largoMáximo) return false;

                    } else {
                        if (elemento0.Length != restricción0.AEntero()) return false;
                    }

                } else {
     
                    var partesRestricción = restricción0.Split(" p ");
                    var partesElemento = elemento0.Split(".");
                    if (elemento0.NoContiene(".")) { // Analiza el caso especial de decimales opcionales.

                        if (partesRestricción[1].EmpiezaPor("0") || partesRestricción[1].EmpiezaPor("(0")) {
                            partesElemento = new string[] { elemento0, "" }; // Si la segunda parte de la restricción empieza por 0, puede ser válido que el elemento no tenga punto entonces se crea un vector con el primer elemento el elemento en sí y el segundo elemento (que correspondería a la parte decimal) en vacío.
                        } else {
                            return false; // Si la segunda parte de la restricción no empieza por 0, es inválido que el elemento no tenga punto.
                        }

                    }
                   
                    if (!EsVálido(elemento0.Reemplazar(".", "·"), partesRestricción[0].Reemplazar("(", "").Reemplazar(")", ""), out _, out _) // Se reemplaza el . por · para que no sea considerado como decimal si no que se compare como un texto cualquiera.
                        || !EsVálido(partesElemento[1], partesRestricción[1].Reemplazar("(", "").Reemplazar(")", ""), out _, out _)) { 
                        return false;
                    }

                }

            } else {

                for (int i = 0; i < restricciones.Length; i++) {
                    if (!EsVálido(elementos[i], restricciones[i], out _, out _)) return false;
                }

            }

            return true;

        } // EsVálido>


        /// <summary>
        /// Revisa el elemento de texto y lo devuelve igual si cumple con la restricción. Genera una excepción si no cumple la restricción.
        /// Si <paramref name="forzarCumplimiento"/> es verdadero y se trata de una restricción de largo de texto sencilla X..Y 
        /// se recortan los elementos muy largos al tamaño máximo permitido y se agregan espacios al final de los elementos muy cortos
        /// para llevarlos al tamaño mínimo permitido.
        /// </summary>
        public static string? Validar(string? texto, string restricción, bool forzarCumplimiento = false) {

            var válido = EsVálido(texto, restricción, out int largoMáximo, out int largoMínimo);
            if (!válido && forzarCumplimiento) {

                if (texto == null) texto = "";
                if (texto.Length < largoMínimo) texto = texto.PadRight(largoMínimo);
                if (texto.Length > largoMáximo) texto = texto.Substring(0, largoMáximo);
                válido = EsVálido(texto, restricción, out _, out _); // Si no se pudo corregir con las restricciones es porque posiblemente la restricción está mal formada, debe lanzar excepción.

            }

            if (!válido) throw new ArgumentException($"El elemento {texto} no cumple con la restricción {restricción}.");
            return texto;

        } // Validar>


        /// <summary>
        /// Revisa el elemento decimal y lo devuelve igual si cumple con la restricción. Genera una excepción si no cumple la restricción.
        /// </summary>
        public static decimal Validar(decimal número, string restricción, int posicionesDecimalesForzadas) {

            var númeroTexto = número.ATexto($"0.{new string('0', posicionesDecimalesForzadas)}");
            var númeroDecimalesForzados = númeroTexto.ADecimal(); // El mismo número decimal pero con las posiciones decimales forzadas.
            if (!EsVálido(númeroDecimalesForzados.ATexto(), restricción, out _, out _)) 
                throw new ArgumentException($"El elemento {número} no cumple con la restricción {restricción}.");
            return númeroDecimalesForzados;

        } // Validar>

        
        /// <summary>
        /// Revisa el elemento long y lo devuelve igual si cumple con la restricción. Genera una excepción si no cumple la restricción.
        /// </summary>
        /// <param name="número"></param>
        /// <param name="restricción"></param>
        /// <returns></returns>
        public static long Validar(long número, string restricción) {

            if (!EsVálido(número.ATexto(), restricción, out _, out _)) 
                throw new ArgumentException($"El elemento {número} no cumple con la restricción {restricción}.");
            return número;

        } // Validar>


        public static coID2TypeSchemeID ObtenerCoID2TypeSchemeID(string dígitoVerificaciónNit) 
            => dígitoVerificaciónNit switch {
                "0" => coID2TypeSchemeID.Item11,
                "1" => coID2TypeSchemeID.Item12,
                "2" => coID2TypeSchemeID.Item13,
                "3" => coID2TypeSchemeID.Item21,
                "4" => coID2TypeSchemeID.Item22,
                "5" => coID2TypeSchemeID.Item31,
                "6" => coID2TypeSchemeID.Item32,
                "7" => coID2TypeSchemeID.Item41,
                "8" => coID2TypeSchemeID.Item42,
                "9" => coID2TypeSchemeID.Item50,
                "10" => coID2TypeSchemeID.Item91,
                _ => throw new ArgumentException(CasoNoConsiderado(dígitoVerificaciónNit)),
            };

 
        public static string ObtenerResponsabilidadFiscal(TipoContribuyente tipoContribuyente) {

            var respuesta = "";
            var tiposContribuyentes = ObtenerValores<TipoContribuyente>().ToList();
            var alMenosUnoNoOrdinario = false;
            foreach (var tipoContribuyenteÚnico in tiposContribuyentes) { 

                if (tipoContribuyenteÚnico == TipoContribuyente.ResponsableIVA) continue; 
                if (tipoContribuyenteÚnico == TipoContribuyente.NoResponsableIVA) continue;
                if (tipoContribuyente.HasFlag(tipoContribuyenteÚnico)) {
                    if (tipoContribuyenteÚnico != TipoContribuyente.Ordinario) alMenosUnoNoOrdinario = true;
                    respuesta += $"{CódigosTiposContribuyentes.ObtenerValorObjeto(tipoContribuyenteÚnico)};";
                }

            }
            if (alMenosUnoNoOrdinario) respuesta = respuesta.Reemplazar(CódigosTiposContribuyentes[TipoContribuyente.Ordinario], ""); // Si tiene algún tipo de contribuyente que no sea ordinario, no se agrega el código de contribuyente ordinario.
            return respuesta.Reemplazar(";;",";").Trim(';');

        } // ObtenerResponsabilidadFiscal>


        public static string ObtenerResponsabilidadIVA(TipoContribuyente tipoContribuyente) {

            if (tipoContribuyente.HasFlag(TipoContribuyente.ResponsableIVA)) {
                return CódigosTiposContribuyentes.ObtenerValorObjeto(TipoContribuyente.ResponsableIVA)!; // Se asegura que el valor ResponsableIVA si existe en el diccionario.
            } else if (tipoContribuyente.HasFlag(TipoContribuyente.NoResponsableIVA)) {
                return CódigosTiposContribuyentes.ObtenerValorObjeto(TipoContribuyente.NoResponsableIVA)!; // Se asegura que el valor NoResponsableIVA si existe en el diccionario.
            } else {
                throw new ArgumentException("No se esperaba que el tipo de contribuyente no especificara si es o no responsable de IVA.");
            }

        } // ObtenerResponsabilidadIVA>


        /// <summary>
        /// Se conecta al servicio web de la DIAN y envía un sobre XML con el contenido de la solicitud y la firma electrónica.
        /// </summary>
        public static bool EnviarSobre(string sobre, Operación operación, out string? mensaje, out XmlDocument? respuestaXml) {

            mensaje = null;
            respuestaXml = null;

            try {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // Obliga a usar TLS 1.2 en las conexiones seguras.
                var solicitud = (HttpWebRequest)WebRequest.Create(new Uri($"https://{BaseUrl}/WcfDianCustomerServices.svc"));
                solicitud.Method = "POST";
                solicitud.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                solicitud.ContentType = $@"application/soap+xml;charset=UTF-8;action=""http://wcf.dian.colombia/IWcfDianCustomerServices/{operación}""";
                solicitud.ContentLength = sobre.Length;
                solicitud.KeepAlive = true;
                solicitud.Host = BaseUrl;
                solicitud.UserAgent = WebAplicación;
                using (var flujo = solicitud.GetRequestStream()) {
                    var bytes = Encoding.UTF8.GetBytes(sobre);
                    flujo.Write(bytes, 0, bytes.Length);
                }

                respuestaXml = ObtenerXml((HttpWebResponse)solicitud.GetResponse());

            } catch (WebException excepciónWeb) {

                if (excepciónWeb.Status == WebExceptionStatus.NameResolutionFailure) 
                    return Falso(out mensaje, $"No se pudo conectar al servidor de la DIAN. Verifica tu conexión de internet." +
                                              $"{DobleLínea}WebExceptionStatus.NameResolutionFailure.");
                if (!(excepciónWeb.Response is HttpWebResponse respuesta)) throw; // Error aún desconocido.

                string? mensajeRespuesta = "";
                if (respuesta.StatusCode == HttpStatusCode.InternalServerError) {

                    respuestaXml = ObtenerXml(respuesta);
                    if (respuestaXml == null) return Falso(out mensaje, $"No se esperaba respuestaXml nulo.{DobleLínea}{excepciónWeb.Message}");
                    mensajeRespuesta = respuestaXml.DocumentElement?["s:Body"]?["s:Fault"]?["s:Code"]?["s:Subcode"]?["s:Value"]?.InnerText;
                    if (mensajeRespuesta == null) mensajeRespuesta = respuestaXml.DocumentElement?["s:Body"]?["s:Fault"]?["s:Reason"].InnerText; // Aplica para el mensaje "Certificado aún no se encuentra vigente.".

                } else {
                    mensajeRespuesta = ""; // En otros casos hasta que no se verifique que no devuelvan XML se asumirá que no lo hacen.
                }

                return mensajeRespuesta switch {
                    "a:InvalidSecurity" => Falso(out mensaje, "Sucedió un error de verificación de seguridad en el servidor de la DIAN. " +
                                                             $"Usualmente esto indica un problema con la firma electrónica, con las horas o un mensaje " +
                                                             $"XML mal formado.{DobleLínea}{excepciónWeb.Message}{DobleLínea}Estado: {respuesta.StatusCode}."),
                    "a:InternalServiceFault" => Falso(out mensaje, $"Error interno de servicio del servidor de la DIAN. Puede suceder cuando se especifica " +
                                                                   $"una operación diferente en el XML y en el encabezado de la solicitud POST.{DobleLínea}" + 
                                                                   $"{excepciónWeb.Message}{DobleLínea}Estado: {respuesta.StatusCode}."),
                    "Certificado aún no se encuentra vigente." => Falso(out mensaje, $"El certificado de facturación electrónica aún no se encuentra vigente."),
                    _ => Falso(out mensaje, $"Sucedió un error desconocido en el servidor de la DIAN.{DobleLínea}{excepciónWeb.Message}{DobleLínea}" +
                                            $"Estado: {respuesta.StatusCode}."),
                };

            } catch (Exception ex) {
                return Falso(out mensaje, $"Sucedió un error desconocido.{DobleLínea}{ex.Message}"); // Antes se estaba suprimiendo la alerta CA1031. No capture tipos de excepción generales. Se desactiva porque se necesita el texto de la excepción. No hay mayor problema porque es común que sucedan errores con el servicio web de la DIAN y esto es controlado con el flujo del programa a continuación.
            }

            return true;

        } // EnviarSobre>


        /// <summary>
        /// Envía una solicitud al servicio de la DIAN usando un <paramref name="cuerpo"/> con el contenido del mensaje y 
        /// el tipo de <paramref name="operación"/>.
        /// </summary>
        public static bool EnviarSolicitud(string cuerpo, Operación operación, out string? mensaje, out XmlDocument? respuestaXml) {

            mensaje = null;
            string? sobreFirmado = null;
            respuestaXml = null;
            if (!ExisteRuta(TipoElementoRuta.Archivo, Equipo.RutaCertificado, "certificado de firma digital", out string? mensajeExiste)) 
                return Falso(out mensaje, mensajeExiste);

            try {

                using var certificado = new X509Certificate2(Equipo.RutaCertificado, Equipo.ClaveCertificado);
                if (certificado.NotAfter < AhoraUtcAjustado)
                        return Falso(out mensaje, $"El certificado electrónico se venció en {certificado.NotAfter.ATexto(FormatoFecha)}.");
                if (certificado.NotBefore > AhoraUtcAjustado) 
                    return Falso(out mensaje, $"El certificado electrónico aún no es válido. Es válido desde {certificado.NotBefore.ATexto(FormatoFecha)}.");

                var claveTitularID = ((X509SubjectKeyIdentifierExtension)certificado.Extensions.Cast<X509Extension>()
                    .Where(e => e is X509SubjectKeyIdentifierExtension).Single()).SubjectKeyIdentifier; // Obtiene el SubjectKeyIdentifier del certificado según requerido por la documentación Oasis WSS X509 Token Profile 1.1. En realidad a la DIAN no le importa este valor mientras sea consistente en todo el documento, pero se prefiere hacer según el estándar Oasis.
                var tokenBinarioDeSeguridad = Convert.ToBase64String(certificado.Export(X509ContentType.Cert, Equipo.ClaveCertificado)); // Según https://stackoverflow.com/questions/32404687/c-sharp-add-wssesecurity-and-binarysecuritytoken-to-envelope-xml-file-programma.

                var wsaTo = @"<wsa:To xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" " +
                            $@"wsu:Id=""ID-{claveTitularID}"">https://{BaseUrl}/WcfDianCustomerServices.svc</wsa:To>"; // Es el único elemento que se firma.
                var wsaToDigerido = ObtenerValorDigerido(wsaTo, certificado, claveTitularID);

                var signedInfo =
                    @"<ds:SignedInfo xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" " +
                    @"xmlns:wcf=""http://wcf.dian.colombia"" xmlns:wsa=""http://www.w3.org/2005/08/addressing"">" +
                        @"<ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"">" +
                            @"<ec:InclusiveNamespaces xmlns:ec=""http://www.w3.org/2001/10/xml-exc-c14n#"" PrefixList=""wsa soap wcf"">" +
                            "</ec:InclusiveNamespaces>" +
                        "</ds:CanonicalizationMethod>" +
                        @"<ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256""></ds:SignatureMethod>" +
                        $@"<ds:Reference URI=""#ID-{claveTitularID}"">" +
                            "<ds:Transforms>" +
                                @"<ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"">" +
                                    @"<ec:InclusiveNamespaces xmlns:ec=""http://www.w3.org/2001/10/xml-exc-c14n#"" PrefixList=""soap wcf"">" +
                                    "</ec:InclusiveNamespaces>" +
                                "</ds:Transform>" +
                            "</ds:Transforms>" +
                            @"<ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""></ds:DigestMethod>" +
                            $@"<ds:DigestValue>{wsaToDigerido}</ds:DigestValue>" +
                        "</ds:Reference>" +
                    "</ds:SignedInfo>"; // En el XML sugerido por la DIAN (desde SoapUI) debería tener el elemento ds:Signed sin espacios de nombres (<ds:SignedInfo>), pero se encontró que así la firma resulta incorrecta y el servidor la rechaza. Se usa entonces con todos los espacios de nombres completos aunque esto pueda ser una infracción al estándar del XML Oasis porque estos espacios de nombres también se agregan al soap:Evelope, pero se acepta porque lo importante es que es aprobado por la DIAN. En el XML original los elementos ec:InclusiveNamespaces, ds:SignatureMethod, ec:InclusiveNamespaces y ds:DigestMethod se usan con autocierre (<.../>) pero para la firma se requieren como elementos completos (<...></...>). Si fuera necesario usar una versión de este elemento para calcular la firma y otra para agregar en el sobre firmado, se puede hacer.

                var utf8 = new UTF8Encoding();
                var firma = Convert.ToBase64String(((RSACng)certificado.PrivateKey).SignData(utf8.GetBytes(signedInfo), HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1));

                sobreFirmado =
                    @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:wcf=""http://wcf.dian.colombia"">" +
                        @"<soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">" +
                            @"<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" " +
                             @"xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">" +
                               $@"<wsu:Timestamp wsu:Id=""TS-{claveTitularID}"">" +
                                    $"<wsu:Created>{DateTime.UtcNow.ATexto("yyyy-MM-ddTHH:mm:ssZ")}</wsu:Created>" +
                                    $"<wsu:Expires>{DateTime.UtcNow.AddSeconds(60000).ATexto("yyyy-MM-ddTHH:mm:ssZ")}</wsu:Expires>" +
                                @"</wsu:Timestamp>" +
                                 "<wsse:BinarySecurityToken " +
                                 @"EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"" " +
                                 @"ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" " +
                                $@"wsu:Id=""X509-{claveTitularID}"">" +
                                      tokenBinarioDeSeguridad +
                                 "</wsse:BinarySecurityToken>" +
                                @"<ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" Id=""SIG-{claveTitularID}"">" +
                                      signedInfo +
                                    $"<ds:SignatureValue>{firma}</ds:SignatureValue>" +
                                   $@"<ds:KeyInfo Id=""KI-{claveTitularID}"">" +
                                       $@"<wsse:SecurityTokenReference wsu:Id=""STR-{claveTitularID}"">" +
                                       $@"<wsse:Reference URI=""#X509-{claveTitularID}"" " +
                                         @"ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3""/>" +
                                         "</wsse:SecurityTokenReference>" +
                                     "</ds:KeyInfo>" +
                                 "</ds:Signature>" +
                            "</wsse:Security>" +
                           $"<wsa:Action>http://wcf.dian.colombia/IWcfDianCustomerServices/{operación}</wsa:Action>" +
                             wsaTo +
                         "</soap:Header>" +
                        $"<soap:Body>{cuerpo}</soap:Body>" +
                    "</soap:Envelope>";

            } catch (CryptographicException) {
                return Falso(out mensaje, "Ocurrió un error criptográfico. La clave del certificado puede ser incorrecta o cambiaste el certificado sin reiniciar SimpleOps.");
            } catch (Exception) {
                throw;
            }

            return EnviarSobre(sobreFirmado, operación, out mensaje, out respuestaXml);

        } // EnviarSolicitud>


        /// <summary>
        /// Usa las clases de firma de xml propias de .Net para obtener el valor digerido del objeto a firmar.
        /// </summary>
        private static string ObtenerValorDigerido(string wsaTo, X509Certificate2 certificado, string id) { // No se usa las clases de .Net para obtener el XML firmado definitivo porque existen muchas inconsistencias entre el XML generado por estas y el requerido por el estándar Oasis. Aquí solo se obtiene el valor digerido que se usa para calcular la firma e insertarlos en el XML definitivo.

            var xml = new XmlDocument();
            xml.LoadXml(@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:wcf=""http://wcf.dian.colombia"">" +
                            $@"<soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">{wsaTo}</soap:Header>" +
                        @"</soap:Envelope>");

            var referencia = new Reference { Uri = $"#ID-{id}" };
            var transformaciónC14N = new XmlDsigExcC14NTransform(includeComments: false, inclusiveNamespacesPrefixList: "wsa soap wcf");
            referencia.AddTransform(transformaciónC14N);

            var xmlFirmado = new XmlFirmadoConWsuID(xml) { SigningKey = certificado.PrivateKey };
            xmlFirmado.AddReference(referencia);

            var informaciónClave = new KeyInfo();
            informaciónClave.AddClause(new KeyInfoX509Data(certificado));
            xmlFirmado.KeyInfo = informaciónClave;

            xmlFirmado.ComputeSignature();

            return xmlFirmado.GetXml()["SignedInfo"]["Reference"]["DigestValue"].InnerText;

        } // ObtenerValorDigerido>


        /// <summary>
        /// Crea, valida, envía a la DIAN el documento electrónico (XML).
        /// electrónico (XML) y su representación gráfica (PDF) al cliente.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="documentoCliente"></param>
        /// <param name="mensaje"></param>
        /// <param name="pruebaHabilitación"></param>
        /// <param name="documentoElectrónico"></param>
        /// <returns></returns>
        public static bool CrearYEnviarDocumentoElectrónico<M>(Factura<Cliente, M> documentoCliente, out string? mensaje,
            out DocumentoElectrónico<Factura<Cliente, M>, M>? documentoElectrónico, bool pruebaHabilitación = false) where M : MovimientoProducto {

            mensaje = null;
            documentoElectrónico = null;

            if (!documentoCliente.CalcularTodo()) 
                return Falso(out mensaje, $"Error calculando documento de cliente (factura, nota crédito o nota débito).{DobleLínea}Error en CalcularTodo().");

            documentoElectrónico = new DocumentoElectrónico<Factura<Cliente, M>, M>(documentoCliente, TipoFacturaVenta.Venta);

            if (!documentoElectrónico.Crear(out string? mensajeCreación)) 
                return Falso(out mensaje, $"Error creando documento electrónico.{DobleLínea}{mensajeCreación}");

            XmlDocument? respuestaXml;
            if (pruebaHabilitación) {
                if (!documentoElectrónico.EnviarPrueba(out string? mensajeSolicitud, out respuestaXml)) 
                    return Falso(out mensaje, $"Error enviando prueba a la DIAN.{DobleLínea}{mensajeSolicitud}");
            } else {
                if (!documentoElectrónico.Enviar(out string? mensajeSolicitud, out respuestaXml))
                    return Falso(out mensaje, $"Error enviando a la DIAN.{DobleLínea}{mensajeSolicitud}");
            }
            if (respuestaXml == null) return Falso(out mensaje, $"Error en respuesta de la DIAN.{DobleLínea}El XML está vacío.");
            var respuestaDian = new RespuestaDian(respuestaXml, pruebaHabilitación ? Operación.SendTestSetAsync : Operación.SendBillSync);
            if (!respuestaDian.Éxito) return Falso(out mensaje, $"Error en respuesta de la DIAN.{DobleLínea}{respuestaDian.MensajeError}");
            documentoElectrónico.RespuestaDian = respuestaDian;

            return true;

        } // CrearYEnviarDocumentoElectrónico>


        public static bool CrearRespuestaElectrónica<M>(out string? mensaje, DocumentoElectrónico<Factura<Cliente, M>, M> documentoElectrónico, 
            out string? rutaXml) 
            where M : MovimientoProducto {

            var respuestaElectrónica = new RespuestaElectrónica<Factura<Cliente, M>, M>(documentoElectrónico);  
            var resultado = respuestaElectrónica.Crear(out mensaje);
            rutaXml = respuestaElectrónica.Ruta;
            return resultado;

        } // CrearRespuestaElectrónica>


        #endregion Métodos y Funciones>


    } // Dian>



} // SimpleOps.Legal>
