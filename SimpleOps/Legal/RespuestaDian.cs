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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using static Vixark.General;
using static SimpleOps.Global;
using static SimpleOps.Legal.Dian;



namespace SimpleOps.Legal {



    class RespuestaDian {


        #region Propiedades

        public DateTime Fecha { get; set; }

        public bool Éxito { get; set; }

        public string? MensajeError { get; set; }

        public List<ReglaDian> Errores { get; set; } = new List<ReglaDian>();

        public int? CódigoEstado { get; set; }

        public string? DescripciónEstado { get; set; }

        public string? MensajeEstado { get; set; }

        public string? ClaveDocumento { get; set; }

        public string? NombreArchivo { get; set; }

        public string? ClaveSeguimiento { get; set; }

        public string? RespuestaAplicación { get; set; } // ApplicationResponse.

        public string? CódigoRespuesta { get; set; } // ResponseCode y ValidationResultCode.

        public DateTime? FechaRespuesta { get; set; } // IssueDate y ValidationDate.

        public DateTime? HoraRespuesta { get; set; } // IssueTime y ValidationTime.

        #endregion Propiedades>


        #region Métodos y Funciones


        public RespuestaDian(XmlDocument respuestaXml, Operación operación) {

            var fecha = 
                respuestaXml.DocumentElement["s:Header"]?["o:Security"]?["u:Timestamp"]?["u:Created"]?.InnerText.AFecha(FormatoFechaHoraTZyMilisegundos);
            if (fecha == null) { Error("La fecha está vacía."); return; }
            Fecha = (DateTime)fecha;

            var cuerpo = respuestaXml.DocumentElement["s:Body"];
            if (cuerpo == null) { Error("No se encontró s:Body."); return; }

            var respuesta = cuerpo[$"{operación}Response"];
            if (respuesta == null) { Error($"No se encontró {operación}Response."); return; }

            var resultado = respuesta[$"{operación}Result"];
            if (resultado == null) { Error($"No se encontró {operación}Result."); return; }

            var válida = false;

            if (operación == Operación.SendBillSync) {

                var errores = resultado["b:ErrorMessage"];
                if (errores != null) {

                    foreach (var error in errores.ChildNodes) {

                        if (!(error is XmlNode nodoError)) { Error("Un nodo no es XmlNode."); return; }

                        var patrón = @"Regla: (.+?), (\bRechazo\b|\bNotificación\b): (.+)";
                        var textoError = nodoError.InnerText;

                        var código = ExtraerConPatrónObsoleta(textoError, patrón, 1, out int coincidenciasCódigo);
                        if (coincidenciasCódigo == 0) { Error($"No se encontraron coincidencias para el código en {textoError}."); return; }

                        var textoTipo = ExtraerConPatrónObsoleta(textoError, patrón, 2, out int coincidenciasTipo);
                        if (coincidenciasTipo == 0) { Error($"No se encontraron coincidencias para el tipo en en {textoError}."); return; }

                        var mensaje = ExtraerConPatrónObsoleta(textoError, patrón, 3, out int coincidenciasMensaje);
                        if (coincidenciasMensaje == 0) { Error($"No se encontraron coincidencias para el mensaje en en {textoError}."); return; }

                        Errores.Add(new ReglaDian(código, textoTipo.AEnumeración<TipoReglaDian>(), mensaje));

                    }

                }

                var isValid = resultado["b:IsValid"]?.InnerText;
                var statusCode = resultado["b:StatusCode"]?.InnerText;
                var statusDescription = resultado["b:StatusDescription"]?.InnerText;
                var statusMessage = resultado["b:StatusMessage"]?.InnerText;
                var xmlDocumentKey = resultado["b:XmlDocumentKey"]?.InnerText;
                var xmlFileName = resultado["b:XmlFileName"]?.InnerText;

                if (isValid == null || statusCode == null || statusDescription == null || statusMessage == null || xmlDocumentKey == null || xmlFileName == null) {
                    Error("No se encontró IsValid, StatusCode, StatusDescription, StatusMessage, XmlDocumentKey o XmlFileName.");
                    return;
                }

                CódigoEstado = statusCode == null ? (int?)null : statusCode.AEntero();
                DescripciónEstado = statusDescription;
                MensajeEstado = statusMessage;
                ClaveDocumento = xmlDocumentKey;
                NombreArchivo = xmlFileName;
                válida = isValid == "true";
                RespuestaAplicación = Base64ATexto(resultado["b:XmlBase64Bytes"]?.InnerText);
                var xmlRespuestaAplicación = new XmlDocument();
                xmlRespuestaAplicación.LoadXml(RespuestaAplicación);
                CódigoRespuesta = xmlRespuestaAplicación.DocumentElement["cac:DocumentResponse"]?["cac:Response"]?["cbc:ResponseCode"]?.InnerText;
                var fechaRespuestaStr = xmlRespuestaAplicación.DocumentElement["cbc:IssueDate"]?.InnerText;
                FechaRespuesta = fechaRespuestaStr == null ? (DateTime?)null : DateTime.Parse(fechaRespuestaStr);
                var horaRespuestaStr = xmlRespuestaAplicación.DocumentElement["cbc:IssueTime"]?.InnerText;
                HoraRespuesta = horaRespuestaStr == null ? (DateTime?)null : DateTime.Parse(horaRespuestaStr);

            }

            if (operación == Operación.SendTestSetAsync) {

                válida = true; // Como es un método Async los errores no los notifica en la respuesta, entonces si contiene resultado (SendTestSetAsyncResult) se considerará válida.
                var zipKey = resultado["b:ZipKey"]?.InnerText;
                if (zipKey == null) {
                    Error("No se encontró zipKey.");
                    return;
                }
                ClaveSeguimiento = zipKey;

            }

            if (Errores.Any(e => e.Tipo == TipoReglaDian.Rechazo)) { Error("Al menos un error es de rechazo."); return; }
            if (!válida) { Error("La solicitud no fue válida."); return; }

            Éxito = true;

        } // RespuestaDian>


        public void Error(string mensaje) {

            Éxito = false;
            MensajeError = $"{mensaje}{DobleLínea}";

            if (CódigoEstado != null) { // Si CódigoEstado no es nulo se puede asegurar que los otros valores tampoco lo son.
                MensajeError += $"Código Estado: {CódigoEstado}{NuevaLínea}Descripción Estado: {DescripciónEstado}{NuevaLínea}Mensaje Estado: {MensajeEstado}" +
                                $"{NuevaLínea}Clave Documento: {ClaveDocumento}{NuevaLínea}Nombre Archivo: {NombreArchivo}";
            }

            if (Errores.Any()) {

                MensajeError += DobleLínea;
                foreach (var error in Errores) {
                    MensajeError += $"{error.Tipo} {error.Código}: {error.Mensaje.TrimEnd('.')}.{NuevaLínea}";
                }

            }

        } // Error>


        #endregion Métodos y Funciones>


    } // RespuestaDian>



} // SimpleOps.Legal>
