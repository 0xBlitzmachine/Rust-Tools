using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text;


namespace Oxide.Plugins;

[Info("Configuration", "Blitzmachine", "0.0.2")]
[Description("Blueprint/Demo of an Oxide Configuration")]
public class DemoPluginConfiguration : RustPlugin
{
    #region Variables
    private PluginConfig _pluginConfig;
    #endregion

    #region Configuration

    #region Plugin Configuration Template
    private class PluginConfig
    {
        [JsonProperty(propertyName: "Chat Settings")]
        public ChatSetting Chat = new()
        {
            Steam64Id = 76561199446355310
        };
        
        [JsonProperty(propertyName: "Player Settings")]
        public PlayerSetting Player = new()
        {
            NeedsAdminPermission = false,
            PlayerInventory = new PlayerInventorySetting()
            {
                InventoryContainer = new Dictionary<string, int>()
                {
                    { "Ak", 5 },
                    { "Sulfur", 2000 }
                },
                InventoryBeltContainer = new List<string>() { "Sulfur", "That" },
                InventoryWearContainer = new List<string>() { "Wolf Hat", "Jacket" }
            }
        };
    }
    

    private class ChatSetting
    {
        [JsonProperty(propertyName: "Steam64 ID")]
        public ulong Steam64Id { get; set; }
    }

    private class PlayerSetting
    {
        [JsonProperty(propertyName: "Needs Admin Permission?")]
        public bool NeedsAdminPermission { get; set; }
        
        [JsonProperty(propertyName: "Player Inventory Settings")] 
        public PlayerInventorySetting PlayerInventory { get; set; }
    }

    private class PlayerInventorySetting
    {
        [JsonProperty(propertyName: "Inventory Container")]
        public Dictionary<string, int> InventoryContainer { get; set; }
        
        [JsonProperty(propertyName: "Inventory Wear Container")]
        public List<string> InventoryWearContainer { get; set; }
        
        [JsonProperty(propertyName: "Inventory Belt Container")]
        public List<string> InventoryBeltContainer { get; set; }
    }
    #endregion
    
    protected override void SaveConfig() => Config.WriteObject(_pluginConfig, true);
    protected override void LoadDefaultConfig() => _pluginConfig = new PluginConfig();
    protected override void LoadConfig()
    {
        base.LoadConfig();
        try
        {
            _pluginConfig = Config.ReadObject<PluginConfig>();
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
        var builder = new StringBuilder("Your command executed!");

        foreach (KeyValuePair<string, int> pair in _pluginConfig.Player.PlayerInventory.InventoryContainer)
        {
            builder.Append(pair.Key + " - Amount: " + pair.Value.ToString());
        }
        
        player.ChatMessage(builder.ToString());
    }
    #endregion
}