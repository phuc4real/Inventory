using AutoMapper;
using Inventory.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Implement
{
    public class BaseService : IBaseService
    {
        #region Ctor & Field

        protected readonly IMapper _mapper;
        protected readonly IRepoWrapper _repoWrapper;
        protected readonly IRedisCacheService _cacheService;


        public BaseService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        #endregion
    }
}
