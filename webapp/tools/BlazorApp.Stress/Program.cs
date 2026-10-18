// Stress tool (plan section 4): starts several real servers sharing one data folder, sends
// many colliding changes through all of them at once, then checks that every acknowledged
// change is in the data, every record reads back, and the workflow rules held.
//
//   dotnet run --project webapp/tools/BlazorApp.Stress -- --strategy PerRecordFile --servers 3
//
// Writes result.json and summary.md to the run folder; see StressOptions.Usage for options.

using BlazorApp.Stress;

using var cancel = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // stop the servers on the way out instead of leaving them running
    cancel.Cancel();
};

return await StressProgram.MainAsync(args, Console.Out, Console.Error, cancel.Token);
