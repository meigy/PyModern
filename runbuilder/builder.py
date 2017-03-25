import sys
import os
import thread
import clr
import wpf
from modulebase import xmlobject, xdist
from modulebuilder import *

clr.AddReference("System.Xml")
from System.Xml import XmlDocument



class configloader():
    def __init__(self):
        #configs(string, buildconfig)
        self.configs = {}
        #configs(string, buildprojects)
        self.projects = {}

    def load(self):
        #do init
        self.loadConfigs()
        self.loadProjects()

    #lookfor builder.*.xml => self.configs{configfile, buildconfig(configfile)}
    def loadConfigs(self):
        for root, dirs, files in os.walk(os.getcwd(), topdown = True):
            for f in files:
                fname = os.path.splitext(f)
                if len(fname) > 1 and fname[1].lower() == '.xml' and fname[0][:7].lower() == 'builder':
                    configfile = (os.path.join(root, f))
                    config = buildconfig(configfile)
                    #config.fromfile(configfile)
                    self.configs[configfile] = config
        return
    
    #load builder.*.xml => self.builders{configfile, buildprojects(configfile)}
    def loadProjects(self):
        for configfile, buildconfig in self.configs.items():
            buildconfig.loadconfig()
            self.projects[configfile] = buildconfig.projectgroups
        return

    def build(self, configfile, option):
        buildconfig = self.configs[configfile]
        buildconfig.buildprojects(option)

#-------------------------------------------------------------------

