﻿using System;
using System.Collections.Generic;

namespace NeoServer.Data.Entities;

public class WorldEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    
    public Continent Continent { get; set; }
    
    public PvpType PvpType { get; set; }
    
    public Mode Mode { get; set; }
    
    public bool RequiresPremium { get; set; }
    
    public bool TransferEnabled { get; set; }
    
    public bool AntiCheatEnabled { get; set; }
    public int MaxCapacity { get; set; }
    public DateTime CreatedAt { get; set; }
    
    
    public DateTime? DeletedAt { get; set; }
        
    public ICollection<WorldRecordEntity> WorldRecords { get; set; }
}


public enum Continent
{
    Africa,
    Asia,
    Australia,
    Europe,
    NorthAmerica,
    SouthAmerica
}

public enum PvpType
{
    Open,
    Optional,
    HardCore,
    RetroOpen,
    RetroHardCore,
}

public enum Mode
{
    Regular,
    Experimental
}
