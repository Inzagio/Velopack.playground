
# What is this thing even?
For a company, I did some initial experimentation on how to deploy internal development tools in an easy manner.
This sample application takes the standard Weatherforecast API and packs it using Velopack. Think of Velopack like Squirrel.Windows and Click-Once (that actually works).  
This is super great for deploying many types of applications, including bogstandard CLI applications. 

# What tools are used to do this
- [Velopack](https://velopack.io)
- [Nerdbank.Gitversioning (through nuke)](https://github.com/dotnet/Nerdbank.GitVersioning/tree/main)
- [Nuke Build](https://nuke.build)
- [CliWrap](https://github.com/Tyrrrz/CliWrap) (Used for calling the underlying `vpk` tool from Velopack)

Bear in mind, that in this developer setup all the tools were installed as global tools and not utilizing the .NET tools manifest, which is something that could be used to have the dependencies local to the solution.

# How does it work?

The way this is setup, we simply have to step the version in the `version.json` file, run `nuke velopack --configuration Release` and we'll have the application build, packed and published with a setup executable in the `Releases` folder. 
