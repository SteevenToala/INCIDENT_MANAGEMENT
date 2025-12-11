-- ========================================================================
-- INCIDENT MANAGEMENT DB - SCRIPT COMPLETO Y CONSOLIDADO
-- Sistema de Gestión de Incidentes Universitarios
-- Incluye: Estructura completa, roles, datos de prueba y técnicos expertos
-- ========================================================================
-- INSTRUCCIONES DE EJECUCIÓN:
-- 1. Abrir SQL Server Management Studio (SSMS)
-- 2. Conectarse al servidor SQL Server
-- 3. Abrir este archivo
-- 4. Habilitar "SQLCMD Mode" en el menú Query > SQLCMD Mode
-- 5. Ejecutar el script completo (F5)
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

-- ========================================================================
-- CREACIÓN DE TABLAS
-- ========================================================================

-- =========================
-- TABLA: Roles
-- =========================
CREATE TABLE Roles (
    RolID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) UNIQUE NOT NULL,
    Descripcion NVARCHAR(500)
);
CREATE INDEX IX_Roles_Nombre ON Roles(Nombre);
PRINT 'Tabla Roles creada.';
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
PRINT 'Tabla Facultades creada.';
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
PRINT 'Tabla Laboratorios creada.';
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
PRINT 'Tabla Computadoras creada.';
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
    EsAsignador BIT DEFAULT 0,
    FOREIGN KEY (RolID) REFERENCES Roles(RolID) ON DELETE NO ACTION,
    FOREIGN KEY (FacultadID) REFERENCES Facultades(FacultadID) ON DELETE SET NULL
);
CREATE INDEX IX_Usuarios_RolID ON Usuarios(RolID);
CREATE INDEX IX_Usuarios_FacultadID ON Usuarios(FacultadID);
CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
PRINT 'Tabla Usuarios creada.';
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
PRINT 'Tabla CatalogoServicios creada.';
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
PRINT 'Tabla ContratosSLA creada.';
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
PRINT 'Tabla Incidentes creada.';
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
PRINT 'Tabla AsignacionesIncidentes creada.';
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
PRINT 'Tabla ActualizacionesIncidentes creada.';
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
PRINT 'Tabla BaseConocimientos creada.';
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
PRINT 'Tabla Notificaciones creada.';
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
PRINT 'Tabla SuscripcionesPush creada.';
GO

-- ========================================================================
-- INSERCIÓN DE DATOS
-- ========================================================================

PRINT '';
PRINT '========================================';
PRINT 'INSERTANDO DATOS INICIALES';
PRINT '========================================';
GO

-- =========================
-- INSERTAR ROLES
-- =========================
PRINT 'Insertando roles...';
INSERT INTO Roles (Nombre, Descripcion)
VALUES 
('Estudiante', 'Usuario que reporta incidentes desde la facultad'),
('Docente', 'Usuario que reporta incidentes y solicita servicios'),
('Administrativo', 'Personal administrativo que gestiona incidentes y asigna expertos'),
('Laboratorista', 'Usuario que atiende incidentes y puede asignarlos'),
('TecnicoExperto', 'Técnico especializado que atiende incidentes complejos');
PRINT 'Roles insertados correctamente.';
GO

-- =========================
-- INSERTAR FACULTADES
-- =========================
PRINT 'Insertando facultades...';
INSERT INTO Facultades (Nombre, Descripcion)
VALUES 
('Facultad de Ingeniería', 'Carreras de ingeniería'),
('Facultad de Ciencias', 'Carreras de ciencias'),
('Facultad de Medicina', 'Carreras de medicina');
PRINT 'Facultades insertadas correctamente.';
GO

-- =========================
-- INSERTAR LABORATORIOS
-- =========================
PRINT 'Insertando laboratorios...';
INSERT INTO Laboratorios (FacultadID, Nombre, Ubicacion, Capacidad)
VALUES
(1, 'Laboratorio de Redes', 'Edif A - Piso 2', 30),
(1, 'Laboratorio de CTT', 'Edif A - Piso 3', 25),
(2, 'Laboratorio de Computacion', 'Edif A - Piso 1', 20),
(3, 'Laboratorio de Redes 2', 'Edif A - Piso 2', 15);
PRINT 'Laboratorios insertados correctamente.';
GO

