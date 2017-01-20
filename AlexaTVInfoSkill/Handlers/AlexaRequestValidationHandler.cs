using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AlexaTVInfoSkill.Handlers
{
    public class AlexaRequestValidationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.IsLocal())
                return await base.SendAsync(request, cancellationToken);

            if (!request.Headers.Contains("Signature") || !request.Headers.Contains("SignatureCertChainUrl"))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var signatureCertChainUrl = request.Headers.GetValues("SignatureCertChainUrl").First().Replace("/../", "/");

            if (string.IsNullOrWhiteSpace(signatureCertChainUrl))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var certUrl = new Uri(signatureCertChainUrl);

            if (!((certUrl.Port == 443 || certUrl.IsDefaultPort)
                && certUrl.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                && certUrl.Host.Equals("s3.amazonaws.com", StringComparison.OrdinalIgnoreCase)
                && certUrl.AbsolutePath.StartsWith("/echo.api/")))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            using (var web = new WebClient())
            {
                var certificate = web.DownloadData(certUrl);
                var cert = new X509Certificate2(certificate);

                DateTime effectiveDate;
                DateTime expiryDate;

                if (!((DateTime.TryParse(cert.GetExpirationDateString(), out expiryDate)
                    && expiryDate > DateTime.UtcNow)
                    && (DateTime.TryParse(cert.GetEffectiveDateString(), out effectiveDate)
                    && effectiveDate < DateTime.UtcNow)))
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

                if (!cert.Subject.Contains("CN=echo-api.amazon.com"))
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

                var signatureString = request.Headers.GetValues("Signature").First();

                var signature = Convert.FromBase64String(signatureString);

                using (var sha1 = new SHA1Managed())
                {
                    var body = await request.Content.ReadAsStringAsync();

                    var data = sha1.ComputeHash(Encoding.UTF8.GetBytes(body));
                    var rsa = (RSACryptoServiceProvider) cert.PublicKey.Key;

                    if (!rsa.VerifyHash(data, CryptoConfig.MapNameToOID("SHA1"), signature))
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}