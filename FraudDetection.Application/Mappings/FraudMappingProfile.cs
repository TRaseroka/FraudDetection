using AutoMapper;
using FraudDetection.Application.DTOs;
using FraudDetection.Domain;

namespace FraudDetection.Application.Mappings;

public class FraudMappingProfile : Profile
{
    public FraudMappingProfile()
    {
        CreateMap<TriggeredRule, TriggeredRuleDto>();
        CreateMap<FraudAssessment, FraudAssessmentDto>();
    }
}