// Ubicación: Attributes/AuthorizeRoleAttribute.cs
using Microsoft.AspNetCore.Mvc;

public class AuthorizeRoleAttribute : TypeFilterAttribute
{
    public AuthorizeRoleAttribute(int idCargo) : base(typeof(RoleAuthorizationFilter))
    {
        Arguments = new object[] { idCargo };
    }
}
