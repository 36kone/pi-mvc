using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("itens_pedido")]
public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int PizzaId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    public Pedido Pedido { get; set; }
    public Pizza Pizza { get; set; }
}
