-- ========================================================================
-- INCIDENT MANAGEMENT DB - SQL SERVER
-- Modificada para incluir "Laboratorista Asignador"
-- ========================================================================
-- IMPORTANTE: Ejecutar este script completo con SQLCMD mode habilitado
-- O ejecutar desde la línea de comandos: sqlcmd -S localhost -E -i baseDatos.sql
-- ========================================================================

USE master;
GO

-- Eliminar base de datos si existe
IF DB_ID('IncidentManagementDB') IS NOT NULL
BEGIN
    PRINT 'Eliminando base de datos existente...';
    ALTER DATABASE IncidentManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE IncidentManagementDB;
    PRINT 'Base de datos eliminada.';
END
GO

-- Crear base de datos limpia
PRINT 'Creando base de datos IncidentManagementDB...';
CREATE DATABASE IncidentManagementDB;
PRINT 'Base de datos creada exitosamente.';
GO

USE IncidentManagementDB;
GO

PRINT 'Creando tablas del esquema...';
GO

-- =========================
-- TABLA: Roles
-- =========================
CREATE TABLE Roles (
    RolID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) UNIQUE NOT NULL,
    Descripcion NVARCHAR(500)
);
CREATE INDEX IX_Roles_Nombre ON Roles(Nombre);
GO

-- =========================
-- TABLA: Facultades
-- =========================
CREATE TABLE Facultades (
    FacultadID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(500),
    FechaCreacion DATETIME DEFAULT GETDATE()
);
CREATE INDEX IX_Facultades_Nombre ON Facultades(Nombre);
GO

-- =========================
-- TABLA: Laboratorios
-- =========================
CREATE TABLE Laboratorios (
    LaboratorioID INT PRIMARY KEY IDENTITY(1,1),
    FacultadID INT NOT NULL,
    Nombre NVARCHAR(255) NOT NULL,
    Ubicacion NVARCHAR(500),
    Capacidad INT,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FacultadID) REFERENCES Facultades(FacultadID) ON DELETE NO ACTION
);
CREATE INDEX IX_Laboratorios_FacultadID ON Laboratorios(FacultadID);
GO

-- =========================
-- TABLA: Computadoras
-- =========================
CREATE TABLE Computadoras (
    ComputadoraID INT PRIMARY KEY IDENTITY(1,1),
    LaboratorioID INT NOT NULL,
    CodigoEquipo NVARCHAR(50) UNIQUE NOT NULL,
    NumeroSerie NVARCHAR(100) NULL,
    Modelo NVARCHAR(255),
    SistemaOperativo NVARCHAR(100),
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (LaboratorioID) REFERENCES Laboratorios(LaboratorioID) ON DELETE NO ACTION
);
CREATE INDEX IX_Computadoras_LaboratorioID ON Computadoras(LaboratorioID);
GO

-- =========================
-- TABLA: Usuarios
-- =========================
CREATE TABLE Usuarios (
    UsuarioID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) UNIQUE NOT NULL,
    NombreCompleto NVARCHAR(255) NOT NULL,
    Contraseña NVARCHAR(255) NOT NULL,
    RolID INT NOT NULL,
    FacultadID INT NULL,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaModificacion DATETIME DEFAULT GETDATE(),
    Telefono NVARCHAR(50) NULL,
    Departamento NVARCHAR(255) NULL,
    EsAsignador BIT DEFAULT 0, -- CAMPO NUEVO: Laboratorista asignador
    FOREIGN KEY (RolID) REFERENCES Roles(RolID) ON DELETE NO ACTION,
    FOREIGN KEY (FacultadID) REFERENCES Facultades(FacultadID) ON DELETE SET NULL
);
CREATE INDEX IX_Usuarios_RolID ON Usuarios(RolID);
CREATE INDEX IX_Usuarios_FacultadID ON Usuarios(FacultadID);
CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
GO

