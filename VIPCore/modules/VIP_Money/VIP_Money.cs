﻿using CounterStrikeSharp.API.Core;
using Modularity;
using VipCoreApi;
using static VipCoreApi.IVipCoreApi;

namespace VIP_Money;

public class VipMoney : BasePlugin, IModulePlugin
{
    public override string ModuleAuthor => "WodiX";
    public override string ModuleName => "[VIP] Money";
    public override string ModuleVersion => "v1.0.2";

    private IVipCoreApi _api = null!;
    private Money _money;

    public void LoadModule(IApiProvider provider)
    {
        _api = provider.Get<IVipCoreApi>();
        _money = new Money(_api);
        _api.RegisterFeature(_money);
    }

    public override void Unload(bool hotReload)
    {
        _api.UnRegisterFeature(_money);
    }
}

public class Money : VipFeatureBase
{
    public override string Feature => "Money";
    
    public Money(IVipCoreApi api) : base(api)
    {
    }

    public override void OnPlayerSpawn(CCSPlayerController player)
    {
        if (IsPistolRound()) return;
        if (!PlayerHasFeature(player)) return;
        if (GetPlayerFeatureState(player) is not FeatureState.Enabled) return;

        var moneyServices = player.InGameMoneyServices;
        if (moneyServices == null) return;
        
        var moneyValue = GetFeatureValue<string>(player);

        if (string.IsNullOrWhiteSpace(moneyValue)) return;

        if (moneyValue.Contains("++"))
            moneyServices.Account += int.Parse(moneyValue.Split("++")[1]);
        else
            moneyServices.Account = int.Parse(moneyValue);
    }
}