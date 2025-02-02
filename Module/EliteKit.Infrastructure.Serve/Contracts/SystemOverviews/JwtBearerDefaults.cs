﻿namespace EliteKit.Infrastructure.Serve.Contracts.SystemOverviews;
public readonly record struct JwtBearerDefaults
{
    public const string AuthenticationScheme = "Bearer";
    public const string FailureMessage = "Identity authentication failed";
}