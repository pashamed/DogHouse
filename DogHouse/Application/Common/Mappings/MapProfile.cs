﻿using AutoMapper;
using DogHouse.Domain.DTOs;
using DogHouse.Domain.Entities;

namespace DogHouse.Application.Common.Mappings
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<Dog, DogDto>()
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => string.Join("&", src.Colors)));

            CreateMap<DogDto, Dog>()
                 .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors.Contains(" & ")
                     ? src.Colors.Split(new[] { " & " }, StringSplitOptions.None)
                     : new[] { src.Colors }));
        }
    }
}