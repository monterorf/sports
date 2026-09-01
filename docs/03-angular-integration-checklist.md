# Checklist: Integración Angular SPA con Sports API

## Información del Proyecto

- **API Backend:** `E:\Projects\Personal\NET\Sports`
- **URL Base API:** `https://localhost:7173` o `http://localhost:5200`
- **SPA Angular:** `E:\Projects\Personal\Angular\sports-web`
- **Versión Angular:** 20.3.26

---

## Endpoints Disponibles

### Teams (Equipos)
- `GET /api/teams` - Obtener todos los equipos
- `GET /api/teams/{id}` - Obtener equipo por ID
- `POST /api/teams` - Crear nuevo equipo

### Players (Jugadores)
- `GET /api/players` - Obtener todos los jugadores
- `POST /api/players` - Crear nuevo jugador

### Matches (Partidos)
- `GET /api/matches` - Obtener todos los partidos

---

## Fase 1: Configuración Inicial del Proyecto Angular

### 1.1 Configuración de Entorno
- [ ] Crear archivo `src/environments/environment.ts`
  ```typescript
  export const environment = {
    production: false,
    apiUrl: 'https://localhost:7173/api'
  };
  ```
- [ ] Crear archivo `src/environments/environment.prod.ts`
  ```typescript
  export const environment = {
    production: true,
    apiUrl: 'https://tu-api-produccion.com/api'
  };
  ```

