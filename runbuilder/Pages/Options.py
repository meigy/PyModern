import wpf
import clr

#clr.AddReference("System.Windows.Controls")

#from System.Windows.Controls import UserControl

clr.AddReference("FirstFloor.ModernUI.App.dll")

from FirstFloor.ModernUI.App.Pages import Navigation

from FirstFloor.ModernUI.App import CTUtils

class Options():
    def __init__(self):
        self.content = Navigation()





