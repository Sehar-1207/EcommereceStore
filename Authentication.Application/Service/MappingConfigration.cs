using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using Mapster;

namespace Authentication.Application.Service
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // DTO -> Entity
            config.NewConfig<AppUserRequestDto, Appuser>()
                .Map(dest => dest.UserName, src => src.Username)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNo)
                .Ignore(dest => dest.Id)        // ignore Id during creation
                .Ignore(dest => dest.Role);     // role will be set manually in repo
                                                // DO NOT map AdminKey (temporary, not stored)

            // Entity -> DTO
            config.NewConfig<Appuser, AppUserRequestDto>()
                .Map(dest => dest.Username, src => src.UserName)
                .Map(dest => dest.PhoneNo, src => src.PhoneNumber)
                .Ignore(dest => dest.Password)  // never expose password
                .Ignore(dest => dest.AdminKey); // not needed in response
        }
    }
}
