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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Dian.Respuesta; // Si se encuentra error aquí, se debe agregar la referencia a Dian.dll (generado con Dian.sln) que se encuentra en \SimpleOps\. Esta librería se maneja independiente para evitar disminuir el rendimiento de Visual Studio con las clases enormes y autogeneradas que contiene.
using static Vixark.General;
using static SimpleOps.Global;
using SimpleOps.Modelo;
using System.Xml;
using static SimpleOps.Legal.Dian;

using System.Collections.ObjectModel;
using System.Linq;


namespace SimpleOps.Legal {



    class RespuestaElectrónica<D, M> where D : Factura<Cliente, M> where M : MovimientoProducto { // Respuesta que se da a los clientes que contiene el documento electrónico, otra información y la respuesta de la DIAN.


        #region Propiedades

        public DocumentoElectrónico<D, M> DocumentoElectrónico { get; set; }

        /// <summary>
        /// Ruta del XML de la respuesta electrónica.
        /// </summary>
        public string? Ruta { get; set; }

        public bool IdentarXml = true; // Se va a permitir identar el XML porque este archivo no necesita ser firmado sin espacios como si lo requiere el XML documento electrónico. Todos los sistemas receptores de este archivo deberían estar en capacidad de leerlo pues es un XML bien formado.

        public string RutaDocumentosElectrónicosHoy { get; set; } // Se maneja como una propiedad para asegurar que su valor es el mismo durante el tiempo de vida del objeto y evitar casos especiales en los que se inicie el procesamiento antes de media noche, se finalice después y no se pueda realizar porque no encuentre el archivo sin firma. Además, porque también se usa para poder guardar la representación gráfica en PDF en la misma carpeta que el documento electrónico.

        #endregion Propiedades>


        #region Constructores 


        public RespuestaElectrónica(DocumentoElectrónico<D, M> documentoElectrónico) {

            DocumentoElectrónico = documentoElectrónico;
            RutaDocumentosElectrónicosHoy = ObtenerRutaDocumentosElectrónicosDeHoy();

        } // RespuestaElectrónica>


        #endregion Constructores>


        #region Métodos y Funciones


