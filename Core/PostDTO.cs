using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBank.Core
{
    public record PostDTO
    {
        public string name;

        //public record CharacterCreateDto
        //{
        //    [StringLength(50)]
        //    public string? GivenName { get; init; }

        //    [StringLength(50)]
        //    public string? Surname { get; init; }

        //    [StringLength(50)]
        //    public string? AlterEgo { get; init; }

        //    [Range(1900, 2100)]
        //    public int? FirstAppearance { get; init; }

        //    [StringLength(50)]
        //    public string? Occupation { get; init; }

        //    public string? City { get; init; }

        //    public Gender Gender { get; init; }

        //    [Required]
        //    public ISet<string> Powers { get; init; } = null!;
        //}
    }
}
