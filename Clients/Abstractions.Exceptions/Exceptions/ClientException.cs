﻿namespace Announcarr.Clients.Abstractions.Exceptions;

public class ClientException : Exception
{
    public ClientException()
    {
    }

    public ClientException(string? message) : base(message)
    {
    }

    public ClientException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}