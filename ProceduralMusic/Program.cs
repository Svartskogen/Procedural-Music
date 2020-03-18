using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Standards;

namespace Procedural_Music
{
    class Program
    {
        static void Main(string[] args)
        {
            var pattern = MelodyGenerator.ReturnParametricMelody(
                DateTime.Now.Millisecond, ScaleIntervals.Major, "Natural Minor", NoteName.ASharp,
                MelodyGenerator.TimeMood.Chill, 55, 2, new int[] { 2, 4 }
                );

            MidiFile midiFile = pattern.ToFile(TempoMap.Default);
            midiFile.Write("test.mid", true, MidiFileFormat.SingleTrack);
            Console.ReadLine();
        }
    }
}
