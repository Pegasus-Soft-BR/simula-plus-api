using Infra.PegasusApi.Dtos;
using System.Threading.Tasks;

namespace Infra.PegasusApi;

public interface IPegasusApiClient
{
    Task<AdminNotificationResponse> NotifyAdminsAsync(AdminNotificationRequest request);
}