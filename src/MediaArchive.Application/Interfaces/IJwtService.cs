using MediaArchive.Domain.Entities;

namespace MediaArchive.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}