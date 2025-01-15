using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Interfaces;
using Questao5.Domain.Entities.Questao5.Domain;

namespace Questao5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IDbConnectionWrapper _dbConnection;

        public ContaCorrenteController(IDbConnectionWrapper dbConnectionWrapper)
        {
            _dbConnection = dbConnectionWrapper;
        }

        [HttpPost("movimentar")]
        public async Task<IActionResult> MovimentarConta([FromBody] MovimentacaoRequest request)
        {
            if (request.Valor <= 0)
            {
                return BadRequest(new { Mensagem = "Valor inválido", Tipo = "INVALID_VALUE" });
            }

            if (request.TipoMovimento.ToLower() != "c" && request.TipoMovimento.ToLower() != "d")
            {
                return BadRequest(new { Mensagem = "Tipo de movimento inválido", Tipo = "INVALID_TYPE" });
            }

            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { request.IdContaCorrente });

            if (conta == null)
            {
                return BadRequest(new { Mensagem = "Conta não cadastrada", Tipo = "INVALID_ACCOUNT" });
            }

            if (conta.Ativo == 0)
            {
                return BadRequest(new { Mensagem = "Conta inativa", Tipo = "INACTIVE_ACCOUNT" });
            }

            var idMovimento = Guid.NewGuid().ToString();
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new { IdMovimento = idMovimento, request.IdContaCorrente, DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"), request.TipoMovimento, request.Valor });

            return Ok(new { IdMovimento = idMovimento });
        }

        [HttpGet("saldo/{idContaCorrente}")]
        public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { IdContaCorrente = idContaCorrente });

            if (conta == null)
            {
                return BadRequest(new { Mensagem = "Conta não cadastrada", Tipo = "INVALID_ACCOUNT" });
            }

            if (conta.Ativo == 0)
            {
                return BadRequest(new { Mensagem = "Conta inativa", Tipo = "INACTIVE_ACCOUNT" });
            }

            var saldo = await _dbConnection.QueryFirstOrDefaultAsync<decimal>(
                "SELECT COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE -valor END), 0) FROM movimento WHERE idcontacorrente = @IdContaCorrente",
                new { IdContaCorrente = idContaCorrente });

            return Ok(new
            {
                Numero = conta.Numero,
                Nome = conta.Nome,
                DataHora = DateTime.Now,
                Saldo = saldo
            });
        }
    }
}
