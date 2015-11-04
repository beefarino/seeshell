# Copyright (c) 2012 Code Owls LLC
#
# psake build script for the SeeShell project
#
# valid configurations:
#  Debug
#  Release
#
# notes:
#

properties {
	$config = 'Debug';
	$local = './_local';
	$slnFile = @(
		'./CodeOwls.SeeShell.sln'
	) | Resolve-Path | select -ExpandProperty path;
	$libPath = "./lib"
    $targetPath = "./CodeOwls.SeeShell/CodeOwls.SeeShell.Providers.Tests/bin";
	$moduleSource = "./Modules";
  $metadataAssembly = 'CodeOwls.SeeShell.PowerShell.Providers.dll';
	$versionMetadataFile = '.\CodeOwls.PowerShell\CodeOwls.SeeShell.AssemblyInfo.cs';
	$buildNumberFile = "./buildNumber.txt";
	$major = 2;
	$minor = 0;
};


$framework = '4.0'

function get-packageDirectory
{
	return "." | resolve-path | join-path -child "/bin/$config";
}

function get-nugetPackageDirectory
{
    return "." | resolve-path | join-path -child "/bin/$config/NuGet";
}

function get-modulePackageDirectory
{
    return "." | resolve-path | join-path -child "/bin/$config/Modules";
}

function get-zipPackageName
{
	"SeeShell.$(get-ProviderVersion).zip"
}

function create-PackageDirectory( [Parameter(ValueFromPipeline=$true)]$packageDirectory )
{
    process
    {
        write-verbose "checking for package path $packageDirectory ..."
        if( !(Test-Path $packageDirectory ) )
    	{
    		Write-Verbose "creating package directory at $packageDirectory ...";
    		mkdir $packageDirectory | Out-Null;
    	}
    }
}

function get-ProviderVersion
{
	$p = get-modulePackageDirectory;
    $md = join-path $p "SeeShell\bin\$metadataAssembly";
	Write-Verbose "reading metadata from assembly at path $md";
    ( get-item $md | select -exp versioninfo | select -exp productversion );
}

function update-versionMetadata
{
	$bld = 0;
	$rev = 0;
	$vn = "$major.$minor.$bld.$rev";
@"
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyProduct("SeeShell")]

[assembly: AssemblyVersion("$vn")]
[assembly: AssemblyFileVersion("$vn")]
"@ | out-file $versionMetadataFile;
}


function assemble-moduleDocumentation
{
	$helpPath = get-modulePackageDirectory | Join-Path -ChildPath "SeeShell\en-US";
	$cmdletTopicPath = $helppath | join-path -child 'cmdlets';
	$outputHelpPath = $helpPath | Join-Path -ChildPath "CodeOwls.SeeShell.PowerShell.Cmdlets.dll-help.xml";

	$help = New-Object xml;
	$help.Load( $outputHelpPath );
	$helpNode = $help.selectSingleNode( '//helpItems' );

	ls $cmdletTopicPath *.help.xml | %{
        write-host $_;
		$node = [xml] ($_ | gc);
		$node = $help.importNode( $node.command, $true );
		$helpNode.AppendChild( $node ) | Out-Null;
	};

	ri $cmdletTopicPath -Force -Recurse;
	$help.Save( $outputHelpPath );
}

task default -depends Test;

. ./psake/commontask.ps1

task IncrementBuildNumber -description "updates version metadata" {
	update-versionMetadata;
}

task Test -depends Build,TestLocal -description "executes functional tests";

task TestLocal -depends Build -description "executes local functional tests" {
	Write-Verbose "running tests ...";

	# we run tests in a nested powershell session so binary modules will be unloaded from memory
	# 	after the tests are complete.8
	#	this will allow us to rerun the build from the same console.
	$xmlfile =  join-path $local 'localtests.xml';


	powershell -command """$xunit"" ""$xunitProject"""

}
# package tasks

task PackageModule -depends CleanModule,Build,__CreateModulePackageDirectory -description "assembles module distribution file hive" -action {
	$mp = get-modulePackageDirectory;

	# copy module src hive to distribution hive
	Copy-Item $moduleSource -container -recurse -Destination $mp -Force;

	# copy bins to module bin area
	mkdir "$mp\SeeShell\bin" -force | Out-Null;
	ls "$targetPath/$config" | where { $_ -notmatch '.license$' } | copy-item -dest "$mp\SeeShell\bin" -recurse -force;

    assemble-moduledocumentation;
}

task PackageZip -depends PackageModule -description "assembles zip archive of module distribution" -action {
	$mp = get-modulePackageDirectory | Get-Item;
	$pp = get-packageDirectory;
	$zp = get-zipPackageName ;

	Import-Module pscx;

	Write-Verbose "module package path: $mp";
	Write-Verbose "package path: $pp";
	Write-Verbose "zip package name: $zp";

	write-zip -path "$mp\SeeShell" -outputpath $zp | Move-Item -Destination $pp -Force ;
}

# install tasks

task Uninstall -description "uninstalls the module from the local user module repository" {
	$modulePath = $Env:PSModulePath -split ';' | select -First 1 | Join-Path -ChildPath 'SeeShell';
	if( Test-Path $modulePath )
	{
		Write-Verbose "uninstalling module from local module repository at $modulePath";

		$modulePath | ri -Recurse -force;
	}
}

task Install -depends InstallModule -description "installs SeeShell to the local machine";

task InstallModule -depends PackageModule -description "installs the module to the local user module repository" {
	$packagePath = get-modulePackageDirectory;

	$modulePath = $Env:PSModulePath -split ';' | select -First 1;
	Write-Verbose "installing module from local module repository at $modulePath";

	ls $packagePath | Copy-Item -recurse -Destination $modulePath -Force;
	#ls $lp | Copy-Item -recurse -Destination $modulePath -Force;
}
