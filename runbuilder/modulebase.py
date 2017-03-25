'''
Created on 2014-8-30

@author: meigy
'''

class xobject(object):
    '''
    _iprop = dict()
    _idict = dict()
    '''    
    _class_props = {}
    _class_dicts = {}
    def __init__(self):   
        xobject._class_props[id(self)] = dict()  
        xobject._class_dicts[id(self)] = dict()  
        #_attrs = dict()
        #_items = dict()
    def __del__(self):
        xobject._class_props[id(self)] = None
        xobject._class_dicts[id(self)] = None
    def __getitem__(self, key):
        #return self._items[key]
        return xobject._class_dicts[id(self)][key]
    def __setitem__(self, key, value):
        #self._items[key] = value
        xobject._class_dicts[id(self)][key] = value
    def __getattr__(self, attr):
        #return self._attrs[attr]
        return xobject._class_props[id(self)][attr]
    def __setattr__(self, attr, value):
        #self._attrs[attr] = value 
        xobject._class_props[id(self)][attr] = value
    def __eq__(self, b):
        return id(self) == id(b)
           

class xdist(list):
    '''
    _iprop = dict()
    _idict = dict()
    '''
    def __init__(self):
        super(xdist, self).__init__()
        self._iprop = dict()
        self._idict = dict()      
         
    def __getitem__(self, key):
        if type(key) == int:
            return super(xdist, self).__getitem__(key)
        #elif (type(key) == str) and self._idict.has_key(key):
        elif self._idict.has_key(key):
            return self._idict[key]
        else:
            return None
    def __setitem__(self, key, value):
        if type(key) == int:
            super(xdist, self).__setitem__(key, value)
        #elif (type(key) == str):
        else:
            self._idict[key] = value  
    def __setattr__(self, attr, value):
        #super(xdist, self).__setattr__(attr, value)
        #if self._iprop and self._iprop.has_key(attr):
        #    self._iprop[attr] = value
        if (attr != '_iprop' and attr != '_idict') and (attr not in dir(self)):
            self._iprop[attr] = value 
        super(xdist, self).__setattr__(attr, value)
    def __delattr__(self, attr):
        if self._iprop.has_key(attr):
            self._iprop.pop(attr)  
        super(xdist, self).__delattr__(attr)         
    def getprop(self, attr):
        return getattr(self, attr)
    def setprop(self, attr, value):
        return setattr(self, attr, value)
    def hasprop(self, attr):
        return hasattr(self, attr)
    def delprop(self, attr):
        return delattr(self, attr)    
    def __eq__(self, b):
        return id(self) == id(b) 
    '''              
    def addprop(self, attr, value):
        if hasprop(self, attr):
            raise Exception('attr exists')
        self._iprop[attr] = value
        setprop(self, attr, value)
    def delprop(self, attr):
        if self._iprop.has_key(attr):
            self._iprop.pop(attr)
        delprop(self, attr)         
    '''
    def props():
        def fget(self):
            return copy.deepcopy(self._iprop)
        return locals()
    props = property(**props())
    def hasitem(self, name):
        return self._idict.has_key(name)
    def dicts():
        def fget(self):
            return self._idict
        def fset(self, value):
            for k, v in value:
                self._idict[k] = v
        return locals()
    dicts = property(**dicts())
    '''
    def getprops(self):
        result = {}
        for a in self._iprop:
            result[a] = self.getprop(a)
        return result
    def getdict(self):
        return self._idict
    '''
    '''
    def extattrvalue(self, attr):
        if self._idict.has_key(attr):
            return self._idict[attr]
    '''
    def clear(self):
        self._iprop.clear();
    def __str__(self):
        return '{self: %s, prop: %s, dict: %s}' % (super(xdist, self).__str__(), str(self.props), str(self.dicts))
    

#/////////////////////////////////////////////////////////////////////


#import xml.dom
#import xml.etree
import xml.dom.minidom
#import xml.dom.xmlbuilder
#import xml.parsers

import copy
import re

'''
def getxmldata(xmlfile):
    xmlr = xmlobject()
    xmlr.xml = xmlnode() 
    xmlr.fromfile(xmlfile) 
    return xmlr.xml   
'''    

