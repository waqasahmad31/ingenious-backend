//using Ingenious.Models.Helpers;
//using Ingenious.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AddressesController : ControllerBase
//    {
//        private readonly IAddressRepository _addressRepository;

//        public AddressesController(IAddressRepository addressRepository)
//        {
//            _addressRepository = addressRepository;
//        }

//        // Get all addresses for a customer
//        [HttpGet("GetAddressesForCustomer/{customerId}")]
//        public async Task<IActionResult> GetAddressesForCustomer(int customerId)
//        {
//            try
//            {
//                var addresses = await _addressRepository.GetAddressesByCustomerIdAsync(customerId);
//                return Ok(new ApiResponse<IEnumerable<AddressDto>>(addresses, "Customer addresses retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving customer addresses: {ex.Message}", 500));
//            }
//        }

//        // Add a new address for a customer
//        [HttpPost("AddAddress")]
//        public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto addressDto)
//        {
//            try
//            {
//                if (addressDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid address data.", 400));
//                }

//                var id = await _addressRepository.AddAddressAsync(addressDto);
//                return CreatedAtAction(nameof(GetAddressesForCustomer), new { id }, new ApiResponse<int>(id, "Address added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the address: {ex.Message}", 500));
//            }
//        }

//        // Update an existing address
//        [HttpPut("UpdateAddress")]
//        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressDto addressDto)
//        {
//            try
//            {
//                if (addressDto == null || addressDto.AddressId <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid address data.", 400));
//                }

//                await _addressRepository.UpdateAddressAsync(addressDto);
//                return Ok(new ApiResponse<string>("Address updated successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the address: {ex.Message}", 500));
//            }
//        }

//        // Delete an address
//        [HttpDelete("DeleteAddress/{id}")]
//        public async Task<IActionResult> DeleteAddress(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid address ID.", 400));
//                }

//                await _addressRepository.DeleteAddressAsync(id);
//                return Ok(new ApiResponse<string>("Address deleted successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the address: {ex.Message}", 500));
//            }
//        }
//    }
//}
