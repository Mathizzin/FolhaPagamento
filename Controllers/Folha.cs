using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaWebAPI.Models;
using MinhaWebAPI.data;

namespace MinhaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolhaController : ControllerBase
    {
        private readonly FolhaContext _context;

        public FolhaController(FolhaContext context)
        {
            _context = context;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<FolhaModel>>> GetFolhas()
        {
            var folhas = await _context.Folhas
                .Include(f => f.Funcionario)
                .ToListAsync();

            var calculadoraImpostoRenda = new CalculadoraImpostoRenda();
            var calculadoraINSS = new CalculadoraINSS();

            var response = folhas.Select(f => new FolhaModel
            {
                FolhaId = f.FolhaId,
                Valor = f.Valor,
                Quantidade = f.Quantidade,
                Mes = f.Mes,
                Ano = f.Ano,
                SalarioBruto = f.Quantidade * f.Valor,
                ImpostoIrrf = calculadoraImpostoRenda.CalcularImpostoRenda(f.Quantidade * f.Valor),
                ImpostoInss = calculadoraINSS.CalcularINSS(f.Quantidade * f.Valor),
                ImpostoFgts = calculadoraImpostoRenda.CalcularFGTS(f.Quantidade * f.Valor),
                SalarioLiquido = calculadoraImpostoRenda.CalcularSalarioLiquido(f.Quantidade * f.Valor, calculadoraImpostoRenda.CalcularImpostoRenda(f.Quantidade * f.Valor), calculadoraINSS.CalcularINSS(f.Quantidade * f.Valor)),
                FuncionarioId = f.FuncionarioId,
                Funcionario = new FuncionarioModel
                {
                    Id = f.FuncionarioId,
                    Nome = f.Funcionario?.Nome,
                    CPF = f.Funcionario?.CPF
                }
            }).ToList();

            return Ok(response);
        }

        [HttpGet("buscar/{cpf}/{mes}/{ano}")]
        public async Task<ActionResult<FolhaModel>> GetFolhaPorCPFMesAno(string cpf, int mes, int ano)
        {
            // Buscar o funcionário pelo CPF
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.CPF == cpf);

            if (funcionario == null)
            {
                return NotFound("Funcionário não encontrado.");
            }

            // Buscar a folha com base no funcionário, mês e ano
            var folha = await _context.Folhas
                .Include(f => f.Funcionario)
                .FirstOrDefaultAsync(f => f.FuncionarioId == funcionario.Id && f.Mes == mes && f.Ano == ano);

            if (folha == null)
            {
                return NotFound("Folha não encontrada para o funcionário, mês e ano especificados.");
            }

            return Ok(folha);
        }


        [HttpPost("cadastrar")]
        public async Task<ActionResult<FolhaModel>> Cadastrar(FolhaModel folha)
        {
            // Verificar se o FuncionarioId é válido
            var funcionario = await _context.Funcionarios.FindAsync(folha.FuncionarioId);
            if (funcionario == null)
            {
                return NotFound("Funcionário não encontrado.");
            }

            // Definir o objeto Funcionario na FolhaModel
            folha.Funcionario = funcionario;

            // Calcular os valores que também estão sendo calculados no método GetFolhas
            var calculadoraImpostoRenda = new CalculadoraImpostoRenda();
            var calculadoraINSS = new CalculadoraINSS();

            folha.SalarioBruto = folha.Quantidade * folha.Valor;
            folha.ImpostoIrrf = calculadoraImpostoRenda.CalcularImpostoRenda(folha.SalarioBruto);
            folha.ImpostoInss = calculadoraINSS.CalcularINSS(folha.SalarioBruto);
            folha.ImpostoFgts = calculadoraImpostoRenda.CalcularFGTS(folha.SalarioBruto);
            folha.SalarioLiquido = calculadoraImpostoRenda.CalcularSalarioLiquido(folha.SalarioBruto, folha.ImpostoIrrf, folha.ImpostoInss);

            // Adicionar a FolhaModel ao contexto
            _context.Folhas.Add(folha);

            // Salvar as mudanças no banco de dados
            await _context.SaveChangesAsync();

            // Retornar a resposta de sucesso com a FolhaModel criada
            return CreatedAtAction(nameof(GetFolhas), new { id = folha.FolhaId }, folha);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFolha(int id, FolhaModel folha)
        {
            if (id != folha.FolhaId)
            {
                return BadRequest();
            }

            _context.Entry(folha).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolhaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolha(int id)
        {
            var folha = await _context.Folhas.FindAsync(id);
            if (folha == null)
            {
                return NotFound();
            }

            _context.Folhas.Remove(folha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FolhaExists(int id)
        {
            return _context.Folhas.Any(e => e.FolhaId == id);
        }
    }

    public class CalculadoraImpostoRenda
    {
        public decimal CalcularImpostoRenda(decimal salarioBruto)
        {
            decimal imposto = 0.0m;

            if (salarioBruto <= 1903.98m)
            {
                imposto = 0.0m;
            }
            else if (salarioBruto > 1903.98m && salarioBruto <= 2826.65m)
            {
                imposto = (salarioBruto * 0.075m) - 142.80m;
            }
            else if (salarioBruto >= 2826.66m && salarioBruto <= 3751.05m)
            {
                imposto = (salarioBruto * 0.15m) - 354.80m;
            }
            else if (salarioBruto >= 3751.06m && salarioBruto <= 4664.68m)
            {
                imposto = (salarioBruto * 0.225m) - 636.13m;
            }
            else
            {
                imposto = (salarioBruto * 0.275m) - 869.36m;
            }

            return imposto;
        }

        public decimal CalcularFGTS(decimal salarioBruto)
        {
            return salarioBruto * 0.08m; // Cálculo do FGTS: 8% do Salário Bruto
        }

        public decimal CalcularSalarioLiquido(decimal salarioBruto, decimal impostoIrrf, decimal impostoINSS)
        {
            return salarioBruto - impostoIrrf - impostoINSS; // Cálculo do Salário Líquido
        }
    }

    public class CalculadoraINSS
    {
        public decimal CalcularINSS(decimal salarioBruto)
        {
            decimal desconto = 0.0m;

            if (salarioBruto <= 1693.00m)
            {
                desconto = salarioBruto * 0.08m;
            }
            else if (salarioBruto <= 2822.90m)
            {
                desconto = salarioBruto * 0.09m;
            }
            else if (salarioBruto <= 5645.80m)
            {
                desconto = salarioBruto * 0.11m;
            }
            else
            {
                desconto = 621.03m;
            }

            return desconto;
        }
    }
}
