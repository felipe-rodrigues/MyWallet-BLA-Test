# Task Management API Generation with AI

  

This project demonstrates how I used an AI assistant to generate a RESTful API and frontend for a simple task management system using .NET (C#) and Angular.

  

# Process Overview

  

1.  **Prompt Creation**

- I created a prompt describing the requirements for the system:

- CRUD operations for tasks

- Each task has a title, description, status, and due_date

- Tasks are associated with a user (assuming a basic User model exists)

  

2.  **Prompt Used**

  

```

Create a RESTful API for a simple task management system using .NET (C#) for the backend and Angular for the frontend. The required functionalities are:

  

- Create, read, update, and delete tasks (CRUD)

- Each task should have the following fields: title, description, status, and due_date

- Each task is associated with a user (assume a basic User model already exists)

  

Please provide:

- The backend API structure (controllers, models, and endpoints) in C#

- The Angular frontend structure (services, components, and models) to interact with the API

```

  

  

# OUTPUT AI

## Task Manager – .NET 8 Web API + Angular 16 Starter Kit

This guide includes a minimal but production‑ready structure for a REST API and an Angular client. It uses:
- **.NET 8**, EF Core (SQLite for simplicity), AutoMapper, and a clean service layer
- **Angular 16+** with HttpClient, routing, and reactive forms

---

## Backend (.NET 8)

### Project layout
```
TaskManager.Api/
  Program.cs
  appsettings.json
  Properties/
    launchSettings.json
  Controllers/
    TasksController.cs
  Services/
    ITaskService.cs
    TaskService.cs
  Data/
    AppDbContext.cs
  Domain/
    TaskItem.cs
    User.cs
  Dtos/
    TaskDtos.cs
  Mapping/
    MappingProfile.cs
```

### Domain models (Domain/TaskItem.cs, Domain/User.cs)
```csharp
// Domain/TaskItem.cs
namespace TaskManager.Api.Domain;

public enum TaskStatus { Todo = 0, InProgress = 1, Done = 2 }

public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateOnly? DueDate { get; set; }

    // Association
    public int UserId { get; set; }
    public User? User { get; set; }
}

// Domain/User.cs (placeholder; assume your real User model already exists)
namespace TaskManager.Api.Domain;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
```

### EF Core DbContext (Data/AppDbContext.cs)
```csharp
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Domain;

namespace TaskManager.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(b =>
        {
            b.Property(p => p.Title).IsRequired().HasMaxLength(200);
            b.Property(p => p.Status).HasConversion<int>();
            b.Property(p => p.DueDate).HasConversion(
                v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : null,
                v => v.HasValue ? DateOnly.FromDateTime(v.Value) : null);
            b.HasOne(t => t.User)
             .WithMany(u => u.Tasks)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed a demo user
        modelBuilder.Entity<User>().HasData(new User { Id = 1, Name = "Demo User" });
    }
}
```

### DTOs (Dtos/TaskDtos.cs)
```csharp
using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Domain;

namespace TaskManager.Api.Dtos;

public record TaskDto(int Id, string Title, string? Description, TaskStatus Status, DateOnly? DueDate, int UserId);

public class CreateTaskDto
{
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateOnly? DueDate { get; set; }
    [Required] public int UserId { get; set; }
}

public class UpdateTaskDto
{
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateOnly? DueDate { get; set; }
}
```

### AutoMapper profile (Mapping/MappingProfile.cs)
```csharp
using AutoMapper;
using TaskManager.Api.Domain;
using TaskManager.Api.Dtos;

namespace TaskManager.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>();
        CreateMap<CreateTaskDto, TaskItem>();
        CreateMap<UpdateTaskDto, TaskItem>();
    }
}
```

### Service layer (Services/ITaskService.cs, Services/TaskService.cs)
```csharp
// Services/ITaskService.cs
using TaskManager.Api.Dtos;

namespace TaskManager.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> GetAllAsync(int? userId = null, string? status = null, DateOnly? dueBefore = null);
    Task<TaskDto?> GetByIdAsync(int id);
    Task<TaskDto> CreateAsync(CreateTaskDto dto);
    Task<bool> UpdateAsync(int id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(int id);
}
```
```csharp
// Services/TaskService.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Domain;
using TaskManager.Api.Dtos;

namespace TaskManager.Api.Services;

public class TaskService(AppDbContext db, IMapper mapper) : ITaskService
{
    public async Task<IReadOnlyList<TaskDto>> GetAllAsync(int? userId = null, string? status = null, DateOnly? dueBefore = null)
    {
        var query = db.Tasks.AsNoTracking().AsQueryable();

        if (userId.HasValue) query = query.Where(t => t.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TaskStatus>(status, true, out var st))
            query = query.Where(t => t.Status == st);
        if (dueBefore.HasValue)
            query = query.Where(t => t.DueDate != null && t.DueDate <= dueBefore.Value);

        return await query
            .OrderBy(t => t.DueDate)
            .ProjectTo<TaskDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TaskDto?> GetByIdAsync(int id)
        => await db.Tasks.AsNoTracking()
            .Where(t => t.Id == id)
            .ProjectTo<TaskDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
    {
        // Ensure User exists (basic check)
        var userExists = await db.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists) throw new ArgumentException($"User {dto.UserId} not found");

        var entity = mapper.Map<TaskItem>(dto);
        db.Tasks.Add(entity);
        await db.SaveChangesAsync();
        return mapper.Map<TaskDto>(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTaskDto dto)
    {
        var entity = await db.Tasks.FindAsync(id);
        if (entity is null) return false;
        mapper.Map(dto, entity);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Tasks.FindAsync(id);
        if (entity is null) return false;
        db.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
```

### Controller (Controllers/TasksController.cs)
```csharp
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Dtos;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService service) : ControllerBase
{
    /// GET /api/tasks?userId=1&status=Todo&dueBefore=2025-09-01
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll([FromQuery] int? userId, [FromQuery] string? status, [FromQuery] DateOnly? dueBefore)
        => Ok(await service.GetAllAsync(userId, status, dueBefore));

    /// GET /api/tasks/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
        => (await service.GetByIdAsync(id)) is { } dto ? Ok(dto) : NotFound();

    /// POST /api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// PUT /api/tasks/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
        => await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    /// DELETE /api/tasks/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await service.DeleteAsync(id) ? NoContent() : NotFound();
}
```

### Program.cs (root)
```csharp
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Mapping;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=tasks.db"));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Ensure DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "Default": "Data Source=tasks.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## Frontend (Angular 16+)

### Project layout
```
src/app/
  models/task.ts
  services/task.service.ts
  features/tasks/
    task-list/
      task-list.component.ts
      task-list.component.html
    task-form/
      task-form.component.ts
      task-form.component.html
  app.routes.ts
  app.component.ts
  app.component.html
```

### Model (models/task.ts)
```ts
export enum TaskStatus { Todo = 0, InProgress = 1, Done = 2 }

export interface Task {
  id: number;
  title: string;
  description?: string | null;
  status: TaskStatus;
  dueDate?: string | null; // ISO 'YYYY-MM-DD'
  userId: number;
}

export interface CreateTask {
  title: string;
  description?: string | null;
  status: TaskStatus;
  dueDate?: string | null;
  userId: number;
}

export interface UpdateTask {
  title: string;
  description?: string | null;
  status: TaskStatus;
  dueDate?: string | null;
}
```

### Service (services/task.service.ts)
```ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTask, UpdateTask } from '../models/task';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private http = inject(HttpClient);
  private baseUrl = '/api/tasks'; // adjust if different base path

  getAll(filters?: { userId?: number; status?: string; dueBefore?: string }): Observable<Task[]> {
    let params = new HttpParams();
    if (filters?.userId) params = params.set('userId', filters.userId);
    if (filters?.status) params = params.set('status', filters.status);
    if (filters?.dueBefore) params = params.set('dueBefore', filters.dueBefore);
    return this.http.get<Task[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateTask): Observable<Task> {
    return this.http.post<Task>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateTask): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
```

### Routes (app.routes.ts)
```ts
import { Routes } from '@angular/router';
import { TaskListComponent } from './features/tasks/task-list/task-list.component';
import { TaskFormComponent } from './features/tasks/task-form/task-form.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },
  { path: 'tasks', component: TaskListComponent },
  { path: 'tasks/new', component: TaskFormComponent },
  { path: 'tasks/:id/edit', component: TaskFormComponent },
];
```

### Task list component (features/tasks/task-list/*.ts, *.html)
```ts
// task-list.component.ts
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TaskService } from '../../../services/task.service';
import { Task, TaskStatus } from '../../../models/task';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent {
  private taskService = inject(TaskService);
  tasks: Task[] = [];

  userId = new FormControl<number | null>(1);
  status = new FormControl<string | null>(null);

  ngOnInit() {
    this.load();
  }

  load() {
    this.taskService.getAll({
      userId: this.userId.value ?? undefined,
      status: this.status.value ?? undefined
    }).subscribe(tasks => this.tasks = tasks);
  }

  asStatus(s: TaskStatus) {
    return TaskStatus[s];
  }
}
```
```html
<!-- task-list.component.html -->
<div class="container">
  <h2>Tasks</h2>

  <div class="filters" style="display:flex; gap:.5rem; align-items:center; margin:.5rem 0;">
    <label>UserId <input type="number" [formControl]="userId" (change)="load()"></label>
    <label>Status
      <select [formControl]="status" (change)="load()">
        <option [ngValue]="null">(all)</option>
        <option value="Todo">Todo</option>
        <option value="InProgress">InProgress</option>
        <option value="Done">Done</option>
      </select>
    </label>
    <a routerLink="/tasks/new">New Task</a>
  </div>

  <table border="1" cellpadding="6" cellspacing="0">
    <thead>
      <tr>
        <th>Id</th><th>Title</th><th>Status</th><th>Due</th><th>User</th><th></th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let t of tasks">
        <td>{{t.id}}</td>
        <td>{{t.title}}</td>
        <td>{{asStatus(t.status)}}</td>
        <td>{{t.dueDate || '-'}}</td>
        <td>{{t.userId}}</td>
        <td>
          <a [routerLink]="['/tasks', t.id, 'edit']">edit</a>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

