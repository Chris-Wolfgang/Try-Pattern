#if !NET6_0_OR_GREATER
using System;
using System.Threading.Tasks;
#endif
namespace Wolfgang.TryPattern.Tests;

public class RunAsyncFuncTests // TODO Rename class and file
{
	[Fact]
	public async Task RunAsync_Func_when_passed_null_throws_ArgumentNullException()
	{
		// Arrange
		Func<Task<int>>? nullFunction = null;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => Try.RunAsync(nullFunction!));
	}

	[Fact]
	public async Task RunAsync_Func_int_when_successful_returns_successful_Result()
	{
		// Arrange
		const int expectedValue = 42;

		static async Task<int> Function()
		{
			await Task.Yield();
			return expectedValue;
		}

		// Act
		var result = await Try.RunAsync((Func<Task<int>>)Function);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.Empty(result.ErrorMessage);
		Assert.Equal(expectedValue, result.Value);
	}



	[Fact]
	public async Task RunAsync_Func_nullable_int_when_successful_returns_successful_Result()
	{
		// Arrange
		int? expectedValue = 42;

		async Task<int?> Function()
		{
			await Task.Yield();
			return expectedValue;
		}

		// Act
		var result = await Try.RunAsync((Func<Task<int?>>)Function);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.Empty(result.ErrorMessage);
		Assert.Equal(expectedValue, result.Value);
	}



	[Fact]
	public async Task RunAsync_Func_string_when_successful_returns_successful_Result()
	{
		// Arrange
		const string expectedValue = "Hello, Async World!";

		static async Task<string?> Function()
		{
			await Task.Yield();
			return expectedValue;
		}

		// Act
		var result = await Try.RunAsync(Function);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(result.Failed);
		Assert.Empty(result.ErrorMessage!);
		Assert.Equal(expectedValue, result.Value);
	}



	[Fact]
	public async Task RunAsync_Func_int_when_exception_is_thrown_returns_failed_Result_with_correct_properties()
	{
		// Arrange
		static async Task<int> Function()
		{
			await Task.Yield();
			throw new InvalidOperationException("Test exception");
		}

		// Act
		var result = await Try.RunAsync((Func<Task<int>>)Function);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(result.Failed);
		Assert.Equal("Test exception", result.ErrorMessage);
		var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
		Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
	}



	[Fact]
	public async Task RunAsync_Func_nullable_int_when_exception_is_thrown_returns_failed_Result_with_correct_properties()
	{
		// Arrange
		static async Task<int?> Function()
		{
			await Task.Yield();
			throw new InvalidOperationException("Test exception");
		}

		// Act
		var result = await Try.RunAsync((Func<Task<int?>>)Function);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.NotEmpty(result.ErrorMessage);
		var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
		Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
	}



	[Fact]
	public async Task RunAsync_Func_string_when_exception_is_thrown_returns_failed_Result_with_correct_properties()
	{
		// Arrange
		static async Task<string?> Function()
		{
			await Task.Yield();
			throw new InvalidOperationException("Test exception");
		}

		// Act
		var result = await Try.RunAsync(Function);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.NotEmpty(result.ErrorMessage);
		var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
		Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_when_passed_null_throws_ArgumentNullException()
	{
		// Arrange
		Func<Task<int>>? nullFunction = null;
		var cts = new CancellationTokenSource();

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => Try.RunAsync(nullFunction!, cts.Token));
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_int_when_successful_returns_successful_Result()
	{
		// Arrange
		const int expectedValue = 42;

		static async Task<int> Function()
		{
			await Task.Yield();
			return expectedValue;
		}

		var cts = new CancellationTokenSource();

		// Act
		var result = await Try.RunAsync((Func<Task<int>>)Function, cts.Token);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.Empty(result.ErrorMessage);
		Assert.Equal(expectedValue, result.Value);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_nullable_int_when_successful_returns_successful_Result()
	{
		// Arrange
		int? expectedValue = 42;

		async Task<int?> Function()
		{
			await Task.Yield();
			return expectedValue;
		}

		var cts = new CancellationTokenSource();


		// Act
		var result = await Try.RunAsync((Func<Task<int?>>)Function, cts.Token);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.Empty(result.ErrorMessage);
		Assert.Equal(expectedValue, result.Value);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_string_when_successful_returns_successful_Result()
	{
		// Arrange
		const string expectedValue = "Hello, Async World!";

		static async Task<string?> Function()
		{
			await Task.Yield();
			return expectedValue;
		}

		var cts = new CancellationTokenSource();


		// Act
		var result = await Try.RunAsync(Function, cts.Token);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(result.Failed);
		Assert.Empty(result.ErrorMessage!);
		Assert.Equal(expectedValue, result.Value);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_int_when_exception_is_thrown_returns_failed_Result_with_correct_properties()
	{
		// Arrange
		static async Task<int> Function()
		{
			await Task.Yield();
			throw new InvalidOperationException("Test exception");
		}

		var cts = new CancellationTokenSource();


		// Act
		var result = await Try.RunAsync((Func<Task<int>>)Function, cts.Token);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(result.Failed);
		Assert.Equal("Test exception", result.ErrorMessage);
		var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
		Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_nullable_int_when_exception_is_thrown_returns_failed_Result_with_correct_properties()
	{
		// Arrange
		static async Task<int?> Function()
		{
			await Task.Yield();
			throw new InvalidOperationException("Test exception");
		}

		var cts = new CancellationTokenSource();


		// Act
		var result = await Try.RunAsync((Func<Task<int?>>)Function, cts.Token);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.NotEmpty(result.ErrorMessage);
		var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
		Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_string_when_exception_is_thrown_returns_failed_Result_with_correct_properties()
	{
		// Arrange
		static async Task<string?> Function()
		{
			await Task.Yield();
			throw new InvalidOperationException("Test exception");
		}

		var cts = new CancellationTokenSource();


		// Act
		var result = await Try.RunAsync(Function, cts.Token);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(result.Failed);
		Assert.NotNull(result.ErrorMessage);
		Assert.NotEmpty(result.ErrorMessage);
		var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
		Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
	}



	[Fact]
	public async Task RunAsync_Func_CancellationToken_int_when_cancellation_is_requested_throws_TaskCanceledException()
	{
		// Arrange
		using var cts = new CancellationTokenSource();
		var functionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

		async Task<int> Function()
		{
			functionStarted.TrySetResult(true);
			await Task.Delay(Timeout.Infinite, cts.Token);
			return 42;
		}

		var task = Try.RunAsync((Func<Task<int>>)Function, cts.Token);

		// Act
		await functionStarted.Task;
		cts.Cancel();

		await Assert.ThrowsAsync<TaskCanceledException>(() => task);
	}
}
