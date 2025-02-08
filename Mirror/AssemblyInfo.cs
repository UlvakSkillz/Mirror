using MelonLoader;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Mirror;
using static System.Net.Mime.MediaTypeNames;
// ...
[assembly: MelonInfo(typeof(MirrorClass), Mirror.BuildInfo.ModName, Mirror.BuildInfo.ModVersion, Mirror.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 195, 0, 255)]
[assembly: MelonAuthorColor(255, 195, 0, 255)]
[assembly: VerifyLoaderVersion(0, 6, 6, true)]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyDescription(Mirror.BuildInfo.Description)]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3b03f008-9dbb-4382-a110-b59ee8b632ca")]
