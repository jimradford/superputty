# User Guide #


## Introduction to SuperPuTTY ##
SuperPutty is a Windows application used primarily as a window manager for the PuTTY SSH Client. It allows you to embed PuTTY terminal instances inside of a windows form providing a better tabbed interface when multiple connections are used. Additionally SuperPutty has support for using pscp.exe to transfer files to and from a remote host. Local terminal sessions can be started with MinTTY.

SuperPutty does not do any ssh or terminal management itself since PuTTY does an excellent job of this.

### License ###
Licensed under a liberal MIT/X11 License, which allows this program and source code to be used in both commercial and non-commercial applications. Complete text can be found in the [License.txt](https://code.google.com/p/superputty/source/browse/trunk/License.txt) file inside the download.

### System Requirements ###
  * A PC running Windows (XP, Vista, Windows 7, Windows 8)
  * The Microsoft .NET Framework 3.5 or newer
  * 32 and 64 bit operating systems are supported
  * The [PuTTY](http://www.chiark.greenend.org.uk/~sgtatham/putty/) SSH Client
### Quick Start ###
#### Download SuperPutty ####
Visit the [Downloads](Downloads.md) page to download the latest stable version of SuperPutty.

Source code can be downloaded from our [Subversion repository](https://code.google.com/p/superputty/source/browse/)
More information on the source code and requirements can be found [in our Developers document](Developers.md)
#### Install SuperPutty ####
SuperPutty is currently packaged as a ZIP file. Simply unzip the files to a chosen location on your local disk.  It is recommended to store it in a directory which maintains the version number of SuperPuTTY.  For example, the directory could be named  "C:\SuperPuTTY\SuperPutty-v1.4.0.5".

Depending on the version, the newly extracted files will look something:
```
License.txt       
SuperPutty.exe         
README.txt        
SuperPutty.exe.config  
WeifenLuo.WinFormsUI.Docking.dll
log4net.dll
ReleaseNotes.txt  
SuperPutty.pdb         
themes
```
## Getting Started ##
## SuperPutty Interface ##
### Menus and Settings ###
  * **File Menu**
    * **Import Sessions** This allows you to import session configuration information from putty, puttyCM, older versions of SuperPutty or files exported by current versions of SuperPutty
    * **Export Sessions** Export all sessions currently configured to an xml file for archival or for importing to another machine running SuperPutty
    * **Open Session** Opens a dialog box with a list of your sessions, double clicking on any session will star that session.
    * **Switch Session** Switch focus to another running sessions, can also be configured as a hot-key for switching in Tools -> Options
    * **Edit Sessions in Notepad** Launches notepad with the raw xml session date for manual updates.
    * **Reload Sessions** Reloads the session database
    * **Save Layout** Saves the current window states and open sessions to the currently running Layout configuration
    * **Save Layout As** Saves the current window states and open sessions with a new name

  * **View Menu**
    * **Sessions** Opens the Sessions List Panel
    * **Layouts** Opens the Layouts List Panel
    * **Log Viewer** Opens the Log Viewer
    * **Toolbars**
      * **Quick Connection** Show or Hide the Quick Connection Toolbar
      * **Send Commands** Show or Hide the Send Commands Toolbar
    * **Status Bar** Show or Hide the Status Bar at the bottom of the window
    * **Menu Bar** `[F3]` Show or Hide the Menu Bar
    * **Always On Top** Forces application to stay in the foreground
    * **Fullscreen** `[F11]` Hides all toolbars, panels giving the maximum amount of screen space to any running sessions

  * **Tools Menu**
    * **PuTTY Configuration** Opens the putty configuration dialog allowing you to configure putty settings
    * **Toggle Command Mask** Enables masking characters with a `*` when input is entered into the Send Commands Toolbar
    * **Options** `[F2]` Open the SuperPutty configuration dialog which allows you to configure the behavior of the application
      * **General Tab** General Settings and external file locations
        * putty.exe location (required): The location on the local disk where putty.exe can be found. Alternatively this could be the location of the kitty ssh application
        * pscp.exe location: The location of the pscp.exe program used to transfer files to and from remote hosts
        * mintty.exe location: The location on the local filesystem of the mintty.exe application
        * Settings Folder (required): The local location you wish to store session and layout databases. Also contains the themes folder for customizing session icons.
        * Default Layout: The layout to load when the application starts which will restore any sessions and tab locations
      * **GUI Tab** Graphical user interface specific settings are configured here. Holding your mouse over any field will open a tooltip describing the settings purpose.
      * **Shortcuts Tab** Here you can define shortcut keys to execute specific functions in SuperPutty
      * **Advanced Tab** Control advance application settings
  * **Help Menu**
    * **Documentation** Opens the local documentation if installed, otherwise prompts you to load the online documentation (this)
    * **Diagnostics** Allows you to configure log window location
    * **Check for updates** Check for a newer version of the SuperPutty application
    * **About SuperPutty** Brings up a dialog box with information on the SuperPutty application
### Hotkeys ###
#### Configuration Files ####
### Windows and Toolbars ###
#### Sessions Toolwindow ####
#### Command Bar ####
#### Layout Toolwindow ####
#### Log Viewer ####
### Command Line Arguments ###
SuperPutty can be started and controlled from the command line. This allows SuperPutty to be started from other applications. Starting _SuperPutty.exe --help_ from a command prompt will open a dialog showing valid command line arguments
### Command Bar ###
The command bar is used to send commands to open sessions. You can also mask any input in this box by enabling Toggle Command Mask `[CTRL+SHIFT+F8]` All keystrokes are passed through. There is a known bug ([Issue 339](https://code.google.com/p/superputty/issues/detail?id=339)) which does not pass the enter key when sent by itself
### File Transfers ###
### Layouts ###
## Scripting ##
A scripting language to automate tasks such as logging in is scheduled to be added to a future release.
### Script Reference ###
### Script Examples ###
## Known Issues ##
  * Conflict with TeamViewer's QuickConnect Feature ([Issue 309](https://code.google.com/p/superputty/issues/detail?id=309))
> > "This issue is being caused by TeamViewer's QuickConnect feature that adds an additional button to every windows toolbar for easy sharing. Adding putty.exe to exception list under the advanced settings solves the problem for me." ([phil.pav](https://code.google.com/p/superputty/issues/detail?id=309#c5))

  * Click the Issues tab for other bug reports from users and developers

## External Articles ##
  * A great guide on getting SuperPutty setup.  http://automation-nation.org/superputty-tutorial

## Known Forks ##
  * https://github.com/phendryx/superputty
  * https://github.com/akngo/superputty
  * https://github.com/revo22/SuperPutty