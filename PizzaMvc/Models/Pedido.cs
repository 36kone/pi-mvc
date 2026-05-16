using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("pedidos")]
public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Feito";
    public decimal Total { get; set; }

    public Cliente? Cliente { get; set; }
    public ICollection<ItemPedido>? Itens { get; set; }
    public Pagamento? Pagamento { get; set; }
}
