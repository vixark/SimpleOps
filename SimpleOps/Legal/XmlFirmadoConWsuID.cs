using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;



namespace SimpleOps.Legal {



    /// <summary>
    /// Clase auxiliar específica para el espacio de nombres wsu del estándar Oasis que permite la obtención de un elemento de un xml firmado usando el wsu:id.
    /// </summary>
    public class XmlFirmadoConWsuID : SignedXml { // Ver https://stackoverflow.com/questions/5099156/malformed-reference-element-when-adding-a-reference-based-on-an-id-attribute-w.


        #region Constructores

        public XmlFirmadoConWsuID(XmlDocument xml) : base(xml) { }

        public XmlFirmadoConWsuID(XmlElement xmlElement) : base(xmlElement) { }

        #endregion Constructores>


        #region Métodos y Funciones


        public override XmlElement GetIdElement(XmlDocument xml, string id) {

            if (xml == null) return null!; // Se debe controlar con el código externo el caso que sea null. Esto se hace por compatibildiad con la función sobreescrita de la clase base. 
            XmlElement? elementoEncontrado = base.GetIdElement(xml, id); 

            if (elementoEncontrado == null) {

                var administradorEspacios = new XmlNamespaceManager(xml.NameTable);
                administradorEspacios.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                elementoEncontrado = xml.SelectSingleNode($@"//*[@wsu:Id=""{id}""]", administradorEspacios) as XmlElement;

            }  

            return elementoEncontrado!; // Para hacerlo compatible con la función override se supondrá que nunca es nulo. Si lo es, su manejo se tendrá que dar en el código que lo llame.

        } // GetIdElement>


        #endregion Métodos y Funciones>


    } // XmlFirmadoConWsuID>



} // SimpleOps.Legal>
