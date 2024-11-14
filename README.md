# PruebaTecnica-IOON
Sistema de gestión de comercios y usuarios con procedimientos almacenados en SQL Server, implementado en C# y ADO.NET.

Guía de Instalación y Configuración
Requisitos previos
.NET SDK 6.0 o superior.
SQL Server Express o cualquier versión compatible con la cadena de conexión proporcionada.
Herramienta de administración de base de datos SQL Server.
Editor de código como Visual Studio o Visual Studio Code.
Pasos de Instalación
1. Clonar el repositorio
Clona el repositorio del proyecto en tu máquina local.

3. Configuración de la Base de Datos
Abre SQL Server Management Studio y conéctate a tu servidor SQL Server.

Crea una nueva base de datos llamada CommerceDB.

Ejecuta los scripts de creación de las tablas y procedimientos almacenados incluidos en el proyecto.

Puedes hacerlo desde el archivo .sql que contiene las tablas y procedimientos almacenados, o ejecutar cada comando manualmente desde el SSMS.

3. Configuración del archivo appsettings.json
appsettings.json. Asegúrate de que la configuración de base de datos y JWT esté configurada correctamente.

json
Copiar código
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "key": "Y4TjD9Pp4HV0lX9QXtLTd3rM5Tw5uZT9Xb9A9+fh/6M=",
    "Issuer": "https://localhost:7130/",
    "Audience": "https://localhost:7130/",
    "Subject": "WebApiIOON"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-GKTE05O\\SQLEXPRESS;Database=CommerceDB;User Id=SecureUserLogin;Password=S3cureP@ssw0rd!2024;"
  }
}
