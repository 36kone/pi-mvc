using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("clientes")]
public class Cliente
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? CpfCnpj { get; set; }
}
