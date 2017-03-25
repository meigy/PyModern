# encoding: utf-8
'''
Created on 2016-11-29

@author: Meigy
'''

import sys
import os
import copy
import subprocess
import threading
import re
import traceback

from modulebase import varxml, xmlobject, globalspace

class traceableobject:
    def __init__(self):
        pass

    def log(self, message):
        try:
            if globalspace.hasprop('log'):
                #print message
                return globalspace.log(message + "\r\n")
        except:
            pass

class buildconfig(varxml):
    def __init__(self, configfile):
        super(buildconfig, self).__init__(True)
        self.options = {}
        self.configfile = configfile
        self.xml = None
        self.builders = None
        self.projectgroups = None
        self.buildoption = None
            
    def loadconfig(self):
        self.fromfile(self.configfile, 'utf8')
        self.processvariants()
        self.builders = builders(self.xml.builders)
        #print self.xml.builders
        self.builders.loadbuilders()
        self.projectgroups = projectgroups(self.xml.projectgroups)
        self.projectgroups.loadprojectgroups()
        for prjgroup in self.projectgroups.projectgroup:
            prjgroup.loadprojects()        

    def __buildproject(self, _project):
        builder = None
        error, message = 0, "[UNKOWN]"
        print self.buildoption.onbuildbegin
        self.buildoption.onbuildbegin(_project)
        try:
            for b in self.builders.builder:
                if _project.builder == b.name:
                    builder = b
                    break
            try:
                _project.loadproject()
                builder.build(_project)    
                error, message = 0, '[OK]' 
                #_project.buildoption['error'] = 0
                #_project.buildoption['result'] = '[OK]' 
            except Exception as e:
                error, message = -1, e.message 
                #_project.buildoption['error'] = -1
                #_project.buildoption['result'] = e.message # + traceback.format_exc()               
        finally:
            self.buildoption.onbuildend(_project, error, message)
        return 0  

    def __buildprojects__singlethread__(self, projectstobuild):
        #progress = 0
        #progressstep = 1.0 / len(projectstobuild)
        for project in projectstobuild:        
            self.__buildproject(project)
            #progress += progressstep
            #options.callback(progress)
        #options.callback(1)
        return
    
    def __buildprojects__multithread__(self, projectstobuild):
        #progress = 0
        #progressstep = 1.0 / len(projectstobuild)
        threads = []
        for project in projectstobuild:        
            t = threading.Thread(target=self.__buildproject,args=(project))
            t.setDaemon(True)
            t.start()
            threads.append(t)
        for thread in threads:
            thread.join()
            #progress += progressstep
            #options.callback(progress)
        #options.callback(1)
        return
    
    def buildprojects(self, options):
        '''
        options: +> buildoptions
        '''
        self.buildoption = options
        projectstobuild = []
        for prjgroup in self.projectgroups.projectgroup:
            for project in prjgroup.projects:
                #projectoption = options.projectoption(project.name)
                #project.buildoption = projectoption 
                #if projectoption['checked']:
                    #projectstobuild.append(project) 
                if not (project in self.buildoption.projects):
                    continue
                projectstobuild.append(project) 
        if len(projectstobuild) == 0 :
            return              
        if options.multithread :
            return self.__buildprojects__multithread__(projectstobuild)
        else:
            return self.__buildprojects__singlethread__(projectstobuild)
        self.buildoption.onbuildend(None, 0, "")

        

class buildoptions(xmlobject):
    '''
    multithread : bool
    projectgroup : string
    projects : dict => projectname : dict{checked, project, result}
    '''  
    def __init__(self):
        super(buildoptions, self).__init__()
        #self.config = self.xml
        self.xml.multithread = False
        #self.xml.projectgroup = ""
        #self.xml.callback = None
        self.xml.projects = {}
        self.onbuildbegin = None
        self.onbuildend = None

    def multithread():
        def fget(self):
            return self.xml.multithread
        def fset(self, value):
            self.xml.multithread = value
        return locals();
    multithread = property(**multithread()) 

    def projectgroup():
        def fget(self):
            return self.xml.projectgroup
        def fset(self, value):
            self.xml.projectgroup = value
        return locals();
    projectgroup = property(**projectgroup()) 
    
    '''
    def callback():
        def fget(self):
            return self.xml.callback
        def fset(self, value):
            self.xml.callback = value
        return locals();
    callback = property(**callback()) 
    '''
    def projectoption(self, projectname):
        if not self.xml.projects.has_key(projectname):
            self.xml.projects[projectname] = {"checked": False}
        return self.xml.projects[projectname]

    
