global using Sandbox;
global using Sandbox.Diagnostics;
global using Sandbox.Physics;
global using Sandbox.Tasks;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using static Sandbox.Internal.GlobalSystemNamespace;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo( "Sandbox.Test.Unit" )]
[assembly: InternalsVisibleTo( "Sandbox.Test.Engine" )]
[assembly: InternalsVisibleTo( "Sandbox.Test.Integration" )]
[assembly: InternalsVisibleTo( "Sandbox.GameInstance" )]
[assembly: InternalsVisibleTo( "Sandbox.Tools" )]
[assembly: InternalsVisibleTo( "Sandbox.Menu" )]
[assembly: InternalsVisibleTo( "Sandbox.AppSystem" )]
[assembly: InternalsVisibleTo( "MenuBuild" )]
[assembly: InternalsVisibleTo( "ShaderCompiler" )]
[assembly: InternalsVisibleTo( "Sandbox.Mounting.Test" )]
[assembly: InternalsVisibleTo( "pyro-launcher" )]
[assembly: InternalsVisibleTo( "panelgallery" )]
[assembly: InternalsVisibleTo( "pyro-server" )]
[assembly: InternalsVisibleTo( "pyro-dev" )]
[assembly: InternalsVisibleTo( "pyro" )]
[assembly: InternalsVisibleTo( "sbox-standalone" )]
[assembly: InternalsVisibleTo( "sbox-profiler" )]
[assembly: InternalsVisibleTo( "pyro-benchmark" )]
[assembly: InternalsVisibleTo( "CreateGameCache" )]

[assembly: TasksPersistOnContextReset]

// Moved filesystem to its own assembly
[assembly: TypeForwardedTo( typeof( BaseFileSystem ) )]
[assembly: TypeForwardedTo( typeof( FileWatch ) )]
