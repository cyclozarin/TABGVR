using HarmonyLib;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(global::Player))]
public class PlayerPatch
{
    private static Transform _rotationTarget;
    private static PlayerManager _playerManager;


    /// <summary>
    ///     Adds <see cref="VRControls" /> and <see cref="RotationTargetDriver" /> on local <see cref="Player" />.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPatch(nameof(global::Player.Start))]
    [HarmonyPostfix]
    public static void Start(global::Player __instance)
    {
        _playerManager = new PlayerManager(__instance.gameObject);

        if (!_playerManager.playerIsClient) return; // run the rest if we are the player

        // run code on player load here

        _rotationTarget = _playerManager.playerRoot.transform.Find("CameraMovement").Find("CameraRotationX");
        // .Find("RotationTarget");

        var playerDriver = _rotationTarget.gameObject.AddComponent<RotationTargetDriver>();

        playerDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        playerDriver.poseSource = TrackedPoseDriver.TrackedPose.Head;

        _playerManager.playerRoot.AddComponent<VRControls>();
    }
}