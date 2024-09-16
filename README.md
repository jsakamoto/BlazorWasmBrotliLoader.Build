# BlazorWasmBrotliLoader.Build

[![NuGet Package](https://img.shields.io/nuget/v/BlazorWasmBrotliLoader.Build.svg)](https://www.nuget.org/packages/BlazorWasmBrotliLoader.Build/) [![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community&labelColor=5865f2&color=gray)](https://discord.com/channels/798312431893348414/1202165955900473375)

## 📝Summary

When you publish your Blazor WebAssembly app, this package rewrites the fallback page file (`wwwroot/index.html`) to be loading Brotli pre-compressed application files, such as `*.wasm.br`.

Pre-compressed files are smaller than original one, so this package will help make your Blazor WebAssembly app boot much faster than usual, sinse it reduce the initial content loading time.

## 🚀Quick Start

Install this package to your Blazor WebAssembly project.

```
dotnet add package BlazorWasmBrotliLoader.Build
```

Basically, **that's all**.

**Once installing this package is done, the output of the `dotnet publish` command will make the `wwwroot/index.html` be loading `*.wasm.br` files!** 🎉

## ⚙️Configurations

## 🎉Release notes

[Release notes](https://github.com/jsakamoto/BlazorWasmBrotliLoader.Build/blob/master/RELEASE-NOTES.txt)

## 📢License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/BlazorWasmBrotliLoader.Build/blob/master/LICENSE)