-- =========================
-- TABLA: CatalogoServicios
-- =========================
CREATE TABLE CatalogoServicios (
    ServicioID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX),
    TipoSolucion NVARCHAR(255) NULL,
    TiempoResolucionPromedio INT NULL,
    UsuariosPermitidos NVARCHAR(MAX) NULL,
    ResponsableUsuarioID INT NULL,
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ResponsableUsuarioID) REFERENCES Usuarios(UsuarioID) ON DELETE SET NULL
);
CREATE INDEX IX_CatalogoServicios_Nombre ON CatalogoServicios(Nombre);
GO

-- =========================
-- TABLA: ContratosSLA
-- =========================
CREATE TABLE ContratosSLA (
    SLA_ID INT PRIMARY KEY IDENTITY(1,1),
    ServicioID INT NOT NULL,
    NombreContrato NVARCHAR(255) NULL,
    Prioridad NVARCHAR(50) NULL,
    TiempoRespuesta INT NULL,
    TiempoResolucion INT NULL,
    ProveedorUsuarioID INT NULL,
    FechaInicio DATETIME NULL,
    FechaFin DATETIME NULL,
    Disponibilidad INT DEFAULT 99,
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ServicioID) REFERENCES CatalogoServicios(ServicioID) ON DELETE CASCADE,
    FOREIGN KEY (ProveedorUsuarioID) REFERENCES Usuarios(UsuarioID) ON DELETE SET NULL
);
CREATE INDEX IX_ContratosSLA_ServicioID ON ContratosSLA(ServicioID);
GO

-- =========================
-- TABLA: Incidentes
-- =========================
CREATE TABLE Incidentes (
    IncidenteID INT PRIMARY KEY IDENTITY(1,1),
    CodigoIncidente NVARCHAR(50) UNIQUE NOT NULL,
    ComputadoraID INT NOT NULL,
    UsuarioReportadorID INT NOT NULL,
    FacultadID INT NULL,
    LaboratorioID INT NULL,
    ServicioID INT NULL,
    Titulo NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Prioridad NVARCHAR(50) DEFAULT 'Media',
    Estado NVARCHAR(50) DEFAULT 'Abierto',
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME DEFAULT GETDATE(),
    FechaResolucion DATETIME NULL,
    Eliminado BIT DEFAULT 0,
    FOREIGN KEY (ComputadoraID) REFERENCES Computadoras(ComputadoraID) ON DELETE NO ACTION,
    FOREIGN KEY (UsuarioReportadorID) REFERENCES Usuarios(UsuarioID) ON DELETE NO ACTION,
    FOREIGN KEY (FacultadID) REFERENCES Facultades(FacultadID) ON DELETE SET NULL,
    FOREIGN KEY (LaboratorioID) REFERENCES Laboratorios(LaboratorioID) ON DELETE SET NULL,
    FOREIGN KEY (ServicioID) REFERENCES CatalogoServicios(ServicioID) ON DELETE SET NULL
);
CREATE INDEX IX_Incidentes_Estado ON Incidentes(Estado);
CREATE INDEX IX_Incidentes_Prioridad ON Incidentes(Prioridad);
CREATE INDEX IX_Incidentes_FechaCreacion ON Incidentes(FechaCreacion);
GO

-- =========================
-- TABLA: AsignacionesIncidentes
-- =========================
CREATE TABLE AsignacionesIncidentes (
    AsignacionID INT PRIMARY KEY IDENTITY(1,1),
    IncidenteID INT NOT NULL,
    UsuarioAsignadoID INT NULL,
    TipoAsignacion NVARCHAR(50),
    EstadoAsignacion NVARCHAR(50) DEFAULT 'Pendiente',
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    FechaAceptacion DATETIME NULL,
    FechaCompletacion DATETIME NULL,
    Notas NVARCHAR(MAX) NULL,
    FOREIGN KEY (IncidenteID) REFERENCES Incidentes(IncidenteID) ON DELETE CASCADE,
    FOREIGN KEY (UsuarioAsignadoID) REFERENCES Usuarios(UsuarioID) ON DELETE SET NULL
);
CREATE INDEX IX_AsignacionesIncidentes_IncidenteID ON AsignacionesIncidentes(IncidenteID);
GO

