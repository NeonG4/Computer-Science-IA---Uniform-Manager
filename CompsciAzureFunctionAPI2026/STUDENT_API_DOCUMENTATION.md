# Student Management API Documentation

## Overview
The Student Management API provides role-based access control for managing students in the uniform management system.

### Permission Levels:
- **??? Viewer (Level 2)**: Can only view student data
- **??? User (Level 1)**: Can only view student data (same as Viewer)
- **?? Administrator (Level 0)**: Full CRUD access to student records

## Base URL
```
http://localhost:7071/api
```

## Endpoints

### 1. Get All Students
**GET** `/GetStudents?userId={userId}&grade={grade}`

Retrieves all students in the system. Optionally filter by grade.

**Permissions**: All authenticated users (Viewer, User, Admin)

**Query Parameters**:
- `userId` (required): The ID of the requesting user
- `grade` (optional): Filter students by grade (1-12)

**Response**:
```json
{
  "success": true,
  "message": "Students retrieved successfully.",
  "students": [
    {
      "studentId": 1,
      "studentIdentifier": "S2024001",
      "firstName": "John",
      "lastName": "Smith",
      "fullName": "John Smith",
      "grade": 9,
      "createdDate": "2026-01-17T10:00:00Z",
      "lastModified": "2026-01-17T12:30:00Z"
    }
  ],
  "totalCount": 1
}
```

**Examples**:
```bash
# Get all students
curl -X GET "http://localhost:7071/api/GetStudents?userId=3"

# Get all 9th graders
curl -X GET "http://localhost:7071/api/GetStudents?userId=3&grade=9"
```

---

### 2. Get Single Student
**GET** `/students/{studentIdentifier}?userId={userId}`

Retrieves a specific student by their identifier.

**Permissions**: All authenticated users

**Path Parameters**:
- `studentIdentifier`: The unique identifier of the student (e.g., "S2024001")

**Query Parameters**:
- `userId` (required): The ID of the requesting user

**Response**:
```json
{
  "success": true,
  "message": "Student found.",
  "student": {
    "studentId": 1,
    "studentIdentifier": "S2024001",
    "firstName": "John",
    "lastName": "Smith",
    "fullName": "John Smith",
    "grade": 9,
    "createdDate": "2026-01-17T10:00:00Z",
    "lastModified": null
  }
}
```

**Example**:
```bash
curl -X GET "http://localhost:7071/api/students/S2024001?userId=3"
```

---

### 3. Create Student
**POST** `/CreateStudent`

Creates a new student in the system.

**Permissions**: ?? **Administrator only**

**Request Body**:
```json
{
  "studentIdentifier": "S2024011",
  "firstName": "Jennifer",
  "lastName": "Lopez",
  "grade": 10,
  "requestingUserId": 1
}
```

**Field Validation**:
- `studentIdentifier`: Required, must be unique
- `firstName`: Required
- `lastName`: Required
- `grade`: Required, must be between 1 and 12
- `requestingUserId`: Required

**Response**:
```json
{
  "success": true,
  "message": "Student created successfully.",
  "student": {
    "studentId": 11,
    "studentIdentifier": "S2024011",
    "firstName": "Jennifer",
    "lastName": "Lopez",
    "fullName": "Jennifer Lopez",
    "grade": 10
  }
}
```

**Error Responses**:
- `403 Forbidden`: User is not an administrator
- `409 Conflict`: Student with this identifier already exists
- `400 Bad Request`: Invalid grade (must be 1-12) or missing required fields

**Example**:
```bash
curl -X POST http://localhost:7071/api/CreateStudent \
  -H "Content-Type: application/json" \
  -d '{
    "studentIdentifier": "S2024011",
    "firstName": "Jennifer",
    "lastName": "Lopez",
    "grade": 10,
    "requestingUserId": 1
  }'
```

---

### 4. Update Student Details
**PUT** `/UpdateStudent`

Updates student information.

**Permissions**: ?? **Administrator only**

**Request Body**:
```json
{
  "studentIdentifier": "S2024011",
  "firstName": "Jenny",
  "lastName": "Lopez-Smith",
  "grade": 11,
  "requestingUserId": 1
}
```

**Notes**:
- All fields except `studentIdentifier` and `requestingUserId` are optional
- You can update any combination of firstName, lastName, and grade
- Grade must be between 1 and 12 if provided

**Response**:
```json
{
  "success": true,
  "message": "Student updated successfully."
}
```

**Examples**:
```bash
# Update only the grade
curl -X PUT http://localhost:7071/api/UpdateStudent \
  -H "Content-Type: application/json" \
  -d '{
    "studentIdentifier": "S2024011",
    "grade": 11,
    "requestingUserId": 1
  }'

# Update first and last name
curl -X PUT http://localhost:7071/api/UpdateStudent \
  -H "Content-Type: application/json" \
  -d '{
    "studentIdentifier": "S2024011",
    "firstName": "Jenny",
    "lastName": "Lopez-Smith",
    "requestingUserId": 1
  }'
```

---

### 5. Delete Student
**DELETE** `/students/{studentIdentifier}?userId={userId}`

Deletes a student from the system.

**Permissions**: ?? **Administrator only**

**Path Parameters**:
- `studentIdentifier`: The unique identifier of the student

**Query Parameters**:
- `userId` (required): The ID of the requesting user

**Response**:
```json
{
  "success": true,
  "message": "Student deleted successfully."
}
```

