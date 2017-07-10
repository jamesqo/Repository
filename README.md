# Repository

An Android app designed for editing code. Written with Xamarin.

## Features

- High-perf editor: open large files instantly.

## Building

Before building, you'll need to do some manual configuration. After you follow the steps below, you'll be able to open the solution in Visual Studio and debug it.

### Supply your GitHub client ID/secret

1. Go to the `Internal/` subdirectory of the main projects and create a file called `Creds.cs`.
2. Copy and paste the following content. Replace the strings with your client ID and secret [from GitHub](https://github.com/settings/applications/new).

```cs
namespace Repository.Internal
{
    internal static class Creds
    {
        public static string ClientId { get; } = "YOUR_CLIENT_ID";

        public static string ClientSecret { get; } = "YOUR_CLIENT_SECRET";
    }
}
```

### Build the AAR file

A small portion of this app is written in Java. The C# code interfaces with it via a bindings library. You'll need to build the Java code before the bindings library will build.

1. Open a command prompt and navigate to this repo's root.
2. Run `powershell ./build-aar.ps1`.

## License

[MIT](LICENSE)
