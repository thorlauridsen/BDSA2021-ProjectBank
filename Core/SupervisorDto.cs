using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record SupervisorDto(int Id, string? Name, int Salary);

    public record SupervisorDetailsDto(int Id, string? Name, int Salary) : SupervisorDto(Id, Name, Salary);

    public record SupervisorCreateDto
    {
        [StringLength(50)]
        public string? Name { get; init; }
        public int Salary { get; init; }
    }

    public record SupervisorUpdateDto : SupervisorCreateDto
    {
        public int Id { get; init; }
    }
}
