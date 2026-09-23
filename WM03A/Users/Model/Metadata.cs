using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class MetadataData
    {
        public ulong SequenceMeta { get; set; }
        public ushort NextLatchSaveIndex { get; set; }
        public ushort NextLatchLoadIndex { get; set; }
        public ushort LatchCount { get; set; }
        public ushort NextEventSaveIndex { get; set; }
        public ushort NextEventLoadIndex { get; set; }
        public ushort EventCount { get; set; }
        public ushort NextLogSaveIndex { get; set; }
        public ushort LogCount { get; set; }

        public ulong SequenceRuntime { get; set; }
        public byte ResetCount { get; set; }
        public List<PulseCountData> PulseCounts { get; set; } = new List<PulseCountData>();
    }

    public class PulseCountData
    {
        public ulong ForwardPulseCount { get; set; }
        public ulong ReversePulseCount { get; set; }
    }
}
