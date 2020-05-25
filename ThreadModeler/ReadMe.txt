================================================
         Plugin of the Month, May 2011
================================================
------------------------
Inventor ThreadModeler
------------------------

Description
-----------
The ThreadModeler tool has been developed to allow Inventor users
to more easily create realistic modelized threads based on an 
existing Inventor thread feature.

Through functionality provided by the tool, users are able to select
an existing thread feature in the model and the ThreadModeler will
then generate a realistic thread based on the feature properties.

The ThreadModeler can reliably generate a thread which can save a 
lot of time and effort for the user when compared with a manual 
approach, and bring to your models a higher degree of realism.

System Requirements
-------------------
Due to the functionalities of the API used by the plugin the 
minimal Inventor version supported is 2011.

An installer package which works for both 32-bit and 64-bit
systems is provided.

The source code has been provided as a Visual Studio 2008 project
containing C# code (not required to run the plugin).

Installation
------------
Run the ThreadModeler.msi installer. Follow the instructions
provided by the installer: you will only need to select the
installation folder for the add-in. There is no particular
restriction concerning the location of this installation folder.

On 32-bit platforms, building the source project inside Visual Studio
should cause the plugin to be registered, providing the application
has been "enabled for COM Interop" via the project settings.

On 64-bit platforms, the plugin cannot be registered by Visual Studio
2008 because it is a 32-bit application. Registration must either be
performed via the installer or a batch file/command prompt.

Using RegAsm.exe /codebase "addin_path"

Usage
-----
Once loaded, an additional panel containing the ThreadModeler
command should be available in the Part Tools Tab of the Ribbon bar. 
If the version of Inventor you are using does not provide a 
Ribbon interface, you should find an equivalent menu item
under the Tools menu.
 
When you run the ThreadModeler command a new modeless form should be
displayed, allowing you to select which existing thread features you
want to modelize.

Uninstallation
--------------
You can unload the plugin without uninstalling it by unchecking the
"Load" checkbox associated with the plugin in the Inventor Add-In
dialog.

Unchecking "Load on Startup" cause the plugin not to be loaded in
future sessions of Inventor.

To remove the plugin completely, uninstall the application via your
system's Control Panel.

Known Issues
------------

1#

The installer - while working on both 32- and 64-bit systems - was
developed using 32-bit technology and therefore suggests an
inappropriate default installation location on 64-bit systems:

ie. "C:\Program Files (x86)" instaed of "C:\Program Files".

Although this will still working, it's adviseable to install the
application into the 64-bit section of your file system.

2# 

The minimal thread pitch that the plug-in allows you 
to modelize is 0.0007 in (or 0.01778 mm).
Keep in mind that around this scale the quality of the 
modelization will look very poor in Inventor.


Author 
------
This plugin was written by Philippe Leefsma.

Further Reading
---------------
The "ThreadModeler Addin - Quick Start.docx" guide in the "doc"
folder provides further details about the use of this plugin.

Feedback
--------
Email us at support@coolorange.com with feedback or requests for
enhancements.

Release History
---------------

1.0 Original release

1.1 Fixed issue for handling small thread sizes.
    Fixed issue concerning inverted right/left handed thread


(C) Copyright 2011 by coolOrange s.r.l 