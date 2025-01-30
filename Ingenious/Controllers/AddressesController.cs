using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;

        public AddressesController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet("GetAddressesByUserId/{aspNetUserId}")]
        public async Task<IActionResult> GetAddressesByUserId(string aspNetUserId)
        {
            try
            {
                var addresses = await _addressRepository.GetAddressesByUserIdAsync(aspNetUserId);
                return Ok(new ApiResponse<IEnumerable<GetAddressDto>>(addresses, "Addresses retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving addresses: {ex.Message}", 500));
            }
        }

        [HttpGet("GetAddressById/{addressId}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            try
            {
                var address = await _addressRepository.GetAddressByIdAsync(addressId);
                if (address == null)
                {
                    return NotFound(new ApiResponse<string>("Address not found.", 404));
                }

                return Ok(new ApiResponse<GetAddressDto>(address, "Address retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving the address: {ex.Message}", 500));
            }
        }

        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressDto addressDto)
        {
            try
            {
                if (addressDto == null)
                {
                    return BadRequest(new ApiResponse<string>("Invalid address data.", 400));
                }

                var addressId = await _addressRepository.AddAddressAsync(addressDto);
                return CreatedAtAction(nameof(GetAddressById), new { addressId }, new ApiResponse<int>(addressId, "Address added successfully.", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the address: {ex.Message}", 500));
            }
        }

        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressDto addressDto)
        {
            try
            {
                if (addressDto == null || addressDto.AddressId <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid address data.", 400));
                }

                var result = await _addressRepository.UpdateAddressAsync(addressDto);
               
                return Ok(new ApiResponse<string>("Address updated successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the address: {ex.Message}", 500));
            }
        }

        [HttpDelete("DeleteAddress/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                if (addressId <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid address ID.", 400));
                }

                var result = await _addressRepository.DeleteAddressAsync(addressId);
                
                return Ok(new ApiResponse<string>("Address deleted successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the address: {ex.Message}", 500));
            }
        }
    }
}
