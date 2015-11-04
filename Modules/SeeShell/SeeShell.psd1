#
#   Copyright (c) 2011 Code Owls LLC, All Rights Reserved.
#
#   Unless required by applicable law or agreed to in writing, software
#   distributed under the License is distributed on an "AS IS" BASIS,
#   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#   See the License for the specific language governing permissions and
#   limitations under the License.
#
#	SeeShell module definition file
#

@{

# Script module or binary module file associated with this manifest
ModuleToProcess = 'bin/codeowls.seeshell.powershell.providers.dll'

# Version number of this module.
ModuleVersion = '2.1'

# ID used to uniquely identify this module
GUID = '969cf279ca7644b59d985f127e0d2810'

# Author of this module
Author = 'Jim Christopher'

# Company or vendor of this module
CompanyName = 'Code Owls LLC'

# Copyright statement for this module
Copyright = 'Copyright (c) 2012 Code Owls LLC, All Rights Reserved'

# Description of the functionality provided by this module
Description = 'Data visualization for PowerShell.'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '4.0'

# Name of the Windows PowerShell host required by this module
PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
PowerShellHostVersion = ''

# Minimum version of the .NET Framework required by this module
DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = ''

# Processor architecture (None, X86, Amd64, IA64) required by this module
ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module
ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
FormatsToProcess = @()

# Modules to import as nested modules of the module specified in ModuleToProcess
NestedModules = @(
  'bin/Caliburn.Micro.dll'
  'bin/Caliburn.Micro.Platform.dll'
  'bin/CodeOwls.PowerShell.Paths.dll'
  'bin/CodeOwls.PowerShell.Provider.dll'
  'bin/CodeOwls.SeeShell.Common.dll'
  'bin/CodeOwls.SeeShell.PowerShell.Cmdlets.dll'
  'bin/CodeOwls.SeeShell.PowerShell.Providers.dll'
  'bin/CodeOwls.SeeShell.Providers.Tests.dll'
  'bin/CodeOwls.SeeShell.Visualizations.dll'
  'bin/Ionic.Zip.dll'
  'bin/log4net.dll'
  'bin/OxyPlot.dll'
  'bin/OxyPlot.Wpf.dll'
)

# Functions to export from this module
FunctionsToExport = '*'

# Cmdlets to export from this module
CmdletsToExport = '*'

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# List of all modules packaged with this module
ModuleList = @()

# List of all files packaged with this module
FileList = @()

# Private data to pass to the module specified in ModuleToProcess
PrivateData = ''

}
