using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaMvc.Models;

[Table("eventos")]
public class Evento
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public DateTime DataEvento { get; set; }
    public string Local { get; set; }
}