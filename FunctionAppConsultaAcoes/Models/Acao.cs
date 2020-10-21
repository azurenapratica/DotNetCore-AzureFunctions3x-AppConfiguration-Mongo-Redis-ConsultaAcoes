using System;

namespace FunctionAppConsultaAcoes.Models
{
    public class Acao
    {
        public string Codigo { get; set; }
        public double? Valor { get; set; }
        public string CodCorretora { get; set; }
        public string NomeCorretora { get; set; }
        public DateTime Data { get; set; }
    }
}