-- =========================
-- TABLA: ActualizacionesIncidentes
-- =========================
CREATE TABLE ActualizacionesIncidentes (
    ActualizacionID INT PRIMARY KEY IDENTITY(1,1),
    IncidenteID INT NOT NULL,
    UsuarioID INT NOT NULL,
    TipoActualizacion NVARCHAR(100),
    Descripcion NVARCHAR(MAX) NULL,
    ValorAnterior NVARCHAR(MAX) NULL,
    ValorNuevo NVARCHAR(MAX) NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IncidenteID) REFERENCES Incidentes(IncidenteID) ON DELETE CASCADE,
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID) ON DELETE NO ACTION
);
CREATE INDEX IX_ActualizacionesIncidentes_IncidenteID ON ActualizacionesIncidentes(IncidenteID);
GO

-- =========================
-- TABLA: BaseConocimientos
-- =========================
CREATE TABLE BaseConocimientos (
    SolucionID INT PRIMARY KEY IDENTITY(1,1),
    Titulo NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Problema NVARCHAR(MAX) NOT NULL,
    Solucion NVARCHAR(MAX) NOT NULL,
    Categoria NVARCHAR(100) NULL,
    Palabras_Clave NVARCHAR(500) NULL,
    IncidenteRelacionadoID INT NULL,
    UsuarioCreadorID INT NOT NULL,
    TiempoResolucion INT NULL,
    Efectividad INT DEFAULT 0,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IncidenteRelacionadoID) REFERENCES Incidentes(IncidenteID) ON DELETE SET NULL,
    FOREIGN KEY (UsuarioCreadorID) REFERENCES Usuarios(UsuarioID) ON DELETE NO ACTION
);
CREATE INDEX IX_BaseConocimientos_Categoria ON BaseConocimientos(Categoria);
GO

-- =========================
-- TABLA: Notificaciones
-- =========================
CREATE TABLE Notificaciones (
    NotificacionID INT PRIMARY KEY IDENTITY(1,1),
    UsuarioID INT NOT NULL,
    IncidenteID INT NULL,
    Titulo NVARCHAR(255) NOT NULL,
    Mensaje NVARCHAR(MAX) NULL,
    Tipo NVARCHAR(50) NULL,
    Leida BIT DEFAULT 0,
    URLAccion NVARCHAR(1000) NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaLectura DATETIME NULL,
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID) ON DELETE CASCADE,
    FOREIGN KEY (IncidenteID) REFERENCES Incidentes(IncidenteID) ON DELETE SET NULL
);
CREATE INDEX IX_Notificaciones_Usuario ON Notificaciones(UsuarioID);
GO

-- =========================
-- TABLA: SuscripcionesPush
-- =========================
CREATE TABLE SuscripcionesPush (
    SuscripcionID INT PRIMARY KEY IDENTITY(1,1),
    UsuarioID INT NOT NULL,
    Endpoint NVARCHAR(MAX) NOT NULL,
    P256DH NVARCHAR(MAX) NULL,
    Auth NVARCHAR(MAX) NULL,
    Activa BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID) ON DELETE CASCADE
);
CREATE INDEX IX_SuscripcionesPush_Usuario ON SuscripcionesPush(UsuarioID);
GO

-- ========================================================================
-- DATOS DE PRUEBA / SEED DATA
-- ========================================================================
USE IncidentManagementDB;
GO

-- Verificar que las tablas existen antes de insertar
IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    RAISERROR('Error: Las tablas no se han creado correctamente. Revise los pasos anteriores.', 16, 1);
    RETURN;
END
GO

