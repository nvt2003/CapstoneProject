using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Address
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly PlanifyContext _context;
        public ProvinceRepository(PlanifyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<District>> getAllDistrictByProvinceID(int id)
        {
            var district = await _context.Districts.Where(d => d.ProvinceId == id).ToListAsync();
            return district;
        }

        public async Task<IEnumerable<Province>> getAllProvince()
        {
            var p = await _context.Provinces.ToListAsync();
            return p;
        }

        public async Task<IEnumerable<Ward>> getAllWardByDistrictID(int id)
        {
            var p = await _context.Wards.Where(w=>w.DistrictId==id).ToListAsync();
            return p;
        }

        public bool UpdateAddress(Models.Address address)
        {
            try
            {
                var updateAddress = _context.Addresses.FirstOrDefault(a=>a.Id ==address.Id);
                updateAddress.AddressDetail = address.AddressDetail;
                updateAddress.WardId = address.WardId;
                _context.Update(updateAddress);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int CreateAddress(Models.Address address)
        {
            try
            {
                _context.Add(address);
                _context.SaveChanges();
                return address.Id;
            }
            catch
            {
                return 0;
            }
        }
    }
}
