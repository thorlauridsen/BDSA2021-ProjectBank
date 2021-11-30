using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record SupervisorDto(int Id, string? Name);

    public record SupervisorDetailsDto(int Id, string? Name) : SupervisorDto(Id, Name);

    public record SupervisorCreateDto
    {
        [StringLength(50)]
        public string Name { get; init; }

    }

    public record SupervisorUpdateDto : SupervisorCreateDto
    {
        public int Id { get; init; }
    }
}
