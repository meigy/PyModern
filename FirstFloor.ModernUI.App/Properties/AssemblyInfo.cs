using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ModernUI Demo App")]
[assembly: AssemblyDescription("Demonstrating the features of Modern UI for WPF")]
[assembly: AssemblyConfiguration("retail")]
[assembly: AssemblyCompany("First Floor Software")]
[assembly: AssemblyProduct("ModernUI demo")]
[assembly: AssemblyCopyright("Copyright © First Floor Software 2013-2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f267fe5a-70ee-4907-8d06-156e4c988e7d")]



[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://firstfloorsoftware.com/ModernUIApp", "FirstFloor.ModernUI.App")]
[assembly: XmlnsPrefix("http://firstfloorsoftware.com/ModernUIApp", "muiapp")]
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
