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

            //RandomMelodyMenu();
            var pattern = MelodyGenerator.ScaleChords();
            MidiFile midiFile = pattern.ToFile(TempoMap.Default);
            
            midiFile.Write("out.mid", true, MidiFileFormat.SingleTrack);
            Console.ReadLine();
        }


        private static void RandomMelodyMenu()
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
            Console.ReadLine();
        }
        private static Pattern InputParametricMelody()
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
            Console.WriteLine("0: Progression \n1:Dull \n2:Chill \n3:Complex \n4:Dissonant");
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
    }
}