        /// <summary>
        /// Devuelve verdadero si se creó exitosamente.
        /// </summary>
        public bool Crear(out string? mensaje) {

            mensaje = null;

            if (DocumentoElectrónico == null) return Falso(out mensaje, "El documento electrónico está vacío.");
            if (DocumentoElectrónico.RespuestaDian == null) return Falso(out mensaje, "La respuesta de la DIAN está vacía.");
            if (DocumentoElectrónico.RespuestaDian.FechaRespuesta == null || DocumentoElectrónico.RespuestaDian.HoraRespuesta == null) 
                return Falso(out mensaje, "La fecha u hora de respuesta de la DIAN está vacía.");

            var documento = DocumentoElectrónico.Documento;
            var fechaHora = documento.FechaHora;
            var cliente = documento.EntidadEconómica;
            if (cliente == null) return Falso(out mensaje, "No se ha cargado el cliente.");


            var tipoImpuesto = documento.IVA > 0 ? (documento.ImpuestoConsumo > 0 ? TipoImpuesto.IVAeINC : TipoImpuesto.IVA)
                : (documento.ImpuestoConsumo > 0 ? TipoImpuesto.INC : TipoImpuesto.NoAplica);

            var registrationNameCliente = new RegistrationNameType { Value = Validar(cliente.Nombre, "5..450", true) }; 

            var registrationNameFacturador = new RegistrationNameType { Value = Validar(Empresa.RazónSocial, "5..450", true) };

            var companyIDFacturador = new CompanyIDType {
                Value = Validar(Empresa.Nit, "5..12"), // 1..1 AE12.
                schemeAgencyID = AgenciaID195, // 0..1 AE13.
                schemeID = Empresa.DígitoVerificaciónNit, // 1..1 AE14.
                schemeName = DocumentoIdentificación.Nit.AValor(), // 0..1 AE15.
            };

            var companyIDCliente = new CompanyIDType {
                Value = Validar(cliente.NitLegalEfectivo, "5..12"), // 1..1 AE24.
                schemeAgencyID = AgenciaID195, // 1..1 AE25.
                schemeID = cliente.DígitoVerificaciónNit, // 0..1 AE26.
                schemeName = cliente.CódigoDocumentoIdentificación // 1..1 AE27.
            };

            if (!File.Exists(DocumentoElectrónico.Ruta)) return Falso(out mensaje, $"No se encontró el documento electrónico {DocumentoElectrónico.Ruta}.");
            var xmlDocumentoElectrónico = File.ReadAllText(DocumentoElectrónico.Ruta);

            #pragma warning disable IDE0017 // Simplificar la inicialización de objetos. Se omite para no generar demasiada complejidad e identación en el código de creación.
            var respuesta = new AttachedDocumentType();
            #pragma warning restore IDE0017
            respuesta.UBLVersionID = new UBLVersionIDType { Value = Validar(VersiónUBL, "7..8") }; // 1..1 AE02.
            respuesta.CustomizationID = new CustomizationIDType { Value = Validar("Documentos adjuntos", "19") }; // 1..1 AE03. Dice que el largo debe ser 20, pero exige "Documentos adjuntos", entonces el largo debe ser de 19.
            respuesta.ProfileID = new ProfileIDType { Value = Validar("Factura Electrónica de Venta", "1..55") }; // 1..1 AE03. Dice que el largo debe ser de 55, pero el texto requerido es mucho menor, entonces se pone la restricción de 1 a 55.
            respuesta.ProfileExecutionID = new ProfileExecutionIDType { Value = Empresa.AmbienteFacturaciónElectrónica.AValor() }; // 1..1 AE04.
            respuesta.ID = new IDType { Value = documento.Código.ToString() }; // 1..1 AE04b. Consecutivo propio del generador del documento. Cómo no se maneja consecutivo para las respuesta se usa el código de la factura/nota crédito/nota débito. Esto puede generar problemas porque un sistema tercero podría esperar un número consecutivo, pero parece que no es un campo muy importante porque una empresa grande que se usó de ejemplo ponía su NIT en este lugar.
            respuesta.IssueDate = new IssueDateType { Value = fechaHora }; // 1..1 AE05.
            respuesta.IssueTime = new IssueTimeType { Value = fechaHora }; // 1..1 AE06.
            respuesta.DocumentType = new DocumentTypeType { Value = Validar("Contenedor de Factura Electrónica", "33") }; // 1..1 AE08.
            respuesta.ParentDocumentID = new ParentDocumentIDType { Value = documento.Código.ToString() }; // 1..1 AE08a.

            respuesta.SenderParty = new PartyType { // 0..1 AE09.
                PartyTaxScheme = new PartyTaxSchemeType[1] { new PartyTaxSchemeType { // 1..1 AE10.
                    RegistrationName = registrationNameFacturador, // 1..1 AE11.
                    CompanyID = companyIDFacturador, // 1..1 AE12.
                    TaxLevelCode = new TaxLevelCodeType { // 1..1 AE16.
                        Value = Validar(ObtenerResponsabilidadFiscal(Empresa.TipoContribuyente), "1..30"), // 1..1 AE16.
                        listName = "No aplica" // 0..1 AE17.
                    }, 
                    TaxScheme = new TaxSchemeType { // 1..1 AE18.
                        ID = new IDType { Value = Validar(ObtenerCódigoTipoImpuesto(tipoImpuesto, forzar01: true), "2..10") }, // 1..1 AE18.
                        Name = new NameType1 { Value = Validar(ObtenerNombreImpuesto(tipoImpuesto, forzarIVA: true), "3..30") } // 1..1 AE19. 
                    },
                } }
            }; 

            respuesta.ReceiverParty = new PartyType { // 0..1 AE21.
                PartyTaxScheme = new PartyTaxSchemeType[1] { new PartyTaxSchemeType { // 1..1 AE22.
                    RegistrationName = registrationNameCliente, // 1..1 AE23.
                    CompanyID = companyIDCliente, // 1..1 AE24.
                    TaxLevelCode = new TaxLevelCodeType { // 1..1 AE28.
                        Value = Validar(ObtenerResponsabilidadFiscal(cliente.TipoContribuyente), "1..30"), // 1..1 AE28. 
                        listName = "No aplica" // 0..1 AE29.
                    },
                    TaxScheme = new TaxSchemeType { // 1..1 AE30.
                        ID = new IDType { Value = Validar(ObtenerCódigoTipoImpuesto(tipoImpuesto, forzar01: true), "2..10") }, // 1..1 AE31. 
                        Name = new NameType1 { Value = Validar(ObtenerNombreImpuesto(tipoImpuesto, forzarIVA: true), "3..30") } // 1..1 AE32. 
                    },
                } }
            };

            respuesta.Attachment = new AttachmentType { 
                ExternalReference = new ExternalReferenceType {
                    MimeCode = new MimeCodeType { Value = "text/xml" },
                    EncodingCode = new EncodingCodeType { Value = "UTF-8" },
                    Description = new DescriptionType[] { new DescriptionType { Value = xmlDocumentoElectrónico } }
                }
            };

            respuesta.ParentDocumentLineReference = new LineReferenceType[] { // 1..N AE38.
                new LineReferenceType {
                    LineID = new LineIDType { Value = "1" }, // 1..1 AE39.
                    DocumentReference = new DocumentReferenceType {
                        ID = new IDType { Value = Validar(documento.Código, "1..20") },
                        UUID = new UUIDType {
                            Value = Validar(documento.Cude, "96"), // 1..1 .
                            schemeName = Validar(AlgoritmoCufe, "11") // 1..1 .
                        },
                        IssueDate = new IssueDateType { Value = documento.FechaHora }, // 1..1 .
                        DocumentType = new DocumentTypeType { Value = "ApplicationResponse" }, 
                        Attachment = new AttachmentType { 
                            ExternalReference = new ExternalReferenceType {
                                MimeCode = new MimeCodeType { Value = "text/xml" },
                                EncodingCode = new EncodingCodeType { Value = "UTF-8" },
                                Description = new DescriptionType[] { 
                                    new DescriptionType { Value = DocumentoElectrónico?.RespuestaDian?.RespuestaAplicación } 
                                }
                            }
                        },
                        ResultOfVerification = new ResultOfVerificationType {
                            ValidatorID = new ValidatorIDType { Value = "Unidad Especial Dirección de Impuestos y Aduanas Nacionales"},
                            ValidationResultCode = new ValidationResultCodeType { Value = DocumentoElectrónico?.RespuestaDian?.CódigoRespuesta },
                            ValidationDate = new ValidationDateType { Value = (DateTime)DocumentoElectrónico?.RespuestaDian?.FechaRespuesta! }, // Se asegura que no es nulo porque al inicio del procedimiento se verifica.
                            ValidationTime = new ValidationTimeType { Value = (DateTime)DocumentoElectrónico?.RespuestaDian?.HoraRespuesta! } // Se asegura que no es nulo porque al inicio del procedimiento se verifica.
                        }
                    }
                }           
            };

            #region Escritura XML

            Ruta = ObtenerRuta();
            var espaciosNombres = new XmlSerializerNamespaces();    
            espaciosNombres.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
            espaciosNombres.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            espaciosNombres.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            espaciosNombres.Add("ccts", "urn:un:unece:uncefact:data:specification:CoreComponentTypeSchemaModule:2");
            espaciosNombres.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            espaciosNombres.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
            espaciosNombres.Add("xades141", "http://uri.etsi.org/01903/v1.4.1#");

            XmlSerializer? serializadorXml = new XmlSerializer(typeof(AttachedDocumentType), 
                "urn:oasis:names:specification:ubl:schema:xsd:AttachedDocument-2");

            using var flujoEscritura = new StreamWriter(Ruta);
            using var escritorXml = new XmlTextWriterPersonalizado(flujoEscritura); // Para escribir automáticamente las valores que sean un XML como CDATA y evitar que se codifique los carácteres < y > como &lt; y &gt;.
            if (IdentarXml) escritorXml.Formatting = Formatting.Indented;
            serializadorXml.Serialize(escritorXml, respuesta, espaciosNombres);
            flujoEscritura.Close();

            #endregion Escritura XML>

            return true;

        } // Crear>


        public string ObtenerRuta() {

            var nombreArchivo = $"{DocumentoElectrónico.Documento.Código}.xml"; // Para la respuesta electrónica se usa el código de la factura para que todos los archivos relacionados con la factura tengan el mismo nombre.
            return Path.Combine(RutaDocumentosElectrónicosHoy, nombreArchivo);

        } // ObtenerRuta>


        #endregion Métodos y Funciones>


    } // RespuestaElectrónica>



} // SimpleOps.Legal>
