using System.Security.Claims;

namespace Identity.WebAPI.Services
{
    public interface ICommonService
    {
        public Guid GetUserId();
    }
    public class CommonService(IHttpContextAccessor _httpContextAccessor) : ICommonService
    {
        public Guid GetUserId()
        {
            Claim? userIdClaim = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                throw new ArgumentException("User cannot found");
            }

            string userIdString = userIdClaim.Value;
            Guid userId = Guid.Parse(userIdString);
            return userId;
        }
    }
}
