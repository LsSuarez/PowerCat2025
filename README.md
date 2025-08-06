Proyecto GGStore
GGStore es una plataforma de e-commerce diseñada para ofrecer productos de diversas categorías en línea. El proyecto está implementado utilizando tecnologías como ASP.NET Core, MVC, y PostgreSQL para la gestión de datos. El objetivo principal de este proyecto es optimizar la experiencia de compra de los usuarios, así como facilitar la administración de productos, inventarios y pedidos.

Descripción
Este repositorio contiene el código fuente y los recursos para la plataforma GGStore, donde los usuarios pueden comprar productos de diferentes categorías. El sistema incluye funcionalidades como el registro de usuarios, gestión de productos, integración con pasarelas de pago, y administración de pedidos.

Tecnologías
Frontend: HTML, CSS, JavaScript (con jQuery y AJAX).

Backend: ASP.NET Core (MVC).

Base de Datos: PostgreSQL.

Otros: Git, Visual Studio Code, GitHub.

Instalación
Requisitos previos
Antes de empezar, asegúrate de tener instalados los siguientes programas:

Visual Studio Code

PostgreSQL

.NET 8.0 SDK

PgAdmin (opcional para gestionar la base de datos de forma gráfica)

Pasos para instalar y ejecutar el proyecto
Clona el repositorio en tu máquina local:

git clone https://github.com/tuusuario/GGStore.git
Abre el proyecto en Visual Studio Code:
cd GGStore
code .
Restaura los paquetes NuGet:
dotnet restore
Configuración de PostgreSQL:

Crea una base de datos en PostgreSQL (por ejemplo, ggstore_db).

Configura la conexión a PostgreSQL en el archivo appsettings.json de tu proyecto, asegurándote de que la cadena de conexión sea correcta:
json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Username=postgres;Password=tu_contraseña;Database=ggstore_db"
}
Si usas PgAdmin, puedes crear la base de datos de forma gráfica y asignarle un nombre como ggstore_db.

Crea y migra la base de datos:

dotnet ef database update
Esto creará las tablas necesarias en tu base de datos PostgreSQL.

Ejecuta el proyecto:

dotnet run
Accede a la aplicación en tu navegador en la URL:


http://localhost:5000
Uso
Registro de usuario
Los usuarios pueden registrarse en la plataforma para crear una cuenta, lo cual les permite realizar compras y gestionar su historial de pedidos.

Gestión de productos
El administrador puede agregar, editar y eliminar productos en la tienda. Cada producto incluye información como nombre, precio, descripción y cantidad disponible.

Realización de pedidos
Los usuarios pueden añadir productos al carrito de compras y proceder al pago utilizando las opciones disponibles (PayPal, tarjeta de crédito, etc.).

Contribuciones
Si deseas contribuir a este proyecto, por favor sigue los siguientes pasos:

Haz un fork de este repositorio.

Crea una rama para tu característica (git checkout -b feature/nueva-caracteristica).

Realiza tus cambios y haz commit (git commit -m 'Agregado nueva característica').

Empuja tus cambios (git push origin feature/nueva-caracteristica).

Abre un pull request con una descripción clara de lo que hiciste.

Licencia
Este proyecto está licenciado bajo la MIT License - consulta el archivo LICENSE para más detalles.

