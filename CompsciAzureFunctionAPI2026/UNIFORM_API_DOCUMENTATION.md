# Uniform Management API Documentation

## Overview
The Uniform Management API provides role-based access control for managing band/marching uniforms. The API enforces three permission levels:

- **??? Viewer (Level 2)**: Can only view uniform data
- **?? User (Level 1)**: Can view, check-out/in uniforms, and update conditions
- **?? Administrator (Level 0)**: Full access to create, update, and delete uniforms

## Base URL
```
http://localhost:7071/api
```

## Endpoints

### 1. Get All Uniforms
**GET** `/GetUniforms?userId={userId}`

Retrieves all uniforms in the system.

**Permissions**: All authenticated users (Viewer, User, Admin)

**Query Parameters**:
- `userId` (required): The ID of the requesting user

**Response**:
```json
{
  "success": true,
  "message": "Uniforms retrieved successfully.",
  "uniforms": [
    {
      "uniformId": 1,
      "uniformIdentifier": "CC-001",
      "uniformType": 0,
      "uniformTypeName": "ConcertCoat",
      "size": 42,
      "isCheckedOut": true,
      "assignedStudentId": "S12345",
      "conditions": [0, 1],
      "conditionNames": ["Stain", "BrokenButton"],
      "isInGoodCondition": false,
      "createdDate": "2026-01-17T10:00:00Z",
      "lastModified": "2026-01-17T12:30:00Z"
    }
  ],
  "totalCount": 1
}
```

---

### 2. Get Single Uniform
**GET** `/uniforms/{uniformIdentifier}?userId={userId}`

Retrieves a specific uniform by its identifier.

**Permissions**: All authenticated users

**Path Parameters**:
- `uniformIdentifier`: The unique identifier of the uniform (e.g., "CC-001")

**Query Parameters**:
- `userId` (required): The ID of the requesting user

**Response**:
```json
{
  "success": true,
  "message": "Uniform found.",
  "uniform": {
    "uniformId": 1,
    "uniformIdentifier": "CC-001",
    "uniformType": 0,
    "uniformTypeName": "ConcertCoat",
    "size": 42,
    "isCheckedOut": false,
    "assignedStudentId": null,
    "conditions": [],
    "conditionNames": [],
    "isInGoodCondition": true,
    "createdDate": "2026-01-17T10:00:00Z",
    "lastModified": null
  }
}
```

---

### 3. Create Uniform
**POST** `/CreateUniform`

Creates a new uniform in the system.

**Permissions**: ?? **Administrator only**

**Request Body**:
```json
{
  "uniformIdentifier": "CC-001",
  "uniformType": 0,
  "size": 42,
  "requestingUserId": 1
}
```

**Response**:
```json
{
  "success": true,
  "message": "Uniform created successfully.",
  "uniform": {
    "uniformId": 1,
    "uniformIdentifier": "CC-001",
    "uniformType": 0,
    "uniformTypeName": "ConcertCoat",
    "size": 42,
    "isCheckedOut": false,
    "isInGoodCondition": true
  }
}
```

**Error Responses**:
- `403 Forbidden`: User is not an administrator
- `409 Conflict`: Uniform with this identifier already exists

---

### 4. Update Uniform Details
**PUT** `/UpdateUniform`

Updates uniform type or size.

**Permissions**: ?? **Administrator only**

**Request Body**:
```json
{
  "uniformIdentifier": "CC-001",
  "uniformType": 1,
  "size": 44,
  "requestingUserId": 1
}
```

**Response**:
```json
{
  "success": true,
  "message": "Uniform updated successfully."
}
```

**Note**: You can update either `uniformType`, `size`, or both.

---

### 5. Delete Uniform
**DELETE** `/uniforms/{uniformIdentifier}?userId={userId}`

Deletes a uniform from the system.

**Permissions**: ?? **Administrator only**

**Path Parameters**:
- `uniformIdentifier`: The unique identifier of the uniform

**Query Parameters**:
- `userId` (required): The ID of the requesting user

**Response**:
```json
{
  "success": true,
  "message": "Uniform deleted successfully."
}
```

---

### 6. Check Out/In Uniform
**POST** `/uniforms/checkout`

Checks out or checks in a uniform to/from a student.

**Permissions**: ?? **User and Administrator**

**Request Body**:
```json
{
  "uniformIdentifier": "CC-001",
  "studentId": "S12345",
  "checkOut": true,
  "requestingUserId": 2
}
```

**Parameters**:
- `checkOut`: `true` to check out, `false` to check in
- `studentId`: Required when checking out, can be null when checking in

**Response**:
```json
{
  "success": true,
  "message": "Uniform checked out successfully."
}
```

**Use Cases**:
```json
// Check OUT uniform to student
{
  "uniformIdentifier": "CC-001",
  "studentId": "S12345",
  "checkOut": true,
  "requestingUserId": 2
}

// Check IN uniform from student
{
  "uniformIdentifier": "CC-001",
  "studentId": null,
  "checkOut": false,
  "requestingUserId": 2
}
```

---

### 7. Update Uniform Conditions
**PUT** `/uniforms/conditions`

Updates the condition status of a uniform.

**Permissions**: ?? **User and Administrator**

**Request Body**:
```json
{
  "uniformIdentifier": "CC-001",
  "conditions": [0, 3],
  "requestingUserId": 2
}
```

**Condition Values**:
- `0`: Stain
- `1`: BrokenButton
- `2`: BrokenZipper
- `3`: Torn
- `4`: Missing
- `5`: Faded

