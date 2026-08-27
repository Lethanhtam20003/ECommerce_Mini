# ECommerce Mini — Project Rules

## 1. Tổng quan Dự án

Đây là hệ thống E-commerce Microservice sử dụng **.NET Aspire** (target `net10.0`) làm Orchestration trung tâm.

- **Orchestration**: `ECommerce_Mini.AppHost` — quản lý vòng đời container và Service Discovery.
- **Shared Defaults**: `ECommerce_Mini.ServiceDefaults` — OpenTelemetry, Health Checks.
- **Microservices**: `AuthService`, `ProductService`, `CartService`, `OrderService`, `NotificationService`, `TrackingService`.
- **API Gateway**: YARP — single entry point, JWT validation, Rate Limiting.
- **Databases**: PostgreSQL (per service), Redis (cache, cart, rate limiting).
- **Messaging**: RabbitMQ + MassTransit (business events), Apache Kafka (event streaming).

---

## 2. Kiến trúc & Cấu trúc Thư mục

### 2.1 Clean Architecture per Service

Mỗi microservice PHẢI tuân thủ cấu trúc thư mục phân lớp:

```
<ServiceName>/
├── Domain/                  # Entities, Value Objects, Domain Events, Interfaces
│   ├── Common/              # Result, Error, base classes dùng chung trong domain
│   ├── Entities/            # Domain Entities
│   └── Extensions/          # DI registration cho domain layer
├── Application/             # Use Cases, DTOs, Validators, Mappings
│   ├── Dtos/                # Request/Response DTOs
│   │   └── Common/          # ApiResponse, ApiError
│   ├── Features/            # Use case handlers (nhóm theo feature)
│   └── Interfaces/          # Application-level interfaces (Repositories, Services)
├── Infrastructure/          # Implementations cụ thể (DB, External APIs, Messaging)
│   ├── Persistence/         # DbContext, Configurations, Migrations, Repositories
│   └── Services/            # External service implementations
├── Controllers/             # API Controllers
├── Program.cs               # Entry point, DI composition root
└── appsettings.json
```

### 2.2 Quy tắc phân lớp

- **Domain** KHÔNG được phụ thuộc vào bất kỳ layer nào khác.
- **Application** chỉ phụ thuộc vào **Domain**.
- **Infrastructure** phụ thuộc vào **Application** và **Domain**.
- **Controllers** chỉ gọi vào **Application** layer (thông qua interfaces/services).

---

## 3. Quy ước Code C#

### 3.1 Naming Conventions

| Loại                    | Quy ước            | Ví dụ                              |
| :---------------------- | :------------------ | :---------------------------------- |
| Class, Record, Enum     | PascalCase          | `User`, `ApiResponse<T>`           |
| Interface               | `I` + PascalCase    | `IUserRepository`                  |
| Method                  | PascalCase          | `GetUserByIdAsync()`               |
| Property                | PascalCase          | `IsSuccess`, `PasswordHash`        |
| Private field           | `_camelCase`        | `_userRepository`                  |
| Parameter, local var    | camelCase           | `userId`, `result`                 |
| Constant                | PascalCase          | `MaxRetryCount`                    |
| Async methods           | Suffix `Async`      | `CreateUserAsync()`               |

### 3.2 Coding Style

- Sử dụng `record` cho DTOs và Value Objects (immutable by default).
- Sử dụng `required` keyword cho các property bắt buộc trong entities.
- Ưu tiên **expression-bodied members** khi logic chỉ 1 dòng.
- Sử dụng **file-scoped namespaces** (`namespace Foo;`) khi file chỉ có 1 namespace.
- KHÔNG dùng `var` khi kiểu dữ liệu không rõ ràng từ context.
- Luôn dùng **nullable reference types** (`T?`) khi giá trị có thể null.

---

## 4. Design Patterns Bắt buộc

### 4.1 Result Pattern

Mọi operation trong Application/Domain layer PHẢI trả về `Result<T>` thay vì throw exception cho business logic errors.

