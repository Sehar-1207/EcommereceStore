public record AuthenticatedUserDto(
    string FullName,
    string Email,
    string Token // The JWT
);
