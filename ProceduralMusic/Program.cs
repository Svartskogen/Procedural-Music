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
        const string VERSION = "1.0.1";
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(VERSION);
                ParametricTracksMenu();
                Console.WriteLine("Press enter to reload the generator");
                Console.ReadLine();
            }
        }

        private static void ParametricTracksMenu()
        {
            Console.WriteLine("Welcome to the parametric musical tracks generator");
            Console.WriteLine("Choose a track type:");
            Console.WriteLine("1 - Melody (notes)");
            Console.WriteLine("2 - Progression (chords)");
            switch (Console.ReadLine())
            {
                case "1":
                {
                    ParametricMelodyMenu();
                    break;
                }
                case "2":
                {
                    ParametricProgressionMenu();
                    break;
                }
            }
        }
        private static void ParametricMelodyMenu()
        {
            Pattern pattern = null;
            Console.WriteLine("Random Melody Generator");
            Console.WriteLine("Choose a melody generation method:");
            Console.WriteLine("1 - parametric");
            Console.WriteLine("2 - seeded");
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                {
                    pattern = InputParametricMelody();
                    if(pattern == null)
                    {
                        return;
                    }
                    break;
                }
                case "2":
                {
                    Console.Clear();
                    Console.WriteLine("enter a seed and an amount of notes");
                    pattern = MelodyGenerator.RandomParametricStandaloneMelody(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
                    break;
                }
                default:
                {
                    Console.WriteLine("error");
                    return;
                }
            }
            Console.WriteLine("Saving composition to file output.mid");
            MidiFile midiFile = pattern.ToFile(TempoMap.Default);

            midiFile.Write("output.mid", true, MidiFileFormat.SingleTrack);
        }
        static Pattern InputParametricMelody()
        {
            Console.WriteLine("Enter a random numeric seed for the notes:");
            int seed = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter a scale for the notes:");
            Console.WriteLine("1: Major");
            Console.WriteLine("2: Natural Minor");
            IEnumerable<Interval> scaleIntervals = null;
            string scaleName = null;
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                {
                    scaleIntervals = ScaleIntervals.Major;
                    scaleName = "Major";
                    break;
                }
                case "2":
                {
                    scaleIntervals = ScaleIntervals.Minor;
                    scaleName = "Natural Minor";
                    break;
                }
                default:
                {
                    Console.WriteLine("error");
                    return null;
                }
            }

            Console.WriteLine("Enter a tonic for the scale:");
            Console.WriteLine("0: C\n 1:C#\n 2:D\n 3:D#\n 4:E\n 5:F\n 6:F#\n 7:G\n 8:G#\n 9:A\n 10:A#\n 11:B");
            NoteName tonic = (NoteName)Convert.ToInt32(Console.ReadLine()); 

            Console.WriteLine("Enter a time mood preset:");
            Console.WriteLine("0:Progression \n1:Dull \n2:Chill \n3:Complex \n4:Dissonant");
            MelodyGenerator.TimeMood timeMood = (MelodyGenerator.TimeMood)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the amount of notes you want to generate:");
            int notesAmount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the maximum variation between the previous note and the next relative to the scale:");
            int stepVariance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the amount of octaves you wish to use:");
            int[] octaves = new int[Convert.ToInt32(Console.ReadLine())];
            Console.WriteLine("Enter each octave value:");
            for(int i = 0; i < octaves.Length; i++)
            {
                octaves[i] = Convert.ToInt32(Console.ReadLine());
            }
            return MelodyGenerator.ParametricStandaloneMelody(seed, scaleIntervals, scaleName, tonic, timeMood, notesAmount, stepVariance, octaves);
        }
        private static void ParametricProgressionMenu()
        {
            Pattern pattern = null;
            Console.WriteLine("Random Chord Progression Generator");
            Console.WriteLine("Choose a progression generation method:");
            Console.WriteLine("1 - parametric");
            Console.WriteLine("2 - seeded");
            Console.WriteLine("3 - scale chords");
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                {
                    pattern = InputParametricProgression();
                    if (pattern == null)
                    {
                        return;
                    }
                    break;
                }
                case "2":
                {
                    Console.Clear();
                    Console.WriteLine("enter a seed and an amount of chords");
                    pattern = MelodyGenerator.RandomParametricStandaloneChords(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
                    break;
                }
                case "3":
                {
                    Console.Clear();
                    pattern = InputScaleChords();
                    break;
                }
                default:
                {
                    Console.WriteLine("error");
                    return;
                }
            }
            Console.WriteLine("Saving composition to file output.mid");
            MidiFile midiFile = pattern.ToFile(TempoMap.Default);

            midiFile.Write("output.mid", true, MidiFileFormat.SingleTrack);
        }
        static Pattern InputParametricProgression()
        {
            Console.WriteLine("Enter a random numeric seed for the chords:");
            int seed = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter a scale for the chords:");
            Console.WriteLine("1: Major");
            Console.WriteLine("2: Natural Minor");
            IEnumerable<Interval> scaleIntervals = null;
            ChordQuality chordQuality;
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                {
                    scaleIntervals = ScaleIntervals.Major;
                    chordQuality = ChordQuality.Major;
                    break;
                }
                case "2":
                {
                    scaleIntervals = ScaleIntervals.Minor;
                    chordQuality = ChordQuality.Minor;
                    break;
                }
                default:
                {
                    Console.WriteLine("error");
                    return null;
                }
            }

            Console.WriteLine("Enter a tonic for the scale:");
            Console.WriteLine("0:C\n 1:C#\n 2:D\n 3:D#\n 4:E\n 5:F\n 6:F#\n 7:G\n 8:G#\n 9:A\n 10:A#\n 11:B");
            NoteName tonic = (NoteName)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter a time mood preset:");
            Console.WriteLine("0:Progression \n1:Dull \n2:Chill \n3:Complex \n4:Dissonant");
            MelodyGenerator.TimeMood timeMood = (MelodyGenerator.TimeMood)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the progression type");
            Console.WriteLine("0:Random \n1:Coherent \n2:Popular");
            MelodyGenerator.ChordProgressionType progressionType = (MelodyGenerator.ChordProgressionType)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the amount of chords you want to generate:");
            int notesAmount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the maximum variation between the previous chord and the next relative to the scale:");
            Console.WriteLine("Only used by the Random progression type");
            int stepVariance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the amount of octaves you wish to use:");
            int[] octaves = new int[Convert.ToInt32(Console.ReadLine())];
            Console.WriteLine("Enter each octave value:");
            for (int i = 0; i < octaves.Length; i++)
            {
                octaves[i] = Convert.ToInt32(Console.ReadLine());
            }
            return MelodyGenerator.ParametricStandaloneChords(seed, scaleIntervals, chordQuality, tonic, timeMood,progressionType, notesAmount, stepVariance, octaves);
        }
        static Pattern InputScaleChords()
        {
            Console.WriteLine("Enter a scale for the notes:");
            Console.WriteLine("1: Major");
            Console.WriteLine("2: Natural Minor");
            IEnumerable<Interval> scaleIntervals = null;
            ChordQuality chordQuality;
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                {
                    scaleIntervals = ScaleIntervals.Major;
                    chordQuality = ChordQuality.Major;
                    break;
                }
                case "2":
                {
                    scaleIntervals = ScaleIntervals.Minor;
                    chordQuality = ChordQuality.Minor;
                    break;
                }
                default:
                {
                    Console.WriteLine("error");
                    return null;
                }
            }

            Console.WriteLine("Enter a tonic for the scale:");
            Console.WriteLine("0:C\n 1:C#\n 2:D\n 3:D#\n 4:E\n 5:F\n 6:F#\n 7:G\n 8:G#\n 9:A\n 10:A#\n 11:B");
            NoteName tonic = (NoteName)Convert.ToInt32(Console.ReadLine());
            return MelodyGenerator.ScaleChords(chordQuality,scaleIntervals,tonic);
        }
    }
}
