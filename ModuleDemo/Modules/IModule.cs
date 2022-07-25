namespace ModuleDemo.Modules;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection services);
}