namespace BlazorApp.Stress.Tests;

/// <summary>A new empty folder under the system temp folder, deleted on dispose.</summary>
public sealed class TempFolder : IDisposable
{
    /// <summary>Creates the folder.</summary>
    public TempFolder()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"stress-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
    }

    /// <summary>Full path of the folder.</summary>
    public string Path { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, recursive: true);
        }
        catch (IOException)
        {
            // Best effort.
        }
    }
}
