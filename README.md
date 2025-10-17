## Estructura de la Solucion

El sistema sigue una arquitectura por capas para separar las responsabilidades, lo que facilita su mantenimiento y escalabilidad:

* **`Dominio`**: Contiene las entidades del negocio (`Paciente`, `Ingreso`, `Enfermera`), los objetos de valor (`TensionArterial`, `Frecuencia`) y las reglas de negocio principales. Es el corazón de la aplicación.
* **`Aplicacion`**: Orquesta la lógica del dominio a través de servicios (`ServicioUrgencias`). Actúa como intermediario entre la interfaz de usuario y el dominio.
* **`Infraestructura`**: Implementa las interfaces definidas en el dominio, como los repositorios. Actualmente contiene una implementación en memoria (`DBPruebaMemoria`) para las pruebas.
* **`Webb`**: Proyecto (No decidido todavia) destinado a ser la capa de presentación (frontend) de la aplicación.
* **`Tests`**: Contiene todas las pruebas BDD. Aquí se encuentran los archivos `.feature` con las especificaciones en Gherkin y los *Step Definitions* que los conectan con el código C#.

## Cómo Ejecutar las Pruebas

Para verificar el comportamiento implementado, puedes ejecutar las pruebas de aceptación automatizadas desde tu editor de preferencia.

### En Visual Studio

1.  **Abrir el Proyecto**: Abre el archivo de solución `TFI.sln` con Visual Studio.
2.  **Restaurar Dependencias**: Visual Studio debería restaurar los paquetes NuGet automáticamente. Si no, haz clic derecho en la solución en el "Explorador de Soluciones" y selecciona "Restaurar paquetes NuGet".
3.  **Compilar la Solución**: Compila el proyecto para asegurarte de que todo esté en orden (puedes usar el atajo `Ctrl+Shift+B`).
4.  **Abrir el Explorador de Pruebas**: Ve al menú `Test > Explorador de pruebas`.
5.  **Ejecutar las Pruebas**: En la ventana del Explorador de Pruebas, verás los escenarios definidos en tus archivos `.feature`. Puedes ejecutarlos todos haciendo clic en el botón "Run All Tests".

### En Visual Studio Code

1.  **Instalar Extensiones**: Asegúrate de tener instaladas las siguientes extensiones desde el Marketplace de VS Code:
    * **C# Dev Kit** (de Microsoft).
    * **Test Explorer UI** (opcional, para una mejor interfaz gráfica).

2.  **Abrir la Carpeta del Proyecto**: Abre la carpeta raíz del repositorio en VS Code.

3.  **Cargar la Solución**: El C# Dev Kit detectará automáticamente el archivo `TFI.sln`. Espera a que cargue la solución en el panel del "Explorador de Soluciones".

4.  **Abrir la Vista de Pruebas**: Haz clic en el ícono de matraz (Testing) en la barra de actividades del lado izquierdo.

5.  **Ejecutar las Pruebas**: En el panel de "Testing", verás el árbol de pruebas con tus escenarios. Haz clic en el botón de "play" en la parte superior para ejecutar todas las pruebas, o ejecuta escenarios individuales haciendo clic en el "play" que aparece junto a cada uno.