-- =========================
-- INSERTAR COMPUTADORAS
-- =========================
PRINT 'Insertando computadoras...';

-- Laboratorio de Redes - 20 PCs
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(1, 'INF-LAB01-PC01', 'SN1001', 'Dell Optiplex', 'Windows 10'),
(1, 'INF-LAB01-PC02', 'SN1002', 'Dell Optiplex', 'Windows 10'),
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

-- Laboratorio de CTT - 20 PCs
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(2, 'PRG-LAB01-PC01', 'SN2001', 'HP ProDesk', 'Windows 11'),
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

-- Laboratorio de Computación - 20 PCs
INSERT INTO Computadoras (LaboratorioID, CodigoEquipo, NumeroSerie, Modelo, SistemaOperativo)
VALUES
(3, 'FIS-LAB01-PC01', 'SN3001', 'Lenovo ThinkCentre', 'Ubuntu 22.04'),
(3, 'FIS-LAB01-PC02', 'SN3002', 'Dell Optiplex', 'Ubuntu 22.04'),
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
PRINT 'Computadoras insertadas correctamente (60 equipos).';
GO

-- =========================
-- INSERTAR USUARIOS
-- =========================
PRINT 'Insertando usuarios...';
INSERT INTO Usuarios (Email, NombreCompleto, Contraseña, RolID, FacultadID, EsAsignador)
VALUES
-- Estudiantes
('estudiante1@uni.edu', 'Juan Pérez', 'pass123', 1, 1, 0),
('estudiante2@uni.edu', 'Lucía Fernández', 'pass123', 1, 2, 0),
-- Docentes
('docente1@uni.edu', 'María García', 'pass123', 2, 1, 0),
('docente2@uni.edu', 'José Ramírez', 'pass123', 2, 3, 0),
-- Administrativos
('admin1@uni.edu', 'Pedro Morales', 'pass123', 3, 1, 0),
-- Laboratoristas
('lab1@uni.edu', 'Carlos López', 'pass123', 4, 1, 1),
('lab2@uni.edu', 'Ana Gómez', 'pass123', 4, 2, 1),
('lab3@uni.edu', 'Roberto Díaz', 'pass123', 4, 1, 0),
-- Técnicos Expertos
('experto.hardware@uni.edu', 'Luis Martinez - Experto Hardware', 'pass123', 5, 1, 0),
('experto.software@uni.edu', 'Carmen Rodriguez - Experta Software', 'pass123', 5, 1, 0),
('experto.redes@uni.edu', 'Miguel Sanchez - Experto Redes', 'pass123', 5, 2, 0);
PRINT 'Usuarios insertados correctamente.';
GO

-- =========================
-- INSERTAR CATALOGO DE SERVICIOS
-- =========================
PRINT 'Insertando catálogo de servicios...';
INSERT INTO CatalogoServicios (Nombre, Descripcion, TipoSolucion, UsuariosPermitidos, ResponsableUsuarioID)
VALUES
('Reparación de Hardware', 'Reparación de componentes físicos', 'Hardware', '["Laboratorista","TecnicoExperto"]', 6),
('Instalación de Software', 'Instalación de software autorizado', 'Software', '["Laboratorista","Docente"]', 6),
('Mantenimiento Preventivo', 'Chequeo periódico de equipos', 'Hardware', '["Laboratorista","Administrativo"]', 7);
PRINT 'Catálogo de servicios insertado correctamente.';
GO

-- =========================
-- INSERTAR CONTRATOS SLA
-- =========================
PRINT 'Insertando contratos SLA...';
INSERT INTO ContratosSLA (ServicioID, NombreContrato, Prioridad, TiempoRespuesta, TiempoResolucion, ProveedorUsuarioID)
VALUES
(1, 'SLA-Hardware', 'Alta', 2, 24, 6),
(2, 'SLA-Software', 'Media', 4, 48, 7),
(3, 'SLA-Mantenimiento', 'Baja', 8, 72, 8);
PRINT 'Contratos SLA insertados correctamente.';
GO

