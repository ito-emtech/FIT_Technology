using DynamicDll.Db;
using FIT_Technology.Models.Entities;

namespace FIT_Technology.Models.Daos
{
    public class AccountDao : BaseDao<UserEntity>
    {
        public AccountDao() { }

        public override UserEntity Find(params object[] pkeys)
        {
            throw new NotImplementedException();
        }

        public override int Insert(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Update(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(UserEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
