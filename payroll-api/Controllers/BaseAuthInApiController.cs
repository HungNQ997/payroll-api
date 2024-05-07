using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Core.Constants;
using payroll_api.Common;
using System.Security.Claims;

namespace payroll_api.Controllers
{
    [Produces("application/json")]
    [Authorize(Policy = AuthorizeConst.POLICY_INTERNAL)]
    [Authorize(Roles = AuthorizeConst.ROLE_ADMIN)]
    [ApiController, ApiAreaRoute, Area("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class BaseAuthInApiController : ControllerBase
    {
        public string Username
        {
            get
            {
                var userIdentity = HttpContext.User;
                var username = userIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                return username?.Value ?? "not user";
            }
        }
    }
}
