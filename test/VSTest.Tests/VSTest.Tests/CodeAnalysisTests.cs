using System.Reflection;
using Xunit;

namespace VSTest.Tests;

public class CodeAnalysisTests
{
    [Fact]
    public void GetCustomAttributes_Test()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (var property in type.GetProperties())
                {
                    //  Could not load type 'System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute' from assembly 'Microsoft.TestPlatform.CoreUtilities'
                    var attributes = property.GetCustomAttributes();
                }
            }
        }
    }
}
