namespace Pooja.src.Attributes;

/// <summary>
/// `CommandsNextEventAttribute`. Used to essentially "mark" classes as a registerable `CommandsNextExtension` event.
/// Classes marked with this attribute must match the target event name
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CommandsNextEventAttribute : Attribute
{

}
