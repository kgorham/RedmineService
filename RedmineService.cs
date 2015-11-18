/**
The MIT License (MIT)

Copyright (c) 2015 Gorham Software Services, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
**/
using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Specialized;
using System.IO;

namespace RedmineService {

    class RedmineService {

        private Process process;
        const string EVENT_LOG = "RedmineSvc";

        public RedmineService() {
            if (!EventLog.SourceExists(EVENT_LOG)) {
                EventLog.CreateEventSource(EVENT_LOG,"Application");
            }
        }

        /**
        * Setup the enviroment and start thin
        *
        * requirements;
        *
        * A local user account must be created on the system.
        * This install assumes the name of 'redmine'
        *
        * The RUBY_HOME environment variable must point the root of the ruby
        * installation (eg. c:\tools\ruby21-x64
        *
        * The REDMINE_HOME environment variable must point to the root of the redmine
        * installation path
        *
        * Both environment variables must defined in the SYSTEM environment variable set
        * so that the REDMINE user has access to them.
        */
        public void Start() {
            try {
                if (process != null) {
                    Stop();
                }
                process = new Process();

                string redmine_home = process.StartInfo.EnvironmentVariables["REDMINE_HOME"];
                if ((redmine_home == null) || (redmine_home.Length == 0)) {
                    EventLog.WriteEntry(EVENT_LOG, "REDMINE_HOME is not set", EventLogEntryType.Error);
                    return;
                }

                string ruby_home = process.StartInfo.EnvironmentVariables["RUBY_HOME"];
                if ((ruby_home == null) || (ruby_home.Length == 0)) {
                    EventLog.WriteEntry(EVENT_LOG, "RUBY_HOME is not set", EventLogEntryType.Error);
                    return;
                }
                ruby_home += "\\bin";

                process.StartInfo.FileName = ruby_home + "\\ruby.exe";
                process.StartInfo.Arguments = "thin start";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = redmine_home;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;

                String path = process.StartInfo.EnvironmentVariables["PATH"];
                if ((path == null) || (path.Length == 0)) {
                    EventLog.WriteEntry(EVENT_LOG, "Unable to retrieve system PATH", EventLogEntryType.Error);
                    return;
                }
                // put ruby in the front
                path = ruby_home + ";" + path;
                process.StartInfo.EnvironmentVariables["PATH"] = path;

                //EventLog.WriteEntry(EVENT_LOG, "fileName=" + process.StartInfo.FileName);
                //EventLog.WriteEntry(EVENT_LOG, "arguments=" + process.StartInfo.Arguments);
                //EventLog.WriteEntry(EVENT_LOG, "workingdirectory=" + process.StartInfo.WorkingDirectory);
                //EventLog.WriteEntry(EVENT_LOG, "path=" + process.StartInfo.EnvironmentVariables["PATH"]);

                if (!process.Start()) {
                    EventLog.WriteEntry(EVENT_LOG, "process failed to start", EventLogEntryType.Error);
                    return;
                }
 
            } catch (Exception e) {
                EventLog.WriteEntry(EVENT_LOG, "Failed to start thin. " + e.Message, EventLogEntryType.Error);
            }
        }

        /**
        * Kils the currently running ruby-thin process
        */
        public void Stop() {
            try {
                if (process != null) {
                    if (!process.HasExited) {
                        process.Kill();
                    }
                    process = null;
                }
            } catch (Exception e) {
                EventLog.WriteEntry(EVENT_LOG, "Failed to stop thin. " + e.Message, EventLogEntryType.Error);
            }
        }
    }
}
