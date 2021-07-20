﻿using CAPService.Impl;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAPService.Interface
{
    public class UserSubscriberService : IUserSubscriberService, ICapSubscribe
    {
        private readonly DbContext _dbContext;

        public UserSubscriberService(Model.Cap_test_dbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [CapSubscribe("UserService.AddUserWithCompany")]
        public async ValueTask InsertUser(dynamic userCompanyMap)
        {
            ulong userId = userCompanyMap.UserId;
            ulong companyId = userCompanyMap.CompanyId;
            Model.UserCompanyMap map = new()
            {
                Id = Utils.IdGenerator.GetSnowflakeId(),
                UserId = userId,
                CompanyId = companyId
            };
            await _dbContext.Set<Model.UserCompanyMap>().AddAsync(map);
            _ = await _dbContext.SaveChangesAsync();
        }
    }
}