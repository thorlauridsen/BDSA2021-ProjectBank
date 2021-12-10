using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDto(string Oid, string? Name, string? Email);

    public record UserDetailsDto(string Oid, string? Name, string? Email, string image) : UserDto(Oid, Name, Email);

    public record UserCreateDto
    {
        public string Oid { get; init; }

        [StringLength(50)]
        public string Name { get; init; }

        [EmailAddress]
        public string? Email { get; init; }
    }
}
