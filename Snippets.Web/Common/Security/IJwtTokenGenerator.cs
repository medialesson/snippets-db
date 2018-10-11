using System.Threading.Tasks;

namespace Snippets.Web.Common.Security
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(string userId);
    }
}