class projectgroups(traceableobject):
    '''    
    projectgroupsxmlnode : xml.builders => xmlnode
    xmlpath: <xml>...<projectgroups>...</projectgroups></xml>
    '''
    def __init__(self, projectgroupsxmlnode):
        self.__config = projectgroupsxmlnode
        self.projectgroup = []
    
    def loadprojectgroups(self):
        if not self.__config.haschild('projectgroup'):
            return
        for prjgroup in self.__config.projectgroup.items:
            agroup = projectgroup(prjgroup)
            self.projectgroup.append(agroup)
        return    
    
class projectgroup(traceableobject):
    '''
    projectgroupxml : xml.projectgroups.projectgroup => xmlnode
    xmlpath: <xml>...<projectgroups><projectgroup name=?/>...</projectgroups></xml>
        *auto:
            <xml>...<projectgroups><projectgroup name=? path=? filetype=? builder=?/>...</projectgroups></xml>
    '''    
    #def __init__(self, _projectbuilder, projectspath, loadtype, projectsobj):
    def __init__(self, projectgroupxml):
        self.__config = projectgroupxml
        #self.path = projectspath
        #self.loadtype = loadtype
        #self.projectbuilder = _projectbuilder
        #self.projectsdefine = projectsobj
        self.name = self.__config['name']
        self.projects = []
        self.options = self.__config.attrs

    def loadprojects(self):
        if len(self.__config.project.items) > 0:
            for projectnode in self.__config.project.items:
                prj = project(projectnode)
                self.projects.append(prj)
        elif self.options.hasattr('path') and self.options.hasattr('filetype') and self.options.hasattr('builder') and self.options.hasattr('targettype'):
            path = self.options['path']
            filetype = self.options['filetype']
            targettype = self.options['targettype']
            builder = self.options['builder']
            self.__loadprojects_auto(path, filetype, targettype, builder)
    
    def __loadprojects_auto(self, path, filetypes, targettype, builder):
        if not os.path.exists(path):
            return
        if len(filetypes) == 0:
            return        
        checkfiletype = lambda x: re.match(filetypes, x, re.IGNORECASE);
        for root, dirs, files in os.walk(path, topdown = True):
            for f in files:
                if checkfiletype(f.lower()): 
                    prj = project(None, os.path.join(root, f), targettype, builder) 
                    self.projects.append(prj)
        return 

        
class project(traceableobject):
    '''
    projectxmlnode: xml.prjectgroups.prjectgroup.prject => xmlnode
    nodeattribs:
        name : project name in english
        describe : project name in chinese
        projectdir : project root path  (use macros)
        projectfile: project filename (macros && regexpresion)
        builder : builder name (defined in builder) 
    xmlpath:
        <xml><prjectgroups>...<projectgroup ...><project name=? describe=?>heer</project>...</projectgroup></prjectgroups></xml>
    '''
    #def __init__(self, refilename, describ, projectdir, projectbuilder):
    def __init__(self, projectxmlnode, projectfile = "", targettype = "", builder = ""):
        if projectxmlnode == None:
            return self.__init__overload__(projectfile, targettype, builder)
        self.__config = projectxmlnode
        self.projectfile = self.__config['projectfile']
        #self.projectfiles = []
        self.projectdir = self.__config['projectdir']
        self.projecttarget = self.__config['projecttarget']
        self.describe = self.__config['describe']
        self.name = self.__config['projectname']
        self.builder = self.__config['builder'] 
        self.options = self.__config.attrs           #out of standa
        #self.buildoption = {}                       #for builder
        #print self.projectfile
        #print self.name

    def __init__overload__(self, projectfile, targettype, builder):
        self.__config = xmlnode(projectfile, {}, 0)
        self.projectfile = projectfile
        #self.projectfiles = []
        self.projectdir = os.path.dirname(projectfile)
        self.describe = os.path.basename(projectfile)
        self.projecttarget = os.path.splitext(os.path.basename(projectfile))[0] + targettype
        self.name = os.path.basename(projectfile)
        self.builder = builder
        self.options = {}           #out of standa
        #self.buildoption = {}

    def loadproject(self):
        if os.path.exists(self.projectfile):
            return
        if os.path.exists(os.path.join(self.projectdir, self.projectfile)):
            self.projectfile = os.path.join(self.projectdir, self.projectfile)
            return            
        #print "find %s" % (self.projectfile)
        for root, dirs, files in os.walk(self.projectdir, topdown = True):
            for f in files:
                if re.match(self.projectfile, os.path.join(root, f), re.IGNORECASE):
                    self.projectfile = os.path.join(root, f)
                    print "Load Project: %s" % (self.projectfile)
                    break;
        return

    def projectinfo(self):
        return "[project : %s \r\n %s \r\n %s \r\n %s]" % (self.name, self.describe, self.projectdir, self.projectfile)    

