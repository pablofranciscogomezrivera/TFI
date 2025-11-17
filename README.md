# 🎓 Sistema de Gestión de Urgencias Clínicas
### Trabajo Final Integrador (TFI)

Proyecto desarrollado para la materia **Ingeniería de Software** del curso 4K2 en la **Universidad Tecnológica Nacional - Facultad Regional Tucumán**. 

#### Integrantes del Grupo

| Legajo | Apellido y Nombre      |
| :--- |:---------------------|
| 47731  | Marcial, Gabriel       |
| 46646  | Ponce, Facundo         |
| 46380  | Cancino, Micaela       |
| 48235  | Bellor, Maria          |
| 48308  | Herrera, Macarena      |
| 52467  | Gómez, Pablo           |

---

## 📜 Descripción del Proyecto

El sistema está diseñado para gestionar el flujo de admisiones en la sala de urgencias de una clínica. Permite al personal de enfermería registrar el ingreso de pacientes, capturar sus signos vitales y priorizarlos automáticamente en una cola de atención.

La lógica de negocio se basa en el enfoque de **Desarrollo Guiado por el Comportamiento (BDD)**, a partir de las siguientes historias de usuario:
* **IS2025-001**: Modelado del módulo de urgencias, incluyendo el registro de ingresos y el ordenamiento de la cola de atención por prioridad.

## 🛠️ Tecnologías Utilizadas

El backend está construido sobre la plataforma .NET, mientras que la tecnología para el frontend está en proceso de definición.

| Componente | Tecnología |
| :--- | :--- |
| **Framework** | `.NET 8` |
| **Lenguaje** | `C#` |
| **Pruebas BDD** | `Reqnroll (Gherkin)` |
| **Framework de Pruebas** | `xUnit` |
| **Aserciones** | `Fluent Assertions` |
| **Interfaz de Usuario**| `Blazor` o `React` (A definir) |

## 📁 Estructura del Proyecto

El sistema sigue una arquitectura por capas para garantizar la separación de responsabilidades y la mantenibilidad.

* **`Dominio`**: 🧠 Contiene las entidades, objetos de valor y reglas de negocio. Es el corazón de la aplicación.
* **`Aplicacion`**: ⚙️ Orquesta la lógica del dominio a través de servicios, actuando como intermediario entre la UI y el núcleo.
* **`Infraestructura`**: 🧱 Implementa las interfaces del dominio (ej. repositorios). Actualmente usa una base de datos en memoria para las pruebas.
* **`Webb`**: 🖥️ Proyecto destinado a la capa de presentación (frontend) de la aplicación.
* **`Tests`**: 🧪 Contiene todas las pruebas BDD, con los archivos `.feature` y sus implementaciones.

## 🚀 Cómo Ejecutar las Pruebas

Para verificar el comportamiento implementado, puedes ejecutar las pruebas de aceptación automatizadas desde tu editor de preferencia.

### En Visual Studio

1.  **Abrir la Solución**: Abre el archivo `TFI.sln`.
2.  **Restaurar Dependencias**: Haz clic derecho en la solución y selecciona "Restaurar paquetes NuGet".
3.  **Compilar**: Usa el atajo `Ctrl+Shift+B`.
4.  **Ejecutar Pruebas**: Abre el **Explorador de Pruebas** (`Test > Explorador de pruebas`) y haz clic en "Run All Tests".

### En Visual Studio Code

1.  **Instalar Extensiones**: Asegúrate de tener instalada la extensión **C# Dev Kit** de Microsoft.
2.  **Abrir la Carpeta**: Abre la carpeta raíz del repositorio en VS Code.
3.  **Cargar Solución**: Espera a que el C# Dev Kit cargue el archivo `TFI.sln`.
4.  **Ejecutar Pruebas**: Ve al ícono de matraz (Testing) en la barra de actividades del lado izquierdo y ejecuta las pruebas desde allí.
