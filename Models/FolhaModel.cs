using System.ComponentModel.DataAnnotations;



namespace MinhaWebAPI.Models;
public class FolhaModel
{
    [Key]
    public int FolhaId { get; set; }

    [Required]
    public decimal Valor { get; set; }

    [Required]
    public int Quantidade { get; set; }

    [Required]
    public int Mes { get; set; }

    [Required]
    public int Ano { get; set; }

    [Required]
    public decimal SalarioBruto { get; set; }

    [Required]
    public decimal ImpostoIrrf { get; set; }

    [Required]
    public decimal ImpostoInss { get; set; }

    [Required]
    public decimal ImpostoFgts { get; set; }

    [Required]
    public decimal SalarioLiquido { get; set; }

    [Required]
    public int FuncionarioId { get; set; }

    public FuncionarioModel? Funcionario { get; set; }
}
