# ?? Sistema de Gesti¨®n de Urgencias Cl¨ªnicas
### Trabajo Final Integrador (TFI)

Proyecto desarrollado para la materia **Ingenier¨ªa de Software** del curso 4K2 en la **Universidad Tecnol¨®gica Nacional - Facultad Regional Tucum¨¢n**. 

#### Integrantes del Grupo

| Legajo | Apellido y Nombre      |
| :--- |:---------------------|
| 47731  | Marcial, Gabriel       |
| 46646  | Ponce, Facundo         |
| 46380  | Cancino, Micaela       |
| 48235  | Bellor, Maria          |
| 48308  | Herrera, Macarena      |
| 52467  | G¨®mez, Pablo           |

---

## ?? Descripcion del Proyecto

El sistema est¨¢ dise?ado para gestionar el flujo de admisiones en la sala de urgencias de una cl¨ªnica. Permite al personal de enfermer¨ªa registrar el ingreso de pacientes, capturar sus signos vitales y priorizarlos autom¨¢ticamente en una cola de atenci¨®n.

La l¨®gica de negocio se basa en el enfoque de **Desarrollo Guiado por el Comportamiento (BDD)**, a partir de las siguientes historias de usuario:
* **IS2025-001**: Modelado del m¨®dulo de urgencias, incluyendo el registro de ingresos y el ordenamiento de la cola de atenci¨®n por prioridad.

## ??? Tecnolog¨ªas Utilizadas

El backend est¨¢ construido sobre la plataforma .NET, mientras que la tecnolog¨ªa para el frontend est¨¢ en proceso de definici¨®n.

| Componente | Tecnolog¨ªa |
| :--- | :--- |
| **Framework** | `.NET 8` |
| **Lenguaje** | `C#` |
| **Pruebas BDD** | `Reqnroll (Gherkin)` |
| **Framework de Pruebas** | `xUnit` |
| **Aserciones** | `Fluent Assertions` |
| **Interfaz de Usuario**| `React`  |

## ?? Estructura del Proyecto

El sistema sigue una arquitectura por capas para garantizar la separaci¨®n de responsabilidades y la mantenibilidad.

* **`Dominio`**: ?? Contiene las entidades, objetos de valor y reglas de negocio. Es el coraz¨®n de la aplicaci¨®n.
* **`Aplicacion`**: ?? Orquesta la l¨®gica del dominio a trav¨¦s de servicios, actuando como intermediario entre la UI y el n¨²cleo.
* **`Infraestructura`**: ?? Implementa las interfaces del dominio (ej. repositorios). Actualmente usa una base de datos en memoria para las pruebas.
* **`Webb`**: ??? Proyecto destinado a la capa de presentaci¨®n (frontend) de la aplicaci¨®n.
* **`Tests`**: ?? Contiene todas las pruebas BDD, con los archivos `.feature` y sus implementaciones.

## ?? C¨®mo Ejecutar las Pruebas

Para verificar el comportamiento implementado, puedes ejecutar las pruebas de aceptaci¨®n automatizadas desde tu editor de preferencia o desde la l¨ªnea de comandos.

### ?? Scripts de Ejecuci¨®n R¨¢pida


### ?? Desde la L¨ªnea de Comandos

```bash
# Restaurar dependencias
dotnet restore TFI.sln

# Compilar la soluci¨®n
dotnet build TFI.sln --configuration Release

# Ejecutar todos los tests
dotnet test TFI.sln --configuration Release

```

### En Visual Studio

1.  **Abrir la Soluci¨®n**: Abre el archivo `TFI.sln`.
2.  **Restaurar Dependencias**: Haz clic derecho en la soluci¨®n y selecciona "Restaurar paquetes NuGet".
3.  **Compilar**: Usa el atajo `Ctrl+Shift+B`.
4.  **Ejecutar Pruebas**: Abre el **Explorador de Pruebas** (`Test > Explorador de pruebas`) y haz clic en "Run All Tests".

### En Visual Studio Code

1.  **Instalar Extensiones**: Aseg¨²rate de tener instalada la extensi¨®n **C# Dev Kit** de Microsoft.
2.  **Abrir la Carpeta**: Abre la carpeta ra¨ªz del repositorio en VS Code.
3.  **Cargar Soluci¨®n**: Espera a que el C# Dev Kit cargue el archivo `TFI.sln`.
4.  **Ejecutar Pruebas**: Ve al ¨ªcono de matraz (Testing) en la barra de actividades del lado izquierdo y ejecuta las pruebas desde all¨ª.


## ?? CI/CD - GitHub Actions

El proyecto incluye workflows de GitHub Actions que se ejecutan autom¨¢ticamente en Pull Requests para garantizar la calidad del c¨®digo.

### Workflows Disponibles

#### 2. **Backend Tests ** 
- ? Se ejecuta autom¨¢ticamente en PRs a `master` o `main`
- ? Permite ejecuci¨®n manual
- ? Feedback r¨¢pido sin reportes adicionales

### ?? Ejecutar Manualmente

1. Ve a la pesta?a **"Actions"** en GitHub
2. Selecciona el workflow deseado
3. Click en **"Run workflow"**
4. Selecciona la rama
5. Click en **"Run workflow"** (bot¨®n verde)

### ?? Interpretar Resultados

- ? **Verde**: Todos los tests pasaron
- ? **Rojo**: Uno o m¨¢s tests fallaron - revisa los logs
- ?? **Amarillo**: Tests con warnings - verifica la cobertura

### ?? Documentaci¨®n Completa de CI/CD

Para m¨¢s informaci¨®n sobre los workflows, consulta [.github/workflows/README.md](.github/workflows/README.md).

prueba de workflow
