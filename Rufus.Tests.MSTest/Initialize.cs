using Microsoft.Windows.ApplicationModel.DynamicDependency;

[assembly: WinUITestTarget(typeof(Rufus.App))]

namespace Rufus.Tests.MSTest;

[TestClass]
public class Initialize
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        // Major/minor version of the Windows App SDK, packed as 0xMMMMNNNN.
        // Must track the Microsoft.WindowsAppSDK package reference: 2.5 -> 0x00020005.
        Bootstrap.TryInitialize(0x00020005, out var _);
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        Bootstrap.Shutdown();
    }
}
