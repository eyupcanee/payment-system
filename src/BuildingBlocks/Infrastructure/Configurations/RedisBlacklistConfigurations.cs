﻿namespace Infrastructure.Configurations;

public class RedisBlacklistConfigurations
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Password { get; set; } = string.Empty;
}