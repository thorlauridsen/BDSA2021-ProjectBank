using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDto(string oid, string? Name, bool IsSupervisor);

    public record UserDetailsDto(string oid, string? Name, string image, bool IsSupervisor) : UserDto(oid, Name, IsSupervisor);

    
    public record UserCreateDto
    {
        public string oid { get; init; }
        
        [StringLength(50)]
        public string Name { get; init; }

        public bool IsSupervisor { get; init; }
    }

    public record UserUpdateDto : UserCreateDto
    {
        public string oid { get; init; }
    }
}
