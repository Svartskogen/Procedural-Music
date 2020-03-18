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
    static class MelodyGenerator
    {
        public static Pattern ReturnParametricMelody(int notesSeed, IEnumerable<Interval> scaleInterval,
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
                    //TODO
                    break;
                }
                case TimeMood.Dissonant:
                {
                    //TODO
                    break;
                }
            }
            return MusicalTimeSpan.Whole;
        }
        public enum TimeMood { Progression, Dull, Chill, Complex, Dissonant }
        //Progression: Mitad wholes mitad Halfs
        //Dull: todas las notas en Quarter
        //Chill: mayoria en Eighth, poca probabilidad de Quarter
        //Complex: mezcla entre Eight, Quarter y Sixt.
        //Dissonant: mezcla entre Quarter y Sixt, poca chance de Eight
    }

}
