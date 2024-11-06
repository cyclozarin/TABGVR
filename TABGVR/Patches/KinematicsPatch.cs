using Epic.OnlineServices;
using Epic.OnlineServices.Presence;
using HarmonyLib;
using TABGVR.Player;
using UltimateIK;
using UnityEngine;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(Holding))]
class KinematicsPatch
{
    static void SetupConnection(Rigidbody joint)
    {
        joint.GetComponentInChildren<Collider>().enabled = false;

        // disable gravity in arms
        joint.useGravity = false;
        joint.transform.parent.Find(joint.gameObject.name).GetComponent<Rigidbody>().useGravity = false;
    }

    static void UpdateConnection(Rigidbody joint, GameObject controller)
    {
        var controllerRelativeToGameCamera = controller.transform.position - Controllers.Head.transform.position +
                                             Camera.current.transform.position;

        if (Vector3.Distance(joint.position, controllerRelativeToGameCamera) < 0.01f) return;

        joint.MovePosition(joint.position + controllerRelativeToGameCamera -
            joint.transform.GetChild(0).position);
    }

    [HarmonyPatch(nameof(Holding.Start))]
    [HarmonyPostfix]
    static void StartPostfix(Holding __instance)
    {
        SetupConnection(__instance.rightHand);
        SetupConnection(__instance.leftHand);
    }

    [HarmonyPatch(nameof(Holding.Update))]
    [HarmonyPostfix]
    static void UpdatePostfix(Holding __instance)
    {
        if (!Controllers.LeftHand || !Controllers.RightHand || !Controllers.Head) return;
        if (__instance.player != global::Player.localPlayer) return;

        // Plugin.Logger.LogInfo(
        //     $"KP {__instance.player} / Head: {Controllers.Head.transform.position} / Left: {Controllers.LeftHand.transform.position} / Right: {Controllers.RightHand.transform.position}");

        UpdateConnection(__instance.rightHand, Controllers.RightHand);
        UpdateConnection(__instance.leftHand, Controllers.LeftHand);
    }
}