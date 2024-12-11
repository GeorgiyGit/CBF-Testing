using CBF_Testing.Application.Data.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.Data.CommandHandlers
{
    public class LoadAllDataHandler(IMediator mediator) : IRequestHandler<LoadAllData, bool>
    {
        private readonly IMediator _mediator = mediator;
        public async Task<bool> Handle(LoadAllData request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new LoadAnimeData());
            await _mediator.Send(new LoadRatingFeetbackData());
            await _mediator.Send(new LoadViewFeetbackData());
            return true;
        }
    }
}
