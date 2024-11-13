using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text;
using ConVar;

namespace Oxide.Plugins;

[Info("Chat Message Library", "Blitzmachine", "1.0.0")]
[Description("A library to handle chat messages. Customize a constant prefix and message style and use it anywhere else")]
public class ChatMessageLibrary : RustPlugin
{
    #region Variables
    private ConfigData configData;
    #endregion

    #region Configuration

    #region Plugin Configuration Template
    private class ConfigData
    {
        [JsonProperty(PropertyName = "Settings")]
        public ChatSettings Chat { get; set; }

        [JsonProperty(PropertyName = "Chat Title Settings")]
        public ChatPrefixSettings ChatPrefix { get; set; }

        [JsonProperty(PropertyName = "Chat Message Settings")]
        public ChatMessageSettings ChatMessage { get; set; }

        [JsonProperty(PropertyName = "Server Message Settings")]
        public ServerMessageSettings ServerMessage { get; set; }
    }

    private class ChatSettings
    {
        [JsonProperty(PropertyName = "Icon (Steam64 ID)")]
        public ulong SteamID { get; set; }

        [JsonProperty(PropertyName = "Chat Format")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "Break message into new line after prefix")]
        public bool BreaklineAfterPrefix { get; set; }
    }

    private class ChatPrefixSettings
    {
        [JsonProperty(PropertyName = "Prefix")]
        public string Prefix { get; set; }

        [JsonProperty(PropertyName = "Color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "Size")]
        public int Size { get; set; }
    }

    private class ChatMessageSettings
    {
        [JsonProperty(PropertyName = "Color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "Size")]
        public int Size { get; set; }
    }

    private class ServerMessageSettings
    {
        [JsonProperty(PropertyName = "Intercept Server Messages")]
        public bool Intercept { get; set; }

        [JsonProperty(PropertyName = "Stop Intercept Server Messages containing")]
        public HashSet<string> StopInterceptCollection { get; set; }
    }
    #endregion

    protected override void SaveConfig() => Config.WriteObject(configData, true);
    protected override void LoadDefaultConfig()
    {
        configData = new ConfigData
        {
            Chat = new ChatSettings
            {
                SteamID = 76561199446355310,
                Format = "{prefix} {message}",
                BreaklineAfterPrefix = true
            },
            ChatPrefix = new ChatPrefixSettings
            {
                Prefix = "Paradox Gaming »",
                Color = "#d42f3f",
                Size = 15
            },
            ChatMessage = new ChatMessageSettings
            {
                Color = "#fff",
                Size = 13
            },
            ServerMessage = new ServerMessageSettings
            {
                Intercept = true,
                StopInterceptCollection = new HashSet<string>
                {
                    "gave"
                }
            }
        };
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();
        try
        {
            configData = Config.ReadObject<ConfigData>();
            PrintWarning("Configuration loaded!");
        }
        catch (Exception ex)
        {
            var builder = new StringBuilder("Failed to read Configuration (Invalid JSON Format):");
            builder.AppendLine(string.Empty);
            builder.AppendLine(ex.Message);

            PrintError(builder.ToString());
            PrintWarning("Using default values. Fix your configuration!");
            LoadDefaultConfig();
        }
    }
    #endregion

    #region Oxide Hooks
    private object? OnServerMessage(string serverMessage, string playerName)
    {
        if (configData.ServerMessage.Intercept)
        {
            foreach (var word in configData.ServerMessage.StopInterceptCollection)
            {
                if (serverMessage.Contains(word) && playerName.Contains("SERVER"))
                    return false;
            }
            API_SendMessageToServer(serverMessage);
            return true;
        }
        return null;
    }
    #endregion

    #region Developer Hooks
    /// <summary>
    /// API function that will send customized message to a specific user.
    /// </summary>
    /// <param name="player">The player you want to target.</param>
    /// <param name="message">The message the player will receive.</param>
    /// <returns>Returns true when player was not null. Otherwise false</returns>
    private bool API_SendMessageToPlayer(BasePlayer player, string message)
    {
        if (player == null)
            return false;

        Player.Message(player, FormatMessage(message), configData.Chat.SteamID);
        return true;
    }

    /// <summary>
    /// API function that will broadcast your customized message to the server using the set configuration.
    /// </summary>
    /// <param name="message">The message you want to send to the server.</param>
    private void API_SendMessageToServer(string message)
    {
        if (BasePlayer.activePlayerList.Count < 1)
            return;

        Server.Broadcast(FormatMessage(message), configData.Chat.SteamID);
    }
    #endregion

    #region Helper
    private string FormatMessage(string message)
    {
        string prefix = configData.Chat.BreaklineAfterPrefix ? "\n" + GetStyledPrefix() : GetStyledPrefix();

        return configData.Chat.Format
            .Replace("{prefix}", prefix)
            .Replace("{message}", GetStyledMessage(message));
    }

    private string GetStyledPrefix()
    {
        return "<size={size}><color={color}>{prefix}</color></size>"
            .Replace("{size}", configData.ChatPrefix.Size.ToString())
            .Replace("{color}", configData.ChatPrefix.Color)
            .Replace("{prefix}", configData.ChatPrefix.Prefix);
    }

    private string GetStyledMessage(string message)
    {
        return "<size={size}><color={color}>{message}</color></size>"
            .Replace("{size}", configData.ChatMessage.Size.ToString())
            .Replace("{color}", configData.ChatMessage.Color)
            .Replace("{message}", message);
    }
    #endregion
}