**Example**:
```bash
curl -X DELETE "http://localhost:7071/api/students/S2024011?userId=1"
```

---

## Permission Matrix

| Operation | Viewer (2) | User (1) | Admin (0) |
|-----------|------------|----------|-----------|
| View all students | ? | ? | ? |
| View single student | ? | ? | ? |
| Filter by grade | ? | ? | ? |
| Create student | ? | ? | ? |
| Update student | ? | ? | ? |
| Delete student | ? | ? | ? |

---

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Invalid request. Student identifier, first name, and last name are required."
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "You do not have permission to view students."
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "Only administrators can create students."
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Student not found."
}
```

### 409 Conflict
```json
{
  "success": false,
  "message": "A student with this identifier already exists."
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred while retrieving students."
}
```

---

## Usage Examples

### Example 1: Admin Creates Students
```bash
# Create multiple students
curl -X POST http://localhost:7071/api/CreateStudent \
  -H "Content-Type: application/json" \
  -d '{
    "studentIdentifier": "S2024011",
    "firstName": "Alex",
    "lastName": "Johnson",
    "grade": 9,
    "requestingUserId": 1
  }'

curl -X POST http://localhost:7071/api/CreateStudent \
  -H "Content-Type: application/json" \
  -d '{
    "studentIdentifier": "S2024012",
    "firstName": "Maria",
    "lastName": "Garcia",
    "grade": 10,
    "requestingUserId": 1
  }'
```

### Example 2: User Views Students
```bash
# View all students
curl -X GET "http://localhost:7071/api/GetStudents?userId=5"

# View specific student
curl -X GET "http://localhost:7071/api/students/S2024011?userId=5"
```

### Example 3: Admin Updates Student
```bash
# Student promoted to next grade
curl -X PUT http://localhost:7071/api/UpdateStudent \
  -H "Content-Type: application/json" \
  -d '{
    "studentIdentifier": "S2024011",
    "grade": 10,
    "requestingUserId": 1
  }'
```

### Example 4: Get Students by Grade
```bash
# Get all 9th graders
curl -X GET "http://localhost:7071/api/GetStudents?userId=3&grade=9"

# Get all 12th graders
curl -X GET "http://localhost:7071/api/GetStudents?userId=3&grade=12"
```

---

## Database Schema

### Students Table
```sql
CREATE TABLE [dbo].[Students]
(
    [StudentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [StudentIdentifier] NVARCHAR(50) NOT NULL UNIQUE,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Grade] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [LastModified] DATETIME NULL,
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_Students_ModifiedBy] 
        FOREIGN KEY ([ModifiedBy]) REFERENCES [UserInfo]([UserId]),
    CONSTRAINT [CK_Students_Grade] 
        CHECK ([Grade] >= 1 AND [Grade] <= 12)
)
```

**Indexes**:
- Primary Key on `StudentId`
- Unique constraint on `StudentIdentifier`
- Non-clustered index on `Grade` (for filtering)
- Non-clustered index on `LastName` (for sorting)
- Composite non-clustered index on `FirstName, LastName` (for full name searches)

---

## Integration with Uniform Management

The Student Management API is designed to work seamlessly with the Uniform Management API:

### Linking Students to Uniforms
```bash
# 1. Get a student
curl -X GET "http://localhost:7071/api/students/S2024001?userId=5"

# 2. Check out a uniform to that student
curl -X POST http://localhost:7071/api/uniforms/checkout \
  -H "Content-Type: application/json" \
  -d '{
    "uniformIdentifier": "MC-001",
    "studentId": "S2024001",
    "checkOut": true,
    "requestingUserId": 5
  }'
```

### Reporting by Grade
```bash
# Get all 9th graders
curl -X GET "http://localhost:7071/api/GetStudents?userId=3&grade=9"

# Then check which uniforms are assigned to each student using the uniform API
```

---

## Testing Workflow

### 1. Setup (Admin)
```bash
# Create test students
POST /CreateStudent { "studentIdentifier": "S2024100", ... }
POST /CreateStudent { "studentIdentifier": "S2024101", ... }
```

### 2. View (Anyone)
```bash
# List all students
GET /GetStudents?userId=10

# View specific student
GET /students/S2024100?userId=10
```

### 3. Update (Admin)
```bash
# Promote student
PUT /UpdateStudent { "studentIdentifier": "S2024100", "grade": 10, ... }
```

### 4. Delete (Admin)
```bash
# Remove graduated student
DELETE /students/S2024100?userId=1
```

---

## Best Practices

1. **Use consistent student identifiers**: Follow a pattern like "S" + year + number (e.g., "S2024001")
2. **Grade validation**: Always ensure grade is between 1-12
3. **Bulk operations**: For importing many students, consider creating a batch endpoint
4. **Audit trail**: The `ModifiedBy` field tracks who made changes
5. **Case sensitivity**: Student names are case-sensitive in the database
6. **Filtering**: Use the grade filter to organize students by class

---

## Security Considerations

1. **Always include `requestingUserId`** in all requests
2. **Only admins can modify student data** - ensures data integrity
3. **All users can view students** - necessary for checking out uniforms
4. **Database tracks who modified what** - full audit trail
5. **All operations are logged** for security monitoring

---

## Next Steps

1. Run the database migration: `Migration_CreateStudents.sql`
2. Start the Azure Function
3. Test endpoints using the examples above
4. Integrate with your WinForms application
5. Link students to uniforms using the Uniform Management API

**Happy coding! ??**
