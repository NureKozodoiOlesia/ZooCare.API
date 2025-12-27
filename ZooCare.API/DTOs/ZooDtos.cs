namespace ZooCare.API.DTOs
{
    // Auth DTOs
    public record RegisterDto(string Email, string Password, string FirstName, string LastName);
    public record LoginDto(string Email, string Password);
    public record AuthResultDto(string Token, string RefreshToken, int UserId, string Email, string FirstName, string LastName, List<string> Roles);
    public record RefreshTokenDto(string RefreshToken);
    public record AssignRoleDto(int UserId, string RoleName);

    // User DTOs
    public record UserDto(int Id, string UserName, string Email, string FirstName, string LastName, List<string> Roles);
    public record CreateUserDto(string UserName, string Email, string Password, string FirstName, string LastName);
    public record UpdateUserDto(string? FirstName, string? LastName, string? Email);

    // Enclosure DTOs
    public record EnclosureDto(int Id, string Name, string Type, string Location, int AnimalCount);
    public record CreateEnclosureDto(string Name, string Type, string Location);
    public record UpdateEnclosureDto(string? Name, string? Type, string? Location);

    // Animal DTOs
    public record AnimalDto(int Id, int EnclosureId, string EnclosureName, string Name, string Species, int Age);
    public record CreateAnimalDto(int EnclosureId, string Name, string Species, int Age);
    public record UpdateAnimalDto(int? EnclosureId, string? Name, string? Species, int? Age);

    // Task DTOs
    public record TaskDto(int Id, int EnclosureId, string EnclosureName, int AssignedUserId, string AssignedUserName, string Title, string Description, string Status, DateTime DueDate, DateTime? CompletedAt);
    public record CreateTaskDto(int EnclosureId, int AssignedUserId, string Title, string? Description, DateTime DueDate);
    public record UpdateTaskStatusDto(string Status);

    // Alert DTOs
    public record AlertDto(int Id, int EnclosureId, string EnclosureName, string Message, string Severity, bool IsResolved, DateTime CreatedAt);
    public record CreateAlertDto(int EnclosureId, string Message, string Severity);

    // IoT DTOs
    public record SensorDataDto(string SerialNumber, string SensorType, decimal Value);

    // Admin DTOs
    public record RoleDto(int Id, string Name, string NormalizedName);
    public record CreateRoleDto(string Name);
    public record AdminStatsDto(
        int TotalUsers,
        int TotalAnimals,
        int TotalEnclosures,
        int TotalTasks,
        int PendingTasks,
        int CompletedTasks,
        int ActiveAlerts,
        int TotalAlerts,
        int OnlineDevices,
        int TotalDevices
    );
    public record UpdatePasswordDto(string NewPassword);
    public record RemoveRoleDto(int UserId, string RoleName);
}