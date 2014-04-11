#!/usr/bin/python

import wx
import simplejson as json
from pprint import pprint
import matplotlib.pyplot as pyplot
from numpy import *
import sys

class Vector(object):
    duration = 0
    amplitude = 0
    
    def __init__(self, duration, amplitude):
        self.amplitude = amplitude
        self.duration = duration
            
class NibbleTiming(object):
    pulse = None
    noise = None
    optimal = 0
    deviation = None
    decimal = 0 #TODO: Remove this
    
    def __init__(self, pulse_duration, noise_duration, optimal, deviation, decimal):
        self.pulse = Vector(pulse_duration, 1)
        self.noise = Vector(noise_duration, 0)
        self.optimal = optimal
        self.deviation = deviation
        self.decimal = decimal
        
    def __str__(self):
        return 'pulse start: ' + self.pulse_start + '\npulse end: ' + self.pulse_end + '\nnoise start: ' + self.noise_start + '\nnoise end: ' + self.noise_end
                
    def timing_list(self):
        return [self.pulse_start.time, self.pulse_end.time, self.noise_start.time, self.noise_end.time]
    
    def amplitude_list(self):
        return [self.pulse_start.amplitude, self.pulse_end.amplitude, self.noise_start.amplitude, self.noise_end.amplitude]
    
    def as_list_of_lists(self):
        return [[self.pulse_start.time, self.pulse_end.time, self.noise_start.time, self.noise_end.time],
                [self.pulse_start.amplitude, self.pulse_end.amplitude, self.noise_start.amplitude, self.noise_end.amplitude]]

class Add2Packet(object):
    nibble_timing_list = None
    
    def __init__(self):
        self.nibble_timing_list = []
    
class Heartbeat(object):
    add2packet_list = None
    filename = ''
    
    def __init__(self, filename):
        self.add2packet_list = []
        self.filename = filename
    


#---------------------------------------------------------------

def get_paths(wildcard):
    app = wx.App(None)
    style = wx.FD_MULTIPLE | wx.FD_FILE_MUST_EXIST
    dialog = wx.FileDialog(None, 'Open', wildcard=wildcard, style=style)
    if dialog.ShowModal() == wx.ID_OK:
        paths = dialog.GetPaths()
    else:
        paths = None
    dialog.Destroy()
    return paths

def open_files(filenames):
    heartbeats = []
    for name in filenames:
        file = open(name, 'r')
        text = file.read()
        heartbeats.append(CreateHeartbeat(text, name))
    return heartbeats

#----------------------------------------------------------------


class FileDrop(wx.FileDropTarget):
    def __init__(self, window):
        wx.FileDropTarget.__init__(self)
        self.window = window
        
    def OnDropFiles(self, x, y, filenames):
        heartbeats = []
        for name in filenames:
            try:
                if not 'json' in name:
                    raise IOError
                file = open(name, 'r')
                text = file.read()
                heartbeats.append(CreateHeartbeat(text, name))
                #self.window.WriteText(text)
                file.close()
            except IOError, error:
                dlg = wx.MessageDialog(None, 'Error opening file\n' + str(error))
                dlg.ShowModal()
            except UnicodeDecodeError, error:
                dlg = wx.MessageDialog(None, 'Cannot open non ascii files\n' + str(error))
                dlg.ShowModal()
                
        plot_data(heartbeats)

        #t = threading.Thread(target=plot_data,args=("plot thread", heartbeats))
        #t.daemon = True
        #print('daemon started')
        #t.start()
        
        #thread.start_new_thread(plot_data, ("plot thread", heartbeats))
        
        #p = Process(target=plot_data, args=("plot thread", heartbeats))
        #p.start()

class DropFile(wx.Frame):
    def __init__(self, parent, id, title):
        wx.Frame.__init__(self, parent, id, title, size = (450, 400))

        self.text = wx.TextCtrl(self, -1, style = wx.TE_MULTILINE)
        dt = FileDrop(self.text)
        self.text.SetDropTarget(dt)
        self.Centre()
        self.Show(True)

def CreateHeartbeat(text, filename):
    json_data = json.loads(text)
    heartbeat = Heartbeat(filename)

    for add2packet in json_data['XmpPacketTransmission']['IrPackets']:
        newAdd2Packet = Add2Packet()
        for nibble in add2packet['InformationUnits']:
            timing = NibbleTiming(nibble['PulseDuration'],
                                  nibble['NoiseDuration'],
                                  nibble['OptimalDuration'],
                                  nibble['TimeDeviation'],
                                  nibble['DecimalValue']
                                  )
            newAdd2Packet.nibble_timing_list.append(timing)
        heartbeat.add2packet_list.append(newAdd2Packet)
    
    
    return heartbeat

