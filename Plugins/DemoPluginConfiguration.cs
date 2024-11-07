using ConVar;
using Epic.OnlineServices.Sessions;
using System;
using Newtonsoft.Json;
using UnityEngine;
using Oxide.Core.Plugins;
using Oxide.Core.Configuration;
using Console = System.Console;
using DDraw = ConVar.DDraw;

namespace Oxide.Plugins;

[Info("Configuration", "Blitzmachine", "0.0.2")]
[Description("Blueprint/Demo of an Oxide Configuration")]
public class DemoPluginConfiguration : RustPlugin
{
    #region Variables
    private Configuration? _configuration;
    #endregion

    #region Configuration
    private class Configuration
    {
        [JsonProperty(propertyName: "Chat Settings")]
        public ChatSettings ChatSettings = new()
        {
            Steam64ID = 76561199446355310
        };
    }

    private class ChatSettings
    {
        [JsonProperty(propertyName: "Steam64 ID")]
        public ulong Steam64ID { get; set; }
    }

    private void Init()
    {
        try
        {
            _configuration = Config.ReadObject<Configuration>();
        }
        catch
        {
            PrintError("Failed to load configuration file! Invalid JSON Format");
            PrintWarning("Using default configuration.");
            LoadDefaultConfig();
        }
    }

    protected override void SaveConfig() => Config.WriteObject(_configuration, true);
    protected override void LoadDefaultConfig() => _configuration = new Configuration();


    #endregion

    #region Chat Commands
    [ChatCommand("test")]
    private void CmdPlayground(BasePlayer player, string command, string[] args)
    {
        SendReply(player, $"{_configuration!.ChatSettings.Steam64ID}");
    }
    #endregion
}