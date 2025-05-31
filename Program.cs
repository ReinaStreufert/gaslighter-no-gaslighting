
using gaslighter_no_gaslighting;

var manager = new SnapshotManager("persistence.json", "Few-Soil-8566", string.Empty, string.Empty);
await manager.SnapshotRoutineAsync(TimeSpan.FromHours(1));