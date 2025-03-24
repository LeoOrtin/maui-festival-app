﻿namespace FestivalPlannerApp.Models;

public class Token
{
    public string Access_token { get; set; } = string.Empty;
    public string Token_type { get; set; } = string.Empty;
    public int Expires_in { get; set; }
    public string Scope { get; set; } = string.Empty;
    public string Refresh_token { get; set; } = string.Empty;
}
