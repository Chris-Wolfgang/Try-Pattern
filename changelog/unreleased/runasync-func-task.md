type: feature

Add `Try.RunAsync(Func<Task>, CancellationToken)`, so an `async () => { ... }` lambda with no return value is awaited and its exception becomes a failed `Result` instead of binding to the `Action` overload as `async void`, reporting success early and crashing the process.
