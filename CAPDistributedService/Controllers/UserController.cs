using CAPService.Model;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAPDistributedService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICapPublisher _capBus;
        private readonly Cap_test_dbContext _dbContext;

        public UserController(ICapPublisher capBus, Cap_test_dbContext dbContext)
        {
            _capBus = capBus;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async ValueTask<IActionResult> Post(InsertUserParam param)
        {
            User cur = param;
            cur.Id = CAPService.Utils.IdGenerator.GetSnowflakeId();
            using (IDbContextTransaction transaction = _dbContext.Database.BeginTransaction(_capBus, true))
            {
                _ = await _dbContext.Set<User>().AddAsync(cur);
                _ = await _dbContext.SaveChangesAsync();
                await _capBus.PublishAsync(
                    "UserService.AddUserWithCompany",
                    new UserCompanyMap { UserId = param.Id, CompanyId = param.CompanyId }
                    );
            }
            return Ok();
        }

        //[HttpPost]
        //public async ValueTask<bool> UpdateByTransaction(InsertUserParam param)
        //{
        //}

    }

    public class InsertUserParam : User
    {
        public ulong CompanyId { get; set; }
    }

}