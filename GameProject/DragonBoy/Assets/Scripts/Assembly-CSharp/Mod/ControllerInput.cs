using System;
using GameController;
using Mod;
using Mod.ModMenu;
using UnityEngine;

internal static class ControllerInput
{
    static GameControls gameControls;

    static float leftTrigger, rightTrigger;
    static bool leftButton, rightButton, selectButton, startButton, westButton, eastButton, northButton, southButton, joystickButton1, joystickButton2;
    static Vector2 dPad, joystick1, joystick2;

    static bool isResetMoveMyChar;
    static bool isNextMap;
    static bool isResetCamera;

    internal static bool IsLeftButtonPressed => leftButton;
    internal static bool IsRightButtonPressed => rightButton;
    internal static bool IsSelectButtonPressed => selectButton;
    internal static bool IsStartButtonPressed => startButton;
    internal static bool IsWestButtonPressed => westButton;
    internal static bool IsEastButtonPressed => eastButton;
    internal static bool IsNorthButtonPressed => northButton;
    internal static bool IsSouthButtonPressed => southButton;
    internal static bool IsJoystickButton1Pressed => joystickButton1;
    internal static bool IsJoystickButton2Pressed => joystickButton2;
    internal static float LeftTriggerValue => leftTrigger;
    internal static float RightTriggerValue => rightTrigger;
    internal static Vector2 DPadValue => dPad;
    internal static Vector2 Joystick1Value => joystick1;
    internal static Vector2 Joystick2Value => joystick2;

    internal static void OnAwake()
    {
        gameControls = new GameControls();
        gameControls.Input.LeftButton.performed += ctx => leftButton = true;
        gameControls.Input.LeftButton.canceled += ctx => leftButton = false;
        gameControls.Input.RightButton.performed += ctx => rightButton = true;
        gameControls.Input.RightButton.canceled += ctx => rightButton = false;
        gameControls.Input.SelectButton.performed += ctx => selectButton = true;
        gameControls.Input.SelectButton.canceled += ctx => selectButton = false;
        gameControls.Input.StartButton.performed += ctx => startButton = true;
        gameControls.Input.StartButton.canceled += ctx => startButton = false;
        gameControls.Input.WestButton.performed += ctx => westButton = true;
        gameControls.Input.WestButton.canceled += ctx => westButton = false;
        gameControls.Input.EastButton.performed += ctx => eastButton = true;
        gameControls.Input.EastButton.canceled += ctx => eastButton = false;
        gameControls.Input.NorthButton.performed += ctx => northButton = true;
        gameControls.Input.NorthButton.canceled += ctx => northButton = false;
        gameControls.Input.SouthButton.performed += ctx => southButton = true;
        gameControls.Input.SouthButton.canceled += ctx => southButton = false;
        gameControls.Input.JoystickButton1.performed += ctx => joystickButton1 = true;
        gameControls.Input.JoystickButton1.canceled += ctx => joystickButton1 = false;
        gameControls.Input.JoystickButton2.performed += ctx => joystickButton2 = true;
        gameControls.Input.JoystickButton2.canceled += ctx => joystickButton2 = false;

        gameControls.Input.LeftTrigger.performed += ctx => leftTrigger = ctx.ReadValue<float>();
        gameControls.Input.LeftTrigger.canceled += ctx => leftTrigger = 0;
        gameControls.Input.RightTrigger.performed += ctx => rightTrigger = ctx.ReadValue<float>();
        gameControls.Input.RightTrigger.canceled += ctx => rightTrigger = 0;
        gameControls.Input.DPad.performed += ctx => dPad = ctx.ReadValue<Vector2>();
        gameControls.Input.DPad.canceled += ctx => dPad = Vector2.zero;
        gameControls.Input.Joystick1.performed += ctx => joystick1 = ctx.ReadValue<Vector2>();
        gameControls.Input.Joystick1.canceled += ctx => joystick1 = Vector2.zero;
        gameControls.Input.Joystick2.performed += ctx => joystick2 = ctx.ReadValue<Vector2>();
        gameControls.Input.Joystick2.canceled += ctx => joystick2 = Vector2.zero;
        gameControls.Enable();
    }

