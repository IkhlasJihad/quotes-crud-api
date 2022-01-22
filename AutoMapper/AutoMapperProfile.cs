using AutoMapper;
using QuotesAPI.Models;
using QuotesAPI.ViewModels;
namespace QuotesAPI.AutoMapper;
public class AutoMapperProfile : Profile {
    public AutoMapperProfile(){
        CreateMap<QuotesDBModel, QuotesViewModel>();
    }
}