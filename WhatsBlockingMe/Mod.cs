using GDWeave;

namespace WhatsBlockingMe;

public class Mod : IMod {
    public Config Config;
    private IModInterface modInterface;

    public Mod(IModInterface modInterface)
    {
        // init
        this.modInterface = modInterface;
        this.Config = modInterface.ReadConfig<Config>();

        // register script
        this.modInterface.RegisterScriptMod(new LoadingMenuScript());

        Log("general", "Loaded stedee.WhatsBlockingMe!");
    }

    public void Log(string name, string data)
    {
        this.modInterface.Logger.Information($"[WhatsBlockingMe.{name}] {data}");
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
