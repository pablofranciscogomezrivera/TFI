Feature: Registro de admisión de pacientes en urgencias
  Como enfermera
  Quiero poder registrar las admisiones de los pacientes a urgencias
  Para determinar qué pacientes tienen mayor prioridad de atención

  Background:
    Given que la enfermera "María Pérez" está logueada en el sistema

  @Exito
  Scenario: Registrar ingreso de un paciente existente con todos los datos correctos
    Given que el paciente con DNI "12345678" existe en el sistema
    When registro un ingreso con los siguientes datos:
      | informe                 | "Paciente presenta dolor toracico agudo" |
      | nivelEmergencia        | "Critica"                                 |
      | temperatura            | 38.5                                      |
      | frecuenciaCardiaca     | 95                                        |
      | frecuenciaRespiratoria | 20                                        |
      | tensionSistolica       | 120                                       |
      | tensionDiastolica      | 80                                        |
    Then el ingreso se registra exitosamente con estado "PENDIENTE"
    And el paciente entra a la cola de atención
    And la enfermera "María Pérez" queda registrada en el ingreso

  @CreacionPaciente
  Scenario: Registrar ingreso de un paciente no existente
    Given que el paciente con DNI "87654321" no existe en el sistema
    When intento registrar su ingreso con los siguientes datos:
      | informe                 | "Paciente con fractura expuesta"         |
      | nivelEmergencia        | "Emergencia"                              |
      | temperatura            | 37.0                                      |
      | frecuenciaCardiaca     | 88                                        |
      | frecuenciaRespiratoria | 22                                        |
      | tensionSistolica       | 130                                       |
      | tensionDiastolica      | 85                                        |
    Then el sistema crea al paciente
    And el ingreso queda registrado con estado "PENDIENTE"

  @Validaciones
  Scenario: Registrar ingreso con datos mandatorios faltantes
    Given que el paciente con DNI "12345678" existe en el sistema
    When intento registrar el ingreso sin informar la "frecuenciaCardiaca"
    Then el sistema muestra un error indicando que "frecuenciaCardiaca" es mandatorio

  @Validaciones
  Scenario: Registrar ingreso con valores negativos en frecuencia
    Given que el paciente con DNI "12345678" existe en el sistema
    When intento registrar el ingreso con frecuenciaCardiaca -70 y frecuenciaRespiratoria -15
    Then el sistema muestra un error indicando que las frecuencias no pueden ser negativas

  @Validaciones
  Scenario: Registrar ingreso sin el informe del paciente
     Given que el paciente con DNI "12345678" existe en el sistema
     When intento registrar el ingreso sin informar el "informe"
     Then el sistema muestra un error indicando que "informe" es mandatorio

  @Prioridad
  Scenario: Ingreso de paciente con mayor prioridad que los ya en espera
    Given que hay un paciente "B" en espera con nivel de emergencia "Urgencia"
    And que el paciente "A" con DNI "11111111" existe en el sistema
    When registro un ingreso para el paciente "A" con nivel de emergencia "Crítica"
    Then el paciente "A" debe ser atendido antes que el paciente "B"

  @Prioridad
  Scenario: Ingreso de paciente con igual nivel de emergencia que otro
    Given que hay un paciente "B" en espera con nivel de emergencia "Emergencia" ingresado a las 14:00
    And que el paciente "A" con DNI "22222222" existe en el sistema
    When registro un ingreso para el paciente "A" con nivel de emergencia "Emergencia" a las 14:05
    Then el paciente "B" debe ser atendido antes que el paciente "A"

  @Prioridad
  Scenario: Ingreso de paciente con menor prioridad que los ya en espera
    Given que hay un paciente "B" en espera con nivel de emergencia "Emergencia"
    And que el paciente "C" con DNI "33333333" existe en el sistema
    When registro un ingreso para el paciente "C" con nivel de emergencia "Urgencia"
    Then el paciente "B" debe ser atendido antes que el paciente "C"