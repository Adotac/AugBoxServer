using FishNet;
using FishNet.Managing.Scened;
using FishNet.Plugins.FishyEOS.Util;
using System;
namespace MultiP2P
{
    public class UIPanelMenuSettings : UIPanelSettings
    {
        public void Back()
        {
            UIPanelManager.Instance.HidePanel<UIPanelMenuSettings>(false);
        }
    }
}