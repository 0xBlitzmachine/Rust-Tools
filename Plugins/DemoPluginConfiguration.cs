using Newtonsoft.Json;
using System;
using Oxide.Core.Plugins;

namespace Oxide.Plugins;

[Info("Configuration", "Blitzmachine", "0.0.2")]
[Description("Blueprint/Demo of an Oxide Configuration")]
public class DemoPluginConfiguration : RustPlugin
{
    #region Variables
    private Configuration _configuration;
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
    
    protected override void SaveConfig() => Config.WriteObject(_configuration, true);
    protected override void LoadDefaultConfig() => _configuration = new Configuration();
    protected override void LoadConfig()
    {
        base.LoadConfig();
        try
        {
            _configuration = Config.ReadObject<Configuration>();
        }
        catch (Exception e)
        {
            PrintError($"The configuration file is invalid:\n\n\n {e.Message}");
            PrintWarning("Using default values. Fix your configuration!");
            LoadDefaultConfig();
        }
    }

    #endregion

    #region Chat Commands
    [ChatCommand("test")]
    private void CmdPlayground(BasePlayer player, string command, string[] args)
    {
        SendReply(player, $"{_configuration.ChatSettings.Steam64ID}");
    }
    #endregion
}