    internal static void OnUpdate()
    {
        if (CanControlGameScr())
        {
            CheckJoystickMoveMyChar();
            CheckJoystickMoveCamera();
        }
        else
            CheckDPadButtons();
        if (IsSouthButtonPressed)
        {
            ButtonEnter = true;
            southButton = false;
        }
        else if (IsEastButtonPressed)
        {
            GameCanvas.keyPressed[13] = true;
            eastButton = false;
        }
        else if (IsWestButtonPressed)
        {
            GameScr.gI().doUseHP();
            westButton = false;
        }
        else if (IsNorthButtonPressed)
        {
            Utils.useCapsule();
            northButton = false;
        }
        else if (IsSelectButtonPressed)
        {
            if (!GameCanvas.panel.isShow)
                GameScr.gI().cmdMenu.performAction();
            selectButton = false;
        }
        else if (IsStartButtonPressed)
        {
            if (GameCanvas.panel2 == null || (GameCanvas.panel2 != null && !GameCanvas.panel2.isShow))
                ModMenuMain.ShowPanel();
            startButton = false;
        }
        else if (CanControlGameScr())
        {
            if (IsLeftButtonPressed)
            {
                int index = Array.IndexOf(GameScr.keySkill, Char.myCharz().myskill);
                int count = 0;
                do
                {
                    if (--index < 0)
                        index = GameScr.keySkill.Length - 1;
                    count++;
                }
                while (GameScr.keySkill[index] == null && count < GameScr.keySkill.Length);
                Skill skill = Char.myCharz().myskill = GameScr.keySkill[index];
                Service.gI().selectSkill(skill.template.id);
                GameScr.gI().saveRMSCurrentSkill(skill.template.id);
                GameScr.gI().resetButton(); leftButton = false;
            }
            else if (IsRightButtonPressed)
            {
                int index = Array.IndexOf(GameScr.keySkill, Char.myCharz().myskill);
                int count = 0;
                do
                {
                    if (++index == GameScr.keySkill.Length)
                        index = 0;
                    count++;
                }
                while (GameScr.keySkill[index] == null && count < GameScr.keySkill.Length);
                Skill skill = Char.myCharz().myskill = GameScr.keySkill[index];
                Service.gI().selectSkill(skill.template.id);
                GameScr.gI().saveRMSCurrentSkill(skill.template.id);
                GameScr.gI().resetButton();
                rightButton = false;
            }
        }
    }

    static bool CanControlGameScr()
    {
        if (GameCanvas.currentScreen is not GameScr)
            return false;
        if (GameCanvas.panel != null && GameCanvas.panel.isShow)
            return false;
        if (GameCanvas.panel2 != null && GameCanvas.panel2.isShow)
            return false;
        if (InfoDlg.isShow)
            return false;
        if (GameCanvas.currentDialog != null && GameCanvas.currentDialog is MsgDlg)
            return false;
        if (ChatTextField.gI().isShow)
            return false;
        if (GameCanvas.menu.showMenu)
            return false;
        if (ChatPopup.serverChatPopUp != null || ChatPopup.currChatPopup != null)
            return false;
        return true;
    }

