using CBF_Testing.Domain.DTOs.Animes;
using CBF_Testing.Domain.DTOs.Genres;
using CBF_Testing.Domain.DTOs.User;
using CBF_Testing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.MappingProfiles
{
    public class AllMappingProfiles : AutoMapper.Profile
    {
        public AllMappingProfiles()
        {
            CreateMap<Genre, GenreResponse>();
            CreateMap<Anime, AnimeResponse>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.Name));

            CreateMap<ViewFeedback, ViewFeetbackResponse>()
                .ForMember(dest => dest.AnimeName, opt => opt.MapFrom(src => src.Anime.Name));


            CreateMap<RatingFeedback, RatingFeetbackResponse>()
                .ForMember(dest => dest.AnimeName, opt => opt.MapFrom(src => src.Anime.Name));
        }
    }
}
