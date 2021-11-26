using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record SupervisorDto(int Id, string? Name);

    public record SupervisorDetailsDto(int Id, string? Name) : SupervisorDto(Id, Name);

    public record SupervisorCreateDto
    {
        public SupervisorCreateDto(string name)
        {
            Name = name;
        }

        [StringLength(50)]
        public string Name { get; init; }

    }

    public record SupervisorUpdateDto : SupervisorCreateDto
    {
        protected SupervisorUpdateDto(SupervisorCreateDto original) : base(original) { }

        public int Id { get; init; }
    }
}
