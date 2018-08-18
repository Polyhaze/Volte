namespace SIVA.Plugins {
    public interface IPlugin {
        string PluginName { get; }
        string PluginAuthor { get; }
        string PluginVersion { get; }
        string PluginDescription { get; }
        bool RequireServerAdmin { get; }
        string CommandName { get; }
        string[] CommandAliases { get; }
        void Main();
    }
}