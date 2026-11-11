using System.ComponentModel;

namespace Modsen.Portal.Core.Certificate;

public enum JsStackType
{
    None = 0,

    [Description("Node")]
    Node = 1,

    [Description("React")]
    React = 2,

    [Description("React Native")]
    ReactNative = 3,

    [Description("Angular")]
    Angular = 4
}
