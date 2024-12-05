using AutoMapper;

using BloodCount.Domain;


namespace BloodCount.DomainServices.Implementation;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<LLMResultVM, LLM>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Hemoglobin, opt => opt.MapFrom(src => Convert.ToInt32(src.Hemoglobin)))
            .ForMember(dest => dest.RedBloodCells, opt => opt.MapFrom(src => Convert.ToInt32(src.RedBloodCells)))
            .ForMember(dest => dest.Platelets, opt => opt.MapFrom(src => Convert.ToInt32(src.Platelets)))
            .ForMember(dest => dest.Leukocytes, opt => opt.MapFrom(src => Convert.ToInt32(src.Leukocytes)));

        CreateMap<LLMResultVM, LLM2ML>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Hemoglobin, opt => opt.MapFrom(src => Convert.ToInt32(src.Hemoglobin)))
            .ForMember(dest => dest.RedBloodCells, opt => opt.MapFrom(src => Convert.ToInt32(src.RedBloodCells)))
            .ForMember(dest => dest.Platelets, opt => opt.MapFrom(src => Convert.ToInt32(src.Platelets)))
            .ForMember(dest => dest.Leukocytes, opt => opt.MapFrom(src => Convert.ToInt32(src.Leukocytes)));
    }
}