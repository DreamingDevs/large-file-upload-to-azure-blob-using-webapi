Upload large files using JQuery to Azure Blob Storage through Azure Web Role WebApi services
-----------

The main aim of this project is to provide a working scalable solution to large file uploads to server. In this modern era, when web development taking modern cloud workflows, it is time for us to evolve the traditional problems which we encounter in uploading files. We pretty much experience traditional ASP.Net Web problems at the time of working with file uploads. Even though ASP.Net allowed some provisions (which would make uploading files easier) by making some Config changes (for example - maxRequestLength and executionTimeOut), still server timeouts and long running connections are creating chaos in this particular requirements.

So we wanted to preset a simple and elegant solution throught Microsoft's Azure Cloud Services. Please follow below description.

1. In this solution, we provide TWO WebApi endpoints - FileApi/Get and FileApi/Post.
2. Any client tenant which wants to use our upload service, first makes a request to Get endpoint. 
3. One successful request, client tenant would be getting an unique ID to start the transaction.
4. Then client tenant can split the file into chunks (typically Byte arrays) and can send it to Post endpoint. Following are mandatory parameters for this request - 
	* FileId => Unique Id from Step (3).
	* ChunkId => Unique 7 digit Integer (for example - 0000001, 0000002...) to identify which chunk is being received.
	* IsCompleted => Boolean value. True in case last chunk is attachecd to request. False in case of any other chunk.
5. On successful request, server will process the request and sends back a 200 OK HTTP response. In case of any error, there will be 500 Internal Server Error.

IMPORTANT NOTES:
-------------
1. Sending last byte chunk (with IsCompleted:true) is very important. Or else all the other chunks will not be commited to Azure Blob Storage.
2. Chunks can be send in **ANY** order. We have a File Chunk Tracker in place to keep track of received chunks.
3. Sampel C# console test client has been included in the solution. Please try it out. Presently it support 100KB chunks
It can be extended further more if required.

TODO Tasks:
-----------
> 1. Build a simple Web HTML page which would use JQuery to invoke this WebApi and upload file in chunks.
> 2. Implement Validation on server side.
> 3. Implement Basic Authentication with Client Certificate.
> 4. Unit test WebApi endpoints.
> 5. Testing completed with 100MB file. Still we need to test with 1GB file.

TEST RUN:
----------
![Test Iteration1](https://raw.github.com/DreamingDevs/large-file-upload-to-azure-blob-using-webapi/master/Images/Test-Iteration1.png "Test Iteration1")

CREDITS:
-----------
> 1. Do not forget to see contributors section. It is all their hardwork.
> 2. Gaurav Mantri [Azure MVP], for helping me understand Blob Storage through his blog.
> 3. MSDN Documentation.

DISCLAIMER:
-----------
All ideas and implementations made in this project are out of my own self thought process. Credits are given to
respective people, who directly or indirectly helped me in understanding the concepts of latest technologies.
It is also to be remembered that views/suggestions/practices involved in this project are strictly of my own 
representations. Present project/code/[Any thing to that matter which is from DreamingDevs] doesn't relat to my 
present/past employers in any way. Present project may suit some of the real world demands and also may not, 
it’s purely the responsibility of the person who intended to use any of the material placed in this project work.
