using NeutronUI;

[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(NeutronUI.Controls))]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "neutron")]

namespace NeutronUI;
public static class Constants
{
    public const string XamlNamespace = "http://schemas.zuoyi-projects.io/dotnet/maui/NeutronUI";

    public const string NamespacePrefix = $"{nameof(NeutronUI)}.";
}