### Task form component (create + edit)
```ts
// task-form.component.ts
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TaskService } from '../../../services/task.service';
import { Task, TaskStatus } from '../../../models/task';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './task-form.component.html'
})
export class TaskFormComponent {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private taskService = inject(TaskService);

  id = Number(this.route.snapshot.paramMap.get('id')) || null;

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    status: [TaskStatus.Todo, Validators.required],
    dueDate: [''], // YYYY-MM-DD
    userId: [1, Validators.required], // For create; ignored on update
  });

  get isEdit() { return !!this.id; }

  ngOnInit() {
    if (this.isEdit) {
      this.taskService.getById(this.id!).subscribe(t => {
        this.form.patchValue({
          title: t.title,
          description: t.description,
          status: t.status,
          dueDate: t.dueDate ?? '',
          userId: t.userId
        });
      });
    }
  }

  save() {
    const v = this.form.getRawValue();
    if (this.isEdit) {
      const update = { title: v.title!, description: v.description ?? null, status: v.status!, dueDate: v.dueDate || null };
      this.taskService.update(this.id!, update).subscribe(() => this.router.navigate(['/tasks']));
    } else {
      const create = { title: v.title!, description: v.description ?? null, status: v.status!, dueDate: v.dueDate || null, userId: v.userId! };
      this.taskService.create(create).subscribe(() => this.router.navigate(['/tasks']));
    }
  }
}
```
```html
<!-- task-form.component.html -->
<h2 *ngIf="!isEdit">New Task</h2>
<h2 *ngIf="isEdit">Edit Task</h2>

<form [formGroup]="form" (ngSubmit)="save()" style="display:grid; gap:.5rem; max-width:480px;">
  <label>Title
    <input type="text" formControlName="title">
  </label>
  <label>Description
    <textarea formControlName="description" rows="3"></textarea>
  </label>
  <label>Status
    <select formControlName="status">
      <option [ngValue]="0">Todo</option>
      <option [ngValue]="1">InProgress</option>
      <option [ngValue]="2">Done</option>
    </select>
  </label>
  <label>Due Date
    <input type="date" formControlName="dueDate">
  </label>
  <label *ngIf="!isEdit">User Id
    <input type="number" formControlName="userId">
  </label>

  <button type="submit" [disabled]="form.invalid">Save</button>
  <a routerLink="/tasks">Cancel</a>
</form>
```

