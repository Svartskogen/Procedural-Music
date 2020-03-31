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
            /*var pattern = MelodyGenerator.ReturnParametricMelody(
                DateTime.Now.Millisecond, ScaleIntervals.Major, "Natural Minor", NoteName.ASharp,
                MelodyGenerator.TimeMood.Chill, 55, 2, new int[] { 2, 4 }
                );*/

            //var pattern = MelodyGenerator.ReturnRandomParametricMelody(DateTime.Now.Millisecond * DateTime.Now.Second, 50);
            while (true)
            {
                Console.Clear();
                ParametricTracksMenu();
                Console.WriteLine("Press enter to reload the generator");
                Console.ReadLine();
            }
            /*var pattern = MelodyGenerator.ParametricStandaloneChords(DateTime.Now.Millisecond, ScaleIntervals.Minor, ChordQuality.Minor, NoteName.A, MelodyGenerator.TimeMood.Dull, MelodyGenerator.ChordProgressionType.Random, 20, 2, new int[]{3, 2, 4});
            MidiFile midiFile = pattern.ToFile(TempoMap.Default);
            
            midiFile.Write("out.mid", true, MidiFileFormat.SingleTrack);
            Console.ReadLine();*/
        }

        private static void ParametricTracksMenu()
        {
            Console.WriteLine("Welcome to the parametric musical tracks generator");
            Console.WriteLine("Chose a track type:");
            Console.WriteLine("1 -Melody (notes)");
            Console.WriteLine("2 -Progression (chords)");
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
            Console.WriteLine("Welcome to the random Melody Generator");
            Console.WriteLine("Chose a melody generation method:");
            Console.WriteLine("1 -parametric");
            Console.WriteLine("2 -seeded");
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
                    Console.WriteLine("ingresa una seed y luego una cantidad de notas");
                    pattern = MelodyGenerator.RandomParametricStandaloneMelody(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
                    break;
                }
                default:
                {
                    Console.WriteLine("error");
                    return;
                }
            }
            Console.WriteLine("Creando archivo output.mid");
            MidiFile midiFile = pattern.ToFile(TempoMap.Default);

            midiFile.Write("output.mid", true, MidiFileFormat.SingleTrack);
        }
        static Pattern InputParametricMelody()
        {
            Console.WriteLine("Ingrese una seed para las notas:");
            int seed = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese una escala para las notas:");
            Console.WriteLine("-1: Major");
            Console.WriteLine("-2: Natural Minor");
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

            Console.WriteLine("Ingrese una tonica para la escala:");
            Console.WriteLine("0: C\n 1:C#\n 2:D\n 3:D#\n 4:E\n 5:F\n 6:F#\n 7:G\n 8:G#\n 9:A\n 10:A#\n 11:B");
            NoteName tonic = (NoteName)Convert.ToInt32(Console.ReadLine()); 

            Console.WriteLine("Ingrese un modo de tiempo:");
            Console.WriteLine("0:Progression \n1:Dull \n2:Chill \n3:Complex \n4:Dissonant");
            MelodyGenerator.TimeMood timeMood = (MelodyGenerator.TimeMood)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese la cantidad de notas que quiere generar:");
            int notesAmount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese la variacion maxima entre notas con respecto a la escala:");
            int stepVariance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese la cantidad de octavas que desea usar:");
            int[] octaves = new int[Convert.ToInt32(Console.ReadLine())];
            Console.WriteLine("Ingrese las octavas:");
            for(int i = 0; i < octaves.Length; i++)
            {
                octaves[i] = Convert.ToInt32(Console.ReadLine());
            }
            return MelodyGenerator.ParametricStandaloneMelody(seed, scaleIntervals, scaleName, tonic, timeMood, notesAmount, stepVariance, octaves);
        }
        private static void ParametricProgressionMenu()
        {
            Pattern pattern = null;
            Console.WriteLine("Welcome to the random Chord Progression Generator");
            Console.WriteLine("Chose a progression generation method:");
            Console.WriteLine("1 -parametric");
            Console.WriteLine("2 -seeded");
            Console.WriteLine("3 -scale chords");
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
                    Console.WriteLine("ingresa una seed y luego una cantidad de notas");
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
            Console.WriteLine("Creando archivo output.mid");
            MidiFile midiFile = pattern.ToFile(TempoMap.Default);

            midiFile.Write("output.mid", true, MidiFileFormat.SingleTrack);
        }
        static Pattern InputParametricProgression()
        {
            Console.WriteLine("Ingrese una seed para las notas:");
            int seed = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese una escala para las notas:");
            Console.WriteLine("-1: Major");
            Console.WriteLine("-2: Natural Minor");
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

            Console.WriteLine("Ingrese una tonica para la escala:");
            Console.WriteLine("0:C\n 1:C#\n 2:D\n 3:D#\n 4:E\n 5:F\n 6:F#\n 7:G\n 8:G#\n 9:A\n 10:A#\n 11:B");
            NoteName tonic = (NoteName)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese un modo de tiempo:");
            Console.WriteLine("0:Progression \n1:Dull \n2:Chill \n3:Complex \n4:Dissonant");
            MelodyGenerator.TimeMood timeMood = (MelodyGenerator.TimeMood)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese un tipo de progresion:");
            Console.WriteLine("0:Random \n1:Coherent \n2:Popular");
            MelodyGenerator.ChordProgressionType progressionType = (MelodyGenerator.ChordProgressionType)Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese la cantidad de acordes que quiere generar:");
            int notesAmount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese la variacion maxima entre acordes con respecto a la escala:");
            Console.WriteLine("Solo util para tipo de progresion Random");
            int stepVariance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Ingrese la cantidad de octavas que desea usar:");
            int[] octaves = new int[Convert.ToInt32(Console.ReadLine())];
            Console.WriteLine("Ingrese las octavas:");
            for (int i = 0; i < octaves.Length; i++)
            {
                octaves[i] = Convert.ToInt32(Console.ReadLine());
            }
            return MelodyGenerator.ParametricStandaloneChords(seed, scaleIntervals, chordQuality, tonic, timeMood,progressionType, notesAmount, stepVariance, octaves);
        }
        static Pattern InputScaleChords()
        {
            Console.WriteLine("Ingrese una escala para las notas:");
            Console.WriteLine("-1: Major");
            Console.WriteLine("-2: Natural Minor");
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

            Console.WriteLine("Ingrese una tonica para la escala:");
            Console.WriteLine("0:C\n 1:C#\n 2:D\n 3:D#\n 4:E\n 5:F\n 6:F#\n 7:G\n 8:G#\n 9:A\n 10:A#\n 11:B");
            NoteName tonic = (NoteName)Convert.ToInt32(Console.ReadLine());
            return MelodyGenerator.ScaleChords(chordQuality,scaleIntervals,tonic);
        }
    }
}
