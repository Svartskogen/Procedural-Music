using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Standards;
using ProceduralMusic;

namespace Procedural_Music
{
    static class MelodyGenerator
    {
        /// <summary>
        /// Genera una melodia independiente dados diversos parametros
        /// <para>
        /// La melodia se va a encontrar en escala, no va a prestar atencion a compases.
        /// </para>
        /// <para>
        /// Crea una cantidad dada de notas, usando TimeMood para su espaciado
        /// </para>
        /// <para>
        /// Tambien soporta una variable que indica que tanto puede saltar una nota a otra en la escala, 
        /// asi como que octavas usar
        /// </para>
        /// </summary>
        /// <param name="notesSeed">Semilla para las notas</param>
        /// <param name="scaleInterval">Escala a usar para las notas</param>
        /// <param name="scaleName">Nombre de la escala para imprimir informacion</param>
        /// <param name="tonic">Nota base de la escala</param>
        /// <param name="timeMood">TimeMood a usar para el tiempo de las notas</param>
        /// <param name="notesAmount">Cantidad de notas a generar</param>
        /// <param name="stepVariance">Variacion maxima entre nota y nota con respecto a la escala</param>
        /// <param name="octaves">Arreglo con octavas a usar</param>
        /// <returns></returns>
        public static Pattern ParametricStandaloneMelody(int notesSeed, IEnumerable<Interval> scaleInterval,
            string scaleName, NoteName tonic, TimeMood timeMood,
            int notesAmount, int stepVariance, int[] octaves)
        {
            Random random = new Random(notesSeed);
            var patternBuilder = new PatternBuilder();
            patternBuilder.ProgramChange(GeneralMidiProgram.AcousticGuitar1);
            Scale scale;
            scale = new Scale(scaleInterval, tonic);
            NoteName[] notes = new NoteName[notesAmount];
            int lastStep = 0;
            for (int i = 0; i < notes.Length; i++) //Popula el arreglo de notas
            {
                lastStep += random.Next(-stepVariance, stepVariance);

                notes[i] = scale.GetStep(Math.Abs(lastStep));
            }
            //Construye la melodia
            for (int i = 0; i < notes.Length; i++)
            {
                patternBuilder.SetOctave(Octave.Get(octaves[random.Next(0, octaves.Length)]));
                patternBuilder.SetNoteLength(GetTimeSpanFromMood(timeMood, random.Next(0, 101)));
                patternBuilder.Note(notes[i]);
            }

            //Imprime info en la consola y devuelve
            Console.WriteLine("Se creo una melodia en la escala de " + tonic.ToString() + " " + scaleName);
            return patternBuilder.Build();
        }

        /// <summary>
        /// Genera una melodia completamente aleatoria dada la seed, seteando proceduralmente parametros de una
        /// melodia parametrica
        /// </summary>
        /// <param name="seed">seed</param>
        /// <returns></returns>
        public static Pattern RandomParametricStandaloneMelody(int seed, int notesAmount)
        {
            Random random = new Random(seed);
            IEnumerable<Interval> intervalToUse = null;
            string scaleName = "";
            int scaleToUseRnd = random.Next(0, 5);
            switch (scaleToUseRnd)
            {
                case 0:
                {
                    intervalToUse = ScaleIntervals.Major;
                    scaleName = "Major";
                    break;
                }
                case 1:
                {
                    intervalToUse = ScaleIntervals.Minor;
                    scaleName = "Natural Minor";
                    break;
                }
                case 2:
                {
                    intervalToUse = ScaleIntervals.MelodicMinor;
                    scaleName = "Melodic Minor";
                    break;
                }
                case 3:
                {
                    intervalToUse = ScaleIntervals.HarmonicMinor;
                    scaleName = "Harmonic Minor";
                    break;

                }
                case 4:
                {
                    intervalToUse = ScaleIntervals.HarmonicMajor;
                    scaleName = "Harmonic Major";
                    break;
                }
            }
            NoteName tonic = (NoteName)random.Next(0, 12);
            TimeMood timeMood = (TimeMood)random.Next(0, 5);
            int stepVariance = random.Next(1, 5);
            int octavesAmount = random.Next(1, 5);
            int[] octaves = new int[octavesAmount];
            for (int i = 0; i < octavesAmount; i++)
            {
                octaves[i] = random.Next(2, 6);
            }

            Console.WriteLine("Se genero una melodia con semilla: " + seed);
            Console.WriteLine("En la escala de " + tonic.ToString() + " " + scaleName);
            Console.WriteLine("Con un estilo de tiempo: " + timeMood.ToString() + " y " + notesAmount + " notas");
            Console.WriteLine("Variacion entre notas dentro de la escala de +- " + stepVariance);
            Console.WriteLine("Y " + octavesAmount + " niveles de octavas");
            return ParametricStandaloneMelody(seed, intervalToUse, scaleName, tonic, timeMood, notesAmount, stepVariance, octaves);
        }

        static MusicalTimeSpan GetTimeSpanFromMood(TimeMood mood, int rnd)
        {
            switch (mood)
            {
                case TimeMood.Progression:
                {
                    if (rnd < 50)
                    {
                        return MusicalTimeSpan.Whole;
                    }
                    else
                    {
                        return MusicalTimeSpan.Half;
                    }
                }
                case TimeMood.Dull:
                {
                    return MusicalTimeSpan.Quarter;
                    break;
                }
                case TimeMood.Chill:
                {
                    if (rnd < 80)
                    {
                        return MusicalTimeSpan.Eighth;
                    }
                    else
                    {
                        return MusicalTimeSpan.Quarter;
                    }
                    break;
                }
                case TimeMood.Complex:
                {
                    if (rnd < 30)
                    {
                        return MusicalTimeSpan.Sixteenth;
                    }
                    else if (rnd < 60)
                    {
                        return MusicalTimeSpan.Quarter;
                    }
                    else
                    {
                        return MusicalTimeSpan.Eighth;
                    }
                    break;
                }
                case TimeMood.Dissonant:
                {
                    if (rnd < 40)
                    {
                        return MusicalTimeSpan.Sixteenth;
                    }
                    else if (rnd < 80)
                    {
                        return MusicalTimeSpan.Quarter;
                    }
                    else
                    {
                        return MusicalTimeSpan.Eighth;
                    }
                    break;
                }
            }
            return MusicalTimeSpan.Whole;
        }
        //Progression: Mitad wholes mitad Halfs
        //Dull: todas las notas en Quarter
        //Chill: mayoria en Eighth, poca probabilidad de Quarter
        //Complex: mezcla entre Eight, Quarter y Sixt.
        //Dissonant: mezcla entre Quarter y Sixt, poca chance de Eight
        public enum TimeMood { Progression, Dull, Chill, Complex, Dissonant }

        public static Pattern ScaleChords()
        {
            Console.WriteLine("Generando midi con los acordes de la escala:");
            var chords = ExtraTheory.GenerateChordArray(ChordQuality.Minor, ScaleIntervals.Minor, NoteName.D);
            Console.WriteLine(chords[0].GetNames().ToArray<string>()[0]);
            Console.WriteLine("---------------");
            var patternBuilder = new PatternBuilder();
            patternBuilder.ProgramChange(GeneralMidiProgram.AcousticGuitar1);
            for(int i = 0; i < chords.Length; i++)
            {
                patternBuilder.Chord(chords[i], Octave.Get(3));
                Console.Write((ExtraTheory.ChordOrder)(i + 1)  + "- ");
                Console.WriteLine(chords[i].GetNames().ToArray<string>()[0]);
            }
            return patternBuilder.Build();
        }
    }

}
