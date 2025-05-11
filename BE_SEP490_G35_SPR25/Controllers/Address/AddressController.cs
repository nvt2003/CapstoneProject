using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Services.Address;
using Planify_BackEnd.Services.User;

namespace Planify_BackEnd.Controllers.Address
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public AddressController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }
        [HttpGet("Provinces")]
        public async Task<IActionResult> GetAllProvince()
        {
            var pdto = await _provinceService.getAllProvince();
            var response = new ResponseDTO(200, "get all province", pdto);
            return Ok(response);


        }

        [HttpGet("District/id")]
        public async Task<IActionResult> GetAllDistrictByProvinceID(int id)
        {
            var pdto = await _provinceService.getAllDistrictByProvinceID(id);
            var response = new ResponseDTO(200, "get all province", pdto);
            return Ok(response);

        }

        [HttpGet("Ward/id")]
        public async Task<IActionResult> GetAllWardByDistrictID(int id)
        {
            var pdto = await _provinceService.getAllWardByDistrictID(id);
            var response = new ResponseDTO(200, "get all province", pdto);
            return Ok(response);

        }

    }
}