```csharp
// ✅ Đúng
public async Task<Result<UserDto>> GetUserByIdAsync(Guid id) { ... }

// ❌ Sai — không throw exception cho business error
throw new NotFoundException("User not found");
```

### 4.2 Error Handling

- Sử dụng `Error` record với `ErrorType` enum để phân loại lỗi.
- `BaseController.HandleResult<T>()` tự động map `Error` sang HTTP status code tương ứng.
- Chỉ throw exception cho **unexpected errors** (infrastructure failures, bugs).

### 4.3 API Response Format

Mọi API response PHẢI sử dụng `ApiResponse<T>`:

```json
{
  "isSuccess": true,
  "data": { ... },
  "error": null,
  "timestamp": "2026-08-26T12:00:00Z"
}
```

### 4.4 Dependency Injection

- Mỗi layer có extension method `Add<Layer>Services()` trong thư mục `Extensions/`.
- Đăng ký DI trong `Program.cs` theo thứ tự: Domain → Application → Infrastructure.
- Ưu tiên `AddScoped` cho repositories và services.

---

## 5. Database & Entity Framework Core

- Sử dụng **Aspire component** `AddNpgsqlDbContext<TContext>("connectionName")` để đăng ký DbContext.
- Mỗi service có **database riêng** (Database-per-Service pattern).
- Cấu hình entity bằng `IEntityTypeConfiguration<T>` trong `Infrastructure/Persistence/Configurations/`.
- Migration được lưu trong `Infrastructure/Persistence/Migrations/`.
- KHÔNG dùng Data Annotations trên entities — chỉ dùng Fluent API.
- Entity Id mặc định dùng `Guid`.

---

## 6. .NET Aspire Conventions

- Resource được khai báo trong `AppHost.cs` (Redis, PostgreSQL, RabbitMQ, Kafka).
- Connection string KHÔNG hardcode — sử dụng `builder.AddParameter()` cho secrets.
- Mỗi service tham chiếu resource qua `.WithReference()`.
- Mọi service PHẢI gọi `builder.AddServiceDefaults()` và `app.MapDefaultEndpoints()`.

---

## 7. API Controllers

- Mọi controller PHẢI kế thừa `BaseController`.
- Route convention: `api/v1/[controller]`.
- Sử dụng `[ApiController]` attribute.
- Controller chỉ chứa logic điều hướng — business logic nằm trong Application layer.
- Sử dụng `HandleResult()` để trả response thống nhất.

---

## 8. Ngôn ngữ & Comment

- Comment trong code bằng **tiếng Việt** để team hiểu rõ ngữ cảnh nghiệp vụ.
- Tên class, method, property, variable bằng **tiếng Anh**.
- Git commit messages bằng **tiếng Việt**.
- Khi tôi hỏi hoặc chat, hãy trả lời bằng **tiếng Việt**.

---

## 9. Security

- Sử dụng **JWT Bearer Token** cho authentication.
- Password PHẢI được hash (BCrypt hoặc Argon2) — KHÔNG BAO GIỜ lưu plaintext.
- Sensitive config (connection strings, secrets) lưu trong `User Secrets` hoặc Aspire Parameters.
- KHÔNG commit secrets vào source control.

---

## 10. Testing (Khi triển khai)

- Unit tests cho Domain và Application layers.
- Integration tests cho Infrastructure layer và API endpoints.
- Test project đặt tên: `<ServiceName>.Tests`.
- Sử dụng xUnit + FluentAssertions + NSubstitute.

---

## 11. Quy tắc Khi Sinh Code

- Khi tạo service mới, LUÔN tạo đầy đủ cấu trúc Clean Architecture (Domain, Application, Infrastructure, Controllers).
- Khi thêm entity mới, LUÔN tạo kèm `IEntityTypeConfiguration<T>`.
- Khi thêm feature mới, LUÔN đăng ký DI trong extension method tương ứng.
- Khi sửa code, KHÔNG xóa comment hoặc docstring hiện có trừ khi được yêu cầu.
- Ưu tiên tạo migration script khi thay đổi schema database.

