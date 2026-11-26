type: breaking

Source-level only, from the new `Try.RunAsync(Func<Task>, CancellationToken)`: a non-`async` lambda whose end cannot be reached (an endless worker loop, a bare `() => throw ...`) now binds to it and runs on the caller's thread until it ends instead of on the thread pool (cast it to `Action` to keep the old behaviour), and `Try.RunAsync(null)` no longer compiles (cast the `null`).
