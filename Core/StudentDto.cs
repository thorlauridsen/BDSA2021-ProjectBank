using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record StudentDto(int Id, string? Name, string? Course);

    public record StudentDetailsDto(int Id, string? Name, string? Course) : StudentDto(Id, Name, Course);

    public record StudentCreateDto
    {
        [StringLength(50)]
        public string? Name { get; init; }
        [StringLength(50)]
        public string? Course { get; init; }
    }

    public record StudentUpdateDto : StudentCreateDto
    {
        public int Id { get; init; }
    }
}
