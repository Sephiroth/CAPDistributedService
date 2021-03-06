using CAPService.Interface;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CAPService.Impl
{
    public class UserSubscriberService : IUserSubscriberService, ICapSubscribe
    {
        private readonly DbContext _dbContext;

        public UserSubscriberService(Model.Cap_test_dbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [CapSubscribe("UserService.AddUserWithCompany")]
        public async ValueTask InsertUser(Model.UserCompanyMap userCompanyMap)
        {
            Model.UserCompanyMap map = new()
            {
                Id = Utils.IdGenerator.GetSnowflakeId(),
                UserId = userCompanyMap.UserId,
                CompanyId = userCompanyMap.CompanyId
            };
            await _dbContext.Set<Model.UserCompanyMap>().AddAsync(map);
            _ = await _dbContext.SaveChangesAsync();
        }
    }
}