snip2codeNET
============

.NET Client Wrapper for Snip2Code.<br/><br/>
Snip2Code is a web service for software developers to collect, organize and share code snippets.<br/>
Each user can add his own snippets and decide, for each snippet:<br/>
a) to keep it private (nobody else can see it) <br/>
b) to share it with some other users belonging to a group (all the members of the group can see the snippet, only the administrators of the group and the creator of the snippet can edit it)<br/>
c) to publish it to the world (everyone can see the snippet, only the creator can edit it)<br/>
<br/><br/>
Each snippet is provided with:
<ul>
<li>the title (mandatory), providing a quick understanding of the scope (e.g. the signature of a method)</li>
<li>the description (optional), providing a more extended explanation of the behavior of the code, together with eventual notes (e.g. the documentation of a method)</li>
<li>the code (mandatory), that is, the actual content of the snippet (e.g. the body of a method)</li>
<li>the visibility (mandatory), that defines who can see and who can edit the snippet</li>
<li>a set of tags (optional), which can help in retrieving and discovering the snippet among all the others </li>
<li>a set of properties (optional), i.e. a set of key-value pairs that better define the scope and the environment of the snippet (e.g. the language, the operating system where it can run, the framework used, etc.)</li>
</ul>

The snippets can be published in channels, that are simple collections of snippets with a common topic. <br/>
Each snippet may belong to several channels.


