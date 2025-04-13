using Domain;
using MockExams.Infra.CrossCutting.Identity;

namespace MockExams.Infra.CrossCutting.Identity.Interfaces
{
    public interface IApplicationSignInManager
    {
        object GenerateTokenAndSetIdentity(User user, SigningConfigurations signingConfigurations, TokenConfigurations tokenConfigurations);
    }
}
