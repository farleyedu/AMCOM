namespace Questao5.Application.Commands.Requests
{
    public class MovimentacaoRequest
    {
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public string TipoMovimento { get; set; }
    }
}