**Response**:
```json
{
  "success": true,
  "message": "Uniform conditions updated successfully."
}
```

**Examples**:
```json
// Mark uniform as having a stain and torn fabric
{
  "uniformIdentifier": "CC-001",
  "conditions": [0, 3],
  "requestingUserId": 2
}

// Mark uniform as in perfect condition
{
  "uniformIdentifier": "CC-001",
  "conditions": [],
  "requestingUserId": 2
}
```

---

## Enumerations

### UniformClothing Types
```
0 = ConcertCoat
1 = DrumMajorCoat
2 = Hat
3 = MarchingCoat
4 = MarchingShorts
5 = MarchingSocks
6 = Pants
```

### Condition Types
```
0 = Stain
1 = BrokenButton
2 = BrokenZipper
3 = Torn
4 = Missing
5 = Faded
```

### Account Levels
```
0 = Administrator (Full access)
1 = User (Can check out/in and update conditions)
2 = Viewer (Read-only access)
```

---

## Permission Matrix

| Operation | Viewer (2) | User (1) | Admin (0) |
|-----------|------------|----------|-----------|
| View uniforms | ? | ? | ? |
| View single uniform | ? | ? | ? |
| Create uniform | ? | ? | ? |
| Update uniform details | ? | ? | ? |
| Delete uniform | ? | ? | ? |
| Check out/in uniform | ? | ? | ? |
| Update conditions | ? | ? | ? |

---

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Invalid request. Uniform identifier is required."
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "You do not have permission to view uniforms."
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "Only administrators can create uniforms."
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Uniform not found."
}
```

### 409 Conflict
```json
{
  "success": false,
  "message": "A uniform with this identifier already exists."
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred while retrieving uniforms."
}
```

---

## Usage Examples

### Example 1: Admin Creates a Uniform
```bash
curl -X POST http://localhost:7071/api/CreateUniform \
  -H "Content-Type: application/json" \
  -d '{
    "uniformIdentifier": "MC-050",
    "uniformType": 3,
    "size": 40,
    "requestingUserId": 1
  }'
```

### Example 2: User Checks Out Uniform
```bash
curl -X POST http://localhost:7071/api/uniforms/checkout \
  -H "Content-Type: application/json" \
  -d '{
    "uniformIdentifier": "MC-050",
    "studentId": "S67890",
    "checkOut": true,
    "requestingUserId": 5
  }'
```

### Example 3: User Reports Damage
```bash
curl -X PUT http://localhost:7071/api/uniforms/conditions \
  -H "Content-Type: application/json" \
  -d '{
    "uniformIdentifier": "MC-050",
    "conditions": [0, 2],
    "requestingUserId": 5
  }'
```

### Example 4: User Checks In Uniform
```bash
curl -X POST http://localhost:7071/api/uniforms/checkout \
  -H "Content-Type: application/json" \
  -d '{
    "uniformIdentifier": "MC-050",
    "studentId": null,
    "checkOut": false,
    "requestingUserId": 5
  }'
```

### Example 5: Admin Updates Uniform Size
```bash
curl -X PUT http://localhost:7071/api/UpdateUniform \
  -H "Content-Type: application/json" \
  -d '{
    "uniformIdentifier": "MC-050",
    "size": 42,
    "requestingUserId": 1
  }'
```

### Example 6: Get All Uniforms
```bash
curl -X GET "http://localhost:7071/api/GetUniforms?userId=3"
```

---

## Database Schema

### Uniforms Table
```sql
CREATE TABLE [dbo].[Uniforms]
(
    [UniformId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UniformIdentifier] NVARCHAR(50) NOT NULL UNIQUE,
    [UniformType] INT NOT NULL,
    [Size] INT NOT NULL,
    [IsCheckedOut] BIT NOT NULL DEFAULT 0,
    [AssignedStudentId] NVARCHAR(50) NULL,
    [Conditions] NVARCHAR(MAX) NULL, -- JSON array
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [LastModified] DATETIME NULL,
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_Uniforms_ModifiedBy] 
        FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId])
)
```

---

## Testing Workflow

### 1. Setup (Admin)
```bash
# Create uniforms
POST /CreateUniform { "uniformIdentifier": "CC-001", "uniformType": 0, "size": 42, "requestingUserId": 1 }
POST /CreateUniform { "uniformIdentifier": "MC-001", "uniformType": 3, "size": 38, "requestingUserId": 1 }
```

### 2. Check Out (User)
```bash
# Check out to student
POST /uniforms/checkout { "uniformIdentifier": "CC-001", "studentId": "S12345", "checkOut": true, "requestingUserId": 5 }
```

### 3. Report Issues (User)
```bash
# Add condition
PUT /uniforms/conditions { "uniformIdentifier": "CC-001", "conditions": [0], "requestingUserId": 5 }
```

### 4. Check In (User)
```bash
# Check in from student
POST /uniforms/checkout { "uniformIdentifier": "CC-001", "checkOut": false, "requestingUserId": 5 }
```

### 5. View (Anyone)
```bash
# List all
GET /GetUniforms?userId=10

# View specific
GET /uniforms/CC-001?userId=10
```

---

## Security Considerations

1. **Always include `requestingUserId`** in all requests
2. **Account levels are enforced at the API level**
3. **Database tracks who modified what** (audit trail)
4. **All operations are logged** for security monitoring
5. **Connection strings should be secured** in production

---

## Next Steps

1. Run the database migration: `Uniforms.sql`
2. Start the Azure Function
3. Test endpoints using the examples above
4. Integrate with your WinForms application

**Happy coding! ??**
