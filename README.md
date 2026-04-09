# Product CRUD API — ASP.NET Core Web API

A secure and scalable REST API built with ASP.NET Core 8, featuring JWT Authentication, Role-Based Authorization, and a full Product CRUD module.


## Project Structure

```
ProductAPI/
├── Controllers/
│   ├── AuthController.cs       # Register and Login endpoints
│   └── ProductsController.cs   # Product CRUD endpoints
├── Data/
│   └── AppDbContext.cs         # EF Core database context
├── DTOs/
│   └── Dtos.cs                 # Request/Response data models
├── Middleware/
│   └── GlobalExceptionMiddleware.cs   # Handles unhandled exceptions
├── Models/
│   ├── Product.cs              # Product database model
│   └── User.cs                 # User database model
├── Repository/
│   └── ProductRepository.cs    # Database operations
├── Services/
│   ├── AuthService.cs          # Registration, login, JWT generation
│   └── ProductService.cs       # Business logic for products
├── appsettings.json            # App configuration (JWT secret etc.)
└── Program.cs                  # App startup and configuration
```

---

## API Endpoints

## Auth Endpoints 

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new User |
| POST | `/api/auth/register-admin` | Register a new Admin |
| POST | `/api/auth/login` | Login and get JWT token |

### Product Endpoints (Token required)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/products` | User / Admin | Get all products (paginated) |
| GET | `/api/products/{id}` | User / Admin | Get product by ID |
| POST | `/api/products` | **Admin only** | Create a new product |
| PUT | `/api/products/{id}` | **Admin only** | Update a product |
| DELETE | `/api/products/{id}` | **Admin only** | Delete a product |

#### Query Parameters for GET /api/products:
- `page` — Page number (default: 1)
- `pageSize` — Items per page (default: 10, max: 100)
- `search` — Search by name or description

---

## How to Test with Swagger

1. **Run the app** → Swagger opens automatically in your browser

2. **Register an Admin:**
   - Call `POST /api/auth/register-admin`
   - Body: `{ "username": "admin", "password": "admin123" }`
   - Copy the `token` from the response

3. **Authorize in Swagger:**
   - Click the **Authorize** button (top right)
   - Enter: `Bearer <your-token-here>`
   - Click **Authorize**

4. **Now you can test all endpoints!**

---

## Role-Based Authorization

| Role | Can Do |
|---|---|
| **User** | View all products, view product by ID |
| **Admin** | Everything a User can do + Create, Update, Delete products |

---

## Architecture: Clean Layered Design

```
Request → Controller → Service → Repository → Database
                ↑           ↑            ↑
            Validates    Business      DB Queries
            Input        Logic         (EF Core)
```

- **Controllers** — Handle HTTP requests and responses
- **Services** — Contain business logic
- **Repositories** — Handle all database operations
- **DTOs** — Control what data goes in and out (not the raw DB models)

---

## Switching to a Real Database

Open `Program.cs` and replace:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ProductDB"));
```

With (for SQL Server):

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Then add your connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ProductDB;Trusted_Connection=True;"
}
```

And run migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Sample Request/Response

### Register
**POST** `/api/auth/register`
```json
{
  "username": "sona",
  "password": "secret123"
}
```
**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "sona",
  "role": "User"
}
```

### Create Product (Admin only)
**POST** `/api/products`
```json
{
  "name": "Monitor",
  "description": " Display 32 inch",
  "price": 25000
}
```

### Get All Products (with pagination)
**GET** `/api/products?page=1&pageSize=5&search=lap`
```json
{
  "data": [ { "id": 1, "name": "Laptop", ... } ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 5,
  "totalPages": 1
}
```
