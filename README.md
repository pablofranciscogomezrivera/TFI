# ?? Sistema de Gesti車n de Urgencias Cl赤nicas
### Trabajo Final Integrador (TFI)

Proyecto desarrollado para la materia **Ingenier赤a de Software** del curso 4K2 en la **Universidad Tecnol車gica Nacional - Facultad Regional Tucum芍n**. 

#### Integrantes del Grupo

| Legajo | Apellido y Nombre      |
| :--- |:---------------------|
| 47731  | Marcial, Gabriel       |
| 46646  | Ponce, Facundo         |
| 46380  | Cancino, Micaela       |
| 48235  | Bellor, Maria          |
| 48308  | Herrera, Macarena      |
| 52467  | G車mez, Pablo           |

---

## ?? Descripci車n del Proyecto

El sistema est芍 dise?ado para gestionar el flujo de admisiones en la sala de urgencias de una cl赤nica. Permite al personal de enfermer赤a registrar el ingreso de pacientes, capturar sus signos vitales y priorizarlos autom芍ticamente en una cola de atenci車n.

La l車gica de negocio se basa en el enfoque de **Desarrollo Guiado por el Comportamiento (BDD)**, a partir de las siguientes historias de usuario:
* **IS2025-001**: Modelado del m車dulo de urgencias, incluyendo el registro de ingresos y el ordenamiento de la cola de atenci車n por prioridad.

## ??? Tecnolog赤as Utilizadas

El backend est芍 construido sobre la plataforma .NET, mientras que la tecnolog赤a para el frontend est芍 en proceso de definici車n.

| Componente | Tecnolog赤a |
| :--- | :--- |
| **Framework** | `.NET 8` |
| **Lenguaje** | `C#` |
| **Pruebas BDD** | `Reqnroll (Gherkin)` |
| **Framework de Pruebas** | `xUnit` |
| **Aserciones** | `Fluent Assertions` |
| **Interfaz de Usuario**| `React`  |

## ?? Estructura del Proyecto

El sistema sigue una arquitectura por capas para garantizar la separaci車n de responsabilidades y la mantenibilidad.

* **`Dominio`**: ?? Contiene las entidades, objetos de valor y reglas de negocio. Es el coraz車n de la aplicaci車n.
* **`Aplicacion`**: ?? Orquesta la l車gica del dominio a trav谷s de servicios, actuando como intermediario entre la UI y el n迆cleo.
* **`Infraestructura`**: ?? Implementa las interfaces del dominio (ej. repositorios). Actualmente usa una base de datos en memoria para las pruebas.
* **`Webb`**: ??? Proyecto destinado a la capa de presentaci車n (frontend) de la aplicaci車n.
* **`Tests`**: ?? Contiene todas las pruebas BDD, con los archivos `.feature` y sus implementaciones.

## ?? C車mo Ejecutar las Pruebas

Para verificar el comportamiento implementado, puedes ejecutar las pruebas de aceptaci車n automatizadas desde tu editor de preferencia o desde la l赤nea de comandos.

### ?? Scripts de Ejecuci車n R芍pida


### ?? Desde la L赤nea de Comandos

```bash
# Restaurar dependencias
dotnet restore TFI.sln

# Compilar la soluci車n
dotnet build TFI.sln --configuration Release

# Ejecutar todos los tests
dotnet test TFI.sln --configuration Release

```

### En Visual Studio

1.  **Abrir la Soluci車n**: Abre el archivo `TFI.sln`.
2.  **Restaurar Dependencias**: Haz clic derecho en la soluci車n y selecciona "Restaurar paquetes NuGet".
3.  **Compilar**: Usa el atajo `Ctrl+Shift+B`.
4.  **Ejecutar Pruebas**: Abre el **Explorador de Pruebas** (`Test > Explorador de pruebas`) y haz clic en "Run All Tests".

### En Visual Studio Code

1.  **Instalar Extensiones**: Aseg迆rate de tener instalada la extensi車n **C# Dev Kit** de Microsoft.
2.  **Abrir la Carpeta**: Abre la carpeta ra赤z del repositorio en VS Code.
3.  **Cargar Soluci車n**: Espera a que el C# Dev Kit cargue el archivo `TFI.sln`.
4.  **Ejecutar Pruebas**: Ve al 赤cono de matraz (Testing) en la barra de actividades del lado izquierdo y ejecuta las pruebas desde all赤.


## ?? CI/CD - GitHub Actions

El proyecto incluye workflows de GitHub Actions que se ejecutan autom芍ticamente en Pull Requests para garantizar la calidad del c車digo.

### Workflows Disponibles

#### 2. **Backend Tests ** 
- ? Se ejecuta autom芍ticamente en PRs a `master` o `main`
- ? Permite ejecuci車n manual
- ? Feedback r芍pido sin reportes adicionales

### ?? Ejecutar Manualmente

1. Ve a la pesta?a **"Actions"** en GitHub
2. Selecciona el workflow deseado
3. Click en **"Run workflow"**
4. Selecciona la rama
5. Click en **"Run workflow"** (bot車n verde)

### ?? Interpretar Resultados

- ? **Verde**: Todos los tests pasaron
- ? **Rojo**: Uno o m芍s tests fallaron - revisa los logs
- ?? **Amarillo**: Tests con warnings - verifica la cobertura

### ?? Documentaci車n Completa de CI/CD

Para m芍s informaci車n sobre los workflows, consulta [.github/workflows/README.md](.github/workflows/README.md).
<-- Test CI/CD -->
