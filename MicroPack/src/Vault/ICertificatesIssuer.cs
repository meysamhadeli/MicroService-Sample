using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MicroPack.Vault
{
    public interface ICertificatesIssuer
    {
        Task<X509Certificate2> IssueAsync();
    }
}