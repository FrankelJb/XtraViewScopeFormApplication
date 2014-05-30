# XtraView Scope Setup

There are three things that need to be installed to use the XtraView Scope application.

1. [.Net 4.5.1](http://www.microsoft.com/en-za/download/details.aspx?id=40779)
2. Ni-Scope 4.1 from the DVD. The 2GB installer can be downloaded [here](http://www.ni.com/download/ni-scope-4.1/4078/en/) if the DVD is unavailable.
3. [Download](https://bitbucket.org/deqcptyltd/xtraviewscope/downloads/XtraViewScopeApplication.zip) and extract the archive. Run the setup.exe and the application should launch automatically.

## Using the Application

The application performs two main functions. Firstly, it processes heartbeat signal and logs the data to file while keeping track of the shortest, longest and average duration between heartbeats. Secondly, it captures buttons pressed on IR remotes and outputs the result to the text box titled "Key Presses".

To configure the scope for use, click the text box labeled "Config File Path" and select a configuration file. There is a default configuration file located in the "Resources" directory in the installation directory.

By default, the application saves heartbeat files to C:\waveform. This can be changed by clicking the "Output Directory" text box. The heartbeats will always be saved into a sub-directory called "heartbeats".

By default, the heartbeat files are saved with the format "<Device Name>\_<channel number>\_<date\_time>.<format>". The device name, channel number and format are configurable using the config file.

Remote button presses are mapped using a mapping file in the "Resources" directory called "irKeyMapping.properties". To train a new remote, the user can press the button, copy the hex output to the mapping file and equate that value to the name of the button pressed. There are some values in the file by default so the user can copy the format. If there are multiple hex values that equate to the same button (the same button on different remote types), then the user can use multiple hex keys piped (|) together with the same button value.

Then click "Start" and the scope will start to acquire all signal available. All heartbeats will be saved and all key presses will be outputted to the text box. The heartbeats will increment a counter on the start button as a visual cue that there has been activity. Press the "Stop" button to stop acquiring signal.

The application logs are stored in the installation directory in the "log" directory. The logs should be used to determine if there are heartbeats with malformed data.

It is possible to graph the heartbeats, but first install the following:

1. [Python 2.7](https://www.python.org/download/releases/2.7.6)
2. [wxPython](http://www.wxpython.org/download.php#msw). Please use the correct version based on the the version of Windows that is being used (either 32 or 64 bit), and ending in *27* (for Python 2.7)

Once the Python environment has finished installing, click the "Graph Heartbeats" button. This will open a file open dialog. Navigate to where the heartbeats have been saved and select the files to be graphed. Multiple files can be selected. During configuration, the application must be configured to store the heartbeat data in JSON format. By default, the application will save the data in both XML and JSON formats.

There is a considerable amount of data to be displayed when graphing heartbeats and it is possible to zoom in and move each XMP packet's data. In the Figure windows, click the plus-arrows on the bottom of the screen (if this is isn't visible, maximise the window). Right-click, hold and move the mouse to zoom in or out. Left-click, hold and move the mouse to move the graph.

**Notes**

* The application makes no registry changes and is only installed for the current user. The installation directory can be located by clicking "Open Log Directory" and navigating one directory up.

* The scope that is used cannot distinguish what signal has been transmitted along the wire. This means that there can be a corruption of heartbeat and remote button presses if they are interleaved which can affect the displayed timing.

* If a button is long-pressed, the signal is sent multiple times. If the application encounters data that does not look like a button press or a heartbeat then it will be discarded. Keep this in mind when testing remotes as long-presses will affect the Key Presses output and the heartbeat.

* When stopping the application, the scope can cause the computer to hang while waiting for a signal. If the application is stopping, press a button on a remote to allow the scope to acquire signal and release the application. If that does not work, disconnect the scope's USB from the computer and it should release the application.
