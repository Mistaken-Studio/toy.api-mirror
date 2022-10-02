// -----------------------------------------------------------------------
// <copyright file="SynchronizerControllerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Mistaken.Toy.API.Components.Synchronizers;
using UnityEngine;

namespace Mistaken.Toy.API.Components.Controllers
{
    internal class SynchronizerControllerScript : MonoBehaviour
    {
        public void AddSubscriber(Player player)
        {
            if (this.subscribers.Contains(player))
                return;

            this.subscribers.Add(player);

            foreach (var primitiveSynchronizerScript in this.synchronizerScripts.OfType<PrimitiveSynchronizerScript>())
            {
                try
                {
                    primitiveSynchronizerScript.ShowFor(player);
                    primitiveSynchronizerScript.UpdateSubscriber(player);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }
            }

            foreach (var synchronizerScript in this.synchronizerScripts.Where(x => x is not PrimitiveSynchronizerScript))
            {
                try
                {
                    synchronizerScript.UpdateSubscriber(player);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public void RemoveSubscriber(Player player)
        {
            this.subscribers.Remove(player);

            foreach (var primitiveSynchronizerScript in this.synchronizerScripts.OfType<PrimitiveSynchronizerScript>())
            {
                try
                {
                    primitiveSynchronizerScript.HideFor(player);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }
            }

            foreach (var lightSynchronizerScript in this.synchronizerScripts.OfType<LightSynchronizerScript>())
            {
                try
                {
                    lightSynchronizerScript.DisableFor(player);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public virtual IEnumerable<Player> GetSubscribers()
            => this.subscribers;

        public void SyncFor(Player player)
            => this.synchronizerScripts.ForEach(x => x.UpdateSubscriber(player));

        internal virtual void AddScript(SynchronizerScript script)
        {
            this.synchronizerScripts.Add(script);
            script.Controller = this;
        }

        internal virtual void RemoveScript(SynchronizerScript script)
        {
            this.synchronizerScripts.Remove(script);
        }

        private readonly List<SynchronizerScript> synchronizerScripts = new();

        private readonly HashSet<Player> subscribers = new();
    }
}
