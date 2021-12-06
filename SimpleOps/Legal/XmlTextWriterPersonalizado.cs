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
using System.Xml;
using System.Xml.Schema;
using static Vixark.General;



namespace SimpleOps.Legal {



    /// <summary>
    /// XmlTextWriter personalizado que omite la escritura de los atributos con nombre en AtributosAOmitir y escribe como CDATA cualquier texto que empiece por &lt;.
    /// </summary>
    class XmlTextWriterPersonalizado : XmlTextWriter { // Ver https://stackoverflow.com/questions/7656557/remove-xsitype-from-generated-xml-when-serializing.


        #region Variables y Campos

        private bool Saltar = false;

        #pragma warning disable IDE0044 // Agregar modificador de solo lectura. Se suprime porque esta clase es derivada de otra que no se controla, entonces se prefiere no hacer cambios que puedan dañar el funcionamiento.
        private bool EliminarDeRaíz = false;
        #pragma warning restore IDE0044

        private bool GenerarSeccionesCData { get; set; } = false;

        #endregion Variables y Campos>


        #region Constructores

        public XmlTextWriterPersonalizado(TextWriter w, bool generarSeccionesCData) : base(w) => GenerarSeccionesCData = generarSeccionesCData;

        public XmlTextWriterPersonalizado(Stream w, Encoding encoding) : base(w, encoding) { }

        public XmlTextWriterPersonalizado(string filename, Encoding encoding) : base(filename, encoding) { }

        #endregion Constructores>


        #region Métodos y Funciones


        public override void WriteStartAttribute(string prefix, string localName, string ns) {

            if (prefix == "xmlns" && localName == "xsi" && EliminarDeRaíz) {
                Saltar = true;
                return;
            } else if (localName == "type") { // Es equivalente a if (ns == XmlSchema.InstanceNamespace). Pero se prefiere == "type" para ser más explícito que lo que se quiere evitar es ese atributo, por si aparece otro caso que cumpla InstanceNamespace pero no == "type".
                Saltar = true;
                return;
            }
            base.WriteStartAttribute(prefix, localName, ns);

        } // WriteStartAttribute>


        public override void WriteString(string text) {

            if (Saltar) return;

            if (text.StartsWith('<') && GenerarSeccionesCData) {
                WriteCData(text);
            } else {
                base.WriteString(text);
            }

        } // WriteString>


        public override void WriteEndAttribute() {

            if (Saltar) {
                Saltar = false;
                return;
            }
            base.WriteEndAttribute();

        } // WriteEndAttribute>


        #endregion Métodos y Funciones


    } // XmlTextWriterSinXsi>



} // SimpleOps.Legal>