class builders(traceableobject):
    '''    
    buildersxmlnode : xml.builders => xmlnode
    xmlpath: <xml>...<builders>...</builders></xml>
    '''
    def __init__(self, buildersxmlnode):
        self.__config = buildersxmlnode
        self.builder = []
    
    def loadbuilders(self):
        if not self.__config.haschild('builder'):
            return
        for buildernode in self.__config.builder.items:
            abuilder = builder(buildernode)
            self.builder.append(abuilder)
        return
                                         
    
class builder(traceableobject):
    '''    
    builderxmlnode : xml.builders.builder => xmlnode
    nodeattribs:
        name : builder name
    xmlpath: <xml>...<builders><builder name=? describe=?><step type="cmd"></step>...</builder></builders></xml>
    eg: xml.builders.builder name="dephi"
    '''    
    def __init__(self, builderxmlnode):
        self.name = builderxmlnode['name']
        self.builder = builderxmlnode
        #self.filetypes = self.config.filetypes.value.lower().split(',')
        self.steps = self.builder.step.items
        self.options = self.builder.attrs        #out of standar
        
    def __dosteps(self, steps):
        sortedsteps = builder.sortsteps(steps)
        for step in sortedsteps:
            self.log("=> step begin [%s]" % (str(step)))
            r = stepexecute(step).execute()
            self.log("<= step end [%s]" % (str(r)))
            if r["errorcode"] != 0:
                raise Exception(str(r["errormsg"]))            
        return None
    
    def __setproject(self, project):
        for step in self.steps:
            value = step.value
            value = value.replace('@@projectfile', project.projectfile)        
            value = value.replace('@@projectdir', project.projectdir)       
            value = value.replace('@@projecttarget', project.projecttarget)    
            value = value.replace('@@projectname', project.name)    
            for option, v in project.options.items():
                value = value.replace('@@' + option, v)       
            step.nodevalue = value
        return    
        
    def __build(self, project):      
        self.__setproject(project) 
        self.__dosteps(self.steps)
    
    @staticmethod
    def sortsteps(steps):
        sortedsteps = steps
        def cmp_step(x, y):
            if x['order'] > y['order']:
                return 1
            elif x['order'] == y['order']:
                return 0
            else:
                return -1
        sortedsteps.sort(cmp_step)
        return sortedsteps          
    
    def build(self, project):
        self.log("<build begin>")
        self.log(project.projectinfo())

        clonbuilder = self.clone()
        clonbuilder.__build(project)
        #log
        self.log("<build end>")

    #multithread suport
    def clone(self):
        return copy.deepcopy(self)
        
class stepexecute(traceableobject):
    
    def __init__(self, step, debug = True):        
        self.debug = debug
        self.step = step
        self.errorcode = 0
        self.errormsg = ""
        
    def execute(self):
        try:
            self.log(self.step.value)
            if self.step['type'].lower() == "cmd":
                self.executecmd(self.step.value)
        except Exception, ex:
            return {"errorcode" : -1, "errormsg" : ex.errormsg}
        return {"errorcode" : self.errorcode, "errormsg" : self.errormsg + self.step.value}

    def executecmd(self, cmd):
        #p = subprocess.Popen(cmd,shell=True,stdout=subprocess.PIPE)  
        if cmd.strip() == "":
            return
        print cmd
        p = subprocess.Popen(cmd,shell=True,stdout=subprocess.PIPE)  
        out,err = p.communicate()  
        if self.debug:
            if out != None:
                for line in out.splitlines():  
                    self.errormsg = self.errormsg + line + '\r\n'
                    print line
            if err != None:
                for line in err.splitlines():  
                    self.errormsg = self.errormsg + line + '\r\n'
                    print '[ERROR]' + line      
        self.errorcode = p.wait()    



        