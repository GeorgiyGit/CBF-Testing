using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBF_Testing.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public ApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
