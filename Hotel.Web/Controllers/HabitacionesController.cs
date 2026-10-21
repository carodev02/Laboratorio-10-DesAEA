using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class HabitacionesController(HabitacionRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? tipoId)
    {
        ViewBag.Tipos = new SelectList(await repositorio.TiposAsync(), "IdTipoHabitacion", "Nombre", tipoId);
        ViewBag.TipoId = tipoId;
        return View(await repositorio.ListarAsync(tipoId));
    }
    public async Task<IActionResult> Details(int id) { var h = await repositorio.ObtenerAsync(id); return h is null ? NotFound() : View(h); }
    public async Task<IActionResult> Create() { await CargarTipos(); return View(new Habitacion { Activo = true }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Habitacion habitacion)
    {
        ModelState.Remove(nameof(habitacion.IdTipoHabitacionNavigation)); ModelState.Remove(nameof(habitacion.Reservas));
        if (await repositorio.NumeroExisteAsync(habitacion.Numero ?? "")) ModelState.AddModelError(nameof(habitacion.Numero), "Ese número de habitación ya está registrado.");
        if (!ModelState.IsValid) { await CargarTipos(habitacion.IdTipoHabitacion); return View(habitacion); }
        habitacion.Activo = true; await repositorio.GuardarAsync(habitacion); TempData["Mensaje"] = "La habitación se registró correctamente."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id) { var h = await repositorio.ObtenerAsync(id); if (h is null) return NotFound(); await CargarTipos(h.IdTipoHabitacion); return View(h); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Habitacion habitacion)
    {
        if (id != habitacion.IdHabitacion) return NotFound();
        ModelState.Remove(nameof(habitacion.IdTipoHabitacionNavigation)); ModelState.Remove(nameof(habitacion.Reservas));
        if (await repositorio.NumeroExisteAsync(habitacion.Numero ?? "", id)) ModelState.AddModelError(nameof(habitacion.Numero), "Ese número de habitación ya está registrado.");
        if (!ModelState.IsValid) { await CargarTipos(habitacion.IdTipoHabitacion); return View(habitacion); }
        habitacion.Activo = true; await repositorio.ActualizarAsync(habitacion); TempData["Mensaje"] = "Los cambios se guardaron correctamente."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id) { var h = await repositorio.ObtenerAsync(id); return h is null ? NotFound() : View(h); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) { await repositorio.DesactivarAsync(id); TempData["Mensaje"] = "La habitación se dio de baja."; return RedirectToAction(nameof(Index)); }
    private async Task CargarTipos(int? seleccionado = null) => ViewBag.Tipos = new SelectList(await repositorio.TiposAsync(), "IdTipoHabitacion", "Nombre", seleccionado);
}
