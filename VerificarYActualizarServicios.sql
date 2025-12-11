-- Script para verificar y actualizar los ServicioID en incidentes
-- Ejecutar en SQL Server Management Studio

USE IncidentManagementDB;
GO

-- 1. Ver el estado actual de los incidentes
PRINT '=== ESTADO ACTUAL DE INCIDENTES ==='
SELECT 
    i.IncidenteID,
    i.CodigoIncidente,
    i.Titulo,
    i.ServicioID,
    cs.Nombre AS ServicioNombre,
    i.Estado,
    f.Nombre AS FacultadNombre
FROM Incidentes i
LEFT JOIN CatalogoServicios cs ON i.ServicioID = cs.ServicioID
LEFT JOIN Facultades f ON i.FacultadID = f.FacultadID
ORDER BY i.IncidenteID;

-- 2. Identificar incidentes sin ServicioID
PRINT ''
PRINT '=== INCIDENTES SIN SERVICIO ASIGNADO ==='
SELECT 
    i.IncidenteID,
    i.CodigoIncidente,
    i.Titulo,
    f.Nombre AS FacultadNombre
FROM Incidentes i
LEFT JOIN Facultades f ON i.FacultadID = f.FacultadID
WHERE i.ServicioID IS NULL;

-- 3. Ver los servicios disponibles
PRINT ''
PRINT '=== SERVICIOS DISPONIBLES ==='
SELECT ServicioID, Nombre, Descripcion 
FROM CatalogoServicios;

-- 4. Actualizar incidentes de Facultad 1 que no tienen ServicioID
-- Asignar por defecto "Reparación de Hardware" (ServicioID = 1)
PRINT ''
PRINT '=== ACTUALIZANDO INCIDENTES DE FACULTAD 1 SIN SERVICIO ==='
UPDATE Incidentes
SET ServicioID = 1 -- Reparación de Hardware
WHERE FacultadID = 1 
AND ServicioID IS NULL;

SELECT @@ROWCOUNT AS 'Filas actualizadas';

-- 5. Si necesitas actualizar incidentes específicos con otros servicios:
-- Descomentar y ajustar según necesites:

-- Para incidentes relacionados con software:
-- UPDATE Incidentes SET ServicioID = 2 WHERE IncidenteID IN (lista de IDs);

-- Para incidentes de mantenimiento:
-- UPDATE Incidentes SET ServicioID = 3 WHERE IncidenteID IN (lista de IDs);

-- 6. Verificar resultado final
PRINT ''
PRINT '=== VERIFICACIÓN FINAL ==='
SELECT 
    i.IncidenteID,
    i.CodigoIncidente,
    i.Titulo,
    cs.Nombre AS ServicioNombre,
    i.Estado,
    f.Nombre AS FacultadNombre
FROM Incidentes i
LEFT JOIN CatalogoServicios cs ON i.ServicioID = cs.ServicioID
LEFT JOIN Facultades f ON i.FacultadID = f.FacultadID
WHERE i.FacultadID = 1
ORDER BY i.IncidenteID;

PRINT ''
PRINT '=== SCRIPT COMPLETADO ==='
