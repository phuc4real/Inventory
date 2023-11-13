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
        protected readonly IRedisCacheService? _cacheService;
        protected readonly IEmailService? _emailService;

        public BaseService(IRepoWrapper repoWrapper, IMapper mapper)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
        }

        public BaseService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public BaseService(IRepoWrapper repoWrapper, IMapper mapper, IEmailService emailService)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _emailService = emailService;
        }

        public BaseService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService, IEmailService emailService)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _cacheService = cacheService;
            _emailService = emailService;
        }

        #endregion
    }
}
