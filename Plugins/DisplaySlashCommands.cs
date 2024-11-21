using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Oxide.Core;
using UnityEngine;
using System.Text.Json.Serialization;
using Epic.OnlineServices.TitleStorage;

namespace Oxide.Plugins;

[Info("Display Slash Commands", "Blitzmachine", "1.0.0")]
[Description("/")]

public class DisplaySlashCommands : CovalencePlugin
{
    #region Plugin Setup
    private const string PERMISSION_NAME = "displayslash.use";
    #endregion

    #region Configuration
    private class PluginConfiguration
    {

        [JsonProperty(propertyName: "List of external Plugins")]
        public IEnumerable<ExternalPlugin> ListOfExternalPlugins { get; set; }
        public static PluginConfiguration GetDefaultConfig() => new()
        {
            ListOfExternalPlugins = new List<ExternalPlugin>
            {
                {
                    new ExternalPlugin {
                        Title = "My Plugin XY",
                        ListOfExternalCommands = new List<ExternalCommand>
                        {
                            new ExternalCommand
                            {
                                Name = "dostuff",
                                Description = "Will do stuff!"
                            },
                            new ExternalCommand
                            {
                                Name = "dostuffhelp",
                                Description = "Will display how to do stuff!"
                            }
                        }
                    }
                },
                {
                    new ExternalPlugin {
                        Title = "EpicPlugin",
                        ListOfExternalCommands = new List<ExternalCommand>
                        {
                            new ExternalCommand
                            {
                                Name = "epicstuff",
                                Description = "Executing this command will do some epic stuff!"
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

        [JsonProperty(propertyName: "Included commands")]
        public IEnumerable<ExternalCommand> ListOfExternalCommands { get; set; }
    }

    private class ExternalCommand
    {
        [JsonProperty(propertyName: "Command Name (without slash)")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "Description")]
        public string Description { get; set; }
    }
    #endregion
}