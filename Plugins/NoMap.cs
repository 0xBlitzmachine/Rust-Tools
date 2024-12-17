using System;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins;

[Info("No Map", "Blitzmachine", "1.0.1")]
[Description("Adds a full sized panel over the map interface with an custom set text")]
public class NoMap : RustPlugin
{
    #region Variables

    private CuiElementContainer _elementContainer;
    private Configuration _config;
    private const string CONTAINER_IDENTIFIER = "nomap.ui.container";
    private const string PERMISSION_NAME = "nomap.use";

    #endregion

    #region Plugin Configuration

    private class Configuration
    {
        [JsonProperty(propertyName: "Background Color (HEX)")]
        public string BackgroundColor { get; set; }

        [JsonProperty(propertyName: "Text")]
        public string Text { get; set; }

        [JsonProperty(propertyName: "Font Size")]
        public int FontSize { get; set; }

        [JsonProperty(propertyName: "Font Color")]
        public string FontColor { get; set; }

        [JsonProperty(propertyName: "Font Outline Color")]
        public string FontOutlineColor { get; set; }

        [JsonProperty(propertyName: "Font Outline Distance")]
        public string FontOutlineDistance { get; set; }

        public static Configuration GetDefaultConfiguration() => new Configuration
        {
            BackgroundColor = "#1c1c1c",
            Text = "Map Interface has been disabled!",
            FontColor = "#fa4343",
            FontSize = 40,
            FontOutlineColor = "#f1f1f1",
            FontOutlineDistance = "4"
        };
    }

    protected override void LoadDefaultConfig() => _config = Configuration.GetDefaultConfiguration();
    protected override void SaveConfig() => Config.WriteObject(_config);
    protected override void LoadConfig()
    {
        base.LoadConfig();
        try { _config = Config.ReadObject<Configuration>(); }
        catch (Exception e)
        {
            var builder = new StringBuilder("Configuration file is corrupt - (Invalid JSON Format?)").AppendLine();
            builder.AppendLine(e.Message);
            PrintError(builder.ToString());

            LoadDefaultConfig();
            PrintWarning("Configuration Object is using default values.");
        }
    }

    #endregion

    #region Oxide Hooks

    private void Init()
    {
        if (!permission.PermissionExists(PERMISSION_NAME))
            permission.RegisterPermission(PERMISSION_NAME, this);

        _elementContainer = InitializeElementContainer();

        if (_elementContainer.Count == 0)
            PrintWarning("ElementContainer has 0 elements.");
    }

    private void Loaded()
    {
        var players = BasePlayer.activePlayerList;

        if (players.Count == 0)
            return;

        foreach (BasePlayer player in players)
        {
            RefreshMapInterfaceUsage(player);
        }
    }

    private void OnPlayerConnected(BasePlayer player) => RefreshMapInterfaceUsage(player);

    private void Unload()
    {
        var players = BasePlayer.activePlayerList;

        if (players.Count == 0)
            return;

        foreach (BasePlayer player in players)
        {
            if (player is null)
                continue;

            if (!player.IsConnected)
                continue;

            CuiHelper.DestroyUi(player, CONTAINER_IDENTIFIER);
        }
    }

    private void OnUserPermissionGranted(string id, string permName)
    {
        if (permName.Equals(PERMISSION_NAME))
        {
            var player = BasePlayer.FindByID(ulong.Parse(id));

            if (IsPlayerOnline(player))
                CuiHelper.AddUi(player, _elementContainer);
        }
    }

    void OnUserPermissionRevoked(string id, string permName)
    {
        if (permName.Equals(PERMISSION_NAME))
        {
            var player = BasePlayer.FindByID(ulong.Parse(id));

            if (IsPlayerOnline(player))
                CuiHelper.DestroyUi(player, CONTAINER_IDENTIFIER);
        }
    }
    #endregion

    #region Internal Helpers

    private static bool IsPlayerOnline(BasePlayer player) => player != null && player.IsConnected;
    private void RefreshMapInterfaceUsage(BasePlayer player)
    {
        if (IsPlayerOnline(player))
        {
            if (permission.UserHasPermission(player.UserIDString, PERMISSION_NAME))
                CuiHelper.AddUi(player, _elementContainer);
            else
                CuiHelper.DestroyUi(player, CONTAINER_IDENTIFIER);
        }
    }

    #endregion

    #region CUI Internal Helpers

    private CuiElementContainer InitializeElementContainer()
    {
        var container = new CuiElementContainer();

        try
        {
            var panel = new CuiPanel
            {
                Image = { Color = HexToCuiColor(_config.BackgroundColor) },
                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1"}
            };

            var element = new CuiElement
            {
                Parent = CONTAINER_IDENTIFIER,
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = _config.Text,
                        Align = TextAnchor.MiddleCenter,
                        Color = HexToCuiColor(_config.FontColor),
                        FontSize = _config.FontSize
                    },
                    new CuiOutlineComponent
                    {
                        Color = HexToCuiColor(_config.FontOutlineColor),
                        Distance = _config.FontOutlineDistance
                    },
                    new CuiRectTransformComponent { AnchorMin = "0 0", AnchorMax = "1 1" }
                }
            };

            container.Add(panel, "Map", CONTAINER_IDENTIFIER);
            container.Add(element);
        }
        catch (Exception e)
        {
            PrintError("Failed to initialize components for ElementContainer. (Wrong Component usage?)");
            PrintError(e.Message);
            return container;
        }
        return container;
    }

    // Thanks to Mevent - ActiveSort
    private static string HexToCuiColor(string hex, float alpha = 100)
    {
        if (string.IsNullOrEmpty(hex)) hex = "#FFFFFF";

        var str = hex.Trim('#');
        if (str.Length != 6) throw new Exception(hex);
        var r = byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
        var g = byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
        var b = byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber);

        return $"{(double)r / 255} {(double)g / 255} {(double)b / 255} {alpha / 100f}";
    }

    #endregion
}