-- =========================
-- INSERTAR ROLES
-- =========================
INSERT INTO Roles (Nombre, Descripcion)
VALUES 
('Estudiante', 'Usuario que reporta incidentes desde la facultad'),
('Docente', 'Usuario que reporta incidentes y solicita servicios'),
('Administrativo', 'Personal administrativo que puede reportar incidentes'),
('Laboratorista', 'Usuario que atiende incidentes y puede asignarlos');
GO

-- =========================
-- INSERTAR FACULTADES
-- =========================
INSERT INTO Facultades (Nombre, Descripcion)
VALUES 
('Facultad de Ingeniería', 'Carreras de ingeniería'),
('Facultad de Ciencias', 'Carreras de ciencias'),
('Facultad de Medicina', 'Carreras de medicina');
GO

-- =========================
-- INSERTAR LABORATORIOS
-- =========================
INSERT INTO Laboratorios (FacultadID, Nombre, Ubicacion, Capacidad)
VALUES
(1, 'Laboratorio de Redes', 'Edif A - Piso 2', 30),
(1, 'Laboratorio de CTT', 'Edif A - Piso 3', 25),
(2, 'Laboratorio de Computacion', 'Edif A - Piso 1', 20),
(3, 'Laboratorio de Redes 2', 'Edif A - Piso 2', 15);
GO

-- =========================
-- INSERTAR USUARIOS
-- =========================
INSERT INTO Usuarios (Email, NombreCompleto, Contraseña, RolID, FacultadID, EsAsignador)
VALUES
('estudiante1@uni.edu', 'Juan Pérez', 'pass123', 1, 1, 0),
('estudiante2@uni.edu', 'Lucía Fernández', 'pass123', 1, 2, 0),
('docente1@uni.edu', 'María García', 'pass123', 2, 1, 0),
('docente2@uni.edu', 'José Ramírez', 'pass123', 2, 3, 0),
('admin1@uni.edu', 'Pedro Morales', 'pass123', 3, 1, 0),
('lab1@uni.edu', 'Carlos López', 'pass123', 4, 1, 1),
('lab2@uni.edu', 'Ana Gómez', 'pass123', 4, 2, 1),
('lab3@uni.edu', 'Roberto Díaz', 'pass123', 4, 1, 0);--USUARIO 
GO

