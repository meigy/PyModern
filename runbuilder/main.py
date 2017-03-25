import wpf
import clr
import os

clr.AddReference("System.Drawing")
clr.AddReference("System.Xml")

from System.Windows import Application, Window
from System import Uri, UriKind

import ModernUI
from ModernUI import *
from FirstFloor.ModernUI.App import CTUtils

from CSScript import loadcs

from Pages.Introduction import Introduction
from Pages.Options import Options
from Pages.Logs import Logs
from Pages.Builder import Builder
from Content.BuildLog import BuildLog
from Content.Console import Console

from modulebase import globalspace
from builder import configloader

#buildloader = configloader()
#load projects
#buildloader.load()

class windowbuilder(CTModernWindow):
    def __init__(self):
        wpf.LoadComponent(self, 'winMain.xaml')
        self.WindowStartupLocation = WindowStartupLocation.CenterScreen
        self.Loaded += self.OnLoaded        
        self.OnLoadContent = self.LoadContent        
        #user data
        self.configloader = globalspace.loader
        self.InitMenu()
        pass


    def LoadContent(self,url):
        if url.OriginalString == "/Pages/Settings.xaml":
            content = AppSettings()
            return content.content
        elif url.OriginalString == "/Pages/Introduction.xaml":
            return Introduction()
        elif url.OriginalString == "/Pages/Potions.xaml":
            content = Options()
            return content.content
        elif url.OriginalString == "/Pages/Logs.xaml":
            return Logs()
        elif url.OriginalString[:9] == "@builder|":
            return Builder(url.OriginalString[9:], self.configloader)
        return null

    def InitMenu(self):
        #update show
        for LinkGroup in self.MenuLinkGroups:
            if LinkGroup.DisplayName == "Builder":
                LinkGroup.Links.Clear()
                for item in self.configloader.configs.keys():
                    CTUtils.MenuAddLink(LinkGroup, os.path.basename(item), "@builder|%s" % (item))

    def OnLoaded(self, sender, e):      
        globalspace.preloadcontents = {"/Content/BuildLog.xaml" : BuildLog(), "/Content/Console.xaml" : Console()}
        globalspace.loadpreontents = lambda page : globalspace.preloadcontents[page] if globalspace.preloadcontents.has_key(page) else None
        globalspace.log = lambda logtext : globalspace.preloadcontents["/Content/BuildLog.xaml"].log(logtext) if globalspace.preloadcontents.has_key("/Content/BuildLog.xaml") else None

        #for k, v in self.configloader.configs.items():        
        #    print "[globalspace.loader] %s : \r\n %s" % (k, str(v))
        print "[globalspace.workingdir] %s" % (globalspace.workingdir)

if __name__ == '__main__':    
    globalspace.workingdir = os.getcwd()
    globalspace.loader = configloader()
    globalspace.preloadcontents = {}
    globalspace.loadpreontents = None
    globalspace.loader.load()
    globalspace.log = None
    globalspace.loadcs = loadcs
    globalspace.loadcs(os.path.abspath("WpfStyle.cs"), "WpfDataGrid.Style")
    App().Run(windowbuilder())