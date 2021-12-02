using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDto(int Id, string? Name, bool IsSupervisor);

    public record UserDetailsDto(int Id, string? Name, bool IsSupervisor) : UserDto(Id, Name, IsSupervisor);

    public record UserCreateDto
    {
        [StringLength(50)]
        public string Name { get; init; }

        public bool IsSupervisor { get; init; }
    }

    public record UserUpdateDto : UserCreateDto
    {
        public int Id { get; init; }
    }
}
