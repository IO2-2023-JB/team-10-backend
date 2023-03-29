using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MojeWidelo_WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IRepositoryWrapper _repository;
        protected readonly IMapper _mapper;

        public BaseController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}
