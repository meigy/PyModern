// SharpZipLibrary samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Text;
using System.Collections;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;


namespace UnzipTools
{
    public class Unzip
    {			
	    public static void UnzipFile(string zipfile, string directory)
	    {
            string directoryName = string.Empty;
            string fileName = string.Empty;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipfile))) {
		
			    ZipEntry theEntry;
			    while ((theEntry = s.GetNextEntry()) != null) {

                    //Console.WriteLine(theEntry.Name);

                    //string directoryName = Path.GetDirectoryName(theEntry.Name);
                    //string fileName      = Path.GetFileName(theEntry.Name);
                    
                    if (theEntry.IsDirectory)
                    {
                        directoryName = Path.Combine(directory, theEntry.Name);
                        // create directory
                        if ((directoryName.Length > 0) && !Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        continue;
                    }
                    
                    fileName = Path.Combine(directory, theEntry.Name);
                    directoryName = Path.GetDirectoryName(fileName);
                    if ((directoryName.Length > 0) && !Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    if (fileName != String.Empty) {
                        //using (FileStream streamWriter = File.Create(theEntry.Name)) {
                        using (FileStream streamWriter = File.Create(fileName))
                        {
                            int size = 2048;
						    byte[] data = new byte[2048];
						    while (true) {
							    size = s.Read(data, 0, data.Length);
							    if (size > 0) {
								    streamWriter.Write(data, 0, size);
							    } else {
								    break;
							    }
						    }
					    }
				    }
			    }
		    }
	    }
    }
}

