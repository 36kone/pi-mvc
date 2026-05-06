using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("usuarios")]
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Tipo { get; set; } = "Admin";
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}
