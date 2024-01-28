﻿using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Modularity;
using VipCoreApi;

namespace VIP_KillScreen;

public class VipKillScreen : BasePlugin, IModulePlugin
{
    public override string ModuleAuthor => "thesamefabius";
    public override string ModuleName => "[VIP] Kill Screen";
    public override string ModuleVersion => "v1.0.1";

    private KillScreen _killScreen;
    private IVipCoreApi _api = null!;
    
    public void LoadModule(IApiProvider provider)
    {
        _api = provider.Get<IVipCoreApi>();
        _killScreen = new KillScreen(this, _api);
        _api.RegisterFeature(_killScreen);
    }

    public override void Unload(bool hotReload)
    {
        _api.UnRegisterFeature(_killScreen);
    }
}

public class KillScreen : VipFeatureBase
{
    public override string Feature => "Killscreen";
    
    public KillScreen(VipKillScreen vipKillScreen, IVipCoreApi api) : base(api)
    {
        vipKillScreen.RegisterEventHandler<EventPlayerDeath>((@event, info) =>
        {
            var attacker = @event.Attacker;
            if (!attacker.IsValid) return HookResult.Continue;
            if (attacker.PlayerName == @event.Userid.PlayerName) return HookResult.Continue;

            if (!IsClientVip(attacker)) return HookResult.Continue;
            if (!PlayerHasFeature(attacker)) return HookResult.Continue;
            if (GetPlayerFeatureState(attacker) is not IVipCoreApi.FeatureState.Enabled) return HookResult.Continue;
            if(!GetFeatureValue<bool>(attacker)) return HookResult.Continue;
            
            var attackerPawn = attacker.PlayerPawn.Value;
            
            if (attackerPawn == null) return HookResult.Continue;
            
            attackerPawn.HealthShotBoostExpirationTime = Server.CurrentTime + 1.0f;
            Utilities.SetStateChanged(attackerPawn, "CCSPlayerPawn", "m_flHealthShotBoostExpirationTime");

            return HookResult.Continue;
        });
    }
}
