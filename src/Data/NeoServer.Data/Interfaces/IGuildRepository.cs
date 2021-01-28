﻿using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces
{
    public interface IGuildRepository : IBaseRepositoryNeo<GuildModel>
    {
        Task<IEnumerable<GuildModel>> GetAll();
    }
}
