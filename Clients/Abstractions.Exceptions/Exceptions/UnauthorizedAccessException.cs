namespace Announcer.Clients.Abstractions.Exceptions;

public class UnauthorizedAccessException(string? message) : ClientException(message);