USE IncidentManagementDB;
GO

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Nombre = 'TecnicoExperto')
BEGIN
    INSERT INTO Roles (Nombre, Descripcion)
    VALUES ('TecnicoExperto', 'Tecnico especializado que atiende incidentes complejos');
END
GO

DECLARE @RolExpertoID INT;
SELECT @RolExpertoID = RolID FROM Roles WHERE Nombre = 'TecnicoExperto';

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'experto.hardware@uni.edu')
BEGIN
    INSERT INTO Usuarios (Email, NombreCompleto, Contraseña, RolID, FacultadID, EsAsignador)
    VALUES ('experto.hardware@uni.edu', 'Luis Martinez - Experto Hardware', 'pass123', @RolExpertoID, 1, 0);
END

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'experto.software@uni.edu')
BEGIN
    INSERT INTO Usuarios (Email, NombreCompleto, Contraseña, RolID, FacultadID, EsAsignador)
    VALUES ('experto.software@uni.edu', 'Carmen Rodriguez - Experta Software', 'pass123', @RolExpertoID, 1, 0);
END

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'experto.redes@uni.edu')
BEGIN
    INSERT INTO Usuarios (Email, NombreCompleto, Contraseña, RolID, FacultadID, EsAsignador)
    VALUES ('experto.redes@uni.edu', 'Miguel Sanchez - Experto Redes', 'pass123', @RolExpertoID, 2, 0);
END
GO

SELECT u.UsuarioID, u.Email, u.NombreCompleto, r.Nombre as Rol, f.Nombre as Facultad
FROM Usuarios u
INNER JOIN Roles r ON u.RolID = r.RolID
LEFT JOIN Facultades f ON u.FacultadID = f.FacultadID
WHERE r.Nombre = 'TecnicoExperto';
GO
