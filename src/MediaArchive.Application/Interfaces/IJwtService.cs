using MediaArchive.Domain.Entities;

namespace MediaArchive.Application.Authentication.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}