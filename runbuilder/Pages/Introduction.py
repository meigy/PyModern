import wpf
import clr

#clr.AddReference("System.Windows.Controls")

#from System.Windows.Controls import UserControl


from FirstFloor.ModernUI.App import CTUtils, CTUserContorl

class Introduction(CTUserContorl):
    def __init__(self):
        wpf.LoadComponent(self, 'Pages/Introduction.xaml')



