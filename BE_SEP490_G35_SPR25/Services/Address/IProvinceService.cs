using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs.Andress;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Services.Address
{
    public interface IProvinceService
    {
        public Task<IEnumerable<ProvinceVM>> getAllProvince();

        public Task<IEnumerable<DistrictVM>> getAllDistrictByProvinceID(int id);

        public Task<IEnumerable<WardVM>> getAllWardByDistrictID(int id);
    }
}
