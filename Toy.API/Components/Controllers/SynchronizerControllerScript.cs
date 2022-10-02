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

#pragma warning disable SA1401 // Fields should be private

// ReSharper disable UnusedMember.Local
// ReSharper disable once IdentifierTypo
namespace Mistaken.Toy.API.Components.Controllers
{
    internal class SynchronizerControllerScript : MonoBehaviour
    {
        public void AddSubscriber(Player player)
        {
            if (this.subscribers.Contains(player))
                return;

            this.subscribers.Add(player);

            foreach (var primitiveSynchronizerScript in this.SynchronizerScripts.OfType<PrimitiveSynchronizerScript>())
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

            foreach (var synchronizerScript in this.SynchronizerScripts.Where(x => !(x is PrimitiveSynchronizerScript)))
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

            foreach (var primitiveSynchronizerScript in this.SynchronizerScripts.OfType<PrimitiveSynchronizerScript>())
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

            foreach (var lightSynchronizerScript in this.SynchronizerScripts.OfType<LightSynchronizerScript>())
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

        public bool IsSubscriber(Player player)
            => this.subscribers.Contains(player);

        public virtual IEnumerable<Player> GetSubscribers()
            => this.subscribers;

        public void SyncFor(Player player)
            => this.SynchronizerScripts.ForEach(x => x.UpdateSubscriber(player));

        internal virtual void AddScript(SynchronizerScript script)
        {
            this.SynchronizerScripts.Add(script);
            script.Controller = this;
        }

        internal virtual void RemoveScript(SynchronizerScript script)
        {
            this.SynchronizerScripts.Remove(script);
        }

        protected readonly List<SynchronizerScript> SynchronizerScripts = new List<SynchronizerScript>();

        private readonly HashSet<Player> subscribers = new HashSet<Player>();
    }
}
