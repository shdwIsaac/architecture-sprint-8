using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace sprint8;

public class AddRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;

        // Получаем список ролей из claim "realm_access.roles"
        var realmAccess = principal.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
        if (!string.IsNullOrEmpty(realmAccess))
        {
            var roles = System.Text.Json.JsonDocument.Parse(realmAccess).RootElement
                .GetProperty("roles").EnumerateArray().Select(x => x.GetString());

            foreach (var role in roles)
            {
                // Добавляем роли в стандартный тип claim "role"
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }

        return Task.FromResult(principal);
    }
}