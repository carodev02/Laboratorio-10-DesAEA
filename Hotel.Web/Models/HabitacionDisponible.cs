namespace Hotel.Web.Models;
public class HabitacionDisponible
{
    public Habitacion Habitacion { get; set; } = null!;
    public int Noches { get; set; }
    public decimal Total { get; set; }
}
