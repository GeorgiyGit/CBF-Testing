using AutoMapper;
using CBF_Testing.Application.Genres.Queries;
using CBF_Testing.Domain.DTOs.Genres;
using CBF_Testing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.Genres.QueryHandlers
{
    public class GetAllGenresHandler(CBFTestingDbContext dbContext, IMapper mapper) : IRequestHandler<GetAllGenres, ICollection<GenreResponse>>
    {
        private readonly CBFTestingDbContext _dbContext = dbContext;
        private readonly IMapper _mapper;

        public async Task<ICollection<GenreResponse>> Handle(GetAllGenres request, CancellationToken cancellationToken)
        {
            var genres = await _dbContext.Genres.ToListAsync(cancellationToken);
            return mapper.Map<ICollection<GenreResponse>>(genres);
        }
    }
}
