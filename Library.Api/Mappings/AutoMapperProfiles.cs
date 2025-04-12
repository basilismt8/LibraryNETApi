using AutoMapper;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;

namespace Library.Api.Mappings
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<CreateBookRequestDto, Book>().ReverseMap();
            CreateMap<UpdateBookRequestDto, Book>().ReverseMap();
            CreateMap<Fine, FineDto>().ReverseMap().ReverseMap();
            CreateMap<Loan, LoanDto>().ReverseMap().ReverseMap();
            CreateMap<CreateLoanRequestDto, Loan>().ReverseMap();
            CreateMap<CreateLoanRequestDto, Loan>().ReverseMap();
            CreateMap<ExtendLoanRequestDto, Loan>().ReverseMap();
        }
    }
}
