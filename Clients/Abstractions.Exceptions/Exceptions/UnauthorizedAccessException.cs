namespace Announcarr.Clients.Abstractions.Exceptions;

public class UnauthorizedAccessException(string? message) : ClientException(message);