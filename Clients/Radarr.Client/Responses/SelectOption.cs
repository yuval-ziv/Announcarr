﻿namespace Announcarr.Clients.Radarr.Responses;

public class SelectOption
{
    public int Value { get; set; }
    public string? Name { get; set; }
    public int Order { get; set; }
    public string? Hint { get; set; }
    public bool DividerAfter { get; set; }
}