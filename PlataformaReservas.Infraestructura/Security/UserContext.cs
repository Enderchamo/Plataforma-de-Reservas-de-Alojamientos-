using Microsoft.AspNetCore.Http;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Security.Claims;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId => int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
    public bool IsHost => _httpContextAccessor.HttpContext?.User.IsInRole("Host") ?? false;
}