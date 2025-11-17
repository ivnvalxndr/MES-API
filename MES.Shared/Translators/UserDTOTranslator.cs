using MES.Data.Entities;
using MES.Data.Enums;

namespace MES.Shared.Translators;

public class UserDTOTranslator : ITranslator<User, UserDTO>
{
    public UserDTO ToDTO(User entity)
    {
        if (entity == null) return null!;

        return new UserDTO
        {
            Id = entity.Id,
            Username = entity.Username,
            Role = entity.Role,
            LastLogin = entity.LastLogin
        };
    }

    public User ToEntity(UserDTO dto)
    {
        if (dto == null) return null!;

        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Role = dto.Role,
            LastLogin = dto.LastLogin
        };
    }

    public IEnumerable<UserDTO> ToDTOs(IEnumerable<User> entities)
    {
        return entities.Select(ToDTO);
    }

    public IEnumerable<User> ToEntities(IEnumerable<UserDTO> dtos)
    {
        return dtos.Select(ToEntity);
    }

    public User ToNewEntity(string username, string passwordHash, UserRole role = UserRole.Operator)
    {
        return new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}
