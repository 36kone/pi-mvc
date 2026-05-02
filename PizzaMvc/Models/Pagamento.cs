using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("pagamentos")]
public class Pagamento
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public string FormaPagamento { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Pago";

    public Pedido Pedido { get; set; }
}
