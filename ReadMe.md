<p>RedmineService is a C# Windows Service application that is used to control an instance of Redmine running on Windows via Apache and Thin.</p>
<p>The application was built Visual Studio 2015 (Community Version) and tested on Windows 7 using a version of Apache 2.4.16 compiled for Windows.</p>
<p>Requirements:</p>
<li>The service must be run with a local user account, not as LOCAL_SYSTEM, LOCAL_SERVICE or NETWORK_SERVICE.  The install will prompt for a user and password.</li>
<li>A RUBY_HOME environment variable must be defined and must point the root of the Ruby installation directory (eg. c:\tools\ruby21-x64</li>
<li>A REDMINE_HOME environment variable must point to the root of the Redmine installation path.</li>
<li>Both environment variables must defined in the SYSTEM environment variable set so that the REDMINE user has access to them.</li>
<br>
<p>The application assumes you are already able to launch Redmine using 'thin start'.  If that doesn't work, then this service won't work either.</p>
<p>After building the application, you can install the application using 'InstallUtil'.  Use 'InstallUtil RedmineService.exe' to install the service and 'InstallUtil /u RedmineService.exe' to uninstall it.  Both instances must be run 'as adminstrator'</p>

