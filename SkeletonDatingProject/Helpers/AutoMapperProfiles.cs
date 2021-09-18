using AutoMapper;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Extensions;
using System;
using System.Linq;

namespace SkeletonDatingProject.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opts => opts.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opts => opts.MapFrom( src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterUserDto, AppUser>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.UserName.ToLower()));
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.RecipientPhotoUrl, 
                        opts => opts.MapFrom(src => 
                                src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, 
                    opts => opts.MapFrom(src => 
                        src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}
