using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Core
{
    public record StudentDto(int Id, string? Name, string? Course);

    public record StudentDetailsDto(int Id, string? Name, string? Course) : StudentDto(Id, Name, Course);

    public record StudentCreateDto
    {
        public StudentCreateDto(string name, string course)
        {
            Name = name;
            Course = course;
        }

        [StringLength(50)]
        public string Name { get; init; }


        [StringLength(50)]
        public string Course { get; init; }
    }

    public record StudentUpdateDto : StudentCreateDto
    {
        protected StudentUpdateDto(StudentCreateDto original) : base(original) { }

        public int Id { get; init; }
    }
}
