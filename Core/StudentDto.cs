using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record StudentDto(int Id, string? Name);

    public record StudentDetailsDto(int Id, string? Name) : StudentDto(Id, Name);

    public record StudentCreateDto
    {

        [StringLength(50)]
        public string Name { get; init; }
    }

    public record StudentUpdateDto : StudentCreateDto
    {
        public int Id { get; init; }
    }
}
