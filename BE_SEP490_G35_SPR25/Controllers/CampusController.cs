using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Services.Campus;
using Microsoft.AspNetCore.Authorization;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;
        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }
        [HttpGet("List")]
        public async Task<IActionResult> GetAllCampus()
        {
            try
            {
                var response = await _campusService.GetAllCampus();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpGet("{campusName}")]
        public async Task<IActionResult> GetCampusIdByName(string campusName)
        {
            try
            {
                var response = await _campusService.GetCampusByName(campusName);
                if (response==null) return NotFound("Not found campus with name "+ campusName);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCampusById(int id)
        {
            try
            {
                var response = await _campusService.GetCampusById(id);
                return StatusCode(response.Status, response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCampus(CampusDTO campusDTO)
        {

            try
            {
                var response = await _campusService.CreateCampus(campusDTO);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCampus(CampusDTO campusDTO)
        {
            try
            {
                var response = await _campusService.UpdateCampus(campusDTO);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCampus(int id)
        {

            try
            {
                var response = await _campusService.DeleteCampus(id);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
