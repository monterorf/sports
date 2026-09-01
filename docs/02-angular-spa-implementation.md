# Angular SPA - Guía de Implementación Detallada

**Proyecto Backend:** E:\Projects\Personal\NET\Sports  
**Proyecto Frontend:** E:\Projects\Personal\Angular\sports-web  
**Angular Version:** 20.3.26  
**.NET Version:** 10  
**API Base URL:** http://localhost:5200/api  
**Frontend URL:** http://localhost:4200

---

## 📑 Tabla de Contenidos

1. [Configuración Inicial](#configuración-inicial)
2. [Modelos TypeScript](#modelos-typescript)
3. [Servicios HTTP](#servicios-http)
4. [Componentes Compartidos](#componentes-compartidos)
5. [Features - Teams](#features-teams)
6. [Features - Players](#features-players)
7. [Features - Matches](#features-matches)
8. [Configuración de Aplicación](#configuración-de-aplicación)
9. [Ejecución y Pruebas](#ejecución-y-pruebas)

---

## 🔧 Configuración Inicial

### ✅ PASO 1: Configurar CORS en Backend

**Archivo:** `src\Sports.API\Program.cs`

**Modificación 1 - Agregar después de `builder.Services.AddEndpointsApiExplorer();`:**

```csharp
// ⭐ CONFIGURAR CORS PARA ANGULAR
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});
```

**Modificación 2 - Agregar después del bloque `if (app.Environment.IsDevelopment())`:**

```csharp
// ⭐ ACTIVAR CORS
app.UseCors("AllowAngular");
```

**Ubicación exacta en Program.cs:**
```csharp
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

// ⭐ AGREGAR AQUÍ
app.UseCors("AllowAngular");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
```

---

### ✅ PASO 2: Crear Estructura de Carpetas

**Ejecutar en PowerShell desde:** `E:\Projects\Personal\Angular\sports-web`

```powershell
# Crear todas las carpetas necesarias
New-Item -Path "src/app/core/models" -ItemType Directory -Force
New-Item -Path "src/app/core/services" -ItemType Directory -Force
New-Item -Path "src/app/shared/components/navbar" -ItemType Directory -Force
New-Item -Path "src/app/shared/components/loading-spinner" -ItemType Directory -Force
New-Item -Path "src/app/features/teams/team-list" -ItemType Directory -Force
New-Item -Path "src/app/features/teams/team-detail" -ItemType Directory -Force
New-Item -Path "src/app/features/teams/team-form" -ItemType Directory -Force
New-Item -Path "src/app/features/players/player-list" -ItemType Directory -Force
New-Item -Path "src/app/features/players/player-form" -ItemType Directory -Force
New-Item -Path "src/app/features/matches/match-list" -ItemType Directory -Force
```

**Verificación:**
```powershell
tree src/app /F
```

> **📝 Nota sobre convenciones de nombres (Angular 17+):**  
> A partir de Angular 17, el CLI genera archivos de componentes **sin** la palabra "component" en el nombre.  
> - ✅ **Nuevo:** `navbar.ts`, `navbar.html`, `navbar.scss`  
> - ❌ **Anterior:** `navbar.component.ts`, `navbar.component.html`, `navbar.component.scss`  
>  
> Si usas `ng generate component`, verás esta nueva convención automáticamente.

---

## 📐 Modelos TypeScript

### ✅ MODELO 1: Enums

**Archivo:** `src/app/core/models/enums.ts`

```typescript
// ============================================
// ENUMS - Tipos deportivos, posiciones y estados
// ============================================

/**
 * Tipos de deportes disponibles en el sistema
 * Deben coincidir con Sports.Domain.Enums.SportType (C#)
 */
export enum SportType {
  Football = 1,
  Basketball = 2,
  Tennis = 3,
  Volleyball = 4,
  Baseball = 5,
  Hockey = 6,
  Rugby = 7,
  Cricket = 8,
  Golf = 9,
  Boxing = 10
}

/**
 * Posiciones de jugadores
 * Deben coincidir con Sports.Domain.Enums.Position (C#)
 */
export enum Position {
  Goalkeeper = 1,
  Defender = 2,
  Midfielder = 3,
  Forward = 4,
  Center = 5,
  Guard = 6,
  Forward_Basketball = 7,
  Pitcher = 8,
  Catcher = 9,
  Infielder = 10,
  Outfielder = 11
}

/**
 * Estados posibles de un partido
 * Deben coincidir con Sports.Domain.Enums.MatchStatus (C#)
 */
export enum MatchStatus {
  Scheduled = 1,
  InProgress = 2,
  Finished = 3,
  Postponed = 4,
  Cancelled = 5,
  Suspended = 6
}

// ============================================
// HELPERS - Conversión de enums a texto
// ============================================

/**
 * Labels legibles para SportType
 * Uso: SportTypeLabels[SportType.Football] => "Football"
 */
export const SportTypeLabels: Record<SportType, string> = {
  [SportType.Football]: 'Football',
  [SportType.Basketball]: 'Basketball',
  [SportType.Tennis]: 'Tennis',
  [SportType.Volleyball]: 'Volleyball',
  [SportType.Baseball]: 'Baseball',
  [SportType.Hockey]: 'Hockey',
  [SportType.Rugby]: 'Rugby',
  [SportType.Cricket]: 'Cricket',
  [SportType.Golf]: 'Golf',
  [SportType.Boxing]: 'Boxing'
};

/**
 * Labels legibles para Position
 */
export const PositionLabels: Record<Position, string> = {
  [Position.Goalkeeper]: 'Goalkeeper',
  [Position.Defender]: 'Defender',
  [Position.Midfielder]: 'Midfielder',
  [Position.Forward]: 'Forward',
  [Position.Center]: 'Center',
  [Position.Guard]: 'Guard',
  [Position.Forward_Basketball]: 'Forward (Basketball)',
  [Position.Pitcher]: 'Pitcher',
  [Position.Catcher]: 'Catcher',
  [Position.Infielder]: 'Infielder',
  [Position.Outfielder]: 'Outfielder'
};

/**
 * Labels legibles para MatchStatus
 */
export const MatchStatusLabels: Record<MatchStatus, string> = {
  [MatchStatus.Scheduled]: 'Scheduled',
  [MatchStatus.InProgress]: 'In Progress',
  [MatchStatus.Finished]: 'Finished',
  [MatchStatus.Postponed]: 'Postponed',
  [MatchStatus.Cancelled]: 'Cancelled',
  [MatchStatus.Suspended]: 'Suspended'
};

/**
 * Helper para obtener todos los valores de SportType como array
 * Uso en select: sportTypes.map(st => ({ id: st.value, name: st.label }))
 */
export function getSportTypeOptions() {
  return Object.entries(SportTypeLabels).map(([key, value]) => ({
	value: Number(key) as SportType,
	label: value
  }));
}

/**
 * Helper para obtener todos los valores de Position como array
 */
export function getPositionOptions() {
  return Object.entries(PositionLabels).map(([key, value]) => ({
	value: Number(key) as Position,
	label: value
  }));
}

/**
 * Helper para obtener el label de un SportType específico
 * @param sportType - Valor numérico del enum SportType
 * @returns Label del deporte o 'Unknown'
 */
export function getSportTypeLabel(sportType: number): string {
  return SportTypeLabels[sportType as SportType] || 'Unknown';
}

/**
 * Helper para obtener el label de una Position específica
 * @param position - Valor numérico del enum Position
 * @returns Label de la posición o 'Unknown'
 */
export function getPositionLabel(position: number): string {
  return PositionLabels[position as Position] || 'Unknown';
}

/**
 * Helper para obtener el label de un MatchStatus específico
 * @param status - Valor numérico del enum MatchStatus
 * @returns Label del estado o 'Unknown'
 */
export function getMatchStatusLabel(status: number): string {
  return MatchStatusLabels[status as MatchStatus] || 'Unknown';
}
```

---

### ✅ MODELO 2: Team

**Archivo:** `src/app/core/models/team.ts`

```typescript
import { SportType } from './enums';

// ============================================
// TEAM INTERFACES
// ============================================

/**
 * Representa un equipo deportivo (respuesta del API)
 * Mapea a Sports.Application.DTOs.TeamDto (C#)
 */
export interface Team {
  /** ID único del equipo (GUID) */
  id: string;

  /** Nombre completo del equipo */
  name: string;

  /** Nombre corto/abreviatura (ej: "RMA" para Real Madrid) */
  shortName: string;

  /** URL del logo del equipo (opcional) */
  logo?: string;

  /** Fecha de fundación del equipo (ISO 8601) */
  foundedDate: string;

  /** Ciudad donde se ubica el equipo */
  city: string;

  /** País del equipo */
  country: string;

  /** Nombre del estadio del equipo (opcional) */
  stadium?: string;

  /** Tipo de deporte (viene como string desde el API) */
  sportType: string;

  /** Indica si el equipo está activo */
  isActive: boolean;

  /** Fecha de creación del registro (ISO 8601) */
  createdAt: string;

  /** Fecha de última actualización (ISO 8601, opcional) */
  updatedAt?: string;
}

/**
 * Request para crear un nuevo equipo
 * Mapea a Sports.Application.Features.Teams.Commands.CreateTeam.CreateTeamCommand (C#)
 */
export interface CreateTeamRequest {
  /** Nombre completo del equipo (requerido) */
  name: string;

  /** Nombre corto/abreviatura (requerido, máx 10 caracteres) */
  shortName: string;

  /** Fecha de fundación en formato ISO 8601 (requerido) */
  foundedDate: string;

  /** Ciudad del equipo (requerido) */
  city: string;

  /** País del equipo (requerido) */
  country: string;

  /** Tipo de deporte (enviar como número) */
  sportType: SportType;

  /** URL del logo (opcional) */
  logo?: string;

  /** Nombre del estadio (opcional) */
  stadium?: string;
}

/**
 * ViewModel para el formulario de creación
 * Incluye validaciones adicionales del lado del cliente
 */
export interface TeamFormModel {
  name: string;
  shortName: string;
  foundedDate: string;
  city: string;
  country: string;
  sportType: SportType;
  logo: string;
  stadium: string;
}

/**
 * Helper para crear un modelo de formulario vacío
 */
export function createEmptyTeamForm(): TeamFormModel {
  return {
	name: '',
	shortName: '',
	foundedDate: '',
	city: '',
	country: '',
	sportType: SportType.Football,
	logo: '',
	stadium: ''
  };
}

/**
 * Helper para convertir TeamFormModel a CreateTeamRequest
 */
export function teamFormToRequest(form: TeamFormModel): CreateTeamRequest {
  return {
	name: form.name.trim(),
	shortName: form.shortName.trim().toUpperCase(),
	foundedDate: form.foundedDate,
	city: form.city.trim(),
	country: form.country.trim(),
	sportType: form.sportType,
	logo: form.logo.trim() || undefined,
	stadium: form.stadium.trim() || undefined
  };
}
```

---

### ✅ MODELO 3: Player

**Archivo:** `src/app/core/models/player.ts`

```typescript
import { Position } from './enums';

// ============================================
// PLAYER INTERFACES
// ============================================

/**
 * Representa un jugador (respuesta del API)
 * Mapea a Sports.Application.DTOs.PlayerDto (C#)
 */
export interface Player {
  /** ID único del jugador (GUID) */
  id: string;

  /** Nombre del jugador */
  firstName: string;

  /** Apellido del jugador */
  lastName: string;

  /** Nombre completo (calculado en el backend) */
  fullName: string;

  /** Fecha de nacimiento (ISO 8601) */
  dateOfBirth: string;

  /** Edad calculada (calculado en el backend) */
  age: number;

  /** Nacionalidad del jugador */
  nationality: string;

  /** Altura en centímetros */
  height: number;

  /** Peso en kilogramos */
  weight: number;

  /** Posición del jugador (viene como string desde el API) */
  position: string;

  /** Número de camiseta */
  jerseyNumber: number;

  /** URL de la foto del jugador (opcional) */
  photo?: string;

  /** Indica si el jugador está activo */
  isActive: boolean;

  /** ID del equipo al que pertenece */
  teamId: string;

  /** Nombre del equipo (opcional, populated por el backend) */
  teamName?: string;

  /** Fecha de creación del registro (ISO 8601) */
  createdAt: string;

  /** Fecha de última actualización (ISO 8601, opcional) */
  updatedAt?: string;
}

/**
 * Request para crear un nuevo jugador
 * Mapea a Sports.Application.Features.Players.Commands.CreatePlayer.CreatePlayerCommand (C#)
 */
export interface CreatePlayerRequest {
  /** Nombre del jugador (requerido) */
  firstName: string;

  /** Apellido del jugador (requerido) */
  lastName: string;

  /** Fecha de nacimiento en formato ISO 8601 (requerido) */
  dateOfBirth: string;

  /** Nacionalidad (requerido) */
  nationality: string;

  /** Altura en cm (requerido, entre 100-250) */
  height: number;

  /** Peso en kg (requerido, entre 40-150) */
  weight: number;

  /** Posición del jugador (enviar como número) */
  position: Position;

  /** Número de camiseta (requerido, entre 1-99) */
  jerseyNumber: number;

  /** ID del equipo (requerido, GUID) */
  teamId: string;

  /** URL de la foto (opcional) */
  photo?: string;
}

/**
 * ViewModel para el formulario de creación
 */
export interface PlayerFormModel {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  nationality: string;
  height: number;
  weight: number;
  position: Position;
  jerseyNumber: number;
  teamId: string;
  photo: string;
}

/**
 * Helper para crear un modelo de formulario vacío
 */
export function createEmptyPlayerForm(): PlayerFormModel {
  return {
	firstName: '',
	lastName: '',
	dateOfBirth: '',
	nationality: '',
	height: 0,
	weight: 0,
	position: Position.Forward,
	jerseyNumber: 0,
	teamId: '',
	photo: ''
  };
}

/**
 * Helper para convertir PlayerFormModel a CreatePlayerRequest
 */
export function playerFormToRequest(form: PlayerFormModel): CreatePlayerRequest {
  return {
	firstName: form.firstName.trim(),
	lastName: form.lastName.trim(),
	dateOfBirth: form.dateOfBirth,
	nationality: form.nationality.trim(),
	height: Number(form.height),
	weight: Number(form.weight),
	position: form.position,
	jerseyNumber: Number(form.jerseyNumber),
	teamId: form.teamId,
	photo: form.photo.trim() || undefined
  };
}

/**
 * Helper para calcular la edad a partir de fecha de nacimiento
 * Útil para preview en formularios
 */
export function calculateAge(dateOfBirth: string): number {
  if (!dateOfBirth) return 0;

  const today = new Date();
  const birthDate = new Date(dateOfBirth);
  let age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();

  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
	age--;
  }

  return age;
}
```

---

### ✅ MODELO 4: Match

**Archivo:** `src/app/core/models/match.ts`

```typescript
// ============================================
// MATCH INTERFACES
// ============================================

/**
 * Representa un partido (respuesta del API)
 * Mapea a Sports.Application.DTOs.MatchDto (C#)
 */
export interface Match {
  /** ID único del partido (GUID) */
  id: string;

  /** ID del equipo local */
  homeTeamId: string;

  /** Nombre del equipo local */
  homeTeamName: string;

  /** ID del equipo visitante */
  awayTeamId: string;

  /** Nombre del equipo visitante */
  awayTeamName: string;

  /** Fecha programada del partido (ISO 8601) */
  scheduledDate: string;

  /** Fecha de inicio real del partido (ISO 8601, opcional) */
  startDate?: string;

  /** Fecha de finalización del partido (ISO 8601, opcional) */
  endDate?: string;

  /** Lugar/estadio donde se juega */
  venue: string;

  /** Nombre de la competición (opcional) */
  competition?: string;

  /** Temporada del partido (opcional, ej: "2023/2024") */
  season?: string;

  /** Marcador del equipo local */
  homeTeamScore: number;

  /** Marcador del equipo visitante */
  awayTeamScore: number;

  /** Estado del partido (viene como string desde el API) */
  status: string;

  /** Tipo de deporte (viene como string desde el API) */
  sportType: string;

  /** Resultado del partido (calculado en el backend) */
  winner: string;

  /** Fecha de creación del registro (ISO 8601) */
  createdAt: string;

  /** Fecha de última actualización (ISO 8601, opcional) */
  updatedAt?: string;
}

/**
 * Helper para obtener clase CSS según el estado del partido
 */
export function getMatchStatusClass(status: string): string {
  const statusLower = status.toLowerCase().replace(/\s/g, '');

  const classMap: Record<string, string> = {
	'scheduled': 'status-scheduled',
	'inprogress': 'status-inprogress',
	'finished': 'status-finished',
	'postponed': 'status-postponed',
	'cancelled': 'status-cancelled',
	'suspended': 'status-suspended'
  };

  return classMap[statusLower] || 'status-unknown';
}

/**
 * Helper para obtener icono según el estado del partido
 */
export function getMatchStatusIcon(status: string): string {
  const statusLower = status.toLowerCase().replace(/\s/g, '');

  const iconMap: Record<string, string> = {
	'scheduled': '📅',
	'inprogress': '▶️',
	'finished': '✅',
	'postponed': '⏸️',
	'cancelled': '❌',
	'suspended': '⏹️'
  };

  return iconMap[statusLower] || '❓';
}

/**
 * Helper para formatear el resultado del partido
 */
export function formatMatchResult(match: Match): string {
  if (match.status.toLowerCase() !== 'finished') {
	return 'TBD';
  }

  return `${match.homeTeamScore} - ${match.awayTeamScore}`;
}

/**
 * Helper para determinar si un equipo ganó
 */
export function isWinner(match: Match, isHomeTeam: boolean): boolean {
  if (match.status.toLowerCase() !== 'finished') {
	return false;
  }

  if (isHomeTeam) {
	return match.homeTeamScore > match.awayTeamScore;
  } else {
	return match.awayTeamScore > match.homeTeamScore;
  }
}
```

---

## 🔌 Servicios HTTP

### ✅ SERVICIO 1: TeamService

**Generar servicio:**
```powershell
ng generate service core/services/team
```

**Archivo:** `src/app/core/services/team.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Team, CreateTeamRequest } from '../models/team';

/**
 * Servicio para gestionar equipos deportivos
 * Consume el endpoint /api/teams del backend
 */
@Injectable({
  providedIn: 'root'
})
export class TeamService {
  /** URL base del API de equipos */
  private readonly apiUrl = 'http://localhost:5200/api/teams';

  constructor(private http: HttpClient) { }

  /**
   * Obtiene todos los equipos
   * 
   * @returns Observable con array de equipos
   * 
   * Ejemplo de uso:
   * ```
   * this.teamService.getAllTeams().subscribe({
   *   next: (teams) => console.log(teams),
   *   error: (err) => console.error(err)
   * });
   * ```
   */
  getAllTeams(): Observable<Team[]> {
	return this.http.get<Team[]>(this.apiUrl)
	  .pipe(
		catchError(this.handleError)
	  );
  }

  /**
   * Obtiene un equipo por su ID
   * 
   * @param id - GUID del equipo
   * @returns Observable con el equipo encontrado
   * 
   * Ejemplo de uso:
   * ```
   * this.teamService.getTeamById('guid').subscribe({
   *   next: (team) => console.log(team),
   *   error: (err) => console.error('Team not found')
   * });
   * ```
   */
  getTeamById(id: string): Observable<Team> {
	return this.http.get<Team>(`${this.apiUrl}/${id}`)
	  .pipe(
		catchError(this.handleError)
	  );
  }

  /**
   * Crea un nuevo equipo
   * 
   * @param team - Datos del equipo a crear
   * @returns Observable con el ID del equipo creado (GUID)
   * 
   * Ejemplo de uso:
   * ```
   * const newTeam: CreateTeamRequest = { ... };
   * this.teamService.createTeam(newTeam).subscribe({
   *   next: (teamId) => console.log('Created with ID:', teamId),
   *   error: (err) => console.error('Error creating team')
   * });
   * ```
   */
  createTeam(team: CreateTeamRequest): Observable<string> {
	return this.http.post<string>(this.apiUrl, team)
	  .pipe(
		catchError(this.handleError)
	  );
  }

  /**
   * Manejo centralizado de errores HTTP
   * 
   * @param error - Error de HTTP
   * @returns Observable que emite el error
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
	let errorMessage = 'An error occurred';

	if (error.error instanceof ErrorEvent) {
	  // Error del lado del cliente
	  errorMessage = `Client Error: ${error.error.message}`;
	} else {
	  // Error del lado del servidor
	  errorMessage = `Server Error: ${error.status} - ${error.message}`;
	}

	console.error('TeamService Error:', errorMessage);
	return throwError(() => new Error(errorMessage));
  }
}
```

---

### ✅ SERVICIO 2: PlayerService

**Generar servicio:**
```powershell
ng generate service core/services/player
```

**Archivo:** `src/app/core/services/player.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Player, CreatePlayerRequest } from '../models/player';

/**
 * Servicio para gestionar jugadores
 * Consume el endpoint /api/players del backend
 */
@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  /** URL base del API de jugadores */
  private readonly apiUrl = 'http://localhost:5200/api/players';

  constructor(private http: HttpClient) { }

  /**
   * Obtiene todos los jugadores
   * 
   * @returns Observable con array de jugadores
   */
  getAllPlayers(): Observable<Player[]> {
	return this.http.get<Player[]>(this.apiUrl)
	  .pipe(
		catchError(this.handleError)
	  );
  }

  /**
   * Crea un nuevo jugador
   * 
   * @param player - Datos del jugador a crear
   * @returns Observable con el ID del jugador creado (GUID)
   */
  createPlayer(player: CreatePlayerRequest): Observable<string> {
	return this.http.post<string>(this.apiUrl, player)
	  .pipe(
		catchError(this.handleError)
	  );
  }

  /**
   * Manejo centralizado de errores HTTP
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
	let errorMessage = 'An error occurred';

	if (error.error instanceof ErrorEvent) {
	  errorMessage = `Client Error: ${error.error.message}`;
	} else {
	  errorMessage = `Server Error: ${error.status} - ${error.message}`;
	}

	console.error('PlayerService Error:', errorMessage);
	return throwError(() => new Error(errorMessage));
  }
}
```

---

### ✅ SERVICIO 3: MatchService

**Generar servicio:**
```powershell
ng generate service core/services/match
```

**Archivo:** `src/app/core/services/match.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Match } from '../models/match';

/**
 * Servicio para gestionar partidos
 * Consume el endpoint /api/matches del backend
 */
@Injectable({
  providedIn: 'root'
})
export class MatchService {
  /** URL base del API de partidos */
  private readonly apiUrl = 'http://localhost:5200/api/matches';

  constructor(private http: HttpClient) { }

  /**
   * Obtiene todos los partidos
   * 
   * @returns Observable con array de partidos
   */
  getAllMatches(): Observable<Match[]> {
	return this.http.get<Match[]>(this.apiUrl)
	  .pipe(
		catchError(this.handleError)
	  );
  }

  /**
   * Manejo centralizado de errores HTTP
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
	let errorMessage = 'An error occurred';

	if (error.error instanceof ErrorEvent) {
	  errorMessage = `Client Error: ${error.error.message}`;
	} else {
	  errorMessage = `Server Error: ${error.status} - ${error.message}`;
	}

	console.error('MatchService Error:', errorMessage);
	return throwError(() => new Error(errorMessage));
  }
}
```

---

## 🧩 Componentes Compartidos

### ✅ COMPONENTE 1: Navbar

**Generar componente:**
```powershell
ng generate component shared/components/navbar
```

**Archivo:** `src/app/shared/components/navbar/navbar.ts`

```typescript
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

/**
 * Barra de navegación principal de la aplicación
 * Standalone component con routing
 */
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class NavbarComponent {
  /** Título de la aplicación */
  title = 'Sports Management';
}
```

**Archivo:** `src/app/shared/components/navbar/navbar.html`

```html
<nav class="navbar">
  <div class="navbar-container">
    <a routerLink="/" class="navbar-brand">
      ⚽ {{ title }}
    </a>

    <ul class="navbar-menu">
      <li>
        <a routerLink="/teams" routerLinkActive="active">
          Teams
        </a>
      </li>
      <li>
        <a routerLink="/players" routerLinkActive="active">
          Players
        </a>
      </li>
      <li>
        <a routerLink="/matches" routerLinkActive="active">
          Matches
        </a>
      </li>
    </ul>
  </div>
</nav>
```

**Archivo:** `src/app/shared/components/navbar/navbar.scss`

```scss
.navbar {
  background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.navbar-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 64px;
}

.navbar-brand {
  font-size: 1.5rem;
  font-weight: bold;
  color: white;
  text-decoration: none;
  transition: opacity 0.2s;

  &:hover {
    opacity: 0.9;
  }
}

.navbar-menu {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 2rem;

  li {
    margin: 0;
  }

  a {
    color: rgba(255, 255, 255, 0.9);
    text-decoration: none;
    font-weight: 500;
    padding: 0.5rem 1rem;
    border-radius: 4px;
    transition: all 0.2s;

    &:hover {
      background: rgba(255, 255, 255, 0.1);
      color: white;
    }

    &.active {
      background: rgba(255, 255, 255, 0.2);
      color: white;
    }
  }
}
```

---

### ✅ COMPONENTE 2: LoadingSpinner

**Generar componente:**
```powershell
ng generate component shared/components/loading-spinner
```

**Archivo:** `src/app/shared/components/loading-spinner/loading-spinner.ts`

```typescript
import { Component } from '@angular/core';

/**
 * Componente de spinner de carga
 * Muestra un indicador visual mientras se cargan datos
 */
@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [],
  templateUrl: './loading-spinner.html',
  styleUrl: './loading-spinner.scss'
})
export class LoadingSpinnerComponent { }
```

**Archivo:** `src/app/shared/components/loading-spinner/loading-spinner.html`

```html
<div class="spinner-container">
  <div class="spinner"></div>
  <p>Loading...</p>
</div>
```

**Archivo:** `src/app/shared/components/loading-spinner/loading-spinner.scss`

```scss
.spinner-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #1976d2;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

p {
  margin-top: 1rem;
  color: #666;
  font-size: 1.1rem;
}
```

---

## 🏆 Features - Teams

### ✅ COMPONENTE 3: TeamListComponent

**Generar componente:**
```powershell
ng generate component features/teams/team-list
```

**Archivo:** `src/app/features/teams/team-list/team-list.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { DatePipe } from '@angular/common';

/**
 * Componente para listar todos los equipos
 * Features:
 * - Carga de equipos desde el API
 * - Vista de tarjetas con información básica
 * - Navegación a detalle y formulario de creación
 */
@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [RouterLink, LoadingSpinnerComponent, DatePipe],
  templateUrl: './team-list.html',
  styleUrl: './team-list.scss'
})
export class TeamListComponent implements OnInit {
  /** Lista de equipos cargados del API */
  teams: Team[] = [];

  /** Indica si se están cargando los datos */
  loading = false;

  /** Mensaje de error si ocurre algún problema */
  error: string | null = null;

  constructor(private teamService: TeamService) { }

  ngOnInit(): void {
    this.loadTeams();
  }

  /**
   * Carga todos los equipos desde el API
   */
  loadTeams(): void {
    this.loading = true;
    this.error = null;

    this.teamService.getAllTeams().subscribe({
      next: (data) => {
        this.teams = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error loading teams. Please try again.';
        this.loading = false;
        console.error('Error loading teams:', err);
      }
    });
  }
}
```

**Archivo:** `src/app/features/teams/team-list/team-list.html`

```html
<div class="container">
  <div class="header">
    <h1>⚽ Teams</h1>
    <a routerLink="/teams/new" class="btn btn-primary">
      ➕ New Team
    </a>
  </div>

  @if (loading) {
    <app-loading-spinner></app-loading-spinner>
  }

  @if (error) {
    <div class="error-message">
      <p>{{ error }}</p>
      <button (click)="loadTeams()" class="btn btn-secondary">Try Again</button>
    </div>
  }

  @if (!loading && teams.length === 0 && !error) {
    <div class="no-data">
      <p>No teams found. Create your first team! 🏆</p>
      <a routerLink="/teams/new" class="btn btn-primary">Create Team</a>
    </div>
  }

  @if (!loading && teams.length > 0) {
    <div class="teams-grid">
      @for (team of teams; track team.id) {
        <div class="team-card">
          <div class="team-logo-container">
            <img 
              [src]="team.logo || 'assets/default-team.png'" 
              [alt]="team.name" 
              class="team-logo"
              (error)="$event.target.src='assets/default-team.png'">
          </div>

          <div class="team-info">
            <h3>{{ team.name }}</h3>
            <p class="short-name">{{ team.shortName }}</p>
            <p class="sport-type">🏅 {{ team.sportType }}</p>
            <p class="location">📍 {{ team.city }}, {{ team.country }}</p>

            @if (team.stadium) {
              <p class="stadium">🏟️ {{ team.stadium }}</p>
            }

            <p class="founded">📅 Founded: {{ team.foundedDate | date:'yyyy' }}</p>

            <span class="badge" [class.active]="team.isActive">
              {{ team.isActive ? 'Active' : 'Inactive' }}
            </span>
          </div>

          <div class="team-actions">
            <a [routerLink]="['/teams', team.id]" class="btn btn-sm">
              View Details
            </a>
          </div>
        </div>
      }
    </div>
  }
</div>
```

**Archivo:** `src/app/features/teams/team-list/team-list.scss`

```scss
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;

  h1 {
    color: #333;
    margin: 0;
  }
}

.error-message {
  text-align: center;
  padding: 2rem;
  background: #ffebee;
  border-radius: 8px;
  color: #d32f2f;

  p {
    margin-bottom: 1rem;
    font-size: 1.1rem;
  }
}

.no-data {
  text-align: center;
  padding: 3rem;
  color: #666;

  p {
    font-size: 1.2rem;
    margin-bottom: 1.5rem;
  }
}

.teams-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 2rem;
}

.team-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s, box-shadow 0.2s;
  display: flex;
  flex-direction: column;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.15);
  }
}

.team-logo-container {
  width: 100%;
  height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  border-radius: 8px;
  margin-bottom: 1rem;
}

.team-logo {
  max-width: 100%;
  max-height: 140px;
  object-fit: contain;
}

.team-info {
  flex: 1;

  h3 {
    margin: 0.5rem 0;
    color: #1976d2;
    font-size: 1.3rem;
  }

  .short-name {
    color: #666;
    font-weight: bold;
    font-size: 1rem;
    margin: 0.25rem 0;
  }

  .sport-type,
  .location,
  .stadium,
  .founded {
    color: #757575;
    font-size: 0.9rem;
    margin: 0.25rem 0;
  }
}

.badge {
  display: inline-block;
  padding: 0.3rem 0.8rem;
  border-radius: 12px;
  font-size: 0.85rem;
  background: #e0e0e0;
  color: #666;
  margin-top: 0.75rem;
  font-weight: 500;

  &.active {
    background: #4caf50;
    color: white;
  }
}

.team-actions {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #eee;
}

.btn {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 500;
  transition: all 0.2s;
  border: none;
  cursor: pointer;
  text-align: center;

  &.btn-primary {
    background: #1976d2;
    color: white;

    &:hover {
      background: #1565c0;
    }
  }

  &.btn-secondary {
    background: #757575;
    color: white;

    &:hover {
      background: #616161;
    }
  }

  &.btn-sm {
    padding: 0.5rem 1rem;
    font-size: 0.9rem;
    width: 100%;
  }
}
```

---

### ✅ COMPONENTE 4: TeamDetailComponent

**Generar componente:**
```powershell
ng generate component features/teams/team-detail
```

**Archivo:** `src/app/features/teams/team-detail/team-detail.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TeamService } from '../../../core/services/team.service';
import { Team } from '../../../core/models/team';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

/**
 * Componente para mostrar el detalle de un equipo
 * Features:
 * - Carga de equipo por ID desde la ruta
 * - Muestra información completa del equipo
 * - Navegación de regreso al listado
 */
@Component({
  selector: 'app-team-detail',
  standalone: true,
  imports: [RouterLink, DatePipe, LoadingSpinnerComponent],
  templateUrl: './team-detail.html',
  styleUrl: './team-detail.scss'
})
export class TeamDetailComponent implements OnInit {
  /** Equipo cargado desde el API */
  team: Team | null = null;

  /** Indica si se están cargando los datos */
  loading = false;

  /** Mensaje de error si ocurre algún problema */
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private teamService: TeamService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadTeam(id);
    } else {
      this.error = 'No team ID provided';
    }
  }

  /**
   * Carga el equipo por su ID
   * @param id - GUID del equipo
   */
  loadTeam(id: string): void {
    this.loading = true;
    this.error = null;

    this.teamService.getTeamById(id).subscribe({
      next: (data) => {
        this.team = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Team not found or error loading data.';
        this.loading = false;
        console.error('Error loading team:', err);
      }
    });
  }
}
```

**Archivo:** `src/app/features/teams/team-detail/team-detail.html`

```html
<div class="container">
  <a routerLink="/teams" class="back-link">← Back to Teams</a>

  @if (loading) {
    <app-loading-spinner></app-loading-spinner>
  }

  @if (error) {
    <div class="error-message">
      <p>{{ error }}</p>
      <a routerLink="/teams" class="btn btn-primary">Go Back</a>
    </div>
  }

  @if (team && !loading) {
    <div class="team-detail">
      <div class="team-header">
        <div class="team-logo-large">
          <img 
            [src]="team.logo || 'assets/default-team.png'" 
            [alt]="team.name"
            (error)="$event.target.src='assets/default-team.png'">
        </div>
        <div class="team-title">
          <h1>{{ team.name }}</h1>
          <p class="short-name">{{ team.shortName }}</p>
          <span class="badge" [class.active]="team.isActive">
            {{ team.isActive ? 'Active' : 'Inactive' }}
          </span>
        </div>
      </div>

      <div class="team-details">
        <div class="detail-section">
          <h2>📋 General Information</h2>
          <div class="detail-grid">
            <div class="detail-item">
              <span class="label">Sport Type:</span>
              <span class="value">{{ team.sportType }}</span>
            </div>
            <div class="detail-item">
              <span class="label">Founded:</span>
              <span class="value">{{ team.foundedDate | date:'fullDate' }}</span>
            </div>
            <div class="detail-item">
              <span class="label">City:</span>
              <span class="value">{{ team.city }}</span>
            </div>
            <div class="detail-item">
              <span class="label">Country:</span>
              <span class="value">{{ team.country }}</span>
            </div>
            @if (team.stadium) {
              <div class="detail-item">
                <span class="label">Stadium:</span>
                <span class="value">{{ team.stadium }}</span>
              </div>
            }
          </div>
        </div>

        <div class="detail-section">
          <h2>🕒 Timestamps</h2>
          <div class="detail-grid">
            <div class="detail-item">
              <span class="label">Created At:</span>
              <span class="value">{{ team.createdAt | date:'medium' }}</span>
            </div>
            @if (team.updatedAt) {
              <div class="detail-item">
                <span class="label">Updated At:</span>
                <span class="value">{{ team.updatedAt | date:'medium' }}</span>
              </div>
            }
          </div>
        </div>
      </div>
    </div>
  }
</div>
```

**Archivo:** `src/app/features/teams/team-detail/team-detail.scss`

```scss
.container {
  max-width: 900px;
  margin: 0 auto;
  padding: 2rem;
}

.back-link {
  display: inline-block;
  color: #1976d2;
  text-decoration: none;
  margin-bottom: 2rem;
  font-weight: 500;

  &:hover {
    text-decoration: underline;
  }
}

.error-message {
  text-align: center;
  padding: 3rem;
  background: #ffebee;
  border-radius: 8px;
  color: #d32f2f;

  p {
    margin-bottom: 1.5rem;
    font-size: 1.2rem;
  }
}

.team-detail {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.team-header {
  display: flex;
  gap: 2rem;
  align-items: center;
  padding-bottom: 2rem;
  border-bottom: 2px solid #e0e0e0;
  margin-bottom: 2rem;
}

.team-logo-large {
  width: 150px;
  height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  border-radius: 12px;
  flex-shrink: 0;

  img {
    max-width: 140px;
    max-height: 140px;
    object-fit: contain;
  }
}

.team-title {
  flex: 1;

  h1 {
    margin: 0 0 0.5rem 0;
    color: #1976d2;
    font-size: 2.5rem;
  }

  .short-name {
    font-size: 1.2rem;
    color: #666;
    font-weight: bold;
    margin-bottom: 1rem;
  }
}

.badge {
  display: inline-block;
  padding: 0.4rem 1rem;
  border-radius: 12px;
  font-size: 0.9rem;
  background: #e0e0e0;
  color: #666;
  font-weight: 500;

  &.active {
    background: #4caf50;
    color: white;
  }
}

.team-details {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.detail-section {
  h2 {
    color: #333;
    margin-bottom: 1rem;
    font-size: 1.3rem;
  }
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  padding: 1rem;
  background: #f9f9f9;
  border-radius: 8px;

  .label {
    font-size: 0.85rem;
    color: #666;
    margin-bottom: 0.25rem;
    font-weight: 500;
  }

  .value {
    font-size: 1.1rem;
    color: #333;
    font-weight: 600;
  }
}

.btn {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 500;
  transition: all 0.2s;
  border: none;
  cursor: pointer;

  &.btn-primary {
    background: #1976d2;
    color: white;

    &:hover {
      background: #1565c0;
    }
  }
}
```

---

**CONTINUARÁ EN LA SIGUIENTE SECCIÓN: TeamForm, Players, Matches y Configuración de App...**

---

## ✅ Checklist de Progreso

### Backend (.NET)
- [ ] CORS configurado en Program.cs
- [ ] API corriendo en puerto 5200
- [ ] Verificar endpoints: http://localhost:5200/api/teams

### Frontend - Modelos
- [ ] `enums.ts` creado con SportType, Position, MatchStatus
- [ ] `team.ts` creado con interfaces Team y CreateTeamRequest
- [ ] `player.ts` creado con interfaces Player y CreatePlayerRequest
- [ ] `match.ts` creado con interface Match

### Frontend - Servicios
- [ ] TeamService generado e implementado
- [ ] PlayerService generado e implementado
- [ ] MatchService generado e implementado

### Frontend - Componentes Compartidos
- [ ] NavbarComponent creado
- [ ] LoadingSpinnerComponent creado

### Frontend - Teams Feature
- [ ] TeamListComponent creado
- [ ] TeamDetailComponent creado
- [ ] TeamFormComponent (próximo)

### Próximos Pasos
- [ ] Completar TeamFormComponent
- [ ] Implementar features de Players
- [ ] Implementar features de Matches
- [ ] Configurar rutas y app principal

---

## 🏗️ COMPONENTE 5: TeamForm (Crear/Editar Equipos)

### ✅ Archivo TypeScript

**Archivo:** `src/app/features/teams/team-form/team-form.ts`

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TeamService } from '../../../core/services/team.service';
import { SportType } from '../../../core/models/enums';
import { CreateTeamRequest } from '../../../core/models/team';

@Component({
  selector: 'app-team-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-form.html',
  styleUrl: './team-form.scss'
})
export class TeamFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private teamService = inject(TeamService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  teamForm!: FormGroup;
  isEditMode = false;
  teamId: string | null = null;
  loading = false;
  error = '';
  sportTypes = Object.entries(SportType)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ label: key, value: value as SportType }));

  ngOnInit(): void {
    this.teamId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.teamId;

    this.initForm();

    if (this.isEditMode && this.teamId) {
      this.loadTeam(this.teamId);
    }
  }

  private initForm(): void {
    this.teamForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      shortName: ['', [Validators.required, Validators.maxLength(5)]],
      logo: ['', Validators.required],
      foundedDate: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      stadium: ['', Validators.required],
      sportType: [SportType.Football, Validators.required]
    });
  }

  private loadTeam(id: string): void {
    this.loading = true;
    this.teamService.getById(id).subscribe({
      next: (team) => {
        this.teamForm.patchValue({
          name: team.name,
          shortName: team.shortName,
          logo: team.logo,
          foundedDate: team.foundedDate.split('T')[0], // Format for input[type="date"]
          city: team.city,
          country: team.country,
          stadium: team.stadium,
          sportType: team.sportType
        });
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar el equipo';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onSubmit(): void {
    if (this.teamForm.invalid) {
      this.teamForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    const formValue = this.teamForm.value;
    const request: CreateTeamRequest = {
      name: formValue.name,
      shortName: formValue.shortName,
      logo: formValue.logo,
      foundedDate: formValue.foundedDate,
      city: formValue.city,
      country: formValue.country,
      stadium: formValue.stadium,
      sportType: formValue.sportType
    };

    this.teamService.create(request).subscribe({
      next: () => {
        this.router.navigate(['/teams']);
      },
      error: (err) => {
        this.error = 'Error al guardar el equipo';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/teams']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.teamForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.teamForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return 'Este campo es requerido';
      if (field.errors['minlength']) return `Mínimo ${field.errors['minlength'].requiredLength} caracteres`;
      if (field.errors['maxlength']) return `Máximo ${field.errors['maxlength'].requiredLength} caracteres`;
    }
    return '';
  }
}
```

---

### ✅ Archivo HTML

**Archivo:** `src/app/features/teams/team-form/team-form.html`

```html
<div class="team-form-container">
  <div class="form-header">
    <h2>{{ isEditMode ? 'Editar Equipo' : 'Nuevo Equipo' }}</h2>
  </div>

  @if (error) {
    <div class="error-alert">
      <span class="error-icon">⚠️</span>
      <span>{{ error }}</span>
    </div>
  }

  @if (loading && isEditMode) {
    <div class="loading-message">Cargando equipo...</div>
  }

  <form [formGroup]="teamForm" (ngSubmit)="onSubmit()" class="team-form">
    <div class="form-row">
      <div class="form-group">
        <label for="name">Nombre Completo *</label>
        <input 
          type="text" 
          id="name" 
          formControlName="name"
          [class.invalid]="isFieldInvalid('name')"
          placeholder="Real Madrid Club de Fútbol"
        >
        @if (isFieldInvalid('name')) {
          <span class="error-text">{{ getFieldError('name') }}</span>
        }
      </div>

      <div class="form-group">
        <label for="shortName">Nombre Corto *</label>
        <input 
          type="text" 
          id="shortName" 
          formControlName="shortName"
          [class.invalid]="isFieldInvalid('shortName')"
          placeholder="RMA"
          maxlength="5"
        >
        @if (isFieldInvalid('shortName')) {
          <span class="error-text">{{ getFieldError('shortName') }}</span>
        }
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="city">Ciudad *</label>
        <input 
          type="text" 
          id="city" 
          formControlName="city"
          [class.invalid]="isFieldInvalid('city')"
          placeholder="Madrid"
        >
        @if (isFieldInvalid('city')) {
          <span class="error-text">{{ getFieldError('city') }}</span>
        }
      </div>

      <div class="form-group">
        <label for="country">País *</label>
        <input 
          type="text" 
          id="country" 
          formControlName="country"
          [class.invalid]="isFieldInvalid('country')"
          placeholder="España"
        >
        @if (isFieldInvalid('country')) {
          <span class="error-text">{{ getFieldError('country') }}</span>
        }
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="stadium">Estadio *</label>
        <input 
          type="text" 
          id="stadium" 
          formControlName="stadium"
          [class.invalid]="isFieldInvalid('stadium')"
          placeholder="Santiago Bernabéu"
        >
        @if (isFieldInvalid('stadium')) {
          <span class="error-text">{{ getFieldError('stadium') }}</span>
        }
      </div>

      <div class="form-group">
        <label for="foundedDate">Fecha de Fundación *</label>
        <input 
          type="date" 
          id="foundedDate" 
          formControlName="foundedDate"
          [class.invalid]="isFieldInvalid('foundedDate')"
        >
        @if (isFieldInvalid('foundedDate')) {
          <span class="error-text">{{ getFieldError('foundedDate') }}</span>
        }
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="sportType">Deporte *</label>
        <select 
          id="sportType" 
          formControlName="sportType"
          [class.invalid]="isFieldInvalid('sportType')"
        >
          @for (sport of sportTypes; track sport.value) {
            <option [value]="sport.value">{{ sport.label }}</option>
          }
        </select>
        @if (isFieldInvalid('sportType')) {
          <span class="error-text">{{ getFieldError('sportType') }}</span>
        }
      </div>

      <div class="form-group">
        <label for="logo">Logo URL *</label>
        <input 
          type="url" 
          id="logo" 
          formControlName="logo"
          [class.invalid]="isFieldInvalid('logo')"
          placeholder="https://example.com/logo.png"
        >
        @if (isFieldInvalid('logo')) {
          <span class="error-text">{{ getFieldError('logo') }}</span>
        }
      </div>
    </div>

    <div class="form-actions">
      <button type="button" class="btn-cancel" (click)="onCancel()" [disabled]="loading">
        Cancelar
      </button>
      <button type="submit" class="btn-submit" [disabled]="loading || teamForm.invalid">
        {{ loading ? 'Guardando...' : (isEditMode ? 'Actualizar' : 'Crear Equipo') }}
      </button>
    </div>
  </form>
</div>
```

---

### ✅ Archivo SCSS

**Archivo:** `src/app/features/teams/team-form/team-form.scss`

```scss
.team-form-container {
  max-width: 800px;
  margin: 2rem auto;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-header {
  margin-bottom: 2rem;

  h2 {
    color: #1a202c;
    font-size: 1.75rem;
    font-weight: 600;
  }
}

.error-alert {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  margin-bottom: 1.5rem;
  background-color: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c33;

  .error-icon {
    font-size: 1.25rem;
  }
}

.loading-message {
  padding: 1rem;
  text-align: center;
  color: #718096;
  font-style: italic;
}

.team-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;

  @media (max-width: 640px) {
    grid-template-columns: 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  label {
    font-weight: 600;
    color: #2d3748;
    font-size: 0.875rem;
  }

  input, select {
    padding: 0.75rem;
    border: 1px solid #cbd5e0;
    border-radius: 6px;
    font-size: 1rem;
    transition: all 0.2s;

    &:focus {
      outline: none;
      border-color: #3182ce;
      box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
    }

    &.invalid {
      border-color: #fc8181;
      background-color: #fff5f5;
    }

    &:disabled {
      background-color: #f7fafc;
      cursor: not-allowed;
    }
  }

  .error-text {
    color: #e53e3e;
    font-size: 0.875rem;
    margin-top: -0.25rem;
  }
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e2e8f0;

  button {
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 6px;
    font-weight: 600;
    font-size: 1rem;
    cursor: pointer;
    transition: all 0.2s;

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }

  .btn-cancel {
    background-color: #edf2f7;
    color: #4a5568;

    &:hover:not(:disabled) {
      background-color: #e2e8f0;
    }
  }

  .btn-submit {
    background-color: #3182ce;
    color: white;

    &:hover:not(:disabled) {
      background-color: #2c5282;
    }
  }
}
```

---

## 👥 Features - Players

### ✅ COMPONENTE 6: PlayerList

**Archivo:** `src/app/features/players/player-list/player-list.ts`

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PlayerService } from '../../../core/services/player.service';
import { Player } from '../../../core/models/player';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule, RouterModule, LoadingSpinnerComponent],
  templateUrl: './player-list.html',
  styleUrl: './player-list.scss'
})
export class PlayerListComponent implements OnInit {
  private playerService = inject(PlayerService);

  players: Player[] = [];
  loading = true;
  error = '';

  ngOnInit(): void {
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.loading = true;
    this.playerService.getAll().subscribe({
      next: (data) => {
        this.players = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar jugadores';
        this.loading = false;
        console.error(err);
      }
    });
  }

  calculateAge(dateOfBirth: string): number {
    const today = new Date();
    const birth = new Date(dateOfBirth);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    return age;
  }
}
```

---

**Archivo:** `src/app/features/players/player-list/player-list.html`

```html
<div class="players-container">
  <div class="header">
    <h2>Jugadores</h2>
    <a routerLink="/players/new" class="btn-new">
      <span class="icon">➕</span>
      Nuevo Jugador
    </a>
  </div>

  @if (loading) {
    <app-loading-spinner />
  } @else if (error) {
    <div class="error-message">
      <span class="error-icon">⚠️</span>
      <p>{{ error }}</p>
      <button (click)="loadPlayers()" class="btn-retry">Reintentar</button>
    </div>
  } @else if (players.length === 0) {
    <div class="empty-state">
      <span class="empty-icon">👤</span>
      <p>No hay jugadores registrados</p>
      <a routerLink="/players/new" class="btn-create">Crear primer jugador</a>
    </div>
  } @else {
    <div class="table-container">
      <table class="players-table">
        <thead>
          <tr>
            <th>Foto</th>
            <th>Nombre</th>
            <th>Edad</th>
            <th>Posición</th>
            <th>Dorsal</th>
            <th>Equipo</th>
            <th>Nacionalidad</th>
            <th>Estado</th>
          </tr>
        </thead>
        <tbody>
          @for (player of players; track player.id) {
            <tr>
              <td>
                <img 
                  [src]="player.photo || 'assets/default-player.png'" 
                  [alt]="player.fullName"
                  class="player-photo"
                >
              </td>
              <td class="player-name">{{ player.fullName }}</td>
              <td>{{ calculateAge(player.dateOfBirth) }} años</td>
              <td>
                <span class="badge badge-position">
                  {{ player.position }}
                </span>
              </td>
              <td class="jersey-number">#{{ player.jerseyNumber }}</td>
              <td>{{ player.teamName || 'Sin equipo' }}</td>
              <td>{{ player.nationality }}</td>
              <td>
                <span [class]="'badge badge-' + (player.isActive ? 'active' : 'inactive')">
                  {{ player.isActive ? 'Activo' : 'Inactivo' }}
                </span>
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  }
</div>
```

---

**Archivo:** `src/app/features/players/player-list/player-list.scss`

```scss
.players-container {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;

  h2 {
    font-size: 2rem;
    color: #1a202c;
    font-weight: 700;
  }

  .btn-new {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    background-color: #3182ce;
    color: white;
    text-decoration: none;
    border-radius: 8px;
    font-weight: 600;
    transition: background-color 0.2s;

    &:hover {
      background-color: #2c5282;
    }

    .icon {
      font-size: 1.2rem;
    }
  }
}

.table-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow-x: auto;
}

.players-table {
  width: 100%;
  border-collapse: collapse;

  thead {
    background-color: #f7fafc;

    th {
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #4a5568;
      font-size: 0.875rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      border-bottom: 2px solid #e2e8f0;
    }
  }

  tbody {
    tr {
      border-bottom: 1px solid #e2e8f0;
      transition: background-color 0.2s;

      &:hover {
        background-color: #f7fafc;
      }

      td {
        padding: 1rem;
        color: #2d3748;
      }
    }
  }
}

.player-photo {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #e2e8f0;
}

.player-name {
  font-weight: 600;
  color: #1a202c;
}

.jersey-number {
  font-weight: 700;
  color: #3182ce;
  font-size: 1.1rem;
}

.badge {
  display: inline-block;
  padding: 0.375rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.875rem;
  font-weight: 600;

  &.badge-position {
    background-color: #ebf8ff;
    color: #2c5282;
  }

  &.badge-active {
    background-color: #c6f6d5;
    color: #22543d;
  }

  &.badge-inactive {
    background-color: #fed7d7;
    color: #742a2a;
  }
}

.error-message, .empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;

  .error-icon, .empty-icon {
    font-size: 4rem;
    margin-bottom: 1rem;
  }

  p {
    color: #718096;
    font-size: 1.125rem;
    margin-bottom: 1.5rem;
  }

  .btn-retry, .btn-create {
    padding: 0.75rem 1.5rem;
    background-color: #3182ce;
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    text-decoration: none;
    transition: background-color 0.2s;

    &:hover {
      background-color: #2c5282;
    }
  }
}
```

---

### ✅ COMPONENTE 7: PlayerForm

**Archivo:** `src/app/features/players/player-form/player-form.ts`

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PlayerService } from '../../../core/services/player.service';
import { TeamService } from '../../../core/services/team.service';
import { Position } from '../../../core/models/enums';
import { CreatePlayerRequest } from '../../../core/models/player';
import { Team } from '../../../core/models/team';

@Component({
  selector: 'app-player-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './player-form.html',
  styleUrl: './player-form.scss'
})
export class PlayerFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private playerService = inject(PlayerService);
  private teamService = inject(TeamService);
  private router = inject(Router);

  playerForm!: FormGroup;
  teams: Team[] = [];
  loading = false;
  loadingTeams = true;
  error = '';

  positions = Object.entries(Position)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ label: key.replace(/_/g, ' '), value: value as Position }));

  ngOnInit(): void {
    this.initForm();
    this.loadTeams();
  }

  private initForm(): void {
    this.playerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      dateOfBirth: ['', Validators.required],
      nationality: ['', Validators.required],
      height: ['', [Validators.required, Validators.min(50), Validators.max(250)]],
      weight: ['', [Validators.required, Validators.min(30), Validators.max(200)]],
      position: [Position.Midfielder, Validators.required],
      jerseyNumber: ['', [Validators.required, Validators.min(1), Validators.max(99)]],
      photo: [''],
      teamId: ['', Validators.required]
    });
  }

  private loadTeams(): void {
    this.loadingTeams = true;
    this.teamService.getAll().subscribe({
      next: (data) => {
        this.teams = data.filter(t => t.isActive);
        this.loadingTeams = false;
      },
      error: (err) => {
        this.error = 'Error al cargar equipos';
        this.loadingTeams = false;
        console.error(err);
      }
    });
  }

  onSubmit(): void {
    if (this.playerForm.invalid) {
      this.playerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    const formValue = this.playerForm.value;
    const request: CreatePlayerRequest = {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      dateOfBirth: formValue.dateOfBirth,
      nationality: formValue.nationality,
      height: formValue.height,
      weight: formValue.weight,
      position: formValue.position,
      jerseyNumber: formValue.jerseyNumber,
      photo: formValue.photo || '',
      teamId: formValue.teamId
    };

    this.playerService.create(request).subscribe({
      next: () => {
        this.router.navigate(['/players']);
      },
      error: (err) => {
        this.error = 'Error al crear el jugador';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/players']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.playerForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.playerForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return 'Este campo es requerido';
      if (field.errors['minlength']) return `Mínimo ${field.errors['minlength'].requiredLength} caracteres`;
      if (field.errors['min']) return `Valor mínimo: ${field.errors['min'].min}`;
      if (field.errors['max']) return `Valor máximo: ${field.errors['max'].max}`;
    }
    return '';
  }
}
```

---

**Archivo:** `src/app/features/players/player-form/player-form.html`

```html
<div class="player-form-container">
  <div class="form-header">
    <h2>Nuevo Jugador</h2>
  </div>

  @if (error) {
    <div class="error-alert">
      <span class="error-icon">⚠️</span>
      <span>{{ error }}</span>
    </div>
  }

  @if (loadingTeams) {
    <div class="loading-message">Cargando equipos...</div>
  } @else {
    <form [formGroup]="playerForm" (ngSubmit)="onSubmit()" class="player-form">
      <div class="form-row">
        <div class="form-group">
          <label for="firstName">Nombre *</label>
          <input 
            type="text" 
            id="firstName" 
            formControlName="firstName"
            [class.invalid]="isFieldInvalid('firstName')"
            placeholder="Lionel"
          >
          @if (isFieldInvalid('firstName')) {
            <span class="error-text">{{ getFieldError('firstName') }}</span>
          }
        </div>

        <div class="form-group">
          <label for="lastName">Apellido *</label>
          <input 
            type="text" 
            id="lastName" 
            formControlName="lastName"
            [class.invalid]="isFieldInvalid('lastName')"
            placeholder="Messi"
          >
          @if (isFieldInvalid('lastName')) {
            <span class="error-text">{{ getFieldError('lastName') }}</span>
          }
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="dateOfBirth">Fecha de Nacimiento *</label>
          <input 
            type="date" 
            id="dateOfBirth" 
            formControlName="dateOfBirth"
            [class.invalid]="isFieldInvalid('dateOfBirth')"
          >
          @if (isFieldInvalid('dateOfBirth')) {
            <span class="error-text">{{ getFieldError('dateOfBirth') }}</span>
          }
        </div>

        <div class="form-group">
          <label for="nationality">Nacionalidad *</label>
          <input 
            type="text" 
            id="nationality" 
            formControlName="nationality"
            [class.invalid]="isFieldInvalid('nationality')"
            placeholder="Argentina"
          >
          @if (isFieldInvalid('nationality')) {
            <span class="error-text">{{ getFieldError('nationality') }}</span>
          }
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="height">Altura (cm) *</label>
          <input 
            type="number" 
            id="height" 
            formControlName="height"
            [class.invalid]="isFieldInvalid('height')"
            placeholder="170"
            min="50"
            max="250"
          >
          @if (isFieldInvalid('height')) {
            <span class="error-text">{{ getFieldError('height') }}</span>
          }
        </div>

        <div class="form-group">
          <label for="weight">Peso (kg) *</label>
          <input 
            type="number" 
            id="weight" 
            formControlName="weight"
            [class.invalid]="isFieldInvalid('weight')"
            placeholder="72"
            min="30"
            max="200"
          >
          @if (isFieldInvalid('weight')) {
            <span class="error-text">{{ getFieldError('weight') }}</span>
          }
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="position">Posición *</label>
          <select 
            id="position" 
            formControlName="position"
            [class.invalid]="isFieldInvalid('position')"
          >
            @for (pos of positions; track pos.value) {
              <option [value]="pos.value">{{ pos.label }}</option>
            }
          </select>
          @if (isFieldInvalid('position')) {
            <span class="error-text">{{ getFieldError('position') }}</span>
          }
        </div>

        <div class="form-group">
          <label for="jerseyNumber">Número de Camiseta *</label>
          <input 
            type="number" 
            id="jerseyNumber" 
            formControlName="jerseyNumber"
            [class.invalid]="isFieldInvalid('jerseyNumber')"
            placeholder="10"
            min="1"
            max="99"
          >
          @if (isFieldInvalid('jerseyNumber')) {
            <span class="error-text">{{ getFieldError('jerseyNumber') }}</span>
          }
        </div>
      </div>

      <div class="form-row">
        <div class="form-group full-width">
          <label for="teamId">Equipo *</label>
          <select 
            id="teamId" 
            formControlName="teamId"
            [class.invalid]="isFieldInvalid('teamId')"
          >
            <option value="">Seleccionar equipo</option>
            @for (team of teams; track team.id) {
              <option [value]="team.id">{{ team.name }}</option>
            }
          </select>
          @if (isFieldInvalid('teamId')) {
            <span class="error-text">{{ getFieldError('teamId') }}</span>
          }
        </div>
      </div>

      <div class="form-row">
        <div class="form-group full-width">
          <label for="photo">URL de Foto (opcional)</label>
          <input 
            type="url" 
            id="photo" 
            formControlName="photo"
            placeholder="https://example.com/photo.jpg"
          >
        </div>
      </div>

      <div class="form-actions">
        <button type="button" class="btn-cancel" (click)="onCancel()" [disabled]="loading">
          Cancelar
        </button>
        <button type="submit" class="btn-submit" [disabled]="loading || playerForm.invalid">
          {{ loading ? 'Guardando...' : 'Crear Jugador' }}
        </button>
      </div>
    </form>
  }
</div>
```

---

**Archivo:** `src/app/features/players/player-form/player-form.scss`

```scss
.player-form-container {
  max-width: 800px;
  margin: 2rem auto;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-header {
  margin-bottom: 2rem;

  h2 {
    color: #1a202c;
    font-size: 1.75rem;
    font-weight: 600;
  }
}

.error-alert {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  margin-bottom: 1.5rem;
  background-color: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c33;

  .error-icon {
    font-size: 1.25rem;
  }
}

.loading-message {
  padding: 1rem;
  text-align: center;
  color: #718096;
  font-style: italic;
}

.player-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;

  &.full-width {
    grid-template-columns: 1fr;
  }

  @media (max-width: 640px) {
    grid-template-columns: 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  &.full-width {
    grid-column: 1 / -1;
  }

  label {
    font-weight: 600;
    color: #2d3748;
    font-size: 0.875rem;
  }

  input, select {
    padding: 0.75rem;
    border: 1px solid #cbd5e0;
    border-radius: 6px;
    font-size: 1rem;
    transition: all 0.2s;

    &:focus {
      outline: none;
      border-color: #3182ce;
      box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
    }

    &.invalid {
      border-color: #fc8181;
      background-color: #fff5f5;
    }

    &:disabled {
      background-color: #f7fafc;
      cursor: not-allowed;
    }
  }

  .error-text {
    color: #e53e3e;
    font-size: 0.875rem;
    margin-top: -0.25rem;
  }
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e2e8f0;

  button {
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 6px;
    font-weight: 600;
    font-size: 1rem;
    cursor: pointer;
    transition: all 0.2s;

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }

  .btn-cancel {
    background-color: #edf2f7;
    color: #4a5568;

    &:hover:not(:disabled) {
      background-color: #e2e8f0;
    }
  }

  .btn-submit {
    background-color: #3182ce;
    color: white;

    &:hover:not(:disabled) {
      background-color: #2c5282;
    }
  }
}
```

---

## 🏟️ Features - Matches

### ✅ COMPONENTE 8: MatchList

**Archivo:** `src/app/features/matches/match-list/match-list.ts`

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatchService } from '../../../core/services/match.service';
import { Match } from '../../../core/models/match';
import { getMatchStatusLabel, MatchStatus } from '../../../core/models/enums';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-match-list',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent],
  templateUrl: './match-list.html',
  styleUrl: './match-list.scss'
})
export class MatchListComponent implements OnInit {
  private matchService = inject(MatchService);

  matches: Match[] = [];
  loading = true;
  error = '';

  ngOnInit(): void {
    this.loadMatches();
  }

  loadMatches(): void {
    this.loading = true;
    this.matchService.getAll().subscribe({
      next: (data) => {
        this.matches = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar partidos';
        this.loading = false;
        console.error(err);
      }
    });
  }

  getStatusLabel(status: number): string {
    return getMatchStatusLabel(status);
  }

  getStatusClass(status: number): string {
    const statusEnum = status as MatchStatus;
    switch (statusEnum) {
      case MatchStatus.Scheduled:
        return 'scheduled';
      case MatchStatus.InProgress:
        return 'in-progress';
      case MatchStatus.Finished:
        return 'finished';
      case MatchStatus.Postponed:
        return 'postponed';
      case MatchStatus.Cancelled:
        return 'cancelled';
      case MatchStatus.Suspended:
        return 'suspended';
      default:
        return '';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
```

---

**Archivo:** `src/app/features/matches/match-list/match-list.html`

```html
<div class="matches-container">
  <div class="header">
    <h2>Partidos</h2>
  </div>

  @if (loading) {
    <app-loading-spinner />
  } @else if (error) {
    <div class="error-message">
      <span class="error-icon">⚠️</span>
      <p>{{ error }}</p>
      <button (click)="loadMatches()" class="btn-retry">Reintentar</button>
    </div>
  } @else if (matches.length === 0) {
    <div class="empty-state">
      <span class="empty-icon">🏟️</span>
      <p>No hay partidos programados</p>
    </div>
  } @else {
    <div class="matches-grid">
      @for (match of matches; track match.id) {
        <div class="match-card">
          <div class="match-header">
            <span class="competition">{{ match.competition }}</span>
            <span [class]="'status status-' + getStatusClass(match.status)">
              {{ getStatusLabel(match.status) }}
            </span>
          </div>

          <div class="match-teams">
            <div class="team home">
              <div class="team-name">{{ match.homeTeamName }}</div>
              @if (match.homeTeamScore !== null) {
                <div class="team-score">{{ match.homeTeamScore }}</div>
              }
            </div>

            <div class="vs">VS</div>

            <div class="team away">
              @if (match.awayTeamScore !== null) {
                <div class="team-score">{{ match.awayTeamScore }}</div>
              }
              <div class="team-name">{{ match.awayTeamName }}</div>
            </div>
          </div>

          @if (match.winner) {
            <div class="winner-badge">
              🏆 Ganador: {{ match.winner }}
            </div>
          }

          <div class="match-info">
            <div class="info-item">
              <span class="icon">📅</span>
              <span>{{ formatDate(match.scheduledDate) }}</span>
            </div>
            <div class="info-item">
              <span class="icon">🏟️</span>
              <span>{{ match.venue }}</span>
            </div>
            @if (match.season) {
              <div class="info-item">
                <span class="icon">📊</span>
                <span>Temporada {{ match.season }}</span>
              </div>
            }
          </div>
        </div>
      }
    </div>
  }
</div>
```

---

**Archivo:** `src/app/features/matches/match-list/match-list.scss`

```scss
.matches-container {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  margin-bottom: 2rem;

  h2 {
    font-size: 2rem;
    color: #1a202c;
    font-weight: 700;
  }
}

.matches-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.match-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s, box-shadow 0.2s;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  }
}

.match-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid #e2e8f0;

  .competition {
    font-weight: 700;
    color: #1a202c;
    font-size: 1.1rem;
  }

  .status {
    padding: 0.375rem 0.75rem;
    border-radius: 9999px;
    font-size: 0.75rem;
    font-weight: 700;
    text-transform: uppercase;

    &.status-scheduled {
      background-color: #bee3f8;
      color: #2c5282;
    }

    &.status-in-progress {
      background-color: #c6f6d5;
      color: #22543d;
      animation: pulse 2s infinite;
    }

    &.status-finished {
      background-color: #e2e8f0;
      color: #4a5568;
    }

    &.status-postponed {
      background-color: #feebc8;
      color: #7c2d12;
    }

    &.status-cancelled {
      background-color: #fed7d7;
      color: #742a2a;
    }

    &.status-suspended {
      background-color: #fef5e7;
      color: #975a16;
    }
  }
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}

.match-teams {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;

  .team {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 0.75rem;

    &.home {
      flex-direction: row;
    }

    &.away {
      flex-direction: row-reverse;
      text-align: right;
    }

    .team-name {
      font-weight: 600;
      color: #2d3748;
      font-size: 1.1rem;
    }

    .team-score {
      font-size: 2rem;
      font-weight: 700;
      color: #3182ce;
      min-width: 3rem;
      text-align: center;
    }
  }

  .vs {
    font-weight: 700;
    color: #a0aec0;
    font-size: 0.875rem;
  }
}

.winner-badge {
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  color: #744210;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  text-align: center;
  font-weight: 700;
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.match-info {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  .info-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    color: #718096;
    font-size: 0.875rem;

    .icon {
      font-size: 1rem;
    }
  }
}

.error-message, .empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;

  .error-icon, .empty-icon {
    font-size: 4rem;
    margin-bottom: 1rem;
  }

  p {
    color: #718096;
    font-size: 1.125rem;
    margin-bottom: 1.5rem;
  }

  .btn-retry {
    padding: 0.75rem 1.5rem;
    background-color: #3182ce;
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: background-color 0.2s;

    &:hover {
      background-color: #2c5282;
    }
  }
}
```

---

## ⚙️ Configuración de Aplicación

### ✅ PASO 1: Configurar Rutas

**Archivo:** `src/app/app.routes.ts`

```typescript
import { Routes } from '@angular/router';
import { TeamListComponent } from './features/teams/team-list/team-list';
import { TeamDetailComponent } from './features/teams/team-detail/team-detail';
import { TeamFormComponent } from './features/teams/team-form/team-form';
import { PlayerListComponent } from './features/players/player-list/player-list';
import { PlayerFormComponent } from './features/players/player-form/player-form';
import { MatchListComponent } from './features/matches/match-list/match-list';

export const routes: Routes = [
  { path: '', redirectTo: '/teams', pathMatch: 'full' },

  // Teams routes
  { path: 'teams', component: TeamListComponent },
  { path: 'teams/new', component: TeamFormComponent },
  { path: 'teams/:id', component: TeamDetailComponent },
  { path: 'teams/:id/edit', component: TeamFormComponent },

  // Players routes
  { path: 'players', component: PlayerListComponent },
  { path: 'players/new', component: PlayerFormComponent },

  // Matches routes
  { path: 'matches', component: MatchListComponent },

  // Wildcard route
  { path: '**', redirectTo: '/teams' }
];
```

---

### ✅ PASO 2: Configurar App Component

**Archivo:** `src/app/app.component.ts`

```typescript
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  template: `
    <app-navbar />
    <main class="main-content">
      <router-outlet />
    </main>
  `,
  styles: [`
    :host {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .main-content {
      flex: 1;
      background-color: #f7fafc;
    }
  `]
})
export class AppComponent {
  title = 'Sports Web';
}
```

---

### ✅ PASO 3: Configurar App Config

**Archivo:** `src/app/app.config.ts`

```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi())
  ]
};
```

---

### ✅ PASO 4: Estilos Globales

**Archivo:** `src/styles.scss`

```scss
/* Reset básico */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

/* Variables globales */
:root {
  --primary-color: #3182ce;
  --primary-dark: #2c5282;
  --secondary-color: #48bb78;
  --danger-color: #f56565;
  --warning-color: #ed8936;
  --text-primary: #1a202c;
  --text-secondary: #718096;
  --border-color: #e2e8f0;
  --bg-light: #f7fafc;
  --bg-white: #ffffff;
}

/* Estilos globales */
body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen,
    Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
  font-size: 16px;
  line-height: 1.5;
  color: var(--text-primary);
  background-color: var(--bg-light);
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

/* Enlaces */
a {
  color: var(--primary-color);
  text-decoration: none;
  transition: color 0.2s;

  &:hover {
    color: var(--primary-dark);
  }
}

/* Botones globales */
button {
  font-family: inherit;
  cursor: pointer;

  &:disabled {
    cursor: not-allowed;
    opacity: 0.6;
  }
}

/* Utilidades */
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.mt-1 { margin-top: 0.5rem; }
.mt-2 { margin-top: 1rem; }
.mt-3 { margin-top: 1.5rem; }
.mt-4 { margin-top: 2rem; }

.mb-1 { margin-bottom: 0.5rem; }
.mb-2 { margin-bottom: 1rem; }
.mb-3 { margin-bottom: 1.5rem; }
.mb-4 { margin-bottom: 2rem; }

.text-center { text-align: center; }
.text-right { text-align: right; }

.d-flex { display: flex; }
.flex-column { flex-direction: column; }
.align-center { align-items: center; }
.justify-center { justify-content: center; }
.justify-between { justify-content: space-between; }
.gap-1 { gap: 0.5rem; }
.gap-2 { gap: 1rem; }
.gap-3 { gap: 1.5rem; }

/* Scroll suave */
html {
  scroll-behavior: smooth;
}

/* Scrollbar personalizado (Webkit) */
::-webkit-scrollbar {
  width: 10px;
  height: 10px;
}

::-webkit-scrollbar-track {
  background: var(--bg-light);
}

::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 5px;

  &:hover {
    background: var(--text-secondary);
  }
}
```

---

## 🚀 Ejecución y Pruebas

### ✅ PASO 1: Ejecutar Backend

**En PowerShell desde:** `E:\Projects\Personal\NET\Sports\`

```powershell
# Restaurar dependencias (solo primera vez)
dotnet restore

# Ejecutar la API
dotnet run --project src/Sports.API/Sports.API.csproj

# O con hot reload
dotnet watch run --project src/Sports.API/Sports.API.csproj
```

**Verificar que esté corriendo:**
- URL HTTP: http://localhost:5200/api/teams
- URL HTTPS: https://localhost:7173/api/teams
- Swagger: http://localhost:5200/swagger (si está habilitado)

---

### ✅ PASO 2: Ejecutar Frontend

**En PowerShell desde:** `E:\Projects\Personal\Angular\sports-web\`

```powershell
# Instalar dependencias (solo primera vez)
npm install

# Ejecutar en modo desarrollo
ng serve

# O con puerto específico
ng serve --port 4200
```

**Verificar que esté corriendo:**
- URL Frontend: http://localhost:4200
- La aplicación debe cargar y mostrar el navbar
- Al navegar a Teams debe hacer la petición al API

---

### ✅ PASO 3: Verificar Integración

1. **Abrir navegador en**: http://localhost:4200

2. **Verificar Console (F12)**:
   - No debe haber errores CORS
   - Las peticiones HTTP deben llegar a `http://localhost:5200/api/*`
   - Las respuestas deben ser código 200

3. **Probar navegación**:
   - `/teams` → Ver lista de equipos
   - `/teams/new` → Formulario de crear equipo
   - `/teams/{id}` → Detalle de un equipo
   - `/players` → Ver lista de jugadores
   - `/players/new` → Formulario de crear jugador
   - `/matches` → Ver lista de partidos

4. **Probar funcionalidad**:
   - Crear un equipo nuevo
   - Ver el detalle del equipo creado
   - Crear un jugador asignándolo al equipo
   - Verificar que aparezca en la lista

---

## 📝 Checklist Completo de Implementación

### Backend (.NET)
- [x] CORS configurado en Program.cs
- [x] API corriendo en puerto 5200
- [x] Verificar endpoints: http://localhost:5200/api/teams

### Frontend - Modelos
- [x] `enums.ts` creado con SportType, Position, MatchStatus
- [x] `team.ts` creado con interfaces Team y CreateTeamRequest
- [x] `player.ts` creado con interfaces Player y CreatePlayerRequest
- [x] `match.ts` creado con interface Match

### Frontend - Servicios
- [x] TeamService generado e implementado
- [x] PlayerService generado e implementado
- [x] MatchService generado e implementado

### Frontend - Componentes Compartidos
- [x] NavbarComponent creado
- [x] LoadingSpinnerComponent creado

### Frontend - Teams Feature
- [x] TeamListComponent creado
- [x] TeamDetailComponent creado
- [x] TeamFormComponent creado

### Frontend - Players Feature
- [x] PlayerListComponent creado
- [x] PlayerFormComponent creado

### Frontend - Matches Feature
- [x] MatchListComponent creado

### Frontend - Configuración
- [x] app.routes.ts configurado
- [x] app.component.ts actualizado
- [x] app.config.ts configurado
- [x] styles.scss con estilos globales

### Pruebas
- [ ] Backend corriendo sin errores
- [ ] Frontend corriendo sin errores
- [ ] CORS funcionando correctamente
- [ ] Navegación entre rutas funciona
- [ ] Crear equipo funciona
- [ ] Ver detalle de equipo funciona
- [ ] Crear jugador funciona
- [ ] Ver lista de partidos funciona

---

## 🎯 Resumen de Archivos Creados

### Modelos (4 archivos)
1. `src/app/core/models/enums.ts`
2. `src/app/core/models/team.ts`
3. `src/app/core/models/player.ts`
4. `src/app/core/models/match.ts`

### Servicios (3 archivos)
5. `src/app/core/services/team.service.ts`
6. `src/app/core/services/player.service.ts`
7. `src/app/core/services/match.service.ts`

### Componentes Compartidos (6 archivos)
8. `src/app/shared/components/navbar/navbar.ts`
9. `src/app/shared/components/navbar/navbar.html`
10. `src/app/shared/components/navbar/navbar.scss`
11. `src/app/shared/components/loading-spinner/loading-spinner.ts`
12. `src/app/shared/components/loading-spinner/loading-spinner.html`
13. `src/app/shared/components/loading-spinner/loading-spinner.scss`

### Teams Feature (9 archivos)
14. `src/app/features/teams/team-list/team-list.ts`
15. `src/app/features/teams/team-list/team-list.html`
16. `src/app/features/teams/team-list/team-list.scss`
17. `src/app/features/teams/team-detail/team-detail.ts`
18. `src/app/features/teams/team-detail/team-detail.html`
19. `src/app/features/teams/team-detail/team-detail.scss`
20. `src/app/features/teams/team-form/team-form.ts`
21. `src/app/features/teams/team-form/team-form.html`
22. `src/app/features/teams/team-form/team-form.scss`

### Players Feature (6 archivos)
23. `src/app/features/players/player-list/player-list.ts`
24. `src/app/features/players/player-list/player-list.html`
25. `src/app/features/players/player-list/player-list.scss`
26. `src/app/features/players/player-form/player-form.ts`
27. `src/app/features/players/player-form/player-form.html`
28. `src/app/features/players/player-form/player-form.scss`

### Matches Feature (3 archivos)
29. `src/app/features/matches/match-list/match-list.ts`
30. `src/app/features/matches/match-list/match-list.html`
31. `src/app/features/matches/match-list/match-list.scss`

### Configuración (4 archivos)
32. `src/app/app.routes.ts`
33. `src/app/app.component.ts`
34. `src/app/app.config.ts`
35. `src/styles.scss`

**Total: 35 archivos creados**

---

## 🎉 ¡Implementación Completa!

Tu aplicación Angular ahora está lista para consumir el API de Sports. Sigue los pasos de ejecución y pruebas para verificar que todo funcione correctamente.

**Próximas mejoras sugeridas:**
- Agregar paginación en las listas
- Implementar búsqueda y filtros
- Agregar validación de imágenes
- Implementar edición de jugadores
- Agregar estadísticas en el detalle de equipos
- Implementar autenticación y autorización
- Agregar tests unitarios y E2E

---

