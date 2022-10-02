// -----------------------------------------------------------------------
// <copyright file="ToyHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using JetBrains.Annotations;
using MEC;
using Mirror;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.Toy.API.Components.Controllers;
using Mistaken.Toy.API.Components.Synchronizers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mistaken.Toy.API
{
    /// <inheritdoc/>
    public class ToyHandler : Module
    {
        #region Public

        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="type">Toy type.</param>
        /// <param name="parent">Toy's parent.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="hasCollision">If toy should have collision.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <param name="meshRenderer">Color source, if defined color will be synced.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Transform parent, Color color, bool hasCollision, bool syncPosition, byte? movementSmoothing, [CanBeNull] MeshRenderer meshRenderer)
        {
            var toy = SpawnBase(PrimitiveBaseObject, parent, movementSmoothing);

            var primitiveObjectToy = InitializePrimitive(toy, type, color, parent.transform.localScale.x > 0 ? hasCollision : null, syncPosition, meshRenderer);

            FinishSpawningToy(toy);

            return primitiveObjectToy;
        }

        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="type">Toy type.</param>
        /// <param name="position">Toy's position.</param>
        /// <param name="rotation">Toy's rotation.</param>
        /// <param name="scale">Toy's scale.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <param name="meshRenderer">Color source, if defined color will be synced.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool syncPosition, byte? movementSmoothing, [CanBeNull] MeshRenderer meshRenderer)
        {
            var toy = SpawnBase(PrimitiveBaseObject, position, rotation, scale, movementSmoothing);
            var primitiveObjectToy = InitializePrimitive(toy, type, color, null, syncPosition, meshRenderer);

            FinishSpawningToy(toy);

            return primitiveObjectToy;
        }

        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="original">GameObject to clone.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <param name="syncColor">if true color will be synced.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(GameObject original, bool syncPosition, byte? movementSmoothing, bool syncColor)
        {
            var toy = SpawnBase(PrimitiveBaseObject, original.transform, movementSmoothing);

            var primitiveObjectToy = InitializePrimitive(toy, original, syncPosition, syncColor);

            FinishSpawningToy(toy);

            return primitiveObjectToy;
        }

        /// <summary>
        /// Spawns light source admin toy.
        /// </summary>
        /// <param name="parent">Toy's parent.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="intensity">Toy's light intensity.</param>
        /// <param name="range">Toy's light range.</param>
        /// <param name="shadows">Should toy's light cause shadows.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Transform parent, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseLight, parent, movementSmoothing);

            var lightSourceToy = InitializeLightSource(toy, color, intensity, range, shadows, syncPosition);

            FinishSpawningToy(toy);

            return lightSourceToy;
        }

        /// <summary>
        /// Spawns light source admin toy.
        /// </summary>
        /// <param name="position">Toy's position.</param>
        /// <param name="rotation">Toy's rotation.</param>
        /// <param name="scale">Toy's scale.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="intensity">Toy's light intensity.</param>
        /// <param name="range">Toy's light range.</param>
        /// <param name="shadows">Should toy's light cause shadows.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseLight, position, rotation, scale, movementSmoothing);

            var lightSourceToy = InitializeLightSource(toy, color, intensity, range, shadows, syncPosition);

            FinishSpawningToy(toy);

            return lightSourceToy;
        }

        /// <summary>
        /// Spawns light source admin toy.
        /// </summary>
        /// <param name="original">Light to mimic.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Light original, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseLight, original.transform, movementSmoothing);

            var lightSourceToy = InitializeLightSource(toy, original, syncPosition);

            FinishSpawningToy(toy);

            return lightSourceToy;
        }

        /// <summary>
        /// Gets primitive type based on meshFilter mesh name.
        /// </summary>
        /// <param name="filter">MeshFilter.</param>
        /// <returns>PrimitiveType.</returns>
        /// <exception cref="ArgumentException">Thrown when mesh name is not recognized.</exception>
        public static PrimitiveType GetPrimitiveType(MeshFilter filter)
        {
            return filter.mesh.name switch
            {
                "Plane Instance" => PrimitiveType.Plane,
                "Cylinder Instance" => PrimitiveType.Cylinder,
                "Cube Instance" => PrimitiveType.Cube,
                "Capsule Instance" => PrimitiveType.Capsule,
                "Quad Instance" => PrimitiveType.Quad,
                "Sphere Instance" => PrimitiveType.Sphere,
                _ => throw new ArgumentException("Unexpected mesh name " + filter.mesh.name, nameof(filter))
            };
        }

        /// <summary>
        /// Spawns Shooting Target.
        /// </summary>
        /// <param name="type">ShootingTargetType.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        /// <returns>Spawned ShootingTarget.</returns>
        public static ShootingTarget SpawnShootingTarget(ShootingTargetType type, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            AdminToyBase prefab;
            switch (type)
            {
                case ShootingTargetType.Binary:
                    prefab = ShootingTargetObjectBinary;
                    break;
                case ShootingTargetType.Sport:
                    prefab = ShootingTargetObjectSport;
                    break;
                case ShootingTargetType.ClassD:
                    prefab = ShootingTargetObjectDBoy;
                    break;
                case ShootingTargetType.Unknown:
                default:
                    return null;
            }

            var toy = Object.Instantiate(prefab);
            toy.transform.position = position;
            toy.transform.rotation = rotation;
            toy.transform.localScale = scale;
            toy.NetworkScale = toy.transform.lossyScale;
            NetworkServer.Spawn(toy.gameObject);

            toy.UpdatePositionServer();

            return toy.GetComponent<ShootingTarget>();
        }
        #endregion

        /// <inheritdoc cref="Module"/>
        public ToyHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        /// <inheritdoc/>
        public override string Name => nameof(ToyHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.PostRoundCleanup;
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += this.Player_ChangingSpectatedPlayer;
            Exiled.Events.Handlers.Scp079.ChangingCamera += this.Scp079_ChangingCamera;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.PostRoundCleanup;
            Exiled.Events.Handlers.Player.Verified -= Player_Verified;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= this.Player_ChangingSpectatedPlayer;
            Exiled.Events.Handlers.Scp079.ChangingCamera -= this.Scp079_ChangingCamera;
        }

        [UsedImplicitly]
        internal static readonly HashSet<AdminToyBase> ManagedToys = new();

        private static readonly Dictionary<Room, SynchronizerControllerScript> Controllers = new();

        private static readonly Dictionary<Player, Room> LastRooms = new();

        private static GlobalSynchronizerControllerScript globalController;

        private static LightSourceToy primitiveBaseLight;
        private static PrimitiveObjectToy primitiveBaseObject;

        private static ShootingTarget shootingTargetObjectBinary;
        private static ShootingTarget shootingTargetObjectSport;
        private static ShootingTarget shootingTargetObjectDboy;

        private static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if (primitiveBaseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
                            primitiveBaseObject = component;
                    }
                }

                return primitiveBaseObject;
            }
        }

        private static LightSourceToy PrimitiveBaseLight
        {
            get
            {
                if (primitiveBaseLight == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<LightSourceToy>(out var component))
                            primitiveBaseLight = component;
                    }
                }

                return primitiveBaseLight;
            }
        }

        private static ShootingTarget ShootingTargetObjectBinary
        {
            get
            {
                if (shootingTargetObjectBinary == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<ShootingTarget>(out var component) && component.name == "binaryTargetPrefab")
                            shootingTargetObjectBinary = component;
                    }
                }

                return shootingTargetObjectBinary;
            }
        }

        private static ShootingTarget ShootingTargetObjectSport
        {
            get
            {
                if (shootingTargetObjectSport == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<ShootingTarget>(out var component) && component.name == "sportTargetPrefab")
                            shootingTargetObjectSport = component;
                    }
                }

                return shootingTargetObjectSport;
            }
        }

        private static ShootingTarget ShootingTargetObjectDBoy
        {
            get
            {
                if (shootingTargetObjectDboy == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<ShootingTarget>(out var component) && component.name == "dboyTargetPrefab")
                            shootingTargetObjectDboy = component;
                    }
                }

                return shootingTargetObjectDboy;
            }
        }

        private static AdminToyBase SpawnBase(AdminToyBase prefab, Transform parent, byte? movementSmoothing = null)
        {
            var toy = Object.Instantiate(prefab, parent);

            if (movementSmoothing is not null)
                toy.MovementSmoothing = (byte)movementSmoothing;
            toy.transform.localPosition = Vector3.zero;
            toy.transform.localRotation = Quaternion.identity;
            toy.transform.localScale = Vector3.one;
            toy.NetworkScale = toy.transform.localScale;

            ManagedToys.Add(toy);

            return toy;
        }

        private static AdminToyBase SpawnBase(AdminToyBase prefab, Vector3 position, Quaternion rotation, Vector3 scale, byte? movementSmoothing = null)
        {
            var toy = Object.Instantiate(prefab);

            if (movementSmoothing is not null)
                toy.MovementSmoothing = (byte)movementSmoothing;
            toy.transform.position = position;
            toy.transform.rotation = rotation;
            toy.transform.localScale = scale;
            toy.NetworkScale = toy.transform.localScale;

            ManagedToys.Add(toy);

            return toy;
        }

        private static void FinishSpawningToy(AdminToyBase toy)
        {
            toy.UpdatePositionServer();

            var script = toy.GetComponent<SynchronizerScript>();
            if (toy is PrimitiveObjectToy)
            {
                if (script.Controller is not GlobalSynchronizerControllerScript)
                {
                    toy.netIdentity.visible = Visibility.ForceHidden;
                }
            }

            NetworkServer.Spawn(toy.gameObject);
        }

        private static LightSourceToy InitializeLightSource(AdminToyBase toy, Color color, float intensity, float range, bool shadows, bool syncPosition)
        {
            var lightSourceToy = toy.GetComponent<LightSourceToy>();
            lightSourceToy._light.color = color;
            lightSourceToy._light.intensity = intensity;
            lightSourceToy._light.range = range;
            lightSourceToy._light.shadows = shadows ? LightShadows.Soft : LightShadows.None;

            lightSourceToy.NetworkLightColor = color;
            lightSourceToy.NetworkLightIntensity = intensity;
            lightSourceToy.NetworkLightRange = range;
            lightSourceToy.NetworkLightShadows = shadows;

            var syncScript = lightSourceToy.gameObject.AddComponent<LightSynchronizerScript>();
            syncScript.Toy = lightSourceToy;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;
            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            return lightSourceToy;
        }

        private static LightSourceToy InitializeLightSource(AdminToyBase toy, Light original, bool syncPosition)
        {
            var lightSourceToy = toy.GetComponent<LightSourceToy>();

            lightSourceToy.NetworkLightColor = original.color;
            lightSourceToy.NetworkLightIntensity = original.intensity;
            lightSourceToy.NetworkLightRange = original.range;
            lightSourceToy.NetworkLightShadows = original.shadows != LightShadows.None;

            var syncScript = lightSourceToy.gameObject.AddComponent<LightSynchronizerScript>();
            syncScript.Toy = lightSourceToy;

            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            syncScript.ClonedLight = original;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;

            return lightSourceToy;
        }

        private static PrimitiveObjectToy InitializePrimitive(AdminToyBase toy, PrimitiveType type, Color color, bool? hasCollision, bool syncPosition, [CanBeNull] MeshRenderer syncColor)
        {
            var primitiveObjectToy = toy.GetComponent<PrimitiveObjectToy>();
            primitiveObjectToy.NetworkPrimitiveType = type;
            primitiveObjectToy.NetworkMaterialColor = color;

            if (hasCollision.HasValue)
            {
                if (!hasCollision.Value && toy.transform.localScale.x > 0 && (type == PrimitiveType.Plane || type == PrimitiveType.Quad))
                {
                    // Exiled.API.Features.Log.Info("Rotated 180° X to compensate for negative scale");
                    toy.transform.eulerAngles += Vector3.right * 180;
                }

                toy.NetworkScale = new Vector3(
                    Math.Abs(toy.transform.lossyScale.x),
                    Math.Abs(toy.transform.lossyScale.y),
                    Math.Abs(toy.transform.lossyScale.z));
                toy.NetworkScale *= hasCollision.Value ? 1 : -1;

                toy.transform.localScale = new Vector3(
                    Math.Abs(toy.transform.localScale.x),
                    Math.Abs(toy.transform.localScale.y),
                    Math.Abs(toy.transform.localScale.z));
                toy.transform.localScale *= hasCollision.Value ? 1 : -1;
            }

            var syncScript = primitiveObjectToy.gameObject.AddComponent<PrimitiveSynchronizerScript>();
            syncScript.Toy = primitiveObjectToy;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;
            syncScript.MeshRenderer = syncColor;

            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            return primitiveObjectToy;
        }

        private static PrimitiveObjectToy InitializePrimitive(AdminToyBase toy, GameObject original, bool syncPosition, bool syncColor)
        {
            var primitiveObjectToy = toy.GetComponent<PrimitiveObjectToy>();

            if (!original.TryGetComponent(out MeshRenderer renderer))
                throw new ArgumentException($"Can not convert to primitive toy object without {nameof(MeshRenderer)}", nameof(original));

            if (!original.TryGetComponent(out MeshFilter filter))
                throw new ArgumentException($"Can not convert to primitive toy object without {nameof(MeshFilter)}", nameof(original));

            var primitiveType = GetPrimitiveType(filter);
            var hasCollision = original.TryGetComponent<Collider>(out _);

            primitiveObjectToy.NetworkPrimitiveType = primitiveType;
            primitiveObjectToy.NetworkMaterialColor = renderer.material.color;

            toy.NetworkScale = new Vector3(
                Math.Abs(toy.transform.lossyScale.x),
                Math.Abs(toy.transform.lossyScale.y),
                Math.Abs(toy.transform.lossyScale.z));
            toy.NetworkScale *= hasCollision ? 1 : -1;

            if (!hasCollision && toy.transform.localScale.x > 0 && (primitiveType == PrimitiveType.Plane || primitiveType == PrimitiveType.Quad))
            {
                // Exiled.API.Features.Log.Info("Rotated 180° X to compensate for negative scale");
                toy.transform.eulerAngles += Vector3.right * 180;
            }

            toy.transform.localScale = new Vector3(
                Math.Abs(toy.transform.localScale.x),
                Math.Abs(toy.transform.localScale.y),
                Math.Abs(toy.transform.localScale.z));
            toy.transform.localScale *= hasCollision ? 1 : -1;

            var syncScript = primitiveObjectToy.gameObject.AddComponent<PrimitiveSynchronizerScript>();
            syncScript.Toy = primitiveObjectToy;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;
            syncScript.MeshRenderer = syncColor ? renderer : null;

            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            return primitiveObjectToy;
        }

        private static IEnumerator<float> SynchronizationHandler()
        {
            while (!Round.IsStarted)
                yield return Timing.WaitForSeconds(1f);

            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(.25f);

                foreach (var player in RealPlayers.List)
                    UpdateSynchronizationPlayer(player);
            }
        }

        private static void UpdateSynchronizationPlayer(Player player)
        {
            HashSet<SynchronizerControllerScript> toSync = null;
            try
            {
                var curRoom = player.GetCurrentRoom();

                if (curRoom is null && player.Position.y is > 950 and < 1050)
                    curRoom = Room.Get(RoomType.Surface);

                if (LastRooms.TryGetValue(player, out var lastRoom) && lastRoom == curRoom)
                    return; // Skip, room didn't change since last update
                LastRooms[player] = curRoom;

                /*Exiled.API.Features.Log.Debug(
                    $"{player.Nickname} changed room, {lastRoom?.Type.ToString() ?? "NONE"} -> {curRoom?.Type.ToString() ?? "NONE"}");*/

                var room = Mistaken.API.Utilities.Room.Get(curRoom);

                if (room == null)
                {
                    foreach (var item in Controllers.Values)
                        item.RemoveSubscriber(player);

                    return;
                }

                toSync = NorthwoodLib.Pools.HashSetPool<SynchronizerControllerScript>.Shared.Rent();

                if (Controllers.TryGetValue(room.ExiledRoom, out var script))
                    toSync.Add(script);

                foreach (var item in room.FarNeighbors.Select(x => x.ExiledRoom))
                {
                    if (Controllers.TryGetValue(item, out script))
                        toSync.Add(script);
                }

                foreach (var item in Controllers.Values.Where(x => !toSync.Contains(x)))
                    item.RemoveSubscriber(player);

                foreach (var item in toSync)
                    item.AddSubscriber(player);

                // Optimization Example (Code from ColorfulEZ)
                /*try
                {
                    if (!LastRooms.TryGetValue(player, out var lastRoom))
                        lastRoom = null;

                    if (lastRoom == room)
                        return;

                    HashSet<API.Utilities.Room> loaded;
                    if (!(lastRoom is null))
                    {
                        loaded = lastRoom.FarNeighbors.ToHashSet();
                        loaded.Add(lastRoom);
                    }
                    else
                        loaded = new HashSet<API.Utilities.Room>();

                    HashSet<API.Utilities.Room> toLoad;
                    if (!(room is null))
                    {
                        toLoad = room.FarNeighbors.ToHashSet();
                        toLoad.Add(room);
                    }
                    else
                        toLoad = new HashSet<API.Utilities.Room>();

                    var intersect = loaded.Intersect(toLoad).ToArray();

                    foreach (var item in intersect)
                    {
                        loaded.Remove(item);
                        toLoad.Remove(item);
                    }

                    foreach (var item in loaded)
                        UnloadRoomFor(player, item);

                    foreach (var item in toLoad)
                        LoadRoomFor(player, item);

                    LastRooms[player] = room;
                }
                catch (Exception ex)
                {
                    Instance.Log.Error(ex);
                }*/
            }
            catch (Exception ex)
            {
                Exiled.API.Features.Log.Error(ex);
            }
            finally
            {
                if (toSync is not null)
                    NorthwoodLib.Pools.HashSetPool<SynchronizerControllerScript>.Shared.Return(toSync);
            }
        }

        private static void Player_Verified(Exiled.Events.EventArgs.VerifiedEventArgs ev)
        {
            globalController.SyncFor(ev.Player);
        }

        private void Player_ChangingSpectatedPlayer(Exiled.Events.EventArgs.ChangingSpectatedPlayerEventArgs ev)
        {
            this.CallDelayed(0.05f, () => UpdateSynchronizationPlayer(ev.Player), nameof(UpdateSynchronizationPlayer));
        }

        private void Scp079_ChangingCamera(Exiled.Events.EventArgs.ChangingCameraEventArgs ev)
        {
            this.CallDelayed(0.05f, () => UpdateSynchronizationPlayer(ev.Player), nameof(UpdateSynchronizationPlayer));
        }

        private void PostRoundCleanup()
        {
            globalController = Server.Host.GameObject.AddComponent<GlobalSynchronizerControllerScript>();
            ManagedToys.Clear();

            foreach (var room in Room.List)
                Controllers[room] = room.gameObject.AddComponent<SynchronizerControllerScript>();

            this.RunCoroutine(SynchronizationHandler(), nameof(SynchronizationHandler));
        }
    }
}
