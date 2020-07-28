/// ************************************************************************************************************************
/// Autor: https://github.com/vixark
/// Proyecto Principal: https://github.com/vixark/SimpleOps
/// Licencia: MIT
/// ************************************************************************************************************************


using eFacturacionColombia_V2.Firma;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Firmador {


    enum TipoFirma { Factura, NotaDébito, NotaCrédito, Evento }


    class Firmador { // Debe ser compilado como una aplicación Windows en Aplicación > Tipo de Salida para evitar que se muestre la ventana de la consola durante un instante.


        static void Main(string[] args) { // Para probar en depuración agregar en Propiedades > Depurar > Opciones de inicio > Argumentos de la línea de comandos: "D:\Programas\SimpleOps\Firma Electrónica\Certificado.pfx" 123456ABCDEF "D:\Programas\SimpleOps\Documentos Electrónicos\20-03-05\fv08001972680002000000053-sf.xml" "D:\Programas\SimpleOps\Documentos Electrónicos\20-03-05\fv08001972680002000000053.xml" "2019-01-01 20:20:15" Factura true

            bool mostrarMensajeError = true;
            try { mostrarMensajeError = args.Length >= 7 ? bool.Parse(args[6]) : true; } catch { }

            try {

                var rutaCertificado = args[0];
                var claveCertificado = args[1];
                var rutaXmlSinFirmar = args[2];
                var rutaNuevoXmlFirmado = args[3];
                var FechaFirma = DateTime.ParseExact(args[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                TipoFirma tipoFirma = (TipoFirma)Enum.Parse(typeof(TipoFirma), args[5]);
                
                if (!File.Exists(rutaCertificado)) throw new Exception($"No existe el certificado: {rutaCertificado}.");
                if (!File.Exists(rutaXmlSinFirmar)) throw new Exception($"No existe el XML a firmar: {rutaXmlSinFirmar}.");

                var firma = new FirmaElectronica { RolFirmante = RolFirmante.EMISOR, RutaCertificado = rutaCertificado, ClaveCertificado = claveCertificado };
                var xmlSinFirmar = new FileInfo(rutaXmlSinFirmar);

                var datosDocumentoFirmado = new byte[0];
                switch (tipoFirma) {
                    case TipoFirma.Factura:
                        datosDocumentoFirmado = firma.FirmarFactura(xmlSinFirmar, FechaFirma);
                        break;
                    case TipoFirma.NotaDébito:
                        datosDocumentoFirmado = firma.FirmarNotaDebito(xmlSinFirmar, FechaFirma);
                        break;
                    case TipoFirma.NotaCrédito:
                        datosDocumentoFirmado = firma.FirmarNotaCredito(xmlSinFirmar, FechaFirma);
                        break;
                    case TipoFirma.Evento:
                        datosDocumentoFirmado = firma.FirmarEvento(xmlSinFirmar, FechaFirma);
                        break;
                    default:
                        throw new Exception($"Caso no considerado de tipoFirma {tipoFirma.ToString()}.");
                }

                File.WriteAllBytes(rutaNuevoXmlFirmado, datosDocumentoFirmado);
                try { File.Delete(rutaXmlSinFirmar); } catch { } // Si sucede error al borrar es un error que se puede omitir, no pasa nada si no se puede borrar el original.

            } catch (Exception ex) {

                if (mostrarMensajeError) {

                    var error = ex.Message;
                    if (error.StartsWith("La contraseña")) error = "La clave proporcionada no coincide con la clave del certificado.";
                    if (error.StartsWith("Índice fuera de los"))
                        error = @"El número de argumentos no es correcto. Se deben usar en este orden: ""RutaCertificado"" ClaveCertificado " +
                            @"""RutaXmlSinFirmar"" ""RutaNuevoXmlFirmado"" ""FechaFirma""(En formato yyyy-MM-dd HH:mm:ss) TipoFirma(Factura, NotaDébito, NotaCrédito o Evento) " + 
                            "SalirEnError(true o false)" + Environment.NewLine + Environment.NewLine + "Por ejemplo:" + Environment.NewLine +
                            @"""D:\Programas\SimpleOps\Firma Electrónica\Certificado.pfx"" 123456ABCDEF ""D:\Programas\SimpleOps\Documentos Electrónicos\20-03-05" + 
                            @"\fv08001972680002000000053-sf.xml"" ""D:\Programas\SimpleOps\Documentos Electrónicos\20-03-05\fv08001972680002000000053.xml"" " +
                            @"""2019-01-01 20:20:15"" Factura true";
                    MessageBox.Show(error, "Error en Firmador.exe", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                } else {
                    return;
                }
              
            }

        } // Main>


    } // Program>



} // Pruebas>
