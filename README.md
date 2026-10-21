# Laboratorio 10 · Hotel Aurora

Aplicación ASP.NET Core MVC para administrar habitaciones y reservas con EF Core Database First. La solución no contiene migraciones: el esquema se crea con el SQL del laboratorio y el modelo se genera desde SQL Server.

## Abrir en Visual Studio

Abre `Hotel.sln`. El proyecto está configurado para la instancia SQL Server Express `SQLEXPRESS`. Ejecuta en SQL Server Management Studio el archivo `HotelDB.sql` proporcionado por el docente para crear `HotelDB` y sus datos de ejemplo. **El script elimina una base `HotelDB` existente antes de crearla**, así que úsalo solo si puedes reemplazar esa base. El archivo de datos se conserva localmente y no se publica en este repositorio. Luego ejecuta:

```sql
USE HotelDB;
ALTER TABLE dbo.Reserva ADD Observaciones NVARCHAR(200) NULL;
```

En Visual Studio, abre **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**, selecciona `Hotel.Web` como proyecto predeterminado y ejecuta:

```powershell
Scaffold-DbContext "Server=.\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context HotelContext -Force
```

El comando usado es el solicitado por el laboratorio. `-Force` regenera los archivos existentes en `Models`. Si tu servidor SQL tiene otro nombre, actualiza `ConnectionStrings:HotelDB` y usa esa misma cadena en el comando.

## Ejecutar

Restaura los paquetes NuGet y ejecuta `Hotel.Web` desde Visual Studio. También puedes abrir la terminal en `Hotel.Web` y usar `dotnet run`. Los paquetes de SQL Server, Design y Tools ya están referenciados en el proyecto.

## Funciones

- Habitaciones activas: filtro por tipo, detalle, alta, edición y baja lógica.
- Disponibilidad por fechas, con noches y total estimado; las reservas canceladas no bloquean el rango.
- Registro de reserva con validación de fechas y cruce de reservas, huéspedes y habitaciones activos, y observaciones.
- Listado de reservas con total calculado y cancelación de reservas pendientes o confirmadas.
- Repositorios con consultas asíncronas y `AsNoTracking()` para lecturas.

## Capturas y cierre del laboratorio

Para las capturas solicitadas por el enunciado, ejecuta la aplicación y captura las pantallas de Habitaciones, Disponibilidad, Reservas y el formulario de nueva reserva. Como conclusión: se implementó el flujo Database First con SQL Server, EF Core y repositorios; las reservas aplican disponibilidad por rango de fechas y las habitaciones se dan de baja sin eliminar registros.
