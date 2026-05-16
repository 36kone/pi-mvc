using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("bebidas")]
public class Bebida
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Sabor { get; set; }
    public string Descricao { get; set; }
    public decimal Preco { get; set; }
    public string Categoria { get; set; }
}