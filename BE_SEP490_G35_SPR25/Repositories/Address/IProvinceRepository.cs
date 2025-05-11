using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Address
{
    public interface IProvinceRepository
    {
        public Task<IEnumerable<Province>> getAllProvince();

        public Task<IEnumerable<District>> getAllDistrictByProvinceID(int id);

        public Task<IEnumerable<Ward>> getAllWardByDistrictID(int id);
        int CreateAddress(Models.Address address);
        bool UpdateAddress(Models.Address address);
    }
}
