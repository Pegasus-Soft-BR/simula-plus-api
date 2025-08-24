using Infra.PegasusApi.Dtos;
using System.Threading.Tasks;

namespace Infra.PegasusApi;

public interface IPegasusApiClient
{
    Task<ContactUsResponse> SendContactUsAsync(ContactUsRequest request);
}