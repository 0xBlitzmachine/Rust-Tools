using System.Collections.Generic;
using System.Text;
using System;
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

public class DisplaySlashCommands : RustPlugin
{
    #region Plugin Setup

    private PluginConfiguration PluginConfig;
    private CuiElementContainer ElementContainer;

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

    protected override void SaveConfig() => Config.WriteObject(PluginConfig, true);
    protected override void LoadDefaultConfig() => PluginConfig = PluginConfiguration.GetDefaultConfig();

    protected override void LoadConfig()
    {
        base.LoadConfig();
        try
        {
            PluginConfig = Config.ReadObject<PluginConfiguration>();
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
        foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
        {
            CuiHelper.DestroyUi(basePlayer, HEADER_LAYER_IDENTIFIER);
        }
    }

    private void Init()
    {
        if (!permission.PermissionExists(PERMISSION_NAME))
            permission.RegisterPermission(PERMISSION_NAME, this);

        ElementContainer = InitializeElementContainer();
        GenerateUi(ref ElementContainer);

        PrintWarning(ElementContainer == null
        ? "ElementContainer is null!"
        : "ElementContainer successfully initialized!");

        PrintWarning(ElementContainer.Count == 0
        ? "ElementContainer is empty!"
        : $"ElementContainer has {ElementContainer.Count} elements");

    }

    #endregion

    #region Internal Commands

    [ChatCommand("s"), Permission(PERMISSION_NAME)]
    private void ShowSlashCommand(BasePlayer player, string command, string[] args)
    {
        if (player == null)
        {
            PrintWarning("ShowUI: Player was null!");
            return;
        }

        if (ElementContainer == null)
        {
            PrintWarning("ShowUI: ElementContainer is null!");
            return;
        }

        CuiHelper.AddUi(player, ElementContainer);
    }

    [ChatCommand("d"), Permission(PERMISSION_NAME)]
    private void DestroySlashCommand(BasePlayer player, string command, string[] args) => CuiHelper.DestroyUi(player, MAIN_LAYER_IDENTIFIER);

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
            FadeOut = 2f,
            Image =
            {
                Color = "1 5 5 1",
                FadeIn = 2f,
            },
            RectTransform = { AnchorMin = "0.0 0.0", AnchorMax = "0.3 1.0" }
        };

        var body = new CuiPanel
        {
            FadeOut = 2f,
            Image =
            {
                Color = "255 255 255 1",
                FadeIn = 2f,
            },
            RectTransform = { AnchorMin = "0.3 0.0", AnchorMax = "1.0 1.0" }
        };

        var closeButton = new CuiButton
        {
            FadeOut = 2f,
            Button =
            {
                Color = "1 0 0 1",
                Close = MAIN_LAYER_IDENTIFIER,
                ImageType = Image.Type.Filled,
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
