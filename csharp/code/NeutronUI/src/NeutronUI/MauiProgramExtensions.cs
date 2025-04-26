using NeutronUI.Controls;
using NeutronUI.Handlers;

namespace NeutronUI;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseNeutronUI(this MauiAppBuilder builder)
    {
        //配置 Handlers
        //注册所有自定义控件的渲染处理器
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddNeutronUIHandlers();
        });
        return builder;
    }

    /// <summary>
    /// 集中注册所有自定义控件的渲染处理器
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static IMauiHandlersCollection AddNeutronUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection
            .AddHandler(typeof(SelectableLabel), typeof(SelectableLabelHandler));
    }
}
