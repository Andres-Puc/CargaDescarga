using AutoMapper;
using CargaDescarga;
using Scm.Controllers.Dtos;
using Scm.Domain;


namespace Scm.Infrastructure.Mapping
{
    
    public class MappingProfile : Profile {
    public MappingProfile() {
            CreateMap<AppUser, RegisterUserResponseDto>();
            CreateMap<Empleado, EmpleadoDtos>();
            CreateMap<EmpleadoDtos,Empleado>();
            CreateMap<EmpleadoResponseDto,Empleado>();
            CreateMap<Empleado,EmpleadoResponseDto>();
            CreateMap<EmpleadoUpdateDto,Empleado>();
            CreateMap<Empleado,EmpleadoUpdateDto>();
            CreateMap<ValeDto, Vale>()
                .ForMember(dest=> dest.FechaExpedicionVale, 
                          opt=>opt.MapFrom(src=>src.Fecha))
                .ForMember(dest=> dest.FolioVale, 
                         opt=>opt.MapFrom(src=>src.Folio));

            CreateMap<RegisterValesDto, RegistroVale>();
            CreateMap<RegistroVale, RegisterValesDto>();

            CreateMap<RegistroVale, RegisterValesResponseDto>();
            CreateMap<RegisterFacturaResponseDto, RegistroFactura>();
            CreateMap<RegistroFactura, RegisterFacturaResponseDto>();
                
            
        }
    }
}