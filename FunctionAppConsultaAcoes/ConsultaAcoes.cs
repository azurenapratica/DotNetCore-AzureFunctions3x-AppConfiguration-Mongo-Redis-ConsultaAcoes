using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAppConsultaAcoes.Models;
using FunctionAppConsultaAcoes.Data;

namespace FunctionAppConsultaAcoes
{
    public class ConsultaAcoes
    {
        private AcoesRepository _repository;

        public ConsultaAcoes(AcoesRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("ValorAtual")]
        public IActionResult RunValorAtual(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string codigo = req.Query["codigo"];
            if (String.IsNullOrWhiteSpace(codigo))
            {
                log.LogError(
                    $"ValorAtual HTTP trigger - Codigo de Acao nao informado");
                return new BadRequestObjectResult(new
                {
                    Sucesso = false,
                    Mensagem = "Código de Ação não informado"
                });
            }

            log.LogInformation($"ValorAtual HTTP trigger - codigo da Acao: {codigo}");
            Acao acao = null;
            if (!String.IsNullOrWhiteSpace(codigo))
                acao = _repository.Get(codigo.ToUpper());

            if (acao != null)
            {
                log.LogInformation(
                    $"ValorAtual HTTP trigger - Acao: {codigo} | Valor atual: {acao.Valor} | Ultima atualizacao: {acao.Data}");
                return new OkObjectResult(acao);
            }
            else
            {
                log.LogError(
                    $"ValorAtual HTTP trigger - Codigo de Acao nao encontrado: {codigo}");
                return new NotFoundObjectResult(new
                {
                    Sucesso = false,
                    Mensagem = $"Código de Ação não encontrado: {codigo}"
                });
            }
        }

        [FunctionName("Historico")]
        public IActionResult RunHistorico(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var historico = _repository.ListAll();
            log.LogInformation(
                $"Historico HTTP trigger - Qtde. Registros Historico: {historico.Count}");

            return new OkObjectResult(historico);
        }
    }
}