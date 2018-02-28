using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;



public class IdleAnimations : Script
{
    string ScriptName = "Idle Animations";
    string ScriptVer = "1.1";
    int TimeToIdle = 15000;
    int GameTimeRef = Game.GameTime+20000;
    bool IsIdling = false;
    bool GangIdle = true;

    List<string> Idles = new List<string>();
    public IdleAnimations()
    {
        Tick += OnTick;
        LoadSettings();
    }

    void OnTick(object sender, EventArgs e)
    {
        if (IsIdling)
        {
            if (Game.IsControlPressed(2, GTA.Control.VehicleAccelerate) || Game.IsControlPressed(2, GTA.Control.VehicleBrake) || Game.IsControlPressed(2, GTA.Control.VehicleMoveLeft)|| Game.IsControlPressed(2, GTA.Control.VehicleMoveRight)
                || Game.Player.Character.IsRagdoll)
            {
                Game.Player.Character.Task.ClearAll();

                IsIdling = false;
                GameTimeRef = Game.GameTime;
                if (GangIdle)
                {
                    foreach (Ped ped in World.GetAllPeds()) if (ped.CurrentPedGroup == Game.Player.Character.CurrentPedGroup) ped.Task.ClearAll();                        
                }
            }
        }
        else
        {
            if (Idles.Count>0 && Game.Player.Character.IsOnFoot)
            {
                if (Game.Player.Character.IsRagdoll || !Game.Player.Character.IsOnFoot || !Game.Player.Character.IsStopped || Game.Player.Character.IsInCover() || Game.Player.IsAiming) GameTimeRef = Game.GameTime;
                else if (Game.GameTime > GameTimeRef + TimeToIdle)
                {
                    IsIdling = true;
                    Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, Game.Player.Character, Idles[RandomInt(0, Idles.Count - 1)], -1, false);

                    if (GangIdle) foreach (Ped ped in World.GetAllPeds()) if (ped.CurrentPedGroup == Game.Player.Character.CurrentPedGroup) Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, ped, Idles[RandomInt(0, Idles.Count - 1)], -1, false);
                }
            }
        }
    }
    public static Random rnd = new Random();
    public static int RandomInt(int min, int max)
    {
        return rnd.Next(min, max);
    }


    void LoadSettings()
    {
        if (File.Exists(@"scripts\IdleAnimations.ini"))
        {
            ScriptSettings config = ScriptSettings.Load(@"scripts\IdleAnimations.ini");
           string idle=config.GetValue<string>("SETTINGS", "IdleAnimation", "");

            if (idle != "") Idles.Add(config.GetValue<string>("SETTINGS", "IdleAnimation", ""));
            if (idle != "") Idles.Add(config.GetValue<string>("SETTINGS", "IdleAnimation2", ""));
            if (idle != "") Idles.Add(config.GetValue<string>("SETTINGS", "IdleAnimation3", ""));
            if (idle != "") Idles.Add(config.GetValue<string>("SETTINGS", "IdleAnimation4", ""));
            if (idle != "") Idles.Add(config.GetValue<string>("SETTINGS", "IdleAnimation5", ""));
            GangIdle = config.GetValue<bool>("SETTINGS", "GangIdlesToo", true);
            TimeToIdle = config.GetValue<int>("SETTINGS", "TimeToIdle", 10)*1000;
        }

    }
}
