type: internal

Benchmarks now cover `Result.Flatten`, `Result.AnyFailed` and `Result.AllSucceeded`, and a test enforces that `Result.Flatten` of all successes allocates nothing.
