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
