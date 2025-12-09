-- Actualizar el TiempoResolucion de las soluciones existentes basándose en sus incidentes relacionados
UPDATE bc
SET bc.TiempoResolucion = DATEDIFF(MINUTE, i.FechaCreacion, i.FechaResolucion)
FROM BaseConocimiento bc
INNER JOIN Incidentes i ON bc.IncidenteRelacionadoID = i.IncidenteID
WHERE i.FechaResolucion IS NOT NULL
  AND bc.TiempoResolucion IS NULL;

-- Verificar los resultados
SELECT 
    bc.SolucionID,
    bc.Titulo,
    bc.TiempoResolucion,
    i.CodigoIncidente,
    i.FechaCreacion,
    i.FechaResolucion,
    DATEDIFF(MINUTE, i.FechaCreacion, i.FechaResolucion) as TiempoCalculado
FROM BaseConocimiento bc
LEFT JOIN Incidentes i ON bc.IncidenteRelacionadoID = i.IncidenteID
WHERE bc.IncidenteRelacionadoID IS NOT NULL;
