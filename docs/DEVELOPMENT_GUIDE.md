# 💻 Development Guide

## Prerequisites

### Required Software

- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download))
- **Node.js 20+** ([Download](https://nodejs.org/))
- **PostgreSQL 16+** ([Download](https://www.postgresql.org/download/))
- **Redis** ([Download](https://redis.io/download))
- **Flutter 3.16+** ([Download](https://flutter.dev/docs/get-started/install))
- **Git** ([Download](https://git-scm.com/downloads))
- **Docker** (optional, for containerized development)

### Development Tools

- **Visual Studio 2022** or **VS Code** (with C# extension)
- **VS Code** (for Angular development)
- **Android Studio** (for Flutter Android development)
- **Xcode** (for Flutter iOS development, macOS only)
- **Postman** or **Insomnia** (for API testing)
- **Azure Data Studio** or **pgAdmin** (for database management)

---

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/device-ownership-platform.git
cd device-ownership-platform
```

### 2. Set Up the Database

```bash
# Create PostgreSQL database
createdb DeviceOwnership

# Run schema script
psql -d DeviceOwnership -f database/schema.sql

# Or use EF Core migrations (recommended)
cd backend
dotnet ef database update -p src/DeviceOwnership.Infrastructure -s src/DeviceOwnership.API
```

### 3. Configure Backend

Create `backend/src/DeviceOwnership.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=DeviceOwnership;Username=postgres;Password=yourpassword",
    "Redis": "localhost:6379"
  },
  "OpenIddict": {
    "Issuer": "https://localhost:5001"
  },
  "Jwt": {
    "Issuer": "https://localhost:5001",
    "Audience": "device_api",
    "SigningKey": "dev-signing-key-change-in-production"
  },
  "Email": {
    "Provider": "Console",
    "ApiKey": ""
  },
  "Sms": {
    "Provider": "Console"
  },
  "Security": {
    "EncryptionKeyName": "dev-encryption-key-change-in-production"
  }
}
```

### 4. Start Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/DeviceOwnership.API
```

API will be available at: `https://localhost:5001`

### 5. Start Frontend

```bash
cd frontend-web
npm install
ng serve
```

Web app will be available at: `http://localhost:4200`

### 6. Start Mobile App

```bash
cd mobile-app
flutter pub get
flutter run
```

---

## 🏗️ Development Workflow

### Backend Development

#### Creating a New Entity

1. **Create Entity** in `DeviceOwnership.Core/Entities/`

```csharp
public class MyEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

2. **Create Configuration** in `DeviceOwnership.Infrastructure/Data/Configurations/`

```csharp
public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.ToTable("MyEntities");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
    }
}
```

3. **Add to DbContext** in `ApplicationDbContext.cs`

```csharp
public DbSet<MyEntity> MyEntities { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new MyEntityConfiguration());
}
```

4. **Create Migration**

```bash
dotnet ef migrations add AddMyEntity -p src/DeviceOwnership.Infrastructure -s src/DeviceOwnership.API
```

5. **Apply Migration**

```bash
dotnet ef database update -p src/DeviceOwnership.Infrastructure -s src/DeviceOwnership.API
```

#### Creating a New API Endpoint

1. **Create DTOs** in `DeviceOwnership.Application/DTOs/`

```csharp
public record CreateMyEntityRequest(string Name);
public record MyEntityResponse(Guid Id, string Name, DateTime CreatedAt);
```

2. **Create Validator** in `DeviceOwnership.Application/Validators/`

```csharp
public class CreateMyEntityValidator : AbstractValidator<CreateMyEntityRequest>
{
    public CreateMyEntityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
    }
}
```

3. **Create Service Interface** in `DeviceOwnership.Core/Interfaces/Services/`

```csharp
public interface IMyEntityService
{
    Task<MyEntityResponse> CreateAsync(CreateMyEntityRequest request);
    Task<MyEntityResponse> GetByIdAsync(Guid id);
}
```

4. **Implement Service** in `DeviceOwnership.Application/Services/`

```csharp
public class MyEntityService : IMyEntityService
{
    private readonly IMyEntityRepository _repository;

    public MyEntityService(IMyEntityRepository repository)
    {
        _repository = repository;
    }

    public async Task<MyEntityResponse> CreateAsync(CreateMyEntityRequest request)
    {
        var entity = new MyEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        return new MyEntityResponse(entity.Id, entity.Name, entity.CreatedAt);
    }
}
```

5. **Create Controller** in `DeviceOwnership.API/Controllers/`

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MyEntitiesController : ControllerBase
{
    private readonly IMyEntityService _service;

    public MyEntitiesController(IMyEntityService service)
    {
        _service = service;
    }

    [HttpPost]
    [RequireScope("myentity.create")]
    public async Task<ActionResult<MyEntityResponse>> Create([FromBody] CreateMyEntityRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MyEntityResponse>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }
}
```

---

### Frontend Development (Angular)

#### Creating a New Feature

1. **Generate Feature Module**

```bash
ng generate module features/my-feature --routing
ng generate component features/my-feature/my-component
```

2. **Create Service**

```bash
ng generate service core/services/my-entity
```

```typescript
@Injectable({ providedIn: 'root' })
export class MyEntityService {
  private apiUrl = `${environment.apiUrl}/api/v1/myentities`;

  constructor(private http: HttpClient) {}

  create(request: CreateMyEntityRequest): Observable<MyEntityResponse> {
    return this.http.post<MyEntityResponse>(this.apiUrl, request);
  }

  getById(id: string): Observable<MyEntityResponse> {
    return this.http.get<MyEntityResponse>(`${this.apiUrl}/${id}`);
  }
}
```

3. **Create Component**

```typescript
@Component({
  selector: 'app-my-component',
  templateUrl: './my-component.component.html',
  styleUrls: ['./my-component.component.scss']
})
export class MyComponentComponent implements OnInit {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private myEntityService: MyEntityService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.myEntityService.create(this.form.value).subscribe({
        next: (response) => {
          console.log('Created:', response);
        },
        error: (error) => {
          console.error('Error:', error);
        }
      });
    }
  }
}
```

---

### Mobile Development (Flutter)

#### Creating a New Feature

1. **Create Model**

```dart
class MyEntity {
  final String id;
  final String name;
  final DateTime createdAt;

  MyEntity({
    required this.id,
    required this.name,
    required this.createdAt,
  });

  factory MyEntity.fromJson(Map<String, dynamic> json) {
    return MyEntity(
      id: json['id'],
      name: json['name'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}
```

2. **Create Repository**

```dart
class MyEntityRepository {
  final ApiService _apiService;

  MyEntityRepository(this._apiService);

  Future<MyEntity> create(String name) async {
    final response = await _apiService.post(
      '/api/v1/myentities',
      data: {'name': name},
    );

    return MyEntity.fromJson(response.data);
  }
}
```

3. **Create BLoC**

```dart
// Events
abstract class MyEntityEvent {}
class CreateMyEntity extends MyEntityEvent {
  final String name;
  CreateMyEntity(this.name);
}

// States
abstract class MyEntityState {}
class MyEntityInitial extends MyEntityState {}
class MyEntityLoading extends MyEntityState {}
class MyEntityLoaded extends MyEntityState {
  final MyEntity entity;
  MyEntityLoaded(this.entity);
}
class MyEntityError extends MyEntityState {
  final String message;
  MyEntityError(this.message);
}

// BLoC
class MyEntityBloc extends Bloc<MyEntityEvent, MyEntityState> {
  final MyEntityRepository _repository;

  MyEntityBloc(this._repository) : super(MyEntityInitial()) {
    on<CreateMyEntity>(_onCreateMyEntity);
  }

  Future<void> _onCreateMyEntity(
    CreateMyEntity event,
    Emitter<MyEntityState> emit,
  ) async {
    emit(MyEntityLoading());

    try {
      final entity = await _repository.create(event.name);
      emit(MyEntityLoaded(entity));
    } catch (e) {
      emit(MyEntityError(e.toString()));
    }
  }
}
```

4. **Create Screen**

```dart
class MyEntityScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => MyEntityBloc(
        context.read<MyEntityRepository>(),
      ),
      child: MyEntityView(),
    );
  }
}

class MyEntityView extends StatelessWidget {
  final TextEditingController _nameController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('My Entity')),
      body: BlocConsumer<MyEntityBloc, MyEntityState>(
        listener: (context, state) {
          if (state is MyEntityError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message)),
            );
          }
        },
        builder: (context, state) {
          if (state is MyEntityLoading) {
            return Center(child: CircularProgressIndicator());
          }

          return Padding(
            padding: EdgeInsets.all(16),
            child: Column(
              children: [
                TextField(
                  controller: _nameController,
                  decoration: InputDecoration(labelText: 'Name'),
                ),
                SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {
                    context.read<MyEntityBloc>().add(
                      CreateMyEntity(_nameController.text),
                    );
                  },
                  child: Text('Create'),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
```

---

## 🧪 Testing

### Backend Unit Tests

```csharp
public class MyEntityServiceTests
{
    private readonly Mock<IMyEntityRepository> _repositoryMock;
    private readonly MyEntityService _service;

    public MyEntityServiceTests()
    {
        _repositoryMock = new Mock<IMyEntityRepository>();
        _service = new MyEntityService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsResponse()
    {
        // Arrange
        var request = new CreateMyEntityRequest("Test");

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<MyEntity>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MyEntity>()), Times.Once);
    }
}
```

### Frontend Unit Tests

```typescript
describe('MyComponentComponent', () => {
  let component: MyComponentComponent;
  let fixture: ComponentFixture<MyComponentComponent>;
  let service: jasmine.SpyObj<MyEntityService>;

  beforeEach(() => {
    const serviceSpy = jasmine.createSpyObj('MyEntityService', ['create']);

    TestBed.configureTestingModule({
      declarations: [MyComponentComponent],
      providers: [
        { provide: MyEntityService, useValue: serviceSpy }
      ]
    });

    fixture = TestBed.createComponent(MyComponentComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(MyEntityService) as jasmine.SpyObj<MyEntityService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call service on submit', () => {
    service.create.and.returnValue(of({ id: '1', name: 'Test', createdAt: new Date() }));

    component.form.patchValue({ name: 'Test' });
    component.onSubmit();

    expect(service.create).toHaveBeenCalledWith({ name: 'Test' });
  });
});
```

### Flutter Widget Tests

```dart
void main() {
  group('MyEntityScreen', () {
    testWidgets('displays loading indicator when loading', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          home: BlocProvider(
            create: (_) => MyEntityBloc(MockMyEntityRepository())
              ..add(CreateMyEntity('Test')),
            child: MyEntityView(),
          ),
        ),
      );

      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });
  });
}
```

---

## 🐛 Debugging

### Backend Debugging

**Visual Studio:**
1. Set breakpoint in code
2. Press F5 to start debugging
3. Use Watch window to inspect variables

**VS Code:**
1. Set breakpoint in code
2. Press F5
3. Select ".NET Core Launch (web)"

### Frontend Debugging

**Chrome DevTools:**
1. Open Chrome DevTools (F12)
2. Go to Sources tab
3. Find TypeScript file and set breakpoint

**VS Code:**
1. Install "Debugger for Chrome" extension
2. Set breakpoint in TypeScript file
3. Press F5 and select "Chrome"

### Mobile Debugging

**Android:**
```bash
flutter run --debug
flutter logs
```

**iOS:**
```bash
flutter run --debug
# Xcode: Debug > Attach to Process
```

---

## 📦 Package Management

### Backend

```bash
# Add package
dotnet add package PackageName

# Update package
dotnet add package PackageName --version 1.2.3

# Remove package
dotnet remove package PackageName
```

### Frontend

```bash
# Add package
npm install package-name

# Add dev dependency
npm install -D package-name

# Update packages
npm update
```

### Mobile

```bash
# Add package
flutter pub add package_name

# Remove package
flutter pub remove package_name

# Update packages
flutter pub upgrade
```

---

## 🔒 Environment Variables

### Backend (.env file not recommended, use User Secrets)

```bash
dotnet user-secrets init -p src/DeviceOwnership.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;..." -p src/DeviceOwnership.API
```

### Frontend (environment.ts)

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001',
  oauthClientId: 'web_app',
  oauthRedirectUri: 'http://localhost:4200/callback'
};
```

### Mobile (.env with flutter_dotenv)

```
API_URL=https://localhost:5001
OAUTH_CLIENT_ID=mobile_app
```

---

## 🚀 Performance Optimization

### Backend

- Use `AsNoTracking()` for read-only queries
- Implement caching with Redis
- Use pagination for large datasets
- Enable response compression
- Use async/await properly

### Frontend

- Use lazy loading for routes
- Implement virtual scrolling for large lists
- Use OnPush change detection strategy
- Optimize bundle size with tree shaking
- Use CDN for static assets

### Mobile

- Use `const` constructors
- Implement pagination for lists
- Optimize images
- Use cached network images
- Profile with Flutter DevTools

---

## 📖 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Angular Documentation](https://angular.io/docs)
- [Flutter Documentation](https://flutter.dev/docs)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [OpenIddict Documentation](https://documentation.openiddict.com/)

---

Happy coding! 🎉
