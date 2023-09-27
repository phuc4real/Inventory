using AutoMapper;
using Inventory.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Implement
{
    internal class BaseService : IBaseService
    {
        #region Ctor & Field

        private readonly IRepoWrapper _repoWrapper;
        private readonly IMapper _mapper;

        public BaseService(IRepoWrapper repoWrapper, IMapper mapper)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
        }

        #endregion

        #region Private

        private void SetContext()
        {
            _repoWrapper.SetUserContext("");
        }

        #endregion
    }
}