### App component shell (app.component.*)
```ts
// app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html'
})
export class AppComponent { }
```
```html
<!-- app.component.html -->
<h1>Task Manager</h1>
<router-outlet></router-outlet>
```

### Notes
- Adjust `baseUrl` in `TaskService` if your API isn’t served under the same origin (configure Angular proxy accordingly).
- The API accepts filters via query string: `userId`, `status` (Todo|InProgress|Done), `dueBefore` (YYYY‑MM‑DD).
- For auth, plug your Identity/JWT middleware; the service layer already enforces user existence on create.
- The backend uses `DateOnly` and converts it to `DateTime` via EF for SQLite compatibility.
- Run `dotnet ef migrations add Init && dotnet ef database update` to create the DB.
```bash
# Dev run
cd TaskManager.Api && dotnet run
# Angular (assuming standalone bootstrapped app)
ng serve --proxy-config proxy.conf.json
```

### Example Angular proxy (proxy.conf.json)
```json
{
  "/api": {
    "target": "https://localhost:5001",
    "secure": false,
    "changeOrigin": true
  }
}
```

---
***


# Personal comments and considerations

 - For validating the IA , I would set up an environment, create the files and try to run in the first step.
 - I would ask for better improvement in the layout using libraries such as bootstrap or angular material
 - I would try to get improvements on creating authentication in authorization with Identity from Microsoft or just with JWT generator
 - For some edge cases I would asks or would do myself add back end validations and unit tests.