class xmlnode(xdist):
    '''
    {}: for xml attributs    
    props: for childnodes       
    props.[]: for repeat childnodes   
    '''
    TEXTNODE = 0
    TREENODE = 1
    #def __init__(self, name = '', value = '', attr = {}, level = 0):
    def __init__(self, name = '', attr = {}, level = 0):
        super(xmlnode, self).__init__()
        self._name = name
        self._value = ''       #_value only for textnode
        self._nodetype = xmlnode.TEXTNODE         
        self._level = level  
        self._isroot = False   
        self.dicts = copy.deepcopy(attr)  # write _idict 
        self._attrib = self._idict
        
        self._lnk = None        # []special write
        
        self.clear()     #important
        #self.append(self)
        
    def __setattr__(self, attr, value):
        #if attr in self.__dict__:
        #    raise Exception('attr %s is reserved' % (attr))
        super(xmlnode, self).__setattr__(attr, value)  
    
    #xmlname properties 
    #-----------------------------------------------------
    def nodename():
        def fget(self):
            return self._name
        def fset(self, value):
            self._name = value
        return locals()
    nodename = property(**nodename())
        
    def nodevalue():
        def fget(self):
            if self._nodetype == xmlnode.TEXTNODE:               
                return self._value
            else:
                return self.props
        def fset(self, value):           
            if type(value) == type(self):
                if self.hasprop(value.nodename):
                    propnode = self.getprop(value.nodename)
                    if len(propnode) == 0:
                        #propnode.append(copy.deepcopy(propnode))
                        copynode = copy.deepcopy(propnode)
                        copynode._lnk = propnode
                        propnode.append(copynode)    #**************========== [] first element is not nodeself
                    propnode.append(value)
                else:
                    self.setprop(value.nodename, value)
                self._nodetype = xmlnode.TREENODE
            else:
                self._value = value
                self._nodetype = xmlnode.TEXTNODE
                self.realnode._value = value
                self.realnode._nodetype = xmlnode.TEXTNODE
        return locals()
    nodevalue = property(**nodevalue())   
            
    def nodetype():
        def fget(self):
            return self._nodetype
        return locals()
    nodetype = property(**nodetype())
    
    def nodelevel(): 
        def fget(self):
            return self._nodelevel
        return locals()
    nodelevel = property(**nodelevel())     
    
    def nodeattribs():
        def fget(self):
            return self._idict;
        return locals();
    nodeattribs = property(**nodeattribs())  

    #shortname properties 
    #-----------------------------------------------------
    def name():
        def fget(self):
            return self._name
        return locals()
    name = property(**name()) 
    
    def value():
        def fget(self):
            return self.nodevalue
        return locals()
    value = property(**value()) 
    
    def items():
        def fget(self):
            if len(self) > 0:
                return self
            else:
                return [self]
        return locals()
    items = property(**items())

    def childs():
        def fget(self):
            if self._nodetype != xmlnode.TREENODE:
                return []
            else:
                return self._iprop
        return locals()
    childs = property(**childs())

    def attrs():
        def fget(self):
            return self._idict;
        return locals();
    attrs = property(**attrs()) 

    def realnode():
        def fget(self):
            return self if self._lnk == None else self._lnk;
        return locals();
    realnode = property(**realnode())         
    #addon
    #------------------------------------------------    
    def haschild(self, nodename):
        if self._nodetype != xmlnode.TREENODE:
            return False
        else:
            return self.hasprop(nodename)     

    def hasattr(self, attrname):
        return self.hasitem(attrname)   

    def setattr(self, attrname, value):
        if not self.hasitem(attrname):
            return
        self[attrname] = value
        self.realnode[attrname] = value
    
    def addpath(self, path):
        def makeprop(node, p):
            if not node.hasprop(p):
                node.nodevalue = xmlnode(p)
        source = self
        pts = path.split('\\')
        for p in pts:
            makeprop(p)
            source = source.getprop(p)                        
        
    def addchild(self, name):
        child = xmlnode(name, {}, self._level + 1)
        self.nodevalue = child
        return child          
                      
    def __str__(self):
        if len(self) > 0:
            return '\r\n'.join([str(i) for i in self if i != self])
        else:
            sattr = ['%s="%s"' % (attrkey, attrvalue) for attrkey, attrvalue in self._attrib.items()]
            svalue = self._value if self._nodetype == xmlnode.TEXTNODE else '\r\n'.join([str(propvalue) for prop, propvalue in self.nodevalue.items()])
            #print svalue
            return '%s<%s%s>%s</%s>' % ('  ' * self._level, self._name, ' ' + ' '.join(sattr) if len(sattr) > 0 else '', svalue, self._name)
            
        '''
        def str_self():
            if len(self._idict) > 0:
                return '%s%s:%s,%s' % ('  ' * self._level, self._name, self._value, self._attrib)
            return '%s%s:%s' % ('  ' * self._level, self._name, self._value)
        def str_childs():
            datas = self.xml_getdatas()
            text1 = [str(self.getprop(a)) for a in datas if len(self.getprop(a)) == 0]
            text2 = [str_childitem(self.getprop(a)) for a in datas if len(self.getprop(a)) != 0]
            return '\r\n'.join(text1) + '\r\n'.join(text2)                
        def str_childitem(attr):
            text = [str(a) for a in attr if type(a) == type(self)]
            return '\r\n'.join(text)
        r0 = str_self()
        r1 = str_childs()       
        if len(r1) > 0:
            return  '%s%s%s' % (r0, '\r\n', r1)              
        return  '%s%s%s' % (r0, '', r1)    
        '''      

        