    static void CheckJoystickMoveMyChar()
    {
        if (Joystick1Value != Vector2.zero)
        {
            isResetMoveMyChar = true;
            float angle = Mathf.Atan2(Joystick1Value.y, Joystick1Value.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360;
            ResetMoveButtons();
            if (IsJoystickButton1Pressed)
            {
                if (!isNextMap)
                {
                    if (angle >= 45 && angle <= 135)
                        Utils.ChangeMapMiddle();
                    else if (angle >= 135 && angle <= 225)
                        Utils.ChangeMapLeft();
                    else if (angle <= 45 || angle >= 315)
                        Utils.ChangeMapRight();
                    else if (angle >= 225 && angle <= 315)
                        Utils.DonTho();
                    isNextMap = true;
                }
            }
            else if(!isNextMap)
            {
                if (angle >= 45 && angle <= 135)
                    ButtonUpHold = ButtonUp = true;
                else if (angle >= 225 && angle <= 315)
                    ButtonDownHold = ButtonDown = true;
                if (angle >= 135 && angle <= 225)
                    ButtonLeftHold = ButtonLeft = true;
                else if (angle <= 45 || angle >= 315)
                    ButtonRightHold = ButtonRight = true;
            }
        }
        else
        {
            if (isNextMap)
                isNextMap = false;
            if (isResetMoveMyChar)
            {
                isResetMoveMyChar = false;
                ResetMoveButtons();
            }
        }
    }

    static void ResetMoveButtons()
    {
        ButtonUpHold = ButtonUp = false;
        ButtonDownHold = ButtonDown = false;
        ButtonLeftHold = ButtonLeft = false;
        ButtonRightHold = ButtonRight = false;
    }

    static void CheckJoystickMoveCamera()
    {
        if (Joystick2Value != Vector2.zero)
        {
            isResetCamera = true;
            Char.myCharz().cmtoChar = false;
            float angle = Mathf.Atan2(Joystick2Value.y, Joystick2Value.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360;
            if (angle >= 45 && angle <= 135)
                GameScr.cmy = GameScr.cmtoY = Mathf.Clamp(GameScr.cmtoY - (int)Joystick2Value.y * 3, 0, GameScr.cmyLim);
            else if (angle >= 225 && angle <= 315)
                GameScr.cmy = GameScr.cmtoY = Mathf.Clamp(GameScr.cmtoY - (int)Joystick2Value.y * 3, 0, GameScr.cmyLim);
            if (angle >= 135 && angle <= 225)
                GameScr.cmx = GameScr.cmtoX = Mathf.Clamp(GameScr.cmtoX + (int)Joystick2Value.x * 3, 24, GameScr.cmxLim);
            else if (angle <= 45 || angle >= 315)
                GameScr.cmx = GameScr.cmtoX = Mathf.Clamp(GameScr.cmtoX + (int)Joystick2Value.x * 3, 24, GameScr.cmxLim);
        }
        else if (IsJoystickButton2Pressed)
            Char.myCharz().cmtoChar = false;
        else if (isResetCamera)
        {
            isResetCamera = false;
            Char.myCharz().cmtoChar = true;
        }
    }

    static void CheckDPadButtons()
    {
        if (DPadValue != Vector2.zero)
        {
            ResetMoveButtons();
            if (DPadValue.x == 1)
                ButtonRightHold = ButtonRight = true;
            else if (DPadValue.x == -1)
                ButtonLeftHold = ButtonLeft = true;
            else if (DPadValue.y == 1)
                ButtonUpHold = ButtonUp = true;
            else if (DPadValue.y == -1)
                ButtonDownHold = ButtonDown = true;
            dPad = Vector2.zero;
        }
    }

    static bool ButtonUp
    {
        get => GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21];
        set => GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = value;
    }
    static bool ButtonDown
    {
        get => GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22];
        set => GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = value;
    }
    static bool ButtonLeft
    {
        get => GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23];
        set => GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = value;
    }
    static bool ButtonRight
    {
        get => GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24];
        set => GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = value;
    }
    static bool ButtonEnter
    {
        get => GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
        set => GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = value;
    }
    static bool ButtonUpHold
    {
        get => GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
        set => GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = value;
    }
    static bool ButtonDownHold
    {
        get => GameCanvas.keyHold[(!Main.isPC) ? 8 : 22];
        set => GameCanvas.keyHold[(!Main.isPC) ? 8 : 22] = value;
    }
    static bool ButtonLeftHold
    {
        get => GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
        set => GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = value;
    }
    static bool ButtonRightHold
    {
        get => GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
        set => GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = value;
    }
    static bool ButtonEnterHold
    {
        get => GameCanvas.keyHold[(!Main.isPC) ? 5 : 25];
        set => GameCanvas.keyHold[(!Main.isPC) ? 5 : 25] = value;
    }
}