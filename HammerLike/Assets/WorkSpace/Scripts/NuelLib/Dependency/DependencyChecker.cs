using System;
using System.Reflection;

public static class DependencyChecker
{
    public static bool IsLibraryAvailable(string namespaceName)
    {
        try
        {
            // Attempt to load the assembly containing the namespace
            Assembly.Load(namespaceName);
            return true;
        }
        catch
        {
            // Assembly not found
            return false;
        }
    }

    public static bool CheckDependencies(string packageName)
    {
        // Check for Newtonsoft.Json
        if (!IsLibraryAvailable(packageName))
        {
            Console.WriteLine($"{packageName} is not installed. Please install Newtonsoft.Json to continue.");

            return false;
        }
        else
        {
            Console.WriteLine($"{packageName} is installed.");

            return true;
        }
    }
}
