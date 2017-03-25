import wpf
import clr

#clr.AddReference("System.Windows.Controls")

#from System.Windows.Controls import UserControl

from FirstFloor.ModernUI.App import CTUtils, CTUserContorl
from Content.BuildLog import BuildLog
from Content.Console import Console

from modulebase import globalspace

class Logs(CTUserContorl):
    def __init__(self):
        wpf.LoadComponent(self, 'Pages/Logs.xaml')
        CTUtils.SetUserControlItems_LoadContent(self, self.LoadContent)
        #containers =  CTUtils.GetControlChildObjects_byTypeName(self.content, "FirstFloor.ModernUI.Windows.Controls.ModernTab")
        #for obj in containers:
        #    obj.ContentLoader.OnLoadContent = self.LoadContent
        #self.OnLoadContent = self.LoadContent
        self.NavigatedFrom += self.OnNavigatedFrom
        self.NavigatedTo += self.OnNavigatedTo
        self.FragmentNavigation += self.OnFragmentNavigation
        self.NavigatingFrom += self.OnNavigatingFrom

    def LoadContent(self,url):
        print "request page: %s" % (url.OriginalString)
        content = globalspace.loadpreontents(url.OriginalString)
        if content != None:
            return content
        elif url.OriginalString == "/Content/BuildLog.xaml":
            return BuildLog()
        elif url.OriginalString == "/Content/Console.xaml":
            return Console()
        return null

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