-- =========================
-- INSERTAR INCIDENTES DE PRUEBA
-- =========================
PRINT 'Insertando incidentes de prueba...';
INSERT INTO Incidentes (CodigoIncidente, ComputadoraID, UsuarioReportadorID, FacultadID, LaboratorioID, ServicioID, Titulo, Descripcion, Prioridad)
VALUES
('INC-001', 1, 1, 1, 1, 1, 'PC no enciende', 'La computadora no enciende ni hace ningún sonido', 'Alta'),
('INC-002', 2, 3, 1, 1, 2, 'Instalación de Office', 'Necesito instalar Office 2021 en esta computadora', 'Media'),
('INC-003', 3, 2, 2, 2, 3, 'Mantenimiento Preventivo', 'Solicito revisión de todos los PCs del laboratorio', 'Baja'),
('INC-004', 4, 4, 3, 3, 1, 'Problema de red', 'No hay conexión en algunas computadoras', 'Alta'),
('INC-005', 5, 2, 2, 3, 2, 'Actualización de software', 'Actualizar Ubuntu a última versión', 'Media'),
('INC-006', 6, 1, 1, 1, 2, 'Error al iniciar sesión', 'Usuario no puede iniciar sesión en Windows', 'Media'),
('INC-007', 7, 3, 1, 1, 2, 'Falta impresora', 'Impresora no aparece en dispositivos', 'Baja'),
('INC-008', 8, 4, 2, 2, 1, 'Ruido en CPU', 'Equipo hace ruido excesivo', 'Media'),
('INC-009', 9, 2, 2, 2, 3, 'Limpieza y revisión', 'Solicitud de mantenimiento general', 'Baja'),
('INC-010', 10, 1, 3, 3, 1, 'Pantalla parpadea', 'Monitor parpadea cada cierto tiempo', 'Alta');
PRINT 'Incidentes de prueba insertados correctamente.';
GO

-- =========================
-- INSERTAR ASIGNACIONES
-- =========================
PRINT 'Insertando asignaciones de incidentes...';
INSERT INTO AsignacionesIncidentes (IncidenteID, UsuarioAsignadoID, TipoAsignacion, EstadoAsignacion)
VALUES
(1, 6, 'Laboratorista', 'Pendiente'),
(2, 6, 'Laboratorista', 'Pendiente'),
(3, 7, 'Laboratorista', 'Pendiente'),
(4, 7, 'Laboratorista', 'Pendiente'),
(5, 6, 'Laboratorista', 'Pendiente');
PRINT 'Asignaciones insertadas correctamente.';
GO

-- =========================
-- INSERTAR BASE DE CONOCIMIENTO
-- =========================
PRINT 'Insertando soluciones en base de conocimiento...';
INSERT INTO BaseConocimientos (Titulo, Descripcion, Problema, Solucion, Categoria, Palabras_Clave, UsuarioCreadorID, TiempoResolucion, Efectividad)
VALUES
('PC no enciende - Revisión de alimentación', 'Procedimiento para diagnosticar problemas de encendido', 
 'Computadora no enciende, no hay señales de actividad', 
 '1. Verificar cable de alimentación\n2. Revisar fuente de poder\n3. Comprobar botón de encendido\n4. Verificar RAM correctamente insertada',
 'Hardware', 'encendido,fuente,alimentación', 6, 45, 85),

('Instalación de Office 365', 'Guía de instalación de Office en equipos del laboratorio',
 'Se requiere instalar Office 2021/365',
 '1. Descargar desde portal institucional\n2. Ejecutar instalador\n3. Activar con cuenta institucional\n4. Verificar todas las aplicaciones',
 'Software', 'office,instalación,software', 6, 30, 90),

('Mantenimiento preventivo de equipos', 'Checklist completo de mantenimiento',
 'Mantenimiento periódico programado',
 '1. Limpieza exterior e interior\n2. Verificar ventiladores\n3. Actualizar drivers\n4. Escanear malware\n5. Desfragmentar disco',
 'Mantenimiento', 'limpieza,mantenimiento,preventivo', 7, 60, 88),

