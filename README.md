Upload large files to Azure Blob Storage through Azure Web Role WebApi services and Azure Cache Service
-----------

The main aim of this project is to provide a working scalable solution for uploading large files to server. In this modern 
era, when web development taking modern cloud workflows, it is time for us to solve the traditional problems which we 
encounter in uploading files in a more easier and simpler way. We pretty much experience traditional ASP.Net Web 
problems at the time of working with file uploads which range from 4MB to hunderds of MBs. Even though ASP.Net 
allowed some provisions (which would make uploading files easier) by making some 
Config changes (for example - maxRequestLength and executionTimeOut), still server timeouts and long running connections 
are creating chaos in this particular requirements.

So we wanted to present a simple and elegant solution throught Microsoft's Azure Cloud Services. Please follow below 
description.

1. In this solution, we provide TWO WebApi endpoints - FileApi/Get and FileApi/Post.
2. Any client tenant which wants to use our upload service, first makes a request to Get endpoint. 
3. On successful request, client tenant would be getting an unique ID to start the transaction.
4. Then client tenant can split the file into chunks (typically Byte arrays) and can send it to Post endpoint. 
Following are mandatory parameters for this request - 
	* FileId => Unique Id from Step (3).
	* ChunkId => Unique 8 digit Integer (for example - 00000001, 00000002...) to identify which chunk is being received.
	* IsCompleted => Boolean value. True in case if last chunk is attachecd to request. False in case of any other chunk.
5. On successful request, server will process the request and sends back a 200 OK HTTP response. In case of any error, 
there will be 500 Internal Server Error. Here to protect data integrity, we recommend the client to RE-SEND the failed
chunk request back to service again.

Specialities:
-------------
1. WebApi Clients can send chunks of data in any order. We use Windows Azure Cache Service to keep track of arriving chunks and
only commit if client notifies service about the final chunk.
2. WebApi Clients have flxibility to send chunks of any size. But we do recommend to follow Azure BLOCK BLOB storage size limitations for 
seamless integration.
3. Completely scalable Web Role architecture.
4. Loosely couple components because if IoC implementation using Unity framework. We have operations layer through which front
end WebApi is going to communicate with Cache and Blob Repositories.
5. Default protection of Transaction Integrity by Blob Storage.


Technical Stack:
---------------
> 1. Azure WebRoles
> 2. Azure Cache Service
> 3. Azure Blob Storage
> 4. ASP.Net MVC4 Web Application
> 5. .Net 4.5 Console Application (Test)

Transaction Integrity:
-------------
> Q) What if Nth chunk got never uploaded?
> Q) What if there was an Azure Cache exception for Nth chunk?
> Q) What if there was an error while commiting all chunks using PutBlockList?

For all above questions, nswer is going to be very simple and it is as follows - 
** "JUST RE-SEND THE SAME CHUNK AGAIN" **
Transaction commitment scope for Azure Blob storage is very flexible. To be very specific, sending the same chunk with
same ChunkId, would override a block (if already exists) or creates a new one (if it doesn't exist).


Important Notes:
-------------
1. Sending last byte chunk (with IsCompleted:true) is very important. Or else all the other chunks will not be commited to Azure Blob Storage.\
And one should remember sending last chunk AT LAST is very important, or else a premature commitment on intermediae chunks
will happen which would result in loss of data integrity and there by corrupting the complete blob.
2. Sample C# console test client has been included in the solution. Please try it out. Presently it support 100KB chunks
It can be extended further more if required.
3. Azure ConnectionStrings are obsolete. Please replace config information with your own settings to make this solution work.
	* Update CSCFG files in Azure Project with specific Blob storage credentials.
	* Update Web.Config with Azure Cache specific credentials.

TODO Tasks:
-----------
> 1. Build a simple Web HTML page which would use JQuery to invoke this WebApi and upload file in chunks.
> 2. Implement Data validation on server side.
> 3. Implement Basic Authentication with Client Certificate.
> 4. Unit test WebApi endpoints.
> 5. Testing completed with 100MB file. Still we need to test with 1GB file.

Test Run:
----------
![Test Iteration1](https://raw.github.com/DreamingDevs/large-file-upload-to-azure-blob-using-webapi/master/Images/Test-Iteration1.png "Test Iteration1")

Credits:
-----------
> 1. Mohan (Code Ontributor). Also do not forget to see contributors section. It is all their hardwork.
> 2. Gaurav Mantri [Azure MVP], for helping me understand Blob Storage through his blog.
> 3. MSDN Documentation.

Disclaimer:
-----------
All ideas and implementations made in this project are out of my own self thought process. Credits are given to
respective people, who directly or indirectly helped me in understanding the concepts of latest technologies.
It is also to be remembered that views/suggestions/practices involved in this project are strictly of my own 
representations. Present project/code/[Any thing to that matter which is from DreamingDevs] doesn't relate to my 
present/past employers in any way. Present project may suit some of the real world demands and also may not, 
it’s purely the responsibility of the person who intended to use any of the material placed in this project work.
