using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

public class ReservaRepositorio(HotelContext context)
{
    public Task<List<Huesped>> HuespedesAsync() => context.Huespeds.AsNoTracking().Where(h => h.Activo).OrderBy(h => h.Apellidos).ThenBy(h => h.Nombres).ToListAsync();
    public Task<List<Habitacion>> HabitacionesActivasAsync() => context.Habitacions.AsNoTracking().Include(h => h.IdTipoHabitacionNavigation).Where(h => h.Activo).OrderBy(h => h.Numero).ToListAsync();
    public async Task<List<HabitacionDisponible>> DisponiblesAsync(DateOnly ingreso, DateOnly salida)
    {
        var habitaciones = await context.Habitacions.AsNoTracking().Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo && !context.Reservas.Any(r => r.IdHabitacion == h.IdHabitacion && r.Estado != "Cancelada" && r.FechaIngreso < salida && r.FechaSalida > ingreso))
            .OrderBy(h => h.Numero).ToListAsync();
        var noches = salida.DayNumber - ingreso.DayNumber;
        return habitaciones.Select(h => new HabitacionDisponible { Habitacion = h, Noches = noches, Total = noches * h.IdTipoHabitacionNavigation.PrecioNoche }).ToList();
    }
    public Task<bool> EstaReservadaAsync(int habitacionId, DateOnly ingreso, DateOnly salida) => context.Reservas.AsNoTracking().AnyAsync(r => r.IdHabitacion == habitacionId && r.Estado != "Cancelada" && r.FechaIngreso < salida && r.FechaSalida > ingreso);
    public Task<List<Reserva>> ListarAsync() => context.Reservas.AsNoTracking().Include(r => r.IdHuespedNavigation).Include(r => r.IdHabitacionNavigation).ThenInclude(h => h.IdTipoHabitacionNavigation).OrderByDescending(r => r.FechaRegistro).ToListAsync();
    public async Task GuardarAsync(Reserva reserva) { context.Reservas.Add(reserva); await context.SaveChangesAsync(); }
    public async Task<bool> CancelarAsync(int id) { var r = await context.Reservas.FindAsync(id); if (r is null || (r.Estado != "Pendiente" && r.Estado != "Confirmada")) return false; r.Estado = "Cancelada"; await context.SaveChangesAsync(); return true; }
}
