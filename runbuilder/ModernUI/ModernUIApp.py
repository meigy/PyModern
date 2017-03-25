import wpf
import clr

from System.Windows import Application, Window

class App(Application):
    def __init__(self):
        wpf.LoadComponent(self, 'App.xaml')