using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface IAddressRepository
    {
        Task<int> AddAddressAsync(AddAddressDto dto);
        Task<int> AddAddressWithTransactionAsync(AddAddressDto dto, MySqlTransaction transaction);
        Task<int> UpdateAddressAsync(UpdateAddressDto dto);
        Task<int> DeleteAddressAsync(int addressId);
        Task<List<GetAddressDto>> GetAddressesByUserIdAsync(string aspNetUserId);
        Task<GetAddressDto> GetAddressByIdAsync(int addressId);
    }
    public class AddressRepository : IAddressRepository
    {
        private readonly ConnectionStrings _connectionStrings;

        public AddressRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<int> AddAddressAsync(AddAddressDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_Street", dto.Street),
                new MySqlParameter("@p_City", dto.City),
                new MySqlParameter("@p_State", dto.State),
                new MySqlParameter("@p_PostalCode", dto.PostalCode),
                new MySqlParameter("@p_Country", dto.Country),
                new MySqlParameter("@p_IsDefault", dto.IsDefault)
            };

            return await DbHelper.ExecuteNonQuery("Addresses_AddAddress", parameters, _connectionStrings);
        }

        public async Task<int> AddAddressWithTransactionAsync(AddAddressDto dto, MySqlTransaction transaction)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_Street", dto.Street),
                new MySqlParameter("@p_City", dto.City),
                new MySqlParameter("@p_State", dto.State),
                new MySqlParameter("@p_PostalCode", dto.PostalCode),
                new MySqlParameter("@p_Country", dto.Country),
                new MySqlParameter("@p_IsDefault", dto.IsDefault)
            };

            return await DbHelper.ExecuteWithTransaction("Addresses_AddAddress", parameters, _connectionStrings, transaction);
        }


        public async Task<int> UpdateAddressAsync(UpdateAddressDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AddressId", dto.AddressId),
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_Street", dto.Street),
                new MySqlParameter("@p_City", dto.City),
                new MySqlParameter("@p_State", dto.State),
                new MySqlParameter("@p_PostalCode", dto.PostalCode),
                new MySqlParameter("@p_Country", dto.Country),
                new MySqlParameter("@p_IsDefault", dto.IsDefault)
            };

            return await DbHelper.ExecuteQuery("Addresses_Update", parameters, _connectionStrings);
        }

        public async Task<int> DeleteAddressAsync(int addressId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AddressId", addressId)
            };

            return await DbHelper.ExecuteQuery("Addresses_DeleteAddress", parameters, _connectionStrings);
        }

        public async Task<List<GetAddressDto>> GetAddressesByUserIdAsync(string aspNetUserId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", aspNetUserId)
            };

            return await DbHelper.GetList<GetAddressDto>("Addresses_GetByUserId", parameters, _connectionStrings);
        }

        public async Task<GetAddressDto> GetAddressByIdAsync(int addressId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AddressId", addressId)
            };

            return await DbHelper.Get<GetAddressDto>("Addresses_GetById", parameters, _connectionStrings);
        }
    }
}
