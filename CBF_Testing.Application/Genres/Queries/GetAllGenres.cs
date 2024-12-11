using CBF_Testing.Domain.DTOs.Genres;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.Genres.Queries
{
    public class GetAllGenres : IRequest<ICollection<GenreResponse>>
    {
    }
}
