# BlazorWasmBrotliLoader.Build

[![NuGet Package](https://img.shields.io/nuget/v/BlazorWasmBrotliLoader.Build.svg)](https://www.nuget.org/packages/BlazorWasmBrotliLoader.Build/) [![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community&labelColor=5865f2&color=gray)](https://discord.com/channels/798312431893348414/1202165955900473375)

## 📝Summary

When you publish your Blazor WebAssembly app, this package rewrites the fallback page file (`wwwroot/index.html`) to be loading Brotli pre-compressed application files, such as `*.wasm.br`.

![Before: 5.5MB transferred, After: 2.2MB transferred](https://raw.githubusercontent.com/jsakamoto/BlazorWasmBrotliLoader.Build/refs/heads/main/.assets/social-media.png)

Pre-compressed files are smaller than original one, so this package will help make your Blazor WebAssembly app boot much faster than usual, sinse it reduce the initial content loading time.

## 🚀Quick Start

Install this package to your Blazor WebAssembly project.

```
dotnet add package BlazorWasmBrotliLoader.Build
```

Basically, **that's all**.

**Once you install this package, the output of the `dotnet publish` command will make the `wwwroot/index.html` load `*.wasm.br` files!** 🎉

> [!IMPORTANT]  
> If the Blazor WebAssembly app is hosted on a server that supports Brotli compression, **you don't have to use this package**. For example, the ASP.NET Core server of .NET9 will handle them well ([see also here](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/static-files?view=aspnetcore-9.0)). This package is only useful for an app hosted on a server that doesn't support Brotli compression, such as a simple static content server, such as GitHub Pages.

> [!NOTE]  
> In fact, you can implement the feature of loading Brotli pre-compressed files by yourself without depending on this package. [Microsoft's official document](https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly#compression) tells us how to do it. But it can be hard work, particularly if you are implementing a PWA. This NuGet package allows us to use pre-compression application files out of the box!

> [!NOTE]  
> For years, the feature of loading Brotli pre-compressed files has been built into the ["PublishSPAforGitHubPages.Build"](https://github.com/jsakamoto/PublishSPAforGitHubPages.Build) package. However, since it is strongly tied to publishing on GitHub Pages, it can not be used on other platforms. So, I decided to split this feature into an individual package.

## 🛜 PWAs and Service Workers

If your Blazor WebAssembly PWA loading Brotli pre-compressed files needs to run offline, please ensure the service worker loads "*.br" files in the offline cache. For example, if you generated your Blazor WebAssmebly PWA project by the standard project template, you will need to update the `wwwroot/service-worker-published.js` file. In this case, you need to append the `/\.br$/` regex pattern to the definition of the `offlineAssetsInclude` array constant variable in the `wwwroot/service-worker-published.js` file.

```js
// 🗒️ wwwroot/service-worker-published.js
...
const offlineAssetsInclude = [ 
  /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/,
  /\.br$/ // 👈 Add this line
];
```

## ⚙️Configurations

If you don't want to enable the Brotli pre-compressed file loading feature, set the `BrotliLoaderInjectLoader` MSBuild property to `false`. For example, you can do it in the `dotnet publish` command as below.

```
dotnet publish -p BrotliLoaderInjectLoader=false
```

## 🎉Release notes

[Release notes](https://github.com/jsakamoto/BlazorWasmBrotliLoader.Build/blob/master/RELEASE-NOTES.txt)

## 📢License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/BlazorWasmBrotliLoader.Build/blob/master/LICENSE)
