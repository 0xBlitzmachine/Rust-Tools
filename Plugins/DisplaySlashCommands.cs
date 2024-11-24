using System.Collections.Generic;
using System.Text;
using System;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust;
using Oxide.Game.Rust.Cui;
using UnityEngine;
using UnityEngine.UI;

namespace Oxide.Plugins;

[Info("Display Slash Commands", "Blitzmachine", "1.0.0")]
[Description("/")]

public class DisplaySlashCommands : CovalencePlugin
{
    #region Plugin Setup

    private PluginConfiguration PluginConfig;
    private CuiElementContainer ElementContainer = new CuiElementContainer();
    private string Layer = string.Empty;

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

    private void Init()
    {
        if (!permission.PermissionExists(PERMISSION_NAME))
            permission.RegisterPermission(PERMISSION_NAME, this);

        CreateElementContainer(ref ElementContainer, ref Layer);
    }

    #endregion

    #region Internal Commands

    [Command("s"), Permission(PERMISSION_NAME)]
    private void ShowSlashCommand(IPlayer player, string command, string[] args)
    {
        CuiHelper.AddUi(player.Object as BasePlayer, ElementContainer);
    }

    [Command("d"), Permission(PERMISSION_NAME)]
    private void DestroySlashCommand(IPlayer player, string command, string[] args)
    {
        CuiHelper.DestroyUi(player.Object as BasePlayer, Layer);
    }

    [Command("db"), Permission(PERMISSION_NAME)]
    private void DestroyBSlashCommand(IPlayer player, string command, string[] args)
    {
        CuiHelper.DestroyUi(player.Object as BasePlayer, "button");
    }

    #endregion

    #region Internal Cui Helpers

    private static void CreateElementContainer(ref CuiElementContainer container, ref string layer)
    {
        layer = CuiHelper.GetGuid();

        CuiElement mainContainer = new CuiElement
        {
            Name = layer,
            Parent = "Overlay",
            Components =
            {
                new CuiRectTransformComponent
                {
                    AnchorMin = "0.4 0.4",
                    AnchorMax = "0.6 0.6"
                },
                new CuiImageComponent
                {
                    Color = "0.1 0.1 0.1 0.8"
                }
            }
        };
        container.Add(mainContainer);

        CuiElement textElement = new CuiElement()
        {
            Parent = layer,
            FadeOut = 5f,
            Components =
            {
                  new CuiTextComponent
                  {
                      Text = layer,
                      FontSize = 16,
                      Align = TextAnchor.MiddleCenter
                  },
                  new CuiOutlineComponent
                  {
                      Color = "0 0 1 1",
                      Distance = "0.1 0.1",
                      UseGraphicAlpha = true
                  }
            }
        };
        container.Add(textElement);

        CuiElement buttonElement = new CuiElement()
        {
            Name = "button",
            Parent = layer,
            Components =
            {
                new CuiButtonComponent
                {
                    Command = "d",
                    Color = "1 0 0 1"
                },
                new CuiRectTransformComponent
                {
                    AnchorMin = "0.2 0.7",
                    AnchorMax = "0.8 0.9"
                }
            }
        };
        container.Add(buttonElement);

        CuiElement buttonTextElement = new CuiElement()
        {
            Parent = "button",
            Components =
            {
                new CuiTextComponent()
                {
                    Text = "My button Text",
                    Color = "0 1 0 1",
                    Align = TextAnchor.MiddleCenter
                },
                new CuiRectTransformComponent()
                {
                    AnchorMin = "0.2 0.2",
                    AnchorMax = "0.8 0.8"
                }
            }
        };
        container.Add(buttonTextElement);
        #endregion
    }
}
