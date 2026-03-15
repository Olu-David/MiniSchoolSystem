using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Implementation.Interfaces
{
    public interface iJwtService
    {
        string GenerateJwtToken(UserDb user);
    }
}
