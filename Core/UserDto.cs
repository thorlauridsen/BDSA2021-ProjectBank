using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDto(int Id, string? Name);

    public record UserDetailsDto(int Id, string? Name) : UserDto(Id, Name);

    public record UserCreateDto
    {
        [StringLength(50)]
        public string Name { get; init; }
    }

    public record UserUpdateDto : UserCreateDto
    {
        public int Id { get; init; }
    }
}
