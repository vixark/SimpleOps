using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;



namespace SimpleOps.Legal {



    /// <summary>
    /// XmlTextWriter personalizado que omite la escritura de los atributos con nombre en AtributosAOmitir.
    /// </summary>
    class XmlTextWriterSinXsi : XmlTextWriter { // Ver https://stackoverflow.com/questions/7656557/remove-xsitype-from-generated-xml-when-serializing.


        #region Variables y Campos

        private bool Saltar = false;

        private bool EliminarDeRaíz = false;

        #endregion Variables y Campos>


        #region Constructores

        public XmlTextWriterSinXsi(TextWriter w) : base(w) { }

        public XmlTextWriterSinXsi(Stream w, Encoding encoding) : base(w, encoding) { }

        public XmlTextWriterSinXsi(string filename, Encoding encoding) : base(filename, encoding) { }

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
            base.WriteString(text);

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
