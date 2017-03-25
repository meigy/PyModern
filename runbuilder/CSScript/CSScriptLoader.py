import clr
import CSScript
from CSharp.Script import CSCompiler

def loadcs(script, assembly=""):
    error = clr.Reference[int]()
    message = clr.Reference[str]()
    compileresult = CSCompiler.Compile(script, assembly, error, message)   
    if error != 0:
        print message.decode("unicode-escape")
    return error 

testscript = '''
using System;
using System.Collections.ObjectModel;

namespace test
{
    public class testdata
    {
        public int a = 9;
        public string s = "ss";
    }

    public class CTObservableCollection : ObservableCollection<testdata>
    {

    }
}
'''

def testcs():
    error = clr.Reference[int]()
    message = clr.Reference[str]()
    compileresult = CSCompiler.Compile(testscript, "", error, message)
    obj1 = CSCompiler.CreateObject(compileresult, "test.testdata", None, 0);
    obj2 = CSCompiler.CreateObject(compileresult, "test.CTObservableCollection", None, 0);
    print obj1.s;