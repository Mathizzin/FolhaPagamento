using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaWebAPI.Models;
using MinhaWebAPI.data;


namespace FolhaPagamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioController : ControllerBase
    {
        private readonly FolhaContext _context;

        public FuncionarioController(FolhaContext context)
        {
            _context = context;
        }

        [HttpGet("listar")] // Define a rota personalizada para o primeiro método
        public async Task<ActionResult<IEnumerable<FuncionarioModel>>> GetFuncionarios()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();
            return Ok(funcionarios);
        }

        [HttpGet] // Define a rota personalizada para o primeiro método
        public async Task<ActionResult<IEnumerable<FuncionarioModel>>> GetFuncionariosNormal()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();
            return Ok(funcionarios);
        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult<FuncionarioModel>> GetFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);

            if (funcionario == null)
            {
                return NotFound();
            }

            return Ok(funcionario);
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult<FuncionarioModel>> Cadastrar(FuncionarioModel funcionario)
        {
            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFuncionario), new { id = funcionario.Id }, funcionario);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFuncionario(int id, FuncionarioModel funcionario)
        {
            if (id != funcionario.Id)
            {
                return BadRequest();
            }

            _context.Entry(funcionario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuncionarioExists(id))
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
        public async Task<IActionResult> DeleteFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.Id == id);
        }
    }
}
