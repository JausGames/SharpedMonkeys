
using Unity.Netcode;
using System;
using UnityEngine;

public class OnlinePlayerInputs : NetworkBehaviour
{
    OnlinePlayerController controller;
    OnlinePlayerCombat combat;
    public void Start()
    {
        controller = GetComponent<OnlinePlayerController>();
        combat = GetComponent<OnlinePlayerCombat>();
        Debug.Log("PlayerInputs, Start : isLocalPlayer ? " + IsLocalPlayer);
        if (!IsLocalPlayer) return;
        Debug.Log("PlayerInputs, Start : Move = " + OnlineInputManager.Controls.Player.Move);

        OnlineInputManager.Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        OnlineInputManager.Controls.Player.Move.canceled += _ => SetMovement(Vector2.zero);

        OnlineInputManager.Controls.Player.Look.performed += ctx => SetRotation(ctx.ReadValue<Vector2>());
        OnlineInputManager.Controls.Player.Look.canceled += _ => SetRotation(Vector2.zero);

        //OnlineInputManager.Controls.Player.MouseLook.performed += ctx => SetMouseRotation(ctx.ReadValue<Vector2>());
        //OnlineInputManager.Controls.Player.MouseLook.canceled += _ => SetMouseRotation(Vector2.zero);

        OnlineInputManager.Controls.Player.Attack.performed += _ => SetAttack(true, false);
        OnlineInputManager.Controls.Player.Attack.canceled += _ => SetAttack(false, true);

        OnlineInputManager.Controls.Player.Jump.performed += _ => SetJump(true, false);
        OnlineInputManager.Controls.Player.Jump.canceled += _ => SetJump(false, true);

        OnlineInputManager.Controls.Player.Dash.performed += _ => SetDash(true, false);
        OnlineInputManager.Controls.Player.Dash.canceled += _ => SetDash(false, true);
    }

    private void SetMovement(Vector2 direction)
    {
        Debug.Log("PlayerInputs, SetMovement : direction " + direction);
        controller.Move(direction);
    }
    private void SetRotation(Vector2 direction)
    {
        Debug.Log("PlayerInputs, SetMovement : rotation " + direction);
        controller.Look(direction);
    }
    private void SetMouseRotation(Vector2 position)
    {
        var mousePosition = (Camera.main.ScreenToWorldPoint(position) - transform.position).normalized;
        Debug.Log("PlayerInputs, SetMovement : rotation " + mousePosition);
        controller.Look(mousePosition);
    }
    private void SetAttack(bool perf, bool canc)
    {
        Debug.Log("PlayerInputs, SetPunching : value " + perf);
        combat.Attack(perf, canc);
    }
    private void SetJump(bool perf, bool canc)
    {
        Debug.Log("PlayerInputs, SetPunching : value " + perf);
        controller.Jump(perf, canc);
    }
    private void SetDash(bool perf, bool canc)
    {
        Debug.Log("PlayerInputs, SetPunching : value " + perf);
        controller.Dash(perf, canc);
    }
}
