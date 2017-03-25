import wpf
import clr
import sys
import thread
import os

clr.AddReference("System.Drawing")
clr.AddReference("System.Xml")
from System.Windows import Application, Window
from System.Windows import FontWeights
from System.Windows.Media import FontFamily, Brushes
from System.Windows.Documents import TextElement
from System.Drawing import Color
#clr.AddReference("System.Windows.Controls")

#from System.Windows.Controls import UserControl


from FirstFloor.ModernUI.App import CTUtils, CTUserContorl

class Console(CTUserContorl):
    def __init__(self):
        wpf.LoadComponent(self, 'Content/Console.xaml')
        self.__oldstdout = sys.stdout
        self.__olderrout = sys.stderr
        sys.stdout = self
        sys.stderr = self
        self.threadid = thread.get_ident()

    def __del__(self):
        sys.stdout = self.__oldstdout
        sys.stderr = self.__olderrout

    def write(self, text):
        self.__oldstdout.write(text)
        tid = thread.get_ident()
        pid = os.getpid()
        colorbrushes = (Brushes.Aqua, Brushes.Bisque, Brushes.Blue, Brushes.Chartreuse, Brushes.Coral, Brushes.DarkCyan,
                        Brushes.DeepPink, Brushes.Firebrick, Brushes.Gold, Brushes.DarkViolet, Brushes.Fuchsia, Brushes, 
                        Brushes.SteelBlue, Brushes.LimeGreen, Brushes.Indigo, Brushes.Black)
        n = tid % len(colorbrushes)
        self.ConsoleOut.AppendText('[%8d]' % (tid) + text)


