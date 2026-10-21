using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

public class HabitacionRepositorio(HotelContext context)
{
    public async Task<List<Habitacion>> ListarAsync(int? tipoId = null)
    {
        var query = context.Habitacions.AsNoTracking().Include(h => h.IdTipoHabitacionNavigation).Where(h => h.Activo);
        if (tipoId.HasValue) query = query.Where(h => h.IdTipoHabitacion == tipoId.Value);
        return await query.OrderBy(h => h.Numero).ToListAsync();
    }
    public Task<Habitacion?> ObtenerAsync(int id) => context.Habitacions.AsNoTracking().Include(h => h.IdTipoHabitacionNavigation).FirstOrDefaultAsync(h => h.IdHabitacion == id);
    public Task<List<TipoHabitacion>> TiposAsync() => context.TipoHabitacions.AsNoTracking().Where(t => t.Activo).OrderBy(t => t.Nombre).ToListAsync();
    public Task<bool> NumeroExisteAsync(string numero, int? excluirId = null) => context.Habitacions.AsNoTracking().AnyAsync(h => h.Numero == numero && (!excluirId.HasValue || h.IdHabitacion != excluirId.Value));
    public async Task GuardarAsync(Habitacion habitacion) { context.Habitacions.Add(habitacion); await context.SaveChangesAsync(); }
    public async Task ActualizarAsync(Habitacion habitacion) { context.Habitacions.Update(habitacion); await context.SaveChangesAsync(); }
    public async Task DesactivarAsync(int id) { var h = await context.Habitacions.FindAsync(id); if (h is null) return; h.Activo = false; await context.SaveChangesAsync(); }
}
