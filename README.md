# Sense/Net ECM Installer
This is a Windows desktop application for installing [Sense/Net ECM](https://github.com/SenseNet/sensenet) from scratch, or executing upgrade packages. Please visit the release link below for the easiest way to get the installer - or you can check out this repo and build it yourself.

[![Join the chat at https://gitter.im/SenseNet/sn-installer](https://badges.gitter.im/SenseNet/sn-installer.svg)](https://gitter.im/SenseNet/sn-installer?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![GitHub release](https://img.shields.io/github/release/sensenet/sn-installer.png)](https://github.com/SenseNet/sn-installer/releases)

## Prerequisites

### Environment

Before starting the installer, please make sure you have **Internet Information Services (IIS)** and **ASP.Net** installed on the machine and you also have a database server (Microsoft **SQL Server** or SQL Server Express) where you want to install the SenseN/Net Content Repository.

### Install package

The installer is just a tool, you will need a complete Sense/Net ECM **install package** to execute. 

The package is actually an [SnAdmin package](https://github.com/SenseNet/sn-admin), a zip file that you will be able to download separately (Community users: from the main [Sense/Net ECM](https://github.com/SenseNet/sensenet) repository) when it is available.

## Installation

The installer will guide you through the short process of creating a website and choosing a web folder and database for your Sense/Net ECM instance.

![SnInstaller create website](http://wiki.sensenet.com/images/1/1c/Sninstaller-iis.PNG "SnInstaller create website")

The install process will take some time but there will be no user interactions, it is automatic.

### Errors
If the install process fails, you can always check the log file for details - the installer will provide you a link to the log folder at the end of the process.
