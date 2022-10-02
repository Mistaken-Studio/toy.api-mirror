// -----------------------------------------------------------------------
// <copyright file="GlobalSynchronizerControllerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Mistaken.API;

namespace Mistaken.Toy.API.Components.Controllers
{
    internal class GlobalSynchronizerControllerScript : SynchronizerControllerScript
    {
        public override IEnumerable<Player> GetSubscribers()
            => RealPlayers.List;
    }
}