def plot_data(heartbeats):
    all_timing_lists = []
    all_amplitude_lists = []
    y_offset = 0
    
    lines = []
    legend_labels = []
    
    fig = pyplot.figure()
    for heartbeat in heartbeats:
        subplot_number = 911
        xmp_packet_number = 0
        for add2packet in heartbeat.add2packet_list:
            current_position = 0
            timing_list = [-1000, 0]
            amplitude_list = [y_offset, y_offset]
            
            ax = pyplot.subplot(subplot_number)
            #ax = pyplot.subplot(subplot_number)
            pyplot.axis([-100, 20000 , -0.375 + (- 0.375 * y_offset), 1.5 + y_offset])
            #pyplot.plot([current_position, current_position],
            #        [-3.5, 3.5], 'k-', lw=2)
            for nibble in add2packet.nibble_timing_list:
                line_style = 'k-'
                for key, value in nibble.deviation.iteritems():
                    if str(key) == 'Perfect':
                        line_style = 'g-'
                    elif str(key) == 'Good':
                        line_style = 'b-'
                    elif str(key) == 'Warn':
                        line_style = 'y-'
                    elif str(key) == 'Error':
                        line_style = 'r-'
                pyplot.plot([current_position + nibble.optimal, current_position + nibble.optimal],
                    [-3.5, 3.5], line_style)
                timing_list.append(current_position)    
                timing_list.append(current_position + nibble.pulse.duration)
                timing_list.append(current_position + nibble.pulse.duration)
                timing_list.append(current_position + nibble.pulse.duration + nibble.noise.duration)
                
                #ax.annotate(str(nibble.pulse.duration + nibble.noise.duration),
                #                xy=(current_position + (nibble.pulse.duration + nibble.noise.duration) / 2, y_offset),
                #                xytext=(current_position + (nibble.pulse.duration + nibble.noise.duration) / 2, 0.25 + y_offset),
                #            )
                #ax.annotate(str(nibble.decimal),
                #                xy=(current_position + (nibble.pulse.duration + nibble.noise.duration) / 2, y_offset),
                #                xytext=(current_position + (nibble.pulse.duration + nibble.noise.duration) / 2, 0.5 + y_offset),
                #            )
                
                current_position += nibble.pulse.duration + nibble.noise.duration
                
                amplitude_list.append(1 + y_offset)
                amplitude_list.append(1 + y_offset)
                amplitude_list.append(0 + y_offset)
                amplitude_list.append(0 + y_offset)
                
                            
            plot1, = pyplot.plot(timing_list, amplitude_list)
            
            pyplot.ylabel('Seq ' + str(xmp_packet_number))
            pyplot.tick_params(\
                axis='both',
                which='both',
                left='off',      # ticks along the bottom edge are off
                right='off',         # ticks along the top edge are off
                labelleft='off')
            xmp_packet_number += 1

            #ax.plot(timing_list, amplitude_list, label = heartbeat.filename)
            #handles, labels = ax.get_legend_handles_labels()
            #lgd = ax.legend(handles, labels, loc='upper center', bbox_to_anchor=(0.5, -0.1))
            if subplot_number % 911 == 0:
                lines.append(plot1)
            subplot_number += 1
        legend_labels.append(heartbeat.filename,)
        y_offset += 0.125

    #
    #lgd = fig.legend(lines, legend_labels, bbox_to_anchor=[1, 1], 
    #       loc='lower center', ncol=2)
    lgd = fig.legend(lines, legend_labels, ncol=2)
    
    def onclick(event):
        if event.button == 1:
            vis = lgd.get_visible()
            if vis:
                lgd.set_visible(False)
    
            else:
                lgd.set_visible(True)
            
            pyplot.draw()
            
    cid = fig.canvas.mpl_connect('button_press_event', onclick)
    
    
    mng = pyplot.get_current_fig_manager()
    mng.window.wm_geometry("+0+0")
    mng.resize(*mng.window.maxsize())
    
    pyplot.draw()
    pyplot.show()
    pyplot.close('all')

        
#app = wx.App()

#DropFile(None, -1, 'Drop Json')
#app.MainLoop()

while(True):
    paths = get_paths('*.json')
    
    if paths != None:
        plot_data(open_files(paths))
    else:
        sys.exit(0)
