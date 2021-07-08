﻿using System;
using UnityEngine;

namespace BZCommon
{
    public static class InputHelper
    {
        public static string KeyCodeToString(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Alpha0:
                    return "0";
                case KeyCode.Alpha1:
                    return "1";
                case KeyCode.Alpha2:
                    return "2";
                case KeyCode.Alpha3:
                    return "3";
                case KeyCode.Alpha4:
                    return "4";
                case KeyCode.Alpha5:
                    return "5";
                case KeyCode.Alpha6:
                    return "6";
                case KeyCode.Alpha7:
                    return "7";
                case KeyCode.Alpha8:
                    return "8";
                case KeyCode.Alpha9:
                    return "9";
                case KeyCode.Slash:
                    return "/";
                case KeyCode.Equals:
                    return "=";
                case KeyCode.LeftBracket:
                    return "[";
                case KeyCode.RightBracket:
                    return "]";
                case KeyCode.BackQuote:
                    return "`";
                case KeyCode.Backslash:
                    return "\\";
                case KeyCode.Underscore:
                    return "_";
                case KeyCode.LeftParen:
                    return "(";
                case KeyCode.RightParen:
                    return ")";
                case KeyCode.Asterisk:
                    return "*";
                case KeyCode.Plus:
                    return "+";
                case KeyCode.Minus:
                    return "-";
                case KeyCode.Question:
                    return "?";
                case KeyCode.Colon:
                    return ":";
                case KeyCode.Semicolon:
                    return ";";
                case KeyCode.Comma:
                    return ",";
                case KeyCode.Period:
                    return ".";
                case KeyCode.Greater:
                    return ">";
                case KeyCode.Mouse0:
                    return "MouseButtonLeft";
                case KeyCode.Mouse1:
                    return "MouseButtonRight";
                case KeyCode.Mouse2:
                    return "MouseButtonMiddle";
                case KeyCode.JoystickButton0:
                    return "ControllerButtonA";
                case KeyCode.JoystickButton1:
                    return "ControllerButtonB";
                case KeyCode.JoystickButton2:
                    return "ControllerButtonX";
                case KeyCode.JoystickButton3:
                    return "ControllerButtonY";
                case KeyCode.JoystickButton4:
                    return "ControllerButtonLeftBumper";
                case KeyCode.JoystickButton5:
                    return "ControllerButtonRightBumper";
                case KeyCode.JoystickButton6:
                    return "ControllerButtonBack";
                case KeyCode.JoystickButton7:
                    return "ControllerButtonHome";
                case KeyCode.JoystickButton8:
                    return "ControllerButtonLeftStick";
                case KeyCode.JoystickButton9:
                    return "ControllerButtonRightStick";
                default:
                    return keyCode.ToString();
            }
        }

        public static KeyCode StringToKeyCode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            switch (input)
            {
                case "0":
                    return KeyCode.Alpha0;
                case "1":
                    return KeyCode.Alpha1;
                case "2":
                    return KeyCode.Alpha2;
                case "3":
                    return KeyCode.Alpha3;
                case "4":
                    return KeyCode.Alpha4;
                case "5":
                    return KeyCode.Alpha5;
                case "6":
                    return KeyCode.Alpha6;
                case "7":
                    return KeyCode.Alpha7;
                case "8":
                    return KeyCode.Alpha8;
                case "9":
                    return KeyCode.Alpha9;
                case "/":
                    return KeyCode.Slash;
                case "=":
                    return KeyCode.Equals;
                case "[":
                    return KeyCode.LeftBracket;
                case "]":
                    return KeyCode.RightBracket;
                case "`":
                    return KeyCode.BackQuote;
                case "\\":
                    return KeyCode.Backslash;
                case "_":
                    return KeyCode.Underscore;
                case "(":
                    return KeyCode.LeftParen;
                case ")":
                    return KeyCode.RightParen;
                case "*":
                    return KeyCode.Asterisk;
                case "+":
                    return KeyCode.Plus;
                case "-":
                    return KeyCode.Minus;
                case "?":
                    return KeyCode.Question;
                case ":":
                    return KeyCode.Colon;
                case ";":
                    return KeyCode.Semicolon;
                case ",":
                    return KeyCode.Comma;
                case ".":
                    return KeyCode.Period;
                case ">":
                    return KeyCode.Greater;
                case "MouseButtonLeft":
                    return KeyCode.Mouse0;
                case "MouseButtonRight":
                    return KeyCode.Mouse1;
                case "MouseButtonMiddle":
                    return KeyCode.Mouse2;
                case "ControllerButtonA":
                    return KeyCode.JoystickButton0;
                case "ControllerButtonB":
                    return KeyCode.JoystickButton1;
                case "ControllerButtonX":
                    return KeyCode.JoystickButton2;
                case "ControllerButtonY":
                    return KeyCode.JoystickButton3;
                case "ControllerButtonLeftBumper":
                    return KeyCode.JoystickButton4;
                case "ControllerButtonRightBumper":
                    return KeyCode.JoystickButton5;
                case "ControllerButtonBack":
                    return KeyCode.JoystickButton6;
                case "ControllerButtonHome":
                    return KeyCode.JoystickButton7;
                case "ControllerButtonLeftStick":
                    return KeyCode.JoystickButton8;
                case "ControllerButtonRightStick":
                    return KeyCode.JoystickButton9;
                default:
                    try
                    {
                        return (KeyCode)Enum.Parse(typeof(KeyCode), input);
                    }
                    catch
                    {
                        BZLogger.Debug($"Cannot translate input name [{input}] to KeyCode!");
                        return 0;
                    }
            }
        }

    }
}