-- =========================
-- INSERTAR COMPUTADORAS
-- =========================
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(1, 'INF-LAB01-PC01', 'SN1001', 'Dell Optiplex', 'Windows 10'),
(1, 'INF-LAB01-PC02', 'SN1002', 'Dell Optiplex', 'Windows 10'),
(2, 'PRG-LAB01-PC01', 'SN2001', 'HP ProDesk', 'Windows 11'),
(3, 'FIS-LAB01-PC01', 'SN3001', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC02', 'SN3002', 'Dell Optiplex', 'Ubuntu 22.04');
GO

-- LOTES ADICIONALES DE COMPUTADORAS
-- Laboratorio de Informática (1): PCs 03-20
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(1, 'INF-LAB01-PC03', 'SN1003', 'Dell Optiplex', 'Windows 10'),
(1, 'INF-LAB01-PC04', 'SN1004', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC05', 'SN1005', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC06', 'SN1006', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC07', 'SN1007', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC08', 'SN1008', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC09', 'SN1009', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC10', 'SN1010', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC11', 'SN1011', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC12', 'SN1012', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC13', 'SN1013', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC14', 'SN1014', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC15', 'SN1015', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC16', 'SN1016', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC17', 'SN1017', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC18', 'SN1018', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC19', 'SN1019', 'Dell Optiplex', 'Windows 11'),
(1, 'INF-LAB01-PC20', 'SN1020', 'Dell Optiplex', 'Windows 11');
GO

-- Laboratorio de Programación (2): PCs 02-20
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(2, 'PRG-LAB01-PC02', 'SN2002', 'HP ProDesk', 'Windows 11'),
(2, 'PRG-LAB01-PC03', 'SN2003', 'HP ProDesk', 'Windows 11'),
(2, 'PRG-LAB01-PC04', 'SN2004', 'HP ProDesk', 'Windows 11'),
(2, 'PRG-LAB01-PC05', 'SN2005', 'HP ProDesk', 'Windows 11'),
(2, 'PRG-LAB01-PC06', 'SN2006', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC07', 'SN2007', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC08', 'SN2008', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC09', 'SN2009', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC10', 'SN2010', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC11', 'SN2011', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC12', 'SN2012', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC13', 'SN2013', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC14', 'SN2014', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC15', 'SN2015', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC16', 'SN2016', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC17', 'SN2017', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC18', 'SN2018', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC19', 'SN2019', 'HP Z2', 'Windows 11'),
(2, 'PRG-LAB01-PC20', 'SN2020', 'HP Z2', 'Windows 11');
GO

-- Laboratorio de Física (3): PCs 03-20
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(3, 'FIS-LAB01-PC03', 'SN3003', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC04', 'SN3004', 'Lenovo ThinkCentre', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC05', 'SN3005', 'Lenovo ThinkCentre', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC06', 'SN3006', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC07', 'SN3007', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC08', 'SN3008', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC09', 'SN3009', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC10', 'SN3010', 'Lenovo ThinkCentre', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC11', 'SN3011', 'Dell Optiplex', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC12', 'SN3012', 'Dell Optiplex', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC13', 'SN3013', 'Dell Optiplex', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC14', 'SN3014', 'Dell Optiplex', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC15', 'SN3015', 'Dell Optiplex', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC16', 'SN3016', 'Dell Optiplex', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC17', 'SN3017', 'Dell Optiplex', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC18', 'SN3018', 'Dell Optiplex', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC19', 'SN3019', 'Dell Optiplex', 'Ubuntu 24.04'),
(3, 'FIS-LAB01-PC20', 'SN3020', 'Dell Optiplex', 'Ubuntu 24.04');
GO
-- Más equipos en distintos laboratorios (bloque adicional)
-- Nota: ya se insertaron lotes arriba; este bloque es opcional.
-- Si se requiere, descomente y ejecute con un INSERT propio.
-- INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
-- VALUES
-- (1, 'INF-LAB01-PC03', 'SN1003', 'Dell Optiplex', 'Windows 10'),
-- (1, 'INF-LAB01-PC04', 'SN1004', 'Dell Optiplex', 'Windows 11'),
-- (1, 'INF-LAB01-PC05', 'SN1005', 'Dell Optiplex', 'Windows 11'),
-- (2, 'PRG-LAB01-PC02', 'SN2002', 'HP ProDesk', 'Windows 11'),
-- (2, 'PRG-LAB01-PC03', 'SN2003', 'HP ProDesk', 'Windows 11'),
-- (2, 'PRG-LAB01-PC04', 'SN2004', 'HP Z2', 'Windows 11'),
-- (2, 'PRG-LAB01-PC05', 'SN2005', 'HP Z2', 'Windows 11'),
-- (3, 'FIS-LAB01-PC03', 'SN3003', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
-- (3, 'FIS-LAB01-PC04', 'SN3004', 'Lenovo ThinkCentre', 'Ubuntu 24.04'),
-- (3, 'FIS-LAB01-PC05', 'SN3005', 'Dell Optiplex', 'Ubuntu 22.04');

-- =========================
-- INSERTAR CATALOGO DE SERVICIOS
-- =========================
INSERT INTO CatalogoServicios (Nombre, Descripcion, TipoSolucion, UsuariosPermitidos, ResponsableUsuarioID)
VALUES
('Reparación de Hardware', 'Reparación de componentes físicos', 'Hardware', '["Laboratorista","Experto"]', 6),
('Instalación de Software', 'Instalación de software autorizado', 'Software', '["Laboratorista","Docente"]', 6),
('Mantenimiento Preventivo', 'Chequeo periódico de equipos', 'Hardware', '["Laboratorista","Administrativo"]', 7);
GO

-- =========================
-- INSERTAR CONTRATOS SLA
-- =========================
INSERT INTO ContratosSLA (ServicioID, NombreContrato, Prioridad, TiempoRespuesta, TiempoResolucion, ProveedorUsuarioID)
VALUES
(1, 'SLA-Hardware', 'Alta', 2, 24, 9),
(2, 'SLA-Software', 'Media', 4, 48, 9),
(3, 'SLA-Mantenimiento', 'Baja', 8, 72, 9);
GO

-- =========================
-- INSERTAR INCIDENTES
-- =========================
INSERT INTO Incidentes (CodigoIncidente, ComputadoraID, UsuarioReportadorID, FacultadID, LaboratorioID, ServicioID, Titulo, Descripcion)
VALUES
('INC-001', 1, 1, 1, 1, 1, 'PC no enciende', 'La computadora no enciende ni hace ningún sonido'),
('INC-002', 2, 3, 1, 1, 2, 'Instalación de Office', 'Necesito instalar Office 2021 en esta computadora'),
('INC-003', 3, 2, 2, 2, 3, 'Mantenimiento Preventivo', 'Solicito revisión de todos los PCs del laboratorio'),
('INC-004', 4, 4, 3, 3, 1, 'Problema de red', 'No hay conexión en algunas computadoras'),
('INC-005', 5, 2, 2, 3, 2, 'Actualización de software', 'Actualizar Ubuntu a última versión'),
('INC-006', 6, 1, 1, 1, 2, 'Error al iniciar sesión', 'Usuario no puede iniciar sesión en Windows'),
('INC-007', 7, 3, 1, 1, 2, 'Falta impresora', 'Impresora no aparece en dispositivos'),
('INC-008', 8, 4, 2, 2, 1, 'Ruido en CPU', 'Equipo hace ruido excesivo'),
('INC-009', 9, 2, 2, 2, 3, 'Limpieza y revisión', 'Solicitud de mantenimiento general'),
('INC-010', 10, 1, 3, 3, 1, 'Pantalla parpadea', 'Monitor parpadea cada cierto tiempo'),
('INC-011', 11, 3, 3, 3, 2, 'Instalar Python', 'Instalar Python y VS Code'),
('INC-012', 12, 4, 1, 1, 2, 'Office no activa', 'No se puede activar Office'),
('INC-013', 13, 2, 1, 1, 1, 'Temperatura alta', 'CPU supera 85°C en carga'),
('INC-014', 14, 1, 2, 2, 3, 'Actualizar drivers', 'Drivers de red desactualizados'),
('INC-015', 15, 4, 3, 3, 1, 'Sin audio', 'No se escucha audio en el equipo');
GO

-- =========================
-- INSERTAR ASIGNACIONES
-- =========================
INSERT INTO AsignacionesIncidentes (IncidenteID, UsuarioAsignadoID, TipoAsignacion)
VALUES
(1, 6, 'Laboratorista'),
(2, 6, 'Laboratorista'),
(3, 7, 'Laboratorista'),
(4, 7, 'Laboratorista'),
(5, 6, 'Laboratorista'),
(6, 6, 'Laboratorista'),
(7, 6, 'Laboratorista'),
(8, 7, 'Laboratorista'),
(9, 7, 'Laboratorista'),
(10, 6, 'Laboratorista'),
(11, 7, 'Laboratorista'),
(12, 6, 'Laboratorista'),
(13, 7, 'Laboratorista'),
(14, 6, 'Laboratorista'),
(15, 7, 'Laboratorista');
GO

-- =========================
-- INSERTAR ACTUALIZACIONES INCIDENTES
-- =========================
INSERT INTO ActualizacionesIncidentes (IncidenteID, UsuarioID, TipoActualizacion, Descripcion)
VALUES
(1, 6, 'Asignado', 'Incidente asignado al laboratorista Carlos López'),
(2, 6, 'Asignado', 'Incidente asignado al laboratorista Carlos López'),
(3, 7, 'Asignado', 'Incidente asignado al laboratorista Ana Gómez'),
(4, 7, 'Asignado', 'Incidente asignado al laboratorista Ana Gómez'),
(5, 6, 'Asignado', 'Incidente asignado al laboratorista Carlos López'),
(6, 6, 'Diagnóstico', 'Se revisaron credenciales y políticas de dominio'),
(7, 6, 'Instalación', 'Se instaló driver de impresora y se agregó mapeo'),
(8, 7, 'Revisión Hardware', 'Se ajustó ventilador y se limpió polvo'),
(9, 7, 'Mantenimiento', 'Se realizó limpieza general y verificación'),
(10, 6, 'Verificación', 'Se cambió cable de video y revisó fuente'),
(11, 7, 'Instalación', 'Se instaló Python 3.12 y VS Code'),
(12, 6, 'Activación', 'Se validó KMS y se activó Office'),
(13, 7, 'Optimización', 'Se mejoró flujo aire y se aplicó pasta térmica'),
(14, 6, 'Actualización', 'Se actualizaron drivers de red a última versión'),
(15, 7, 'Diagnóstico', 'Se reinstaló driver de audio y verificó conexiones');
GO

-- Soluciones en Base de Conocimiento ligadas a incidentes
INSERT INTO BaseConocimientos (Titulo, Descripcion, Problema, Solucion, Categoria, Palabras_Clave, IncidenteRelacionadoID, UsuarioCreadorID, TiempoResolucion, Efectividad)
VALUES
('Error de inicio de sesión', 'Reset de perfil y políticas', 'No inicia sesión', 'Restablecer perfil y revisar políticas de grupo', 'Sistema', 'perfil,políticas,login', 6, 6, 60, 80),
('Impresora no aparece', 'Instalación y mapeo de impresora', 'No se ve impresora', 'Instalar driver y mapear impresora en servidor', 'Periféricos', 'impresora,driver,mapeo', 7, 6, 40, 85),
('Ruido excesivo CPU', 'Mantenimiento de ventilación', 'CPU hace ruido', 'Limpieza y ajuste de ventiladores', 'Hardware', 'ventilador,ruido,mantenimiento', 8, 7, 30, 90),
('Mantenimiento general', 'Checklist de mantenimiento', 'Solicitud de limpieza', 'Aplicar checklist estándar de mantenimiento', 'Hardware', 'mantenimiento,limpieza,checklist', 9, 7, 120, 75),
('Pantalla parpadea', 'Diagnóstico de cable y fuente', 'Parpadeo de monitor', 'Cambiar cable y validar fuente', 'Hardware', 'pantalla,cable,fuente', 10, 6, 25, 70),
('Instalación Python y VS Code', 'Guía de instalación y configuración', 'Instalar herramientas', 'Instalar Python y configurar VS Code', 'Software', 'python,vscode,instalación', 11, 7, 35, 88),
('Activación de Office', 'Validación KMS', 'Office no activa', 'Verificar conexión a KMS y activar', 'Software', 'office,kms,activación', 12, 6, 20, 82),
('Temperatura alta', 'Mejoras de refrigeración', 'CPU se calienta', 'Limpiar disipador y aplicar pasta térmica', 'Hardware', 'temperatura,pasta térmica', 13, 7, 50, 86),
('Drivers de red', 'Procedimiento de actualización', 'Red inestable', 'Actualizar drivers y reiniciar equipo', 'Red', 'driver,red,actualización', 14, 6, 30, 78),
('Sin audio', 'Reinstalación de driver', 'No hay audio', 'Reinstalar driver de audio y validar BIOS', 'Hardware', 'audio,driver,bios', 15, 7, 25, 83);
GO
