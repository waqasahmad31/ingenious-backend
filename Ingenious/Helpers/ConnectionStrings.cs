
using Microsoft.Extensions.Configuration;

namespace DAS.DataAccess.Helpers
{
    public class ConnectionStrings
    {
        private readonly IConfiguration _configuration;

        public ConnectionStrings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> Get()
        {
            try
            {
                return _configuration.GetConnectionString("DefaultConnection");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
