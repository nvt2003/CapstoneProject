using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs.Andress;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Address;

namespace Planify_BackEnd.Services.Address
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceService(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public async Task<IEnumerable<DistrictVM>> getAllDistrictByProvinceID(int id)
        {
            var d = await _provinceRepository.getAllDistrictByProvinceID(id);
            var ddto = d.Select(d => new DistrictVM
            {
                Id = d.Id,
                DistrictName = d.DistrictName
            }).ToList();
            return ddto;
        }

        public async Task<IEnumerable<ProvinceVM>> getAllProvince()
        {
            var p = await _provinceRepository.getAllProvince();
            var pdto = p.Select(p=>  new ProvinceVM {
                Id = p.Id,
                ProvinceName=p.ProvinceName
            }).ToList();
            return pdto;
        }

        public async Task<IEnumerable<WardVM>> getAllWardByDistrictID(int id)
        {
            var p = await _provinceRepository.getAllWardByDistrictID(id);
            var pdto = p.Select(p => new WardVM
            {
                Id = p.Id,
                WardName = p.WardName
            }).ToList();
            return pdto;
        }
    }
}
