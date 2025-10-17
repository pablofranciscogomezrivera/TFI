Feature: Modulo de Urgencias

Esta feature esta relacionada al registro de ingresos de pacientes en la sala de urgencias

Background: 
	Given que la siguiente enfermera esta registrada:
	| Nombre | Apellido | DNI      | CUIL          | FechaNacimiento | Email        | Telefono | Matricula | User | Password |
	| Susana | Gimenez  | 12345678 | 20-12345678-1 | 23/10/1990      | sg@gmail.com | 12345    | A01       | su   | 1234     |

Scenario: Ingreso del primer paciente a la lista de espera de urgencias
	Given que estan registrados los siguientes pacientes: 
	| Nombre    | Apellido | DNI     | CUIL         | FechaNacimiento | Email                | Telefono  | NumeroAfiliado | ObraSocial        | Calle     | Numero | Ciudad     | Provincia | Localidad             |
	| Marcelo   | Nunez    | 1234567 | 23-1234567-9 | 01/01/1995      | chelonunez@gmail.com | 381987651 | 123            | Subsidio de Salud | Rivadavia | 1050   | San Miguel | Tucuman   | San Miguel de Tucuman |
	| Alejandra | Dufour   | 4567890 | 27-4567890-3 | 01/01/1988      | aledufor@gmail.com   | 381234567 | 456            | Swiss Medical     | Rivadavia | 1051   | San Miguel | Tucuman   | San Miguel de Tucuman |

	When Ingresa a urgencia el siguiente paciente:
	| Cuil         | Informe          | NivelEmergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
	| 23-1234567-9 | Le agarro dengue | EMERGENCIA      | 38          | 70                  | 15                      | 120/80           |

	Then La lista de espera esta ordenada por cuil de la siguiente manera:
	| 23-1234567-9 |