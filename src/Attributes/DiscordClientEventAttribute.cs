namespace Pooja.src.Attributes;

/// <summary>
/// `DiscordEventAttribute`. Used to essentially "mark" classes as a registerable `DiscordClient` event.
/// Classes marked with this attribute must match the target event name
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DiscordClientEventAttribute : Attribute {

}