class xmlobject(object):
    '''
    classdocs
    '''    
    #xml = None    
     
    def __init__(self, strict = True):
        '''
        Constructor
        '''
        self.xml = xmlnode()  
        self.doc = None
        self.strict = strict      
        self.__strictre = re.compile(r'^\s*\r*\n*\s*$')
                
    def __parseelement(self, element, elementdata):
        #def parsechild(e, elementdata):
        if element is None:
            return       
        if element.nodeType != xml.dom.Node.ELEMENT_NODE and element.nodeType != xml.dom.Node.COMMENT_NODE:
            print 'xmlnode, bad nodetype: %s' % (element.nodeType)
            raise Exception('xmlnode, bad nodetype')
            return
        
        if element.attributes != None:
            for k in element.attributes.keys():
                elementdata[k] = str(element.attributes[k].value);
                
        for e in element.childNodes:
            if e is None:
                continue
            if e.nodeType == xml.dom.Node.TEXT_NODE:
                if self.strict:
                    if not self.__strictre.match(e.nodeValue):                    
                        value = e.nodeValue
                        value = value.strip()
                        value = value.strip('\r')
                        value = value.strip('\n')
                        value = value.strip()
                        elementdata.nodevalue = value
                    continue
                else:
                    elementdata.nodevalue = e.nodeValue
            
            if e.nodeType == xml.dom.Node.ELEMENT_NODE or element.nodeType == xml.dom.Node.COMMENT_NODE:
                childnodedata = elementdata.addchild(str(e.nodeName))
                self.__parseelement(e, childnodedata)                
        return
                
    def xmlencoding(self):
        if self.doc is None:
            return None
        return self.doc.encoding;        
                
    def fromxml(self, sxml):       
        self.xml = xmlnode()
        try:
            self.doc = xml.dom.minidom.parseString(sxml)
            root = self.doc.documentElement  
            '''              
            self.xml._name = root.tag
            self.xml._val = ''
            '''
            self.xml = xmlnode(root.nodeName)         
            #self.encoding = self.doc.encoding
            #print self.xml.encoding
            self.xml._isroot = True
            setattr(self, self.xml.nodename, self.xml)        
            
            self.__parseelement(root, self.xml)
        except Exception, ex:
            raise Exception(ex.message)            
        
    def fromfile(self, file, encoding=""):
        f = open(file, 'rb')
        text = f.read()
        f.close()
        #self.xml = xmlnode()       # reset
        if encoding != "":
            text = text.decode(encoding)
        self.fromxml(text)
        
    def toxml(self, encoding):
        #return self.doc.toxml(self.doc.encoding)
        return '<?xml version="1.0" encoding="%s"?>' % (encoding) + str(self.xml)
        #etree.write(root, encoding = self.xml.encoding, xml_declaration = True)
        #xmltext = etree.tostring(root, xml_declaration = True, encoding = self.xml.encoding, pretty_print = True)
        
    def tofile(self, filename, encoding):
        f = open(filename, 'wb')
        f.writelines(self.toxml(encoding))
        f.close()   

    def __str__(self):
        return '<?xml version="1.0" encoding="utf-8"?>' + str(self.xml)
