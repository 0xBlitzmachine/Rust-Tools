using System.Collections.Generic;
using System;
using System.Text;
using Newtonsoft.Json;
using Oxide.Core.Plugins;

namespace Oxide.Plugins;

// WIP
[Info("Advertiser", "Blitzmachine", "1.0.0")]
[Description("Send notifications to the server every set minute")]
public class Advertiser : RustPlugin
{

    #region Definition
    [PluginReference]
    private Plugin ChatMessageController;

    private ConfigData configData;
    #endregion

    #region Config Template
    private class ConfigData
    {
        [JsonProperty(PropertyName = "Plugin Settings")]
        public ExternalPluginConfig ExternalPlugin { get; set; }
    }

    private class ExternalPluginConfig
    {
        [JsonProperty(propertyName: "ChatMessageController Plugin")]
        public bool Enabled { get; set; }
    }
    #endregion

    #region Config Setup
    protected override void SaveConfig() => Config.WriteObject(configData, true);

    protected override void LoadDefaultConfig()
    {
        configData = new ConfigData
        {
            ExternalPlugin = new ExternalPluginConfig
            {
                Enabled = false
            }
        };
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();
        try
        {
            configData = Config.ReadObject<ConfigData>();
        }
        catch (Exception e)
        {
            var builder = new StringBuilder("The configuration file is invalid (Wrong JSON Format)")
                .AppendLine(e.Message);
            
            PrintError(builder.ToString());
            PrintWarning("Initializing default configuration in memory.");
            LoadDefaultConfig();
        }
    }

    private Timer _timer;
    private List<string> myList = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9"};

    private void Unload()
    {
        _timer.Destroy();
    }
    private void Loaded()
    {
        if (configData.ExternalPlugin.Enabled)
        {
            if (ChatMessageController == null)
                PrintWarning("Plugin ChatMessageController not found. (NULL)");

            PrintWarning("Plugin ChatMessageController enabled.");
        }

        if (ChatMessageController != null)
        {
            ResetTimer();
        }
    }

    [ChatCommand("quit")]
    private void QuitCommand(BasePlayer player, string command, string[] args)
    {
        _timer.Destroy();
    }

    [ChatCommand("start")]
    private void StartCommand(BasePlayer player, string command, string[] args)
    {
        if (_timer.Destroyed)
        {
            Puts("Timer was destroyed.");
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        _timer = timer.Every(10,
            () =>
            {
                ChatMessageController.Call("SendMessageToServer", myList.GetRandom());
            });
    }



    #endregion
}
