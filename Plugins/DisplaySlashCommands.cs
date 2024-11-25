using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust;
using Oxide.Game.Rust.Cui;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;
using Facepunch.Models.Database;

namespace Oxide.Plugins;

[Info("Display Slash Commands", "Blitzmachine", "1.0.0")]
[Description("/")]

public class DisplaySlashCommands : CovalencePlugin
{
    #region Plugin Setup

    private PluginConfiguration _config;
    private CuiElementContainer _elementContainer;

    private const string MAIN_LAYER_IDENTIFIER = "displayslashcommands.ui.main";
    private const string HEADER_LAYER_IDENTIFIER = "displayslashcommands.ui.header";
    private const string BODY_LAYER_IDENTIFIER = "displayslashcommands.ui.body";

    private const string PERMISSION_NAME = "displayslashcommands.use";

    #endregion

    #region Configuration

    private class PluginConfiguration
    {

        [JsonProperty(propertyName: "Sequence of External Plugins")]
        public IEnumerable<ExternalPlugin> SeqOfExternalPlugins { get; set; }

        public static PluginConfiguration GetDefaultConfig() => new()
        {
            SeqOfExternalPlugins = new List<ExternalPlugin>
            {
                {
                    new ExternalPlugin
                    {
                        Title = "My Plugin XY",
                        SeqOfExternalCommands = new List<ExternalCommand>
                        {
                            new ExternalCommand
                            {
                                Name = "dostuff",
                                Description = "Will do stuff!",
                                Args = new List<ExternalCommandArgument>
                                {
                                    {
                                        new ExternalCommandArgument
                                        {
                                            Optional = false,
                                            Argument = "player",
                                            Description = "The player you want to target!"
                                        }
                                    },
                                    {
                                        new ExternalCommandArgument
                                        {
                                            Optional = true,
                                            Argument = "color",
                                            Description = "Will send the message with color to the target!"
                                        }
                                    }
                                }
                            },
                            new ExternalCommand
                            {
                                Name = "dostuffhelp",
                                Description = "Will display how to do stuff!",
                                Args = new List<ExternalCommandArgument>()
                            }
                        }
                    }
                },
                {
                    new ExternalPlugin
                    {
                        Title = "EpicPlugin",
                        SeqOfExternalCommands = new List<ExternalCommand>
                        {
                            new ExternalCommand
                            {
                                Name = "epicstuff",
                                Description = "Executing this command will do some epic stuff!",
                                Args = new List<ExternalCommandArgument>()
                            }
                        }
                    }
                },
            }
        };
    }

    private class ExternalPlugin
    {
        [JsonProperty(propertyName: "Plugin Name")]
        public string Title { get; set; }

        [JsonProperty(propertyName: "Commands")]
        public IEnumerable<ExternalCommand> SeqOfExternalCommands { get; set; }
    }

    private class ExternalCommand
    {
        [JsonProperty(propertyName: "Command Name (without slash)")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "Description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "Command Arguments")]
        public IEnumerable<ExternalCommandArgument> Args { get; set; }
    }

    private class ExternalCommandArgument
    {
        [JsonProperty(propertyName: "Argument is optional")]
        public bool Optional { get; set; }

        [JsonProperty(propertyName: "Argument Name")]
        public string Argument { get; set; }

        [JsonProperty(propertyName: "Argument Description")]
        public string Description { get; set; }
    }

    protected override void SaveConfig() => Config.WriteObject(_config);
    protected override void LoadDefaultConfig() => _config = PluginConfiguration.GetDefaultConfig();

    protected override void LoadConfig()
    {
        base.LoadConfig();
        try
        {
            _config = Config.ReadObject<PluginConfiguration>();
        }
        catch (Exception e)
        {
            var builder = new StringBuilder("Configuration file is corrupt - (Invalid JSON Format?)");
            builder.AppendLine(e.Message);

            PrintError(builder.ToString());
            PrintWarning("Configuration Object is using default values.");
            LoadDefaultConfig();
        }
    }

    #endregion

    #region Oxide Hooks