#///////////////////////////////////////////////////////////////////////////////////////////////////////        
        
import base64        
import os
import sys
         
class varxml(xmlobject):
    '''
    xml.var
    config.xml variants process
    '''
    class varobject:
        def __init__(self, name, value):
            self.name = name;
            self.value = value;

    class varresolve:
        def __init__(self, vardict, vartag):
            self.vartag = vartag
            self.vars = []
            for k, v in vardict.items():        # v => xmlnode, TEXTNODE
                self.vars.append(varxml.varobject(k, v.value))
            self.vardict = vardict

        def __varsort(self, _vars):     #_vars (list of varobject)
            def cmp_var(x, y):
                if x.name > y.name:
                    return -1
                elif x.name == y.name:
                    return 0
                else:
                    return 1
            _vars.sort(cmp_var)  

        def __isvardepends(self, var, vars):
            __vardebpens = lambda var, vardepends: var.name != vardepends.name and (var.value.count(self.vartag + vardepends.name) > 0)
            for v in vars:
                if __vardebpens(var, v):
                    return True
            return False

        def __varrank(self, vars, rankedvars):
            if len(vars) == 0:
                return
            undepends = []
            depends = []
            for v in vars:
                if self.__isvardepends(v, vars):
                    depends.append(v)
                else:
                    undepends.append(v)
            rankedvars.append(undepends)
            self.__varrank(depends, rankedvars)

        def __varresolve(self, rankedvars):
            __vardebpens = lambda var, vardepends: var.name != vardepends.name and (var.value.count(self.vartag + vardepends.name) > 0)
            __varresolve = lambda var, vardepends: var.value.replace(self.vartag + vardepends.name, vardepends.value)
            resolvedvars = []
            for unresolvedvars in rankedvars:
                for unvar in unresolvedvars:
                    for v in resolvedvars:
                        if __vardebpens(unvar, v):
                            unvar.value = __varresolve(unvar, v)
                resolvedvars.extend(unresolvedvars)
                self.__varsort(resolvedvars)
            return resolvedvars

        def __resolve_signle(self, vars0):
            for var in vars0:
                vartype = self.vardict[var.name]['type']
                if vartype == 'base64':
                    var.value = base64.decodestring(var.value)
                elif vartype == 'python':
                    var.value = str(eval(var.value))
                else:
                    var.value = str(var.value)  
        
        def resolve(self):
            rankedvars = []
            self.__varsort(self.vars)
            self.__varrank(self.vars, rankedvars)
            self.__resolve_signle(rankedvars[0])
            varresolved = self.__varresolve(rankedvars) 
            self.vars = varresolved           
            for v in self.vars:
                node = self.vardict[v.name]
                node.nodevalue = v.value
            return self.vars

    def __init__(self, strict = True):
        super(varxml, self).__init__(strict)
        self.vartag = '@@'
        self.varnode = 'var'
        #self.xml = copy.deepcopy(self.xml.var)
        self.vars = {}      #'''dict of varname: xmldata '''
        self.varlist = []       # list of varobject  
                                 
    def __getvarlist(self):
        def __getvarloop(path, xmlnode, varlist):
            varprops = xmlnode.props
            for vname, vxml in varprops.items():
                varname = path + ('.' if path != '' else '') + vname
                if vxml.nodetype == xmlnode.TEXTNODE:
                    varlist[varname] = vxml
                else: 
                    __getvarloop(varname, vxml, varlist)         
        self.vars.clear()
        if not self.xml.hasprop(self.varnode):
            return
        __getvarloop('', self.xml.var, self.vars)      
        return    

    def __processtreenode(self, node, varvalues):        
        if len(node) > 0:
            for n in node:
                self.__processtreenode(n, varvalues)
            return   
        for attr in node.attrs:
            value = node[attr]
            if value.count(self.vartag) == 0:
                continue;
            for v in varvalues:
                value = value.replace(self.vartag + v.name, v.value)
            node.setattr(attr, value)
        if node.nodetype == xmlnode.TEXTNODE :
            value = node.nodevalue
            if value.count(self.vartag) == 0:
                return;
            for v in varvalues:
                value = value.replace(self.vartag + v.name, v.value)
            node.nodevalue = value
        elif len(node.childs) > 0:
            for prop in node.childs:
                self.__processtreenode(node.getprop(prop), varvalues)
        return          
        
    def processvariants(self):    
        self.__getvarlist();
        resolve = self.varresolve(self.vars, self.vartag)
        self.varlist = resolve.resolve()        #sorted valued
        for v in self.varlist:
            print "%s:%s" % (v.name, v.value)
        for prop in self.xml.childs:
            #if prop == self.varnode:
            #    continue
            self.__processtreenode(self.xml.getprop(prop), self.varlist)
        #print "processvariants:"
        #print self
        return                   
  
