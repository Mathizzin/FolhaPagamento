using System.ComponentModel.DataAnnotations;

namespace MinhaWebAPI.Models
{
    public class FuncionarioModel
    {
        public int Id { get; set; }

        [Required]
        public string? Nome { get; set; }

        [Required]
        [StringLength(11)]
        public string? CPF { get; set; }
    }
}