# Configuración de Variables de Entorno

Este proyecto utiliza un archivo `.env` para almacenar configuraciones sensibles como credenciales de base de datos y claves secretas.

## Configuración Inicial

1. **Copia el archivo de ejemplo:**
   ```bash
   cp .env.example .env
   ```

2. **Edita el archivo `.env` con tus valores:**

### Configuración de Base de Datos

```env
DB_SERVER=tu-servidor\instancia
DB_NAME=IncidentManagementDB
DB_TRUSTED_CONNECTION=True
```

**Ejemplo para SQL Server local:**
```env
DB_SERVER=localhost\SQLEXPRESS
DB_NAME=IncidentManagementDB
DB_TRUSTED_CONNECTION=True
```

**Ejemplo para SQL Server con instancia nombrada:**
```env
DB_SERVER=DESKTOP-ABC123\MSSQLSERVER01
DB_NAME=IncidentManagementDB
DB_TRUSTED_CONNECTION=True
```

### Configuración JWT

Genera una clave secreta segura (mínimo 32 caracteres):

```env
JWT_SECRET_KEY=A2B6F3C8D1E4G7H9J2K4M6P8Q1S3V5Y7Z9B1C3D5
JWT_ISSUER=IncidentManagement
JWT_AUDIENCE=IncidentManagementUsers
JWT_EXPIRATION_MINUTES=10080
```

### Configuración de Notificaciones Push

Genera las claves VAPID usando el proyecto VapidGen:

```bash
cd VapidGen
dotnet run
```

Copia las claves generadas al archivo `.env`:

```env
PUSH_VAPID_PUBLIC_KEY=tu-clave-publica-generada
PUSH_VAPID_PRIVATE_KEY=tu-clave-privada-generada
PUSH_VAPID_SUBJECT=mailto:tu-email@dominio.com
```

## Seguridad

⚠️ **IMPORTANTE:**
- **NUNCA** subas el archivo `.env` a Git
- El archivo `.env` está incluido en `.gitignore`
- Usa `.env.example` como plantilla para otros desarrolladores
- En producción, usa variables de entorno del sistema o un administrador de secretos

## Verificación

Cuando ejecutes la aplicación, deberías ver mensajes como:

```
[Config] Archivo .env cargado desde: D:\PROYECTOS\...\. env
[Config] Connection String configurado desde .env
[Config] JWT configurado desde .env
[Config] Push Notifications configurado desde .env
```

Si no aparecen, verifica que:
1. El archivo `.env` existe en la raíz del proyecto
2. Las variables están correctamente nombradas
3. No hay espacios antes/después del `=`