    private void Unload()
    {
        var count = 0;
        var onlinePlayers = players.Connected.GetEnumerator();

        while (onlinePlayers.MoveNext())
        {
            var currentPlayer = onlinePlayers.Current;
            if (currentPlayer == null)
                continue;

            var player = currentPlayer.Object as BasePlayer;
            if (player == null)
                continue;

            count++;
            CuiHelper.DestroyUi(player, MAIN_LAYER_IDENTIFIER);
        }

        onlinePlayers.Dispose();
        PrintWarning(string.Format("Tried destroying UI for {0} {1}", count, count > 1 ? "players" : "player"));
    }

    private void Init()
    {
        if (!permission.PermissionExists(PERMISSION_NAME))
            permission.RegisterPermission(PERMISSION_NAME, this);

        _elementContainer = InitializeElementContainer();
        GenerateUi(ref _elementContainer);

        PrintWarning(_elementContainer == null
        ? "ElementContainer is null!"
        : "ElementContainer successfully initialized!");

        PrintWarning(_elementContainer.Count == 0
        ? "ElementContainer is empty!"
        : $"ElementContainer has {_elementContainer.Count} elements");

    }

    #endregion

    #region Commands

    [Command("s"), Permission(PERMISSION_NAME)]
    private void ShowSlashCommand(IPlayer player, string command, string[] args)
    {
        if (player == null)
        {
            PrintWarning("ShowUI: IPlayer IS null!");
            return;
        }

        if (_elementContainer == null)
        {
            PrintWarning("ShowUI: ElementContainer IS null!");
            return;
        }

        var basePlayer = player.Object as BasePlayer;
        if (basePlayer == null)
        {
            PrintWarning("ShowUI: BasePlayer IS null!");
        }

        CuiHelper.AddUi(basePlayer, _elementContainer);
    }

    [Command("d"), Permission(PERMISSION_NAME)]
    private void DestroySlashCommand(IPlayer player, string command, string[] args)
    {
        if (player == null)
        {
            PrintWarning("ShowUI: IPlayer IS null!");
            return;
        }

        if (_elementContainer == null)
        {
            PrintWarning("ShowUI: ElementContainer IS null!");
            return;
        }

        var basePlayer = player.Object as BasePlayer;
        if (basePlayer == null)
        {
            PrintWarning("ShowUI: BasePlayer IS null!");
        }

        CuiHelper.DestroyUi(basePlayer, MAIN_LAYER_IDENTIFIER);
    }

    #endregion

    #region Internal Cui Helpers

    // RawImageSprite: "assets/content/textures/generic/fulltransparent.tga"
    private static CuiElementContainer InitializeElementContainer() => new()
    {
        {
            new CuiPanel
            {
                CursorEnabled = true,
                FadeOut = 2f,
                Image =
                {
                    Color = "0 0 0 0",
                    FadeIn = 2f,
                    Sprite = "assets/content/textures/generic/fulltransparent.tga"
                },
                RectTransform = { AnchorMin = "0.4 0.4", AnchorMax = "0.6 0.6" }
            },
            "Overlay", MAIN_LAYER_IDENTIFIER
        }
    };

    private static void GenerateUi(ref CuiElementContainer container)
    {
        var header = new CuiPanel
        {
            FadeOut = 1f,
            Image =
            {
                Color = "0 0 1 1",
                FadeIn = 1f,
            },
            RectTransform = { AnchorMin = "0.0 0.0", AnchorMax = "0.3 1.0" }
        };

        var body = new CuiPanel
        {
            FadeOut = 1f,
            Image =
            {
                Color = "0 1 0 1",
                FadeIn = 1f
            },
            RectTransform = { AnchorMin = "0.3 0.0", AnchorMax = "1.0 1.0" }
        };

        var closeButton = new CuiButton
        {
            FadeOut = 2f,
            Button =
            {
                Color = "1 0 0 1",
                // Close = MAIN_LAYER_IDENTIFIER,
                ImageType = Image.Type.Filled,
                Command = "d"
            },
            Text =
            {
                Color = "0 0 0 1",
                Text = "X",
                FontSize = 10,
                FadeIn = 2f,
                Align = TextAnchor.MiddleCenter,
            },
            RectTransform = { AnchorMin = "0.0 0.9", AnchorMax = "0.05 1.0" }
        };

        container.Add(header, MAIN_LAYER_IDENTIFIER);
        container.Add(body, MAIN_LAYER_IDENTIFIER);
        container.Add(closeButton, MAIN_LAYER_IDENTIFIER);
    }
    #endregion
}
