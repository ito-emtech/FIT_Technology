using DynamicDll.Db;
using FIT_Technology.Models.Entities;

namespace FIT_Technology.Models.Daos
{
    public class LicenseDao : BaseDao<EmployeeEntity>
    {
        public LicenseDao() { }

        public override EmployeeEntity Find(params object[] pkeys)
        {
            throw new NotImplementedException();
        }

        public override int Insert(EmployeeEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Update(EmployeeEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(EmployeeEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}

