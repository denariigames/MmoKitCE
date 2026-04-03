using UnityEngine.Networking;

namespace Insthync.UnityRestClient
{
    public class SimpleWebRequestCert : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
