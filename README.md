# Sistema de Gestión de Incidentes - Universidad

Sistema de gestión de incidentes para laboratorios de computación universitarios, construido con arquitectura Onion en ASP.NET Core 8 y MudBlazor.

## 🏗️ Arquitectura

Este proyecto implementa **Onion Architecture** (Arquitectura Cebolla) con las siguientes capas:

```
IncidentManagement/
├── src/
│   ├── Domain/              # Núcleo - Entidades de negocio
│   ├── Application/         # Lógica de aplicación - Interfaces, DTOs, Services
│   ├── Infrastructure/      # Implementación - EF Core, Repositories
│   └── Web/                 # Presentación - Blazor Server con MudBlazor
```

### Capas

1. **Domain (Dominio)**: Entidades del negocio sin dependencias externas
2. **Application (Aplicación)**: Interfaces, DTOs y servicios de aplicación
3. **Infrastructure (Infraestructura)**: Implementación de repositorios y acceso a datos
4. **Web (Presentación)**: UI con Blazor Server y MudBlazor

## 🚀 Tecnologías

- **.NET 8.0**
- **ASP.NET Core Blazor Server**
- **MudBlazor** - UI Components
- **Entity Framework Core 8.0**
- **SQL Server**

## 👥 Roles de Usuario

- **Estudiante**: Reportar incidentes y consultar estado
- **Personal Administrativo**: Reportar incidentes
- **Laboratorista**: Atender incidentes y gestionar soluciones
  - **Laboratorista Asignador**: Puede asignar incidentes a otros laboratoristas, expertos o proveedores

## ✨ Características Principales

- ✅ Gestión de incidentes (Crear, Ver, Actualizar, Eliminar)
- ✅ Base de conocimientos para soluciones
- ✅ Sistema de notificaciones
- ✅ Autenticación y autorización por roles
- ✅ Dashboard personalizado por rol
- ✅ Seguimiento de estado de incidentes
- ✅ Asignación de incidentes a técnicos

## 🛠️ Configuración

### Requisitos Previos

- .NET 8.0 SDK
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o VS Code

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   cd "d:\PROYECTOS\PORYECTO IHC\IncidentManagement"
   ```

2. **Configurar la cadena de conexión**
   
   Editar `src/Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=IncidentManagementDB;Integrated Security=true;TrustServerCertificate=True;"
     }
   }
   ```

3. **Crear la base de datos**
   
   Ejecutar el script SQL ubicado en la raíz del proyecto:
   ```bash
   # En SQL Server Management Studio o Azure Data Studio
   # Ejecutar: baseDatos.sql
   ```

4. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

5. **Ejecutar la aplicación**
   ```bash
   cd src/Web
   dotnet run
   ```

6. **Acceder a la aplicación**
   
   Abrir navegador en: `https://localhost:5001` o `http://localhost:5000`

## 🔐 Usuarios de Prueba

| Usuario | Email | Contraseña | Rol |
|---------|-------|------------|-----|
| Juan Pérez | estudiante1@uni.edu | pass123 | Estudiante |
| Pedro Morales | admin1@uni.edu | pass123 | Administrativo |
| Carlos López | lab1@uni.edu | pass123 | Laboratorista (Asignador) |
| Ana Gómez | lab2@uni.edu | pass123 | Laboratorista (Asignador) |

## 📁 Estructura del Proyecto

```
IncidentManagement/
├── baseDatos.sql                          # Script SQL inicial
├── IncidentManagement.sln                 # Solución
└── src/
    ├── Domain/
    │   ├── Entities/                      # Entidades del dominio
    │   │   ├── Usuario.cs
    │   │   ├── Incidente.cs
    │   │   ├── BaseConocimiento.cs
    │   │   └── ...
    │   └── Domain.csproj
    │
    ├── Application/
    │   ├── DTOs/                          # Data Transfer Objects
    │   ├── Interfaces/                    # Interfaces de repositorios y servicios
    │   ├── Services/                      # Servicios de aplicación
    │   └── Application.csproj
    │
    ├── Infrastructure/
    │   ├── Data/
    │   │   └── ApplicationDbContext.cs    # DbContext de EF Core
    │   ├── Repositories/                  # Implementación de repositorios
    │   └── Infrastructure.csproj
    │
    └── Web/
        ├── Components/
        │   ├── Pages/                     # Páginas Blazor
        │   │   ├── Login.razor
        │   │   ├── Estudiante/
        │   │   ├── Administrativo/
        │   │   └── Laboratorista/
        │   ├── Layout/                    # Layouts
        │   └── _Imports.razor
        ├── Services/
        │   ├── AuthService.cs
        │   └── CustomAuthenticationStateProvider.cs
        ├── Program.cs
        ├── appsettings.json
        └── Web.csproj
```

## 🎯 Funcionalidades por Rol

### Estudiante / Docente
- Dashboard con resumen de incidentes
- Reportar nuevos incidentes
- Ver estado de incidentes propios
- Eliminar incidentes (solo si están en estado "Abierto")
- Consultar base de conocimientos
- Recibir notificaciones

### Personal Administrativo
- Dashboard administrativo
- Reportar incidentes
- Ver estado de incidentes
- Consultar base de conocimientos
- Gestionar incidentes de su facultad

### Laboratorista
- Dashboard de trabajo
- Ver incidentes asignados
- Actualizar estado de incidentes
- Registrar soluciones en base de conocimientos
- Consultar base de conocimientos
- **Laboratorista Asignador**: Asignar incidentes a otros técnicos

## 🔄 Flujo de Trabajo

1. **Usuario reporta incidente** → Sistema genera código único
2. **Laboratorista Asignador** → Asigna incidente a técnico apropiado
3. **Laboratorista** → Atiende incidente, cambia estado
4. **Resolución** → Laboratorista registra solución en base de conocimientos
5. **Cierre** → Incidente marcado como resuelto/cerrado

## 📊 Base de Datos

El sistema utiliza las siguientes tablas principales:

- `Usuarios` - Información de usuarios
- `Roles` - Roles del sistema
- `Facultades` - Facultades de la universidad
- `Laboratorios` - Laboratorios de cómputo
- `Computadoras` - Equipos de cómputo
- `Incidentes` - Incidentes reportados
- `BaseConocimientos` - Soluciones documentadas
- `Notificaciones` - Sistema de notificaciones
- `AsignacionesIncidentes` - Asignación de incidentes a técnicos
- `CatalogoServicios` - Catálogo de servicios
- `ContratosSLA` - Contratos de nivel de servicio

## 🛡️ Seguridad

- Autenticación basada en sesión
- Autorización por roles
- Protección de rutas con `[Authorize]`
- Validación de permisos en operaciones

## 📝 Próximas Mejoras

- [ ] Implementar notificaciones push
- [ ] Agregar reportes y estadísticas
- [ ] Sistema de chat/comentarios en incidentes
- [ ] Upload de archivos adjuntos
- [ ] API REST para integración externa
- [ ] Panel de métricas y KPIs

## 👨‍💻 Desarrollo

Para ejecutar en modo desarrollo:

```bash
cd src/Web
dotnet watch run
```

## 📄 Licencia

Este proyecto es para uso educativo.

## 🤝 Contribución

Este es un proyecto universitario. Para contribuir, crear un fork y enviar pull requests.

---

Desarrollado con ❤️ para la gestión eficiente de incidentes en laboratorios universitarios.
