using CBF_Testing.Application.Analysis;
using CBF_Testing.Application.Data.Commands;
using CBF_Testing.Application.Genres.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CBF_Testing.Server.Controllers
{
    public class AnalysisController : ApiController
    {
        public AnalysisController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("DT_analysis")]
        public async Task<IActionResult> DTAnalysis()
        {
            var res = await _mediator.Send(new DecisionTreeAnalysis()
            {
            });
            return Ok(res);
        }

        [HttpGet]
        [Route("KNN_analysis")]
        public async Task<IActionResult> KNNAnalysis()
        {
            var res = await _mediator.Send(new KNNAnalysis()
            {
            });
            return Ok(res);
        }

        [HttpGet]
        [Route("Random_analysis")]
        public async Task<IActionResult> RandomAnalysis()
        {
            var res = await _mediator.Send(new RandomAnalysis()
            {
            });
            return Ok(res);
        }

        [HttpPost]
        [Route("load_data")]
        public async Task<IActionResult> LoadData()
        {
            var res = await _mediator.Send(new LoadAllData()
            {
            });
            return Ok(res);
        }

        [HttpGet]
        [Route("get_genres")]
        public async Task<IActionResult> GetAllGenres()
        {
            var res = await _mediator.Send(new GetAllGenres()
            {
            });
            return Ok(res);
        }
    }
}
