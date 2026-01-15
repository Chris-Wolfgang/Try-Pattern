# Try Pattern

## Definition

The `Try Pattern` is a design pattern in which code is execute but the caller only cares if the code 
succeeded or failed and the exact reason for failure is not important. For example, a user may need 
to update a record in a database via a web page. If the update fails, they only need to know that it 
failed, not the specific reason for the failure. Connection timeout, login failed, user doesn't have 
access, etc., are irrelevant to the user. They just need to know if the update succeeded or failed. 

A variation of this, is calling a method that returns a value (i.e. a function). In this case if the 
call succeeds, the caller wants the return value from the method. However, if the call fails, the 
caller does not care about the specific reason for the failure. This can be seen in many of the 
built-in types in C#, such as `int.TryParse` or `DateTime.TryParse`. In these methods, if the operation 
succeeds, it returns true and the parsed value is returned via an out parameter. If the method fails 
it returns false and the default value is returned via the output parameter. 


## Library Description

This library makes it easy to implement the Try Pattern in your own code. It has a
`static` `class` called [`Try`](src/Wolfgang.TryPattern/Try.cs) that provides a set
of `static` helper methods that implement the Try Pattern for both
actions (methods that do not return a value) and functions (methods that return a value).
These helper methods, execute the [`Action`](https://learn.microsoft.com/en-us/dotnet/api/system.action-1?view=net-10.0)
or [`Func<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.func-1?view=net-10.0)
passed in, handle any exceptions, and returns a [`Result`](src/Wolfgang.TryPattern/Result.cs)
for actions, or [`Result<T>`](src/Wolfgang.TryPattern/Result.cs) for functions.

## Usage
```csharp

    // Try reading a file async
    var result = await Try.RunAsync(() => File.ReadAllTextAsync(@".\sample.txt"));

    // If file read failed, print the error message
    if (result.Succeeded)
    {
        Console.WriteLine("File contents:");
        Console.WriteLine(result.Value);
    }
    else
    {
        Console.WriteLine("Error reading file: " + result.ErrorMessage);
    }
```

## Other Uses for `Result` and `Result<T>`

The `Result` and `Result<T>` types can be used outside of the Try Pattern
The classes work well as return types for methods that need to indicate success or failure
For example consider the following example of a method that validates user input:
```csharp
public Result ValidateUserInput(string input)
{
	if (string.IsNullOrWhiteSpace(input))
	{
		return Result.Failure("Input cannot be empty.");
	}
	if (input.Length < 5)
	{
		return Result.Failure("Input must be at least 5 characters long.");
	}
	return Result.Success();
}
```

A more complex example is a method that processes an order:
```csharp

public class OrderProcessingService
{

	private readonly HttpClient httpClient;
	private readonly string endpoint;

	public OrderProcessingService(HttpClient httpClient, string endpoint)
	{
		if (httpClient == null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}
		if (endpoint == null)
		{
			throw new ArgumentNullException(nameof(endpoint));
		}
		this.httpClient = httpClient;
		this.endpoint = endpoint;
	}

	async Task<Result> CreateOrder(Order order)
	{
		
		if (order == null)
		{
			// Throwing exception here because this is a programming error
			throw new ArgumentNullException(nameof(order));
		}

		var payload = JsonSerializer.Serialize(order);
		using var content = new StringContent(payload, Encoding.UTF8, "application/json");
		using var response = await httpClient.PostAsync(endpoint, content).ConfigureAwait(false);


		if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
		{
			var reason = string.IsNullOrWhiteSpace(response.ReasonPhrase) ? "Client Error" : response.ReasonPhrase;
			return Result.Failure($"Failed to create order: {(int)response.StatusCode} ({reason}).");
		}

		return Result.Failure($"Failed to create order: {(int)response.StatusCode} ({response.StatusCode}).");
	}


	async Task<Result<Order>> GetOrder(string orderId)
	{
		
		if (string.IsNullOrWhiteSpace(orderId))
		{
			// Throwing exception here because this is a programming error
			throw new ArgumentNullException(nameof(orderId));
		}
		using var response = await httpClient.GetAsync($"{endpoint}/{orderId}").ConfigureAwait(false);
		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var confirmation = JsonSerializer.Deserialize<OrderConfirmation>(content);
			return Result<OrderConfirmation>.Success(confirmation);
		}
		return Result<OrderConfirmation>.Failure($"Failed to get order confirmation. Status code: {response.StatusCode}");
	}
}
```




## A Note on Using Exceptions for Control Flow

Exceptions are generally intended for exceptional circumstances and not for regular control flow.
They are expensive to create and handle, and using them for control flow can lead to code that is harder to follow and understand

If you have a function that searches for a specific value, it is better to return some value that indicates not found
such as `null`, `false`, or an empty collection, in the case where more than one result could be returned, rather than throwing an exception when the value is not found.

When creating code that uses the Try Pattern, consider whether exceptions are the best way to indicate failure.
In the case where a user searches for specific data, it is very common that the value
they are searching for does not exist. As such, don't throw an exception when this happens but rather return a value that indicates not found.

However, failing to connect to a database or a webservice are both
situations where exceptions are appropriate as these are not common occurrences and indicate something unexpected happened.

Reading a config file or some required other application file that should always be present 
is another good example of when to use exceptions for control flow. However, 
a application that reads from a user specified file can expect that the file may not exist 
and so in that case throwing an exception is probably not the best approach. 
