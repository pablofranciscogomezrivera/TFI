Feature: Modulo de Urgencias
  Esta feature esta relacionada al registro de ingresos de pacientes en la sala de urgencias,
  respetando su nivel de prioridad y el horario de llegada.

Background:
  Given que la siguiente enfermera esta registrada:
    | Nombre | Apellido |
    | Susana | Gimenez  |
  And que estan registrados los siguientes pacientes:
    | CUIL          | Apellido     | Nombre   | Obra Social         |
    | 23-1234567-9  | Nunez        | Marcelo  | Subsidio de Salud   |
    | 23-4567899-2  | Estrella     | Patricio | Fondo de Bikini SA  |
    | 27-4567890-3  | Dufour       | Ale      | Swiss Medical       |
    | 23-12345678-7 | Gomez Rivera | Pablo    | Swiss Medical       |

Scenario: Ingreso de un paciente de bajo nivel de emergencia y luego otro de alto nivel de emergencia
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL         | Informe         | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-4567899-2 | Le duele el ojo | SIN_URGENCIA         | 37          | 70                  | 15                      | 120/80           |
    | 23-1234567-9 | Le agarro dengue| EMERGENCIA          | 38          | 70                  | 15                      | 120/80           |
  Then La lista de espera esta ordenada por cuil de la siguiente manera:
    | CUIL         |
    | 23-1234567-9 |
    | 23-4567899-2 |

Scenario: Ingreso de un paciente con menor prioridad que uno ya en espera
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL         | Informe          | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-1234567-9 | Le agarro dengue | EMERGENCIA          | 38          | 70                  | 15                      | 120/80           |
    | 27-4567890-3 | Le duele el ojo  | SIN_URGENCIA         | 37          | 70                  | 15                      | 120/80           |
  Then La lista de espera esta ordenada por cuil de la siguiente manera:
    | CUIL         |
    | 23-1234567-9 |
    | 27-4567890-3 |

Scenario: Ingreso de dos pacientes con igual nivel de emergencia (se ordena por llegada)
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL         | Informe          | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-1234567-9 | Le agarro dengue | EMERGENCIA          | 38          | 70                  | 15                      | 120/80           |
    | 23-4567899-2 | Neumonia         | EMERGENCIA          | 39          | 85                  | 22                      | 130/85           |
  Then La lista de espera esta ordenada por cuil de la siguiente manera:
    | CUIL         |
    | 23-1234567-9 |
    | 23-4567899-2 |

Scenario: Ingreso de un paciente que no existe en el sistema
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL          | Informe        | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 20-30111222-1 | Paciente nuevo | URGENCIA            | 37.5        | 80                  | 18                      | 110/70           |
  Then el paciente con CUIL "20-30111222-1" se crea en el sistema
  And La lista de espera esta ordenada por cuil de la siguiente manera:
    | CUIL          |
    | 20-30111222-1 |

Scenario: Registrar ingreso con valores negativos en Frecuencia Cardiaca
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL          | Informe             | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-12345678-7 | Le duele la pestana | SIN_URGENCIA         | 37          | -70                 | 15                      | 120/80           |
  Then el sistema muestra el siguiente error: "La frecuencia cardiaca no puede ser negativa"

Scenario: Registrar ingreso sin un dato mandatorio (informe)
  When Ingresan a urgencias los siguientes pacientes:
    | CUIL          | Informe | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
    | 23-12345678-7 |         | SIN_URGENCIA         | 37          | 70                  | 15                      | 120/80           |
  Then el sistema muestra el siguiente error: "El informe es un dato mandatorio"