### 1.2 Configuración de CORS en el API
- [ ] Verificar/Configurar CORS en `Program.cs` del API para permitir peticiones desde Angular
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowAngular",
          policy => policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
  });
  ```
- [ ] Agregar `app.UseCors("AllowAngular");` en el pipeline

### 1.3 Instalación de Dependencias
- [ ] Verificar que `@angular/common/http` está instalado
- [ ] Instalar librería para manejo de fechas (opcional): `npm install date-fns`
- [ ] Instalar Angular Material (opcional para UI): `ng add @angular/material`

---

## Fase 2: Creación de Modelos (Interfaces TypeScript)

### 2.1 Crear carpeta de modelos
- [ ] Crear carpeta `src/app/models`

### 2.2 Crear interfaces para DTOs

#### TeamDto (`src/app/models/team.model.ts`)
- [ ] Crear interface `Team`
  ```typescript
  export interface Team {
    id: string;
    name: string;
    shortName: string;
    logo?: string;
    foundedDate: Date;
    city: string;
    country: string;
    stadium?: string;
    sportType: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt?: Date;
  }
  ```

#### PlayerDto (`src/app/models/player.model.ts`)
- [ ] Crear interface `Player`
  ```typescript
  export interface Player {
    id: string;
    firstName: string;
    lastName: string;
    fullName: string;
    dateOfBirth: Date;
    age: number;
    nationality: string;
    height: number;
    weight: number;
    position: string;
    jerseyNumber: number;
    photo?: string;
    isActive: boolean;
    teamId: string;
    teamName?: string;
    createdAt: Date;
    updatedAt?: Date;
  }
  ```

#### MatchDto (`src/app/models/match.model.ts`)
- [ ] Crear interface `Match`
  ```typescript
  export interface Match {
    id: string;
    homeTeamId: string;
    homeTeamName: string;
    awayTeamId: string;
    awayTeamName: string;
    scheduledDate: Date;
    startDate?: Date;
    endDate?: Date;
    venue: string;
    competition?: string;
    season?: string;
    homeTeamScore: number;
    awayTeamScore: number;
    status: string;
    sportType: string;
    winner: string;
    createdAt: Date;
    updatedAt?: Date;
  }
  ```

#### MatchStatisticDto (`src/app/models/match-statistic.model.ts`)
- [ ] Crear interface `MatchStatistic`
  ```typescript
  export interface MatchStatistic {
    id: string;
    matchId: string;
    playerId: string;
    playerName: string;
    goals: number;
    assists: number;
    yellowCards: number;
    redCards: number;
    minutesPlayed: number;
    shots: number;
    shotsOnTarget: number;
    passes: number;
    passesCompleted: number;
    fouls: number;
    foulsReceived: number;
    passAccuracy: number;
    shotAccuracy: number;
    createdAt: Date;
    updatedAt?: Date;
  }
  ```

### 2.3 Crear interfaces para Commands

#### CreateTeamCommand (`src/app/models/create-team.model.ts`)
- [ ] Crear interface `CreateTeamCommand`
  ```typescript
  export interface CreateTeamCommand {
    name: string;
    shortName: string;
    foundedDate: Date;
    city: string;
    country: string;
    sportType: number; // SportType enum
    logo?: string;
    stadium?: string;
  }
  ```

#### CreatePlayerCommand (`src/app/models/create-player.model.ts`)
- [ ] Crear interface `CreatePlayerCommand`
  ```typescript
  export interface CreatePlayerCommand {
    firstName: string;
    lastName: string;
    dateOfBirth: Date;
    nationality: string;
    height: number;
    weight: number;
    position: number; // Position enum
    jerseyNumber: number;
    teamId: string;
    photo?: string;
  }
  ```

### 2.4 Crear Enums

#### SportType (`src/app/models/enums/sport-type.enum.ts`)
- [ ] Crear enum `SportType`
  ```typescript
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
  ```

#### Position (`src/app/models/enums/position.enum.ts`)
- [ ] Crear enum `Position`
  ```typescript
  export enum Position {
    Goalkeeper = 1,
    Defender = 2,
    Midfielder = 3,
    Forward = 4,
    Center = 5,
    Guard = 6,
    ForwardBasketball = 7,
    Pitcher = 8,
    Catcher = 9,
    Infielder = 10,
    Outfielder = 11
  }
  ```

#### MatchStatus (`src/app/models/enums/match-status.enum.ts`)
- [ ] Crear enum `MatchStatus`
  ```typescript
  export enum MatchStatus {
    Scheduled = 1,
    InProgress = 2,
    Finished = 3,
    Postponed = 4,
    Cancelled = 5,
    Suspended = 6
  }
  ```

---

## Fase 3: Creación de Servicios HTTP

### 3.1 Configuración del HttpClient
- [ ] En `app.config.ts` (Angular 20), asegurar que `provideHttpClient()` está configurado
  ```typescript
  import { provideHttpClient, withInterceptors } from '@angular/common/http';
  
  export const appConfig: ApplicationConfig = {
    providers: [
      provideHttpClient()
    ]
  };
  ```

### 3.2 Crear carpeta de servicios
- [ ] Crear carpeta `src/app/services`

### 3.3 Servicio Base (opcional pero recomendado)

#### BaseApiService (`src/app/services/base-api.service.ts`)
- [ ] Crear servicio base con configuración común
  ```typescript
  import { Injectable } from '@angular/core';
  import { HttpClient, HttpErrorResponse } from '@angular/common/http';
  import { Observable, throwError } from 'rxjs';
  import { catchError } from 'rxjs/operators';
  import { environment } from '../../environments/environment';

  @Injectable({
    providedIn: 'root'
  })
  export abstract class BaseApiService {
    protected apiUrl = environment.apiUrl;

    constructor(protected http: HttpClient) {}

    protected handleError(error: HttpErrorResponse): Observable<never> {
      let errorMessage = 'An error occurred';
      
      if (error.error instanceof ErrorEvent) {
        errorMessage = `Error: ${error.error.message}`;
      } else {
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
      
      console.error(errorMessage);
      return throwError(() => new Error(errorMessage));
    }
  }
  ```

### 3.4 TeamService (`src/app/services/team.service.ts`)
- [ ] Crear servicio `TeamService`
  ```typescript
  import { Injectable } from '@angular/core';
  import { Observable } from 'rxjs';
  import { catchError } from 'rxjs/operators';
  import { BaseApiService } from './base-api.service';
  import { Team } from '../models/team.model';
  import { CreateTeamCommand } from '../models/create-team.model';

  @Injectable({
    providedIn: 'root'
  })
  export class TeamService extends BaseApiService {
    private endpoint = `${this.apiUrl}/teams`;

    getAllTeams(): Observable<Team[]> {
      return this.http.get<Team[]>(this.endpoint)
        .pipe(catchError(this.handleError));
    }

    getTeamById(id: string): Observable<Team> {
      return this.http.get<Team>(`${this.endpoint}/${id}`)
        .pipe(catchError(this.handleError));
    }

    createTeam(command: CreateTeamCommand): Observable<string> {
      return this.http.post<string>(this.endpoint, command)
        .pipe(catchError(this.handleError));
    }
  }
  ```

### 3.5 PlayerService (`src/app/services/player.service.ts`)
- [ ] Crear servicio `PlayerService`
  ```typescript
  import { Injectable } from '@angular/core';
  import { Observable } from 'rxjs';
  import { catchError } from 'rxjs/operators';
  import { BaseApiService } from './base-api.service';
  import { Player } from '../models/player.model';
  import { CreatePlayerCommand } from '../models/create-player.model';

  @Injectable({
    providedIn: 'root'
  })
  export class PlayerService extends BaseApiService {
    private endpoint = `${this.apiUrl}/players`;

    getAllPlayers(): Observable<Player[]> {
      return this.http.get<Player[]>(this.endpoint)
        .pipe(catchError(this.handleError));
    }

    createPlayer(command: CreatePlayerCommand): Observable<string> {
      return this.http.post<string>(this.endpoint, command)
        .pipe(catchError(this.handleError));
    }
  }
  ```

### 3.6 MatchService (`src/app/services/match.service.ts`)
- [ ] Crear servicio `MatchService`
  ```typescript
  import { Injectable } from '@angular/core';
  import { Observable } from 'rxjs';
  import { catchError } from 'rxjs/operators';
  import { BaseApiService } from './base-api.service';
  import { Match } from '../models/match.model';

  @Injectable({
    providedIn: 'root'
  })
  export class MatchService extends BaseApiService {
    private endpoint = `${this.apiUrl}/matches`;

    getAllMatches(): Observable<Match[]> {
      return this.http.get<Match[]>(this.endpoint)
        .pipe(catchError(this.handleError));
    }
  }
  ```

---

## Fase 4: Creación de Componentes

### 4.1 Estructura de Carpetas de Componentes
- [ ] Crear carpeta `src/app/components/teams`
- [ ] Crear carpeta `src/app/components/players`
- [ ] Crear carpeta `src/app/components/matches`
- [ ] Crear carpeta `src/app/components/shared` (para componentes compartidos)

### 4.2 Componentes de Teams

#### team-list.component
- [ ] Generar componente: `ng generate component components/teams/team-list`
- [ ] Implementar listado de equipos
  - [ ] Inyectar `TeamService`
  - [ ] Cargar equipos en `ngOnInit()`
  - [ ] Mostrar tabla/cards con equipos
  - [ ] Agregar botón para crear nuevo equipo
  - [ ] Agregar navegación a detalle de equipo

#### team-detail.component
- [ ] Generar componente: `ng generate component components/teams/team-detail`
- [ ] Implementar detalle de equipo
  - [ ] Obtener ID desde ruta (`ActivatedRoute`)
  - [ ] Cargar equipo por ID
  - [ ] Mostrar toda la información del equipo
  - [ ] Mostrar jugadores del equipo (filtrados)
  - [ ] Agregar botón de regreso a lista

#### team-form.component
- [ ] Generar componente: `ng generate component components/teams/team-form`
- [ ] Implementar formulario de creación
  - [ ] Crear `FormGroup` con validaciones
  - [ ] Campos: name, shortName, city, country, foundedDate, sportType, logo, stadium
  - [ ] Dropdown para SportType (usar enum)
  - [ ] Date picker para foundedDate
  - [ ] Manejo de errores de validación
  - [ ] Submit con `TeamService.createTeam()`
  - [ ] Redireccionar a lista después de crear

### 4.3 Componentes de Players

#### player-list.component
- [ ] Generar componente: `ng generate component components/players/player-list`
- [ ] Implementar listado de jugadores
  - [ ] Inyectar `PlayerService` y `TeamService`
  - [ ] Cargar jugadores en `ngOnInit()`
  - [ ] Mostrar tabla con jugadores (incluir nombre de equipo)
  - [ ] Agregar filtros (por equipo, posición, etc.)
  - [ ] Agregar botón para crear nuevo jugador
  - [ ] Mostrar foto del jugador si existe

#### player-form.component
- [ ] Generar componente: `ng generate component components/players/player-form`
- [ ] Implementar formulario de creación
  - [ ] Crear `FormGroup` con validaciones
  - [ ] Campos: firstName, lastName, dateOfBirth, nationality, height, weight, position, jerseyNumber, teamId, photo
  - [ ] Dropdown para Position (usar enum)
  - [ ] Dropdown para Team (cargar desde API)
  - [ ] Date picker para dateOfBirth
  - [ ] Validación de jerseyNumber (número positivo)
  - [ ] Submit con `PlayerService.createPlayer()`
  - [ ] Redireccionar a lista después de crear

### 4.4 Componentes de Matches

#### match-list.component
- [ ] Generar componente: `ng generate component components/matches/match-list`
- [ ] Implementar listado de partidos
  - [ ] Inyectar `MatchService`
  - [ ] Cargar partidos en `ngOnInit()`
  - [ ] Mostrar tabla/cards con partidos
  - [ ] Mostrar score de cada equipo
  - [ ] Indicador visual del status (Scheduled, InProgress, Finished, etc.)
  - [ ] Filtros por fecha, status, equipo
  - [ ] Ordenamiento por fecha programada

#### match-detail.component (opcional)
- [ ] Generar componente: `ng generate component components/matches/match-detail`
- [ ] Implementar detalle de partido
  - [ ] Mostrar información completa del partido
  - [ ] Mostrar estadísticas del partido (si están disponibles)
  - [ ] Mostrar alineaciones de equipos

### 4.5 Componentes Compartidos

#### navigation.component
- [ ] Generar componente: `ng generate component components/shared/navigation`
- [ ] Implementar menú de navegación
  - [ ] Links a Teams, Players, Matches
  - [ ] Logo de la aplicación
  - [ ] Responsive design

#### loading-spinner.component
- [ ] Generar componente: `ng generate component components/shared/loading-spinner`
- [ ] Implementar spinner de carga

#### error-message.component
- [ ] Generar componente: `ng generate component components/shared/error-message`
- [ ] Implementar componente para mostrar errores

---

## Fase 5: Configuración de Rutas

### 5.1 Configurar app.routes.ts
- [ ] Crear/Actualizar `src/app/app.routes.ts`
  ```typescript
  import { Routes } from '@angular/router';
  import { TeamListComponent } from './components/teams/team-list/team-list.component';
  import { TeamDetailComponent } from './components/teams/team-detail/team-detail.component';
  import { TeamFormComponent } from './components/teams/team-form/team-form.component';
  import { PlayerListComponent } from './components/players/player-list/player-list.component';
  import { PlayerFormComponent } from './components/players/player-form/player-form.component';
  import { MatchListComponent } from './components/matches/match-list/match-list.component';

  export const routes: Routes = [
    { path: '', redirectTo: '/teams', pathMatch: 'full' },
    { path: 'teams', component: TeamListComponent },
    { path: 'teams/new', component: TeamFormComponent },
    { path: 'teams/:id', component: TeamDetailComponent },
    { path: 'players', component: PlayerListComponent },
    { path: 'players/new', component: PlayerFormComponent },
    { path: 'matches', component: MatchListComponent },
    { path: '**', redirectTo: '/teams' }
  ];
  ```

### 5.2 Configurar RouterModule
- [ ] Verificar que `provideRouter(routes)` está en `app.config.ts`

---

## Fase 6: Utilidades y Helpers

### 6.1 Crear carpeta de utilidades
- [ ] Crear carpeta `src/app/utils`

### 6.2 Enum Helpers
- [ ] Crear `src/app/utils/enum-helpers.ts`
  ```typescript
  import { SportType } from '../models/enums/sport-type.enum';
  import { Position } from '../models/enums/position.enum';
  import { MatchStatus } from '../models/enums/match-status.enum';

  export class EnumHelpers {
    static getSportTypeName(value: number): string {
      return SportType[value] || 'Unknown';
    }

    static getPositionName(value: number): string {
      return Position[value] || 'Unknown';
    }

    static getMatchStatusName(value: number): string {
      return MatchStatus[value] || 'Unknown';
    }

    static getSportTypeOptions(): Array<{value: number, label: string}> {
      return Object.keys(SportType)
        .filter(key => !isNaN(Number(key)))
        .map(key => ({
          value: Number(key),
          label: SportType[Number(key)]
        }));
    }

    static getPositionOptions(): Array<{value: number, label: string}> {
      return Object.keys(Position)
        .filter(key => !isNaN(Number(key)))
        .map(key => ({
          value: Number(key),
          label: Position[Number(key)]
        }));
    }
  }
  ```

### 6.3 Date Helpers
- [ ] Crear `src/app/utils/date-helpers.ts`
  ```typescript
  export class DateHelpers {
    static formatDate(date: Date | string): string {
      const d = new Date(date);
      return d.toLocaleDateString();
    }

    static formatDateTime(date: Date | string): string {
      const d = new Date(date);
      return d.toLocaleString();
    }

    static calculateAge(birthDate: Date | string): number {
      const today = new Date();
      const birth = new Date(birthDate);
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

## Fase 7: Interceptores HTTP (Opcional pero Recomendado)

### 7.1 Loading Interceptor
- [ ] Crear `src/app/interceptors/loading.interceptor.ts`
  - [ ] Mostrar/ocultar spinner durante peticiones HTTP
  - [ ] Crear servicio `LoadingService` para manejar estado global

### 7.2 Error Interceptor
- [ ] Crear `src/app/interceptors/error.interceptor.ts`
  - [ ] Capturar errores HTTP globalmente
  - [ ] Mostrar mensajes de error amigables
  - [ ] Log de errores en consola

### 7.3 Auth Interceptor (para futuro)
- [ ] Crear `src/app/interceptors/auth.interceptor.ts`
  - [ ] Agregar token de autenticación a headers
  - [ ] Manejar 401 Unauthorized

### 7.4 Registrar Interceptores
- [ ] En `app.config.ts`, registrar interceptores con `withInterceptors()`

---

## Fase 8: Manejo de Estado (Opcional - Nivel Avanzado)

### 8.1 Usar Signals (Angular 20)
- [ ] Investigar uso de `signal()`, `computed()`, `effect()`
- [ ] Implementar estado reactivo en servicios
- [ ] Ejemplo:
  ```typescript
  import { signal } from '@angular/core';
  
  teams = signal<Team[]>([]);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  ```

### 8.2 Alternativa: NgRx (si el proyecto crece)
- [ ] Instalar NgRx: `ng add @ngrx/store`
- [ ] Crear store para Teams, Players, Matches
- [ ] Implementar actions, reducers, effects

---

## Fase 9: Testing

### 9.1 Unit Tests de Servicios
- [ ] Test para `TeamService`
  - [ ] Mock HttpClient
  - [ ] Probar `getAllTeams()`
  - [ ] Probar `getTeamById()`
  - [ ] Probar `createTeam()`
- [ ] Test para `PlayerService`
- [ ] Test para `MatchService`

### 9.2 Unit Tests de Componentes
- [ ] Test para `TeamListComponent`
- [ ] Test para `TeamFormComponent`
- [ ] Test para `PlayerListComponent`
- [ ] Test para `MatchListComponent`

### 9.3 E2E Tests (Opcional)
- [ ] Configurar Playwright o Cypress
- [ ] Crear tests para flujos principales:
  - [ ] Crear equipo
  - [ ] Listar equipos
  - [ ] Ver detalle de equipo
  - [ ] Crear jugador

---

## Fase 10: UI/UX y Estilos

### 10.1 Framework CSS
- [ ] Decidir framework: Angular Material, Bootstrap, TailwindCSS
- [ ] Instalar y configurar

### 10.2 Diseño Responsive
- [ ] Asegurar que todos los componentes son responsive
- [ ] Mobile-first approach
- [ ] Probar en diferentes resoluciones

### 10.3 Tema y Branding
- [ ] Definir paleta de colores
- [ ] Crear variables CSS globales
- [ ] Logo y favicon de la aplicación

### 10.4 Componentes UI
- [ ] Cards para equipos/jugadores/partidos
- [ ] Tablas con paginación y ordenamiento
- [ ] Formularios con validación visual
- [ ] Modales/Dialogs
- [ ] Toasts/Snackbars para notificaciones

---

## Fase 11: Funcionalidades Adicionales

### 11.1 Búsqueda y Filtrado
- [ ] Implementar búsqueda en tiempo real en listas
- [ ] Filtros avanzados (por múltiples criterios)
- [ ] Debounce en inputs de búsqueda

### 11.2 Paginación
- [ ] Implementar paginación del lado del cliente
- [ ] Mostrar resultados por página (10, 25, 50)
- [ ] Navegación entre páginas

### 11.3 Ordenamiento
- [ ] Ordenar por columnas en tablas
- [ ] Ascendente/Descendente
- [ ] Indicadores visuales de ordenamiento

### 11.4 Exportación de Datos
- [ ] Exportar a CSV
- [ ] Exportar a PDF
- [ ] Imprimir listados

### 11.5 Gráficos y Estadísticas
- [ ] Instalar Chart.js o similar
- [ ] Dashboard con estadísticas generales
- [ ] Gráficos de rendimiento de jugadores
- [ ] Gráficos de resultados de equipos

---

## Fase 12: Optimización y Performance

### 12.1 Lazy Loading
- [ ] Implementar lazy loading de módulos/componentes
- [ ] Cargar rutas de forma diferida

### 12.2 Caching
- [ ] Implementar cache en servicios HTTP
- [ ] Cache en LocalStorage para datos frecuentes
- [ ] Service Worker para PWA (opcional)

### 12.3 Optimización de Imágenes
- [ ] Lazy loading de imágenes
- [ ] Placeholder mientras cargan
- [ ] Optimizar tamaño y formato

### 12.4 Change Detection Strategy
- [ ] Usar `OnPush` en componentes apropiados
- [ ] Optimizar re-renderizados

---

## Fase 13: Seguridad

### 13.1 Sanitización
- [ ] Sanitizar inputs de usuario
- [ ] Prevenir XSS

### 13.2 Validaciones
- [ ] Validaciones en frontend (complementar backend)
- [ ] Mensajes de error claros

### 13.3 HTTPS
- [ ] Forzar HTTPS en producción
- [ ] Configurar certificados SSL

---

## Fase 14: Deployment

### 14.1 Build de Producción
- [ ] Ejecutar `ng build --configuration production`
- [ ] Verificar que no hay errores
- [ ] Analizar bundle size: `ng build --stats-json`

### 14.2 Configuración de Servidor
- [ ] Configurar servidor web (nginx, IIS, etc.)
- [ ] Configurar redirects para SPA routing
- [ ] Configurar variables de entorno

### 14.3 CI/CD
- [ ] Configurar pipeline de CI/CD (GitHub Actions, Azure DevOps, etc.)
- [ ] Automatizar build y deployment
- [ ] Tests automáticos antes de deploy

---

## Fase 15: Documentación

### 15.1 Documentación de Código
- [ ] Agregar comentarios JSDoc en servicios
- [ ] Documentar interfaces complejas
- [ ] README.md del proyecto Angular

### 15.2 Guía de Usuario
- [ ] Crear manual de usuario
- [ ] Screenshots de la aplicación
- [ ] Video tutorial (opcional)

### 15.3 Documentación Técnica
- [ ] Arquitectura de la aplicación
- [ ] Diagramas de flujo
- [ ] Guía de desarrollo para nuevos contribuidores

---

## Checklist de Validación Final

### Funcionalidad
- [ ] Todas las peticiones HTTP funcionan correctamente
- [ ] CRUD completo para todas las entidades (donde aplique)
- [ ] Manejo de errores en todas las operaciones
- [ ] Validaciones de formularios funcionan
- [ ] Navegación entre páginas funciona

### UI/UX
- [ ] Diseño responsive en móvil, tablet y desktop
- [ ] Loading states en peticiones HTTP
- [ ] Mensajes de éxito/error al usuario
- [ ] Navegación intuitiva
- [ ] Accesibilidad (ARIA labels, contraste, etc.)

### Performance
- [ ] Tiempo de carga inicial < 3 segundos
- [ ] No hay memory leaks
- [ ] Optimización de imágenes
- [ ] Lazy loading implementado

### Seguridad
- [ ] Inputs sanitizados
- [ ] CORS configurado correctamente
- [ ] HTTPS en producción
- [ ] Validaciones en frontend y backend

### Testing
- [ ] Unit tests con > 80% cobertura
- [ ] E2E tests para flujos críticos
- [ ] Tests pasan en CI/CD

### Deployment
- [ ] Build de producción exitoso
- [ ] Aplicación deployada y accesible
- [ ] Variables de entorno configuradas
- [ ] Monitoreo y logs configurados

---

## Recursos Útiles

### Documentación Oficial
- [Angular Documentation](https://angular.dev)
- [RxJS Documentation](https://rxjs.dev)
- [TypeScript Documentation](https://www.typescriptlang.org/docs)

### Tutoriales y Guías
- [Angular HTTP Client Guide](https://angular.dev/guide/http)
- [Angular Forms Guide](https://angular.dev/guide/forms)
- [Angular Routing Guide](https://angular.dev/guide/routing)

### Herramientas
- [Angular DevTools](https://angular.io/guide/devtools)
- [Postman](https://www.postman.com) - Para probar endpoints del API
- [JSON Server](https://github.com/typicode/json-server) - Para mock API durante desarrollo

---

## Notas Importantes

1. **Orden de Implementación:** Seguir el orden sugerido facilita el desarrollo incremental y permite probar cada parte antes de continuar.

2. **API Running:** Asegúrate de que el API de .NET esté corriendo antes de probar la integración Angular.

3. **CORS:** Este es uno de los problemas más comunes. Verifica la configuración de CORS en el API si tienes errores de red.

4. **Versionado:** Commitea cambios frecuentemente con mensajes descriptivos.

5. **Branch Strategy:** Considera usar feature branches para cada funcionalidad nueva.

6. **Code Reviews:** Si trabajas en equipo, implementa code reviews antes de mergear.

7. **Manejo de Fechas:** Ten cuidado con las zonas horarias al trabajar con fechas entre .NET y TypeScript.

8. **Enums:** Los enums de TypeScript se serializan como números, asegúrate de que coincidan con los valores de C#.

---

## Tiempo Estimado por Fase

- **Fase 1:** 1-2 horas
- **Fase 2:** 2-3 horas
- **Fase 3:** 3-4 horas
- **Fase 4:** 10-15 horas (la más extensa)
- **Fase 5:** 1 hora
- **Fase 6:** 2-3 horas
- **Fase 7:** 2-3 horas
- **Fase 8:** 4-6 horas (opcional)
- **Fase 9:** 5-8 horas
- **Fase 10:** 8-12 horas
- **Fase 11:** 6-10 horas
- **Fase 12:** 3-5 horas
- **Fase 13:** 2-3 horas
- **Fase 14:** 3-5 horas
- **Fase 15:** 4-6 horas

**Total Estimado:** 56-87 horas (aprox. 1.5-2.5 semanas de trabajo a tiempo completo)

---

## Próximos Pasos Inmediatos

1. ✅ Revisar este checklist completo
2. ⏭️ Iniciar con **Fase 1: Configuración Inicial**
3. ⏭️ Crear las interfaces de modelos (**Fase 2**)
4. ⏭️ Implementar servicios HTTP (**Fase 3**)
5. ⏭️ Crear componentes básicos (**Fase 4**)

---

**¡Buena suerte con la implementación! 🚀**
