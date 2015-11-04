[![Build status](https://ci.appveyor.com/api/projects/status/0qcjrsefl9g9b9xn?svg=true)](https://ci.appveyor.com/project/beefarino/seeshell)

[![Module Zip](https://ci.appveyor.com/app/images/artifact-zip.png)Download Module Zip](https://ci.appveyor.com/project/beefarino/seeshell/build/artifacts)

# SeeShell

SeeShell is a simple data visualization module for PowerShell that currently supports Charts.

SeeShell brings data visualization capabilities to your PowerShell and ISE sessions.  SeeShell visualizations can monitor data sources asynchronously, allowing you to perform other tasks in your PowerShell session.

SeeShell supports the following features:

## Graphing and Charting

SeeShell supports over two dozen types of charts.  These include linear and radial plots capable of displaying relationships between quantitative and qualitiative data.  In addition, a single chart can display multiple    independent sources of data, sharing axes where appropriate.
##  Data Sources

Use SeeShell data sources to define and describe your data.  Describe the properties of each data item, including scale information, key data ranges to reflect in the visualizations, and visualization colors.

# USING SEESHELL

## Prerequisites

SeeShell requires the following to operate:
* PowerShell 3.0 or higher
* .NET Framework 4.5

## Loading SeeShell

SeeShell is distributed as a module.  Before you can use SeeShell cmdlets, you need to import the SeeShell module into your session using the import-module cmdlet:

      import-module seeshell


## GETTING STARTED

Once the module is loaded, the list of cmdlets available in SeeShell can be found using:

      get-command -module seeshell

For help on a specific SeeShell cmdlet, use the standard get-help PowerShell cmdlet:

      get-help out-chart

## QUICK EXAMPLE

1. Open a PowerShell console.
2. Import the SeeShell module by following the instructions in USING SEESHELL.
3. In the PowerShell console, type the following:

      dir | out-chart -name Files -type Column -plot Length -by Name

This will plot the size of each file (in bytes) by the file name for all files in the current directory.
