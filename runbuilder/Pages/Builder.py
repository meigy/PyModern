import wpf
import clr

#from System.Windows.Controls import UserControl

from FirstFloor.ModernUI.App import CTUtils, CTUserContorl
from FirstFloor.ModernUI.Presentation import Link

from Content.BuilderContent import BuilderContent
from Content.BuildLog import BuildLog
from Pages.Logs import Logs

from modulebase import globalspace

class Builder(CTUserContorl):
    def __init__(self, configitem, buildloader):
        wpf.LoadComponent(self, 'Pages/Builder.xaml')
        self.configitem = configitem
        self.loader = buildloader
        self.InitTabs()
        CTUtils.SetUserControlItems_LoadContent(self, self.LoadContent)
        self.NavigatedFrom += self.OnNavigatedFrom
        self.NavigatedTo += self.OnNavigatedTo
        self.FragmentNavigation += self.OnFragmentNavigation
        self.NavigatingFrom += self.OnNavigatingFrom
        

    def LoadContent(self,url):
        print "request page: %s" % (url.OriginalString)
        content = globalspace.loadpreontents(url.OriginalString)
        if content != None:
            return content
        elif url.OriginalString[:14] == "@buildergroup|":
            return BuilderContent(url.OriginalString[14:], self.configitem, self.loader)
        elif url.OriginalString == "/Content/BuildLog.xaml":
            return BuildLog()
        elif url.OriginalString == "/Pages/Logs.xaml":
            return Logs()
        return null

    def InitTabs(self):
        Tabs = CTUtils.GetControlChildObjects_byTypeName(self, "FirstFloor.ModernUI.Windows.Controls.ModernTab")
        if len(Tabs) <= 0:
            return
        Tab = Tabs[0]
        Tab.Links.Clear()
        builderprojects = self.loader.projects[self.configitem]
        for group in builderprojects.projectgroup:
            CTUtils.TabAddLink(Tab, group.name, '@buildergroup|%s' % group.name)  
        Tab.SelectedSource = Tab.Links[Tab.Links.Count - 1].Source        

    def OnNavigatedFrom(self, sender, e):
        #print "OnNavigatedFrom"
        pass
    
    def OnNavigatedTo(self, sender, e):
        #print "OnNavigatedTo"
        pass

    def OnFragmentNavigation(self, sender, e):
        #print "OnFragmentNavigation"
        pass

    def OnNavigatingFrom(self, sender, e):
        #print "OnNavigatingFrom"
        pass



