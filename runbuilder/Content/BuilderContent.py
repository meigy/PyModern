import wpf
import clr
import os
import traceback

#clr.AddReference("WpfApplication1.NS.dll")
#import WpfApplication1.NS
#from WpfApplication1.NS import *
#clr.AddReference("System.Collections.ObjectModel")
from System.Collections.ObjectModel import ObservableCollection

#from System.Windows.Controls import UserControl

from FirstFloor.ModernUI.App import CTUtils, CTUserContorl
from FirstFloor.ModernUI.Windows.Controls import ModernDialog

from System.Windows import MessageBoxButton
from modulebuilder import buildoptions

class BuilderContent(CTUserContorl):
    def __init__(self, buildgroup, configitem, buildloader):
        wpf.LoadComponent(self, 'Content/BuilderContent.xaml')
        self.loader = buildloader
        self.configitem = configitem
        self.buildgroup = buildgroup
        #parare data_show
        self.buildprojects = ObservableCollection[object]()
        self.buildoption = buildoptions()
        self.InitData()

    def InitData(self):
        self.buildprojects.Clear()
        groups = self.loader.projects[self.configitem]
        for group in groups.projectgroup:
            if self.buildgroup == group.name:
                for proj in group.projects:
                    data = projectdata()
                    data.project = proj
                    data.name = proj.name.decode('utf-8').encode('gb2312')
                    data.describe = proj.describe
                    data.checked = True
                    data.status = 0
                    self.buildprojects.Add(data)
        self.DG1.DataContext = self.buildprojects
    
    '''
    def doProgress(self, value):
        self.Progress.BeginInit()
        try:
            self.Progress.Value = value
        finally:
            self.Progress.EndInit()
    '''

    def CheckAllClick(self, sender, e):
        self.DG1.BeginInit()
        for data in self.buildprojects:
            data.checked = self.CheckAll.IsChecked
            #print data.checked
        self.DG1.EndInit()

    def __getprojectoption(self, project):
        for data in self.buildprojects:
            if data.project == project:
                return data
        return None

    def __build(self):
        option = self.buildoption
        option.multithread = self.Multithread.IsChecked
        #option.projectgroup = self.buildgroup
        option.projects = [data.project for data in self.buildprojects if data.checked]
        option.onbuildbegin = self.doBuildBegin
        option.onbuildend = self.doBuildEnd
        for data in self.buildprojects:
            #projectoption = self.buildoption.projectoption(data.name)
            #projectoption['checked'] = data.checked
            #prjoption['callback'] = self.doProgress
            data.error = False
            data.result = ""        
        self.loader.build(self.configitem, self.buildoption)

    def doBuild(self, sender, e):        
        self.btBuild.IsEnabled = False
        try:
            errorcount = 0
            self.Progress.Value = 0
            try:
                self.__build()
            except:
                pass
            self.Progress.Value = 1
            '''
            self.DG1.BeginInit()
            try:
                for data in self.buildprojects:
                    #prjoption = self.buildoption.projectoption(data.name)
                    if not data.checked:
                        continue
                    data.result = data.project.buildoption['result']                
                    if data.project.buildoption['error'] != 0:
                        data.error = True
                        errorcount += 1
            finally:
                self.DG1.EndInit()
            '''
            for data in self.buildprojects:
                if data.error :
                    errorcount += 1
            if errorcount == 0:
                ModernDialog.ShowMessage("build complete successful!", "success", MessageBoxButton.OK)
            else:
                ModernDialog.ShowMessage("build finished : %d projects failed!" % (errorcount), "finished", MessageBoxButton.OK)
        except Exception as e:
            ModernDialog.ShowMessage("failure:" + e.message + "\r\n" + traceback.format_exc(), "failed", MessageBoxButton.OK)
        self.btBuild.IsEnabled = True
        return

    def doBuildBegin(self, project):
        data = self.__getprojectoption(project)
        if data == None:
            return

        return

    def doBuildEnd(self, project, error, message):
        data = self.__getprojectoption(project)
        print "doBuildEnd: %s" % (data)
        if data == None:
            return
        
        print "%s: %s, %s" % (project.name, error, message)
        self.DG1.BeginInit()
        try:
            data.error = (error != 0)
            print data.error
            data.result = message
        finally:
            self.DG1.EndInit()

        self.Progress.BeginInit()
        try:
            progress = self.Progress.Value
            checkcount = len([data.checked for data in self.buildprojects if data.checked])
            if checkcount > 0:
                progress += (1.0 / checkcount) 
            self.Progress.Value = progress
        finally:
            self.Progress.EndInit() 

        return



class projectdata:
    def __init__(self):
        self.project = self
        self.name = ""
        self.describe = ""
        self.checked = True
        self.result = ""
        self.error = False




