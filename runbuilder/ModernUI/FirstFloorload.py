import wpf
import clr

clr.AddReference("System")
clr.AddReference("FirstFloor.ModernUI.dll")
clr.AddReference("FirstFloor.ModernUI.App.dll")

from System.Windows import Application, Window

from FirstFloor.ModernUI.Presentation import *
from FirstFloor.ModernUI.Windows import *
from FirstFloor.ModernUI.Windows.Controls import *
from FirstFloor.ModernUI.Windows.Converters import *
from FirstFloor.ModernUI.Windows.Navigation import *
#from FirstFloor.ModernUI.Windows.Controls import ModernWindow
from FirstFloor.ModernUI.App import CTModernWindow

#from FirstFloor.ModernUI.App.Pages import Settings

from System import Uri, UriKind
from System.Windows import WindowStartupLocation

#from main import MyWindow
#from Settings import Settings
from AppSettings import AppSettings
