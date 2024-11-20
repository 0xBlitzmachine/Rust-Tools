using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text;
using Network;
using Oxide.Core;
using UnityEngine;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;

namespace Oxide.Plugins;


// WIP! Testing Covalence atm!
[Info("Covalence Global Say", "Blitzmachine", "1.0.0")]
[Description("Predefine multiple custom styled prefixes & messages with possibility for configuration and use it across plugins")]

// Use CovalencePlugin since this plugin logic can be translated universal
// Both RustPlugin and CovalencePlugin are inheritor of CSharpPlugin and Covalence/CSharp Plugin are enough for this.
public class CovalenceGlobalSay : CovalencePlugin
{
    #region Setup
    // Ignore all custom set steam ids for avatar usage in messages when collect_vood's CustomIcon plugin is loaded.
    // https://umod.org/plugins/custom-icon
    [PluginReference]
    private Plugin? CustomIcon;
    #endregion

    #region Configuration
    protected override void LoadDefaultMessages() => lang.RegisterMessages(GetDefaultMessages(), this);
    #endregion

    #region Covalence Commands
    [Command("ping")]
    private void CovalenceCommand(IPlayer player, string command, string[] args)
    {
        player.Reply(LocaleMessage("CustomGlobalSay", player.Name));
    }
    #endregion

    #region Universal Hooks

    private void Loaded()
    {
        if (CustomIcon == null)
            return;

        if (!CustomIcon.IsLoaded)
            Puts("CustomIcon Plugin is not loaded!");
        else
            Puts("CustomIcon plugin loaded");
    }

    private void OnPluginLoaded(Plugin? plugin)
    {
        if (plugin?.Name == this.Name && plugin.Author == this.Author && plugin.Version == this.Version)
            return;

        CustomIcon = plugin is { Name: "CustomIcon", Author: "collect_vood" } && plugin.Version == new VersionNumber(1, 0, 4) ? plugin : null;
        Puts("Found CustomIcon by collect_vood ..");

        if (CustomIcon != null)
            Puts($"{plugin.Name} version {plugin.Version} author: {plugin.Author}" + "loaded!");
        else
            Puts("Failed to load CustomIcon");
    }

    #endregion

    #region Internal Helpers

    private Dictionary<string, string> GetDefaultMessages() => new()
    {
        ["CustomGlobalSay"] = "Replying with Pong - {0}",
    };

    private string LocaleMessage(string key, params object[] args) =>
        string.Format(lang.GetMessage(key, this), args);


    #endregion
}
