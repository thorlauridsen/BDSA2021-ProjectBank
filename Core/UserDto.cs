using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDto(string oid, string? Name);

    public record UserDetailsDto(string oid, string? Name, string image) : UserDto(oid, Name);

    public record UserCreateDto
    {
        public string oid { get; init; }

        [StringLength(50)]
        public string Name { get; init; }
    }
}
