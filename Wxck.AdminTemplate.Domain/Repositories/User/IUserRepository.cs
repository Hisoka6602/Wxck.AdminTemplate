using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.Domain.Entities.User;

namespace Wxck.AdminTemplate.Domain.Repositories.User {

    public interface IUserRepository : IMemoryCacheRepository<UserInfoModel> {

        /// <summary>
        /// 详细信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<KeyValuePair<bool, object>> DetailsInfo(string userCode, CancellationToken token);
    }
}