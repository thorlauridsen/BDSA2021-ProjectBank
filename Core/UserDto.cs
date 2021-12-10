using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDto(string oid, string? Name, string? Email);

    public record UserDetailsDto(string oid, string? Name, string? Email, string image) : UserDto(oid, Name, Email);

    public record UserCreateDto
    {
        public string oid { get; init; }

        [StringLength(50)]
        public string Name { get; init; }

        [EmailAddress]
        public string? Email { get; init; }
    }
}
