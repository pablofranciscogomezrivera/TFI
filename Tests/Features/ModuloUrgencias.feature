Feature: Modulo de Urgencias
  Esta feature esta relacionada al registro de ingresos de pacientes en la sala de urgencias,
  respetando su nivel de prioridad y el horario de llegada.

Background:
  Given que la siguiente enfermera esta registrada:
    | Nombre | Apellido |
    | Susana | Gimenez  |

Scenario: Ingreso de un paciente de bajo nivel de emergencia y luego otro de alto nivel de emergencia
  Given que estan registrados los siguientes pacientes:
    | CUIL         | Apellido | Nombre   | Obra Social         |
    | 23-1234567-9 | Nunez    | Marcelo  | Subsidio de Salud   |
    | 23-4567899-2 | Estrella | Patricio | Fondo de Bikini SA  |
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL         | Informe         | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-4567899-2 | Le duele el ojo | SIN_URGENCIA         | 37          | 70                  | 15                      | 120/80           |
    | 23-1234567-9 | Le agarro dengue| EMERGENCIA          | 38          | 70                  | 15                      | 120/80           |
  Then La lista de espera esta ordenada por cuil de la siguiente manera:
    | CUIL         |
    | 23-1234567-9 |
    | 23-4567899-2 |

Scenario: Registrar ingreso con valores negativos en Frecuencia Cardiaca
  Given que estan registrados los siguientes pacientes:
    | CUIL          | Apellido   | Nombre | Obra Social   |
    | 23-12345678-7 | Gomez Rivera | Pablo  | Swiss Medical |
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL          | Informe           | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-12345678-7 | Le duele la pestana | SinUrgencia         | 37          | -70                 | 15                      | 120/80           |
  Then el sistema muestra el siguiente error: "La frecuencia cardíaca no puede ser negativa"