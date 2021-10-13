# Interface

Open Banking Connector is an in-process functional library providing abstract integration with banks. 

Our aim is to provide a simple, low-risk developer experience to help users make payments and access bank APIs as fast as possible. To this end we will make a DSL via standard C# fluent interface techniques.

```IRequestBuilder``` is an injectable .NET type that serves as the DSL's root object. As an example, take the following expression:

```csharp
var resp = await requestBuilder.Payment().With(ConsentInfo.FromJwt("<some jwt>"))
                .For(123.43).Reference("Your payment ref")
                .From(AccountInfo.Create("01-02-03", "12345677"))
                .To(AccountInfo.Create("Joe Bloggs", "02-03-04", "77665544"))
                .Submit();
```

This interface style is the facade that user code will use. 