#---------------------------------------
globalspace = xdist()            
    
        
def xmlobject_test():
    m = xmlobject()  
    #m.xml = xmlnode()  
    #m.xml.__dict__['aa'] = 'aaa'
    #print len(m.xml)
    #m.xml.append(1)
    #print len(m.xml)  
    m.fromxml('''<?xml version="1.0" encoding="ascii"?>
    <ReplyTokens>
        <LineStartTokens test="t2">
            <StartToken>_____</StartToken>
            <StartToken>--------</StartToken>
        </LineStartTokens>
        <DataTokens NumberRequired="2" StartMatchDataToken="Van:">
            <OptionalDataToken>Verzonden:</OptionalDataToken>
            <OptionalDataToken>Aan:</OptionalDataToken>
            <OptionalDataToken>Onderwerp:</OptionalDataToken>
            <OptionalDataToken>Datum:</OptionalDataToken>
            <OptionalDataToken>Cc:</OptionalDataToken>
            <OptionalDataToken>BCC:</OptionalDataToken>
        </DataTokens>
        <BeginEndTokens>
            <BeginEndToken Begin="Om" End="schreef:">
                <level3>test</level3>
            </BeginEndToken>
            <BeginEndToken Begin="Op" End="schreef:"></BeginEndToken>
            <BeginEndToken Begin="---" End="schreef:"></BeginEndToken>
        </BeginEndTokens>
        <SubjectTokens>
            <SubjectToken>re:</SubjectToken>
            <SubjectToken>Onderwerp:</SubjectToken>
            <SubjectToken>fwd:</SubjectToken>
            <SubjectToken>fw:</SubjectToken>
            <SubjectToken>Geaccepteerd:</SubjectToken>
            <SubjectToken>updated:</SubjectToken>
            <SubjectToken>Geweigerd:</SubjectToken>
            <SubjectToken>Geannuleerd:</SubjectToken>
        </SubjectTokens>
        <Somediffrent>
            <diff1>
                <ROW>SDKLFJLKWEFIOWIFDSFLK</ROW>
                <ROW>KDKDKDIDDKKFDF</ROW>
            </diff1>
            <diff2 comment="empty">ex2</diff2>
        </Somediffrent>
    </ReplyTokens>
    ''')
    print '1-----------------------------------------------------'
    print m.ReplyTokens.DataTokens
    print '2-----------------------------------------------------'
    print m.ReplyTokens.DataTokens._nodetype
    print m.ReplyTokens.DataTokens.nodevalue
    print m.ReplyTokens.DataTokens._attrib
    print m.ReplyTokens.Somediffrent
    print m.toxml(m.xmlencoding())
    #m.savetofile(r'h:\\1.xml')
    #a = xmlobject()  
    #a.xml = xmlnode()    
    #a.fromfile(r'E:\szkingdom.com\Fis_copy\src\trunk_z\lbmdll\bin\kcbpspd.xml')
    #print a.xml 
    
if __name__ == '__main__':
    xmlobject_test()  
