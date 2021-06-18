using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GSM.Utility
{
    public class CertNormal
    {

        internal static Dictionary<string, string> Certificate()
        {

            Dictionary<string, string> certificate = new Dictionary<string, string>();
            string certFolder = @"wwwroot\resources\certificados";
            certificate.Add("environment", "INTEGRACION");
            certificate.Add("public_cert", certFolder + "\\tbk.pem");
            var cert = new X509Certificate2(certFolder + "\\597020000541.pfx", "transbank123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            certificate.Add("webpay_cert", certFolder + "\\597020000541.pfx");
            certificate.Add("password", "transbank123");
            certificate.Add("commerce_code", "597020000541");

            return certificate;

        }
    }
}