('Problemas de conexión de red', 'Diagnóstico y solución de problemas de red',
 'Computadoras sin acceso a internet o red local',
 '1. Verificar cable de red\n2. Reiniciar adaptador de red\n3. Renovar IP (ipconfig /renew)\n4. Verificar configuración DNS\n5. Actualizar driver de red',
 'Red', 'red,internet,conectividad', 7, 40, 82),

('Actualización de sistema Ubuntu', 'Proceso de actualización segura',
 'Actualizar Ubuntu a última versión LTS',
 '1. Hacer backup de datos\n2. sudo apt update\n3. sudo apt upgrade\n4. sudo do-release-upgrade\n5. Reiniciar sistema',
 'Software', 'ubuntu,linux,actualización', 6, 90, 85);
PRINT 'Base de conocimiento insertada correctamente.';
GO

-- ========================================================================
-- VERIFICACIÓN Y RESUMEN
-- ========================================================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICANDO INSTALACIÓN';
PRINT '========================================';
GO

PRINT 'Roles creados:';
SELECT RolID, Nombre, Descripcion FROM Roles ORDER BY RolID;
GO

PRINT '';
PRINT 'Facultades creadas:';
SELECT FacultadID, Nombre FROM Facultades ORDER BY FacultadID;
GO

PRINT '';
PRINT 'Resumen de usuarios por rol:';
SELECT r.Nombre AS Rol, COUNT(u.UsuarioID) AS CantidadUsuarios
FROM Roles r
LEFT JOIN Usuarios u ON r.RolID = u.RolID
GROUP BY r.Nombre
ORDER BY r.RolID;
GO

PRINT '';
PRINT 'Laboratorios creados:';
SELECT l.LaboratorioID, l.Nombre, f.Nombre AS Facultad, l.Capacidad
FROM Laboratorios l
INNER JOIN Facultades f ON l.FacultadID = f.FacultadID
ORDER BY l.LaboratorioID;
GO

PRINT '';
PRINT 'Computadoras por laboratorio:';
SELECT l.Nombre AS Laboratorio, COUNT(c.ComputadoraID) AS CantidadPCs
FROM Laboratorios l
LEFT JOIN Computadoras c ON l.LaboratorioID = c.LaboratorioID
GROUP BY l.Nombre
ORDER BY l.LaboratorioID;
GO

PRINT '';
PRINT 'Servicios disponibles:';
SELECT ServicioID, Nombre, TipoSolucion FROM CatalogoServicios ORDER BY ServicioID;
GO

PRINT '';
PRINT 'Técnicos Expertos:';
SELECT u.UsuarioID, u.Email, u.NombreCompleto, f.Nombre as Facultad
FROM Usuarios u
INNER JOIN Roles r ON u.RolID = r.RolID
LEFT JOIN Facultades f ON u.FacultadID = f.FacultadID
WHERE r.Nombre = 'TecnicoExperto'
ORDER BY u.UsuarioID;
GO

PRINT '';
PRINT 'Incidentes de prueba:';
SELECT i.CodigoIncidente, i.Titulo, i.Prioridad, i.Estado, cs.Nombre AS Servicio
FROM Incidentes i
LEFT JOIN CatalogoServicios cs ON i.ServicioID = cs.ServicioID
ORDER BY i.IncidenteID;
GO

PRINT '';
PRINT '========================================';
PRINT 'INSTALACIÓN COMPLETADA EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'Credenciales de prueba:';
PRINT '- Estudiante: estudiante1@uni.edu / pass123';
PRINT '- Docente: docente1@uni.edu / pass123';
PRINT '- Administrativo: admin1@uni.edu / pass123';
PRINT '- Laboratorista: lab1@uni.edu / pass123';
PRINT '- Técnico Experto Hardware: experto.hardware@uni.edu / pass123';
PRINT '- Técnico Experto Software: experto.software@uni.edu / pass123';
PRINT '- Técnico Experto Redes: experto.redes@uni.edu / pass123';
PRINT '';
PRINT 'Base de datos lista para usar.';
GO
