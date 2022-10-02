// -----------------------------------------------------------------------
// <copyright file="PrimitiveSynchronizerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using AdminToys;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Mistaken.Toy.API.Components.Synchronizers
{
    internal class PrimitiveSynchronizerScript : SynchronizerScript
    {
        internal MeshRenderer MeshRenderer { get; set; } = null;

        internal bool SyncColor => !(this.MeshRenderer is null);

        internal void ShowFor(Player player)
        {
            if (this.visibleFor.Contains(player))
                return;

            if (Server.SendSpawnMessage is null)
                throw new NullReferenceException($"{nameof(Server.SendSpawnMessage)} was null");

            if (player.Connection is null)
                throw new NullReferenceException($"{nameof(player.Connection)} was null");

            Server.SendSpawnMessage.Invoke(null, new object[] { this.Toy.netIdentity, player.Connection });

            this.visibleFor.Add(player);

            this.ResetState(this.GetPlayerState(player));
        }

        internal void HideFor(Player player)
        {
            if (!this.visibleFor.Contains(player))
                return;

            if (player.Connection is null)
                throw new NullReferenceException($"{nameof(player.Connection)} was null");

            player.Connection.Send(new ObjectDestroyMessage { netId = this.Toy.netId });

            this.visibleFor.Remove(player);
        }

        protected override State CurrentState => this.currentPrimitiveState;

        protected override bool ShouldUpdate()
        {
            if (!base.ShouldUpdate() && this.MeshRenderer?.material.color == this.currentPrimitiveState.Color)
                return false;

            this.currentPrimitiveState.Color = this.MeshRenderer?.material.color ?? Color.magenta;
            return true;
        }

        protected override ulong GetStateFlags(State playerState)
        {
            var tor = base.GetStateFlags(playerState);

            if (!(playerState is PrimitiveState state))
                throw new ArgumentException($"Supplied {nameof(playerState)} was not {nameof(PrimitiveState)}, it was {playerState?.GetType().FullName ?? "NULL"}", nameof(playerState));

            if (this.SyncColor && this.currentPrimitiveState.Color != state.Color) tor += 32;
            return tor;
        }

        protected override bool ShouldUpdateFor(Player player) => this.visibleFor.Contains(player);

        protected override Action<NetworkWriter> CustomSyncVarGenerator(ulong flags, Action<NetworkWriter> callBackAction = null)
        {
            return base.CustomSyncVarGenerator(flags, targetWriter =>
            {
                targetWriter.WriteUInt64(flags & 32UL); // color (32) | flags & (~31UL)
                if ((flags & 32) != 0) targetWriter.WriteColor(this.currentPrimitiveState.Color);
                callBackAction?.Invoke(targetWriter);
            });
        }

        protected override void ResetState(State state)
        {
            base.ResetState(state);

            if (!(state is PrimitiveState primitiveState))
                throw new ArgumentException($"Expected {nameof(PrimitiveState)} but got {state.GetType().Name}", nameof(state));

            if (!(this.Toy is PrimitiveObjectToy primitiveObjectToy))
                throw new ArgumentException($"Expected {nameof(PrimitiveObjectToy)} but got {this.Toy.GetType().Name}", nameof(this.Toy));

            primitiveState.Color = primitiveObjectToy.NetworkMaterialColor;
        }

        protected class PrimitiveState : State
        {
            public Color Color { get; set; }

            public override bool Equals(State other)
                =>
                    base.Equals(other) &&
                    other is PrimitiveState primitive &&
                    this.Color == primitive.Color;

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = base.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Color.GetHashCode();
                    return hashCode;
                }
            }

            public override void CopyValues(State other)
            {
                base.CopyValues(other);

                if (!(other is PrimitiveState primitive))
                    throw new ArgumentException("Expected argument of type " + nameof(PrimitiveState), nameof(other));

                primitive.Color = this.Color;
            }
        }

        private readonly PrimitiveState currentPrimitiveState = new PrimitiveState();
        private readonly HashSet<Player> visibleFor = new HashSet<Player>();
    }
}
