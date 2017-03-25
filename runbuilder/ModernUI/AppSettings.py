import wpf
import clr

#clr.AddReference("System.Windows.Controls")

#from System.Windows.Controls import UserControl

clr.AddReference("FirstFloor.ModernUI.App.dll")

from FirstFloor.ModernUI.App.Pages import Settings
from FirstFloor.ModernUI.App.Content import SettingsAppearance

from FirstFloor.ModernUI.App import CTUtils

class AppSettings():
    def __init__(self):
        #wpf.LoadComponent(self, 'Settings.xaml')
        self.content = Settings()
        CTUtils.SetUserControlItems_LoadContent(self.content, self.LoadContent)
        #containers =  CTUtils.GetControlChildObjects_byTypeName(self.content, "FirstFloor.ModernUI.Windows.Controls.ModernTab")
        #for obj in containers:
        #    obj.ContentLoader.OnLoadContent = self.LoadContent

    def LoadContent(self,url):
        if url.OriginalString == "/Content/SettingsAppearance.xaml":
            content = SettingsAppearance()
            #content.Parent = self
            #content.ContentLoader.OnLoadContent = self.LoadContent
            return content
        return null




