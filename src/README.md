

POST http://localhost:7133/api/HandleBlobCreatedEventHttpTrigger
```
  "id": "1135e109-801e-003e-45dc-30d82006730c",
  "source": "/subscriptions/c1bda0fc-c0e3-4b89-be00-0524b96b7b0f/resourceGroups/rg-gioai-lab-ae-01/providers/Microsoft.Storage/storageAccounts/devstoreaccount1",
  "specversion": "1.0",
  "type": "Microsoft.Storage.BlobCreated",
  "subject": "/blobServices/default/containers/books/blobs/software architecture with csharp.pdf",
  "time": "2024-11-07T06:17:03.7185226Z",
  "data": {
    "api": "PutBlob",
    "clientRequestId": "033f9266-8b5d-4962-a551-49b67203200f",
    "requestId": "1135e109-801e-003e-45dc-30d820000000",
    "eTag": "0x8DCFEF3CC4255E5",
    "contentType": "application/pdf",
    "contentLength": 804915,
    "blobType": "BlockBlob",
    "accessTier": "Default",
    "url": "https://devstoreaccount1.blob.core.windows.net/books/software architecture with csharp.pdf",
    "sequencer": "0000000000000000000000000001137000000000027253f3",
    "storageDiagnostics": {
      "batchId": "0e71cc9e-7006-0058-00dc-309700000000"
    }
  }
}
```