using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record UserDetailsDto
    {
        public string Oid { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string Image { get; init; }
    }

    public record UserCreateDto
    {
        public string Oid { get; init; }

        [StringLength(50)]
        public string Name { get; init; }

        [EmailAddress]
        public string? Email { get; init; }

        public string? Image { get; init; }
    }
}
