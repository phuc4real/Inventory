using AutoMapper;
using Inventory.Repository;

namespace Inventory.Service.Implement
{
    public class BaseService : IBaseService
    {
        #region Ctor & Field

        protected readonly IMapper _mapper;
        protected readonly IRepoWrapper _repoWrapper;
        protected readonly IRedisCacheService _cacheService;
        protected readonly IEmailService _emailService;
        protected readonly ICommonService _commonService;

        public BaseService(
            IRepoWrapper repoWrapper,
            IMapper mapper,
            ICommonService commonService,
            IRedisCacheService cacheService,
            IEmailService emailService)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _cacheService = cacheService;
            _emailService = emailService;
            _commonService = commonService;
        }

        #endregion
    }
}
