using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class ReservasController(ReservaRepositorio repositorio) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Disponibles(DateOnly? ingreso, DateOnly? salida)
    {
        ViewBag.Ingreso = ingreso?.ToString("yyyy-MM-dd"); ViewBag.Salida = salida?.ToString("yyyy-MM-dd");
        if (!ingreso.HasValue || !salida.HasValue || salida <= ingreso) return View(new List<HabitacionDisponible>());
        return View(await repositorio.DisponiblesAsync(ingreso.Value, salida.Value));
    }
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());
    public async Task<IActionResult> Create(DateOnly? ingreso, DateOnly? salida) { await CargarListas(); var hoy = DateOnly.FromDateTime(DateTime.Today); return View(new Reserva { FechaIngreso = ingreso ?? hoy, FechaSalida = salida ?? hoy.AddDays(1) }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        ModelState.Remove(nameof(reserva.IdHuespedNavigation)); ModelState.Remove(nameof(reserva.IdHabitacionNavigation));
        if (reserva.FechaSalida <= reserva.FechaIngreso) ModelState.AddModelError(nameof(reserva.FechaSalida), "La salida debe ser posterior al ingreso.");
        if (reserva.FechaIngreso < DateOnly.FromDateTime(DateTime.Today)) ModelState.AddModelError(nameof(reserva.FechaIngreso), "El ingreso no puede ser anterior a hoy.");
        if (reserva.FechaSalida > reserva.FechaIngreso && await repositorio.EstaReservadaAsync(reserva.IdHabitacion, reserva.FechaIngreso, reserva.FechaSalida)) ModelState.AddModelError(nameof(reserva.IdHabitacion), "La habitación ya está reservada en esas fechas.");
        if (!ModelState.IsValid) { await CargarListas(reserva.IdHuesped, reserva.IdHabitacion); return View(reserva); }
        reserva.Estado = "Pendiente"; reserva.FechaRegistro = DateTime.Now; await repositorio.GuardarAsync(reserva); TempData["Mensaje"] = "La reserva se registró correctamente."; return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id) { TempData["Mensaje"] = await repositorio.CancelarAsync(id) ? "La reserva fue cancelada." : "No se pudo cancelar esta reserva."; return RedirectToAction(nameof(Index)); }
    private async Task CargarListas(int? huespedId = null, int? habitacionId = null)
    {
        ViewBag.Huespedes = new SelectList((await repositorio.HuespedesAsync()).Select(h => new { h.IdHuesped, Nombre = $"{h.Apellidos}, {h.Nombres} · {h.Dni}" }), "IdHuesped", "Nombre", huespedId);
        ViewBag.Habitaciones = new SelectList((await repositorio.HabitacionesActivasAsync()).Select(h => new { h.IdHabitacion, Nombre = $"{h.Numero} · {h.IdTipoHabitacionNavigation.Nombre} · S/ {h.IdTipoHabitacionNavigation.PrecioNoche:N2}" }), "IdHabitacion", "Nombre", habitacionId);
    }
}
