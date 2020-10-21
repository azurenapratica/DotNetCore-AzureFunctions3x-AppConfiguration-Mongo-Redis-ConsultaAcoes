using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using StackExchange.Redis;
using FunctionAppConsultaAcoes.Models;
using FunctionAppConsultaAcoes.Documents;

namespace FunctionAppConsultaAcoes.Data
{
    public class AcoesRepository
    {
        private readonly ConnectionMultiplexer _conexaoRedis;
        private readonly string _prefixoChaveRedis;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<AcaoDocument> _collection;

        public AcoesRepository(IConfiguration configuration)
        {
            _conexaoRedis = ConnectionMultiplexer.Connect(
                configuration["Redis:Connection"]);
            _prefixoChaveRedis = configuration["Redis:PrefixoChave"];
            _client = new MongoClient(
                configuration["MongoDB:Connection"]);
            _db = _client.GetDatabase(
                configuration["MongoDB:Database"]);
            _collection = _db.GetCollection<AcaoDocument>(
                configuration["MongoDB:Collection"]);
        }
        public Acao Get(string codigo)
        {
            string strDadosAcao =
                _conexaoRedis.GetDatabase().StringGet(
                    $"{_prefixoChaveRedis}-{codigo}");
            if (!String.IsNullOrWhiteSpace(strDadosAcao))
                return JsonSerializer.Deserialize<Acao>(
                    strDadosAcao,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            else
                return null;
        }        

        public List<AcaoDocument> ListAll()
        {
            return _collection.Find(all => true).ToEnumerable()
                .OrderByDescending(d => d.Data).ToList();
        }
    }
}