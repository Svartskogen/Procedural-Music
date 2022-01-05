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
        /// Generates an independent melody given various parameters
        /// <para>
        /// The melody will be in a given scale, it will not take into account measures</para>
        /// <para>
        /// Creates a given amount of notes, and uses a TimeMood to time them</para>
        /// <para>
        /// Also supports a variabla that limits how much a note can jump from the previous one inside the scale, <paramref name="stepVariance"/>, and also limits notes to a
        /// given octaves array.</para>
        /// </summary>
        /// <param name="notesSeed">Seed for the notes</param>
        /// <param name="scaleInterval">Scale to be used for the notes</param>
        /// <param name="scaleName">Scale name, used for debugging</param>
        /// <param name="tonic">Base note for the scale</param>
        /// <param name="timeMood">TimeMood preset for notes timing</param>
        /// <param name="notesAmount">Amount of notes to generate</param>
        /// <param name="stepVariance">Max variation between a note and the next one</param>
        /// <param name="octaves">Set of octaves to use</param>
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
            //Builds the melody
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
        /// Generates a completely random melody given a seed, setting procedurally its parameters
        /// </summary>
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

        /// <summary>
        /// TimeMood is used to define a patter to time notes, used with <see cref="GetTimeSpanFromMood(TimeMood, int)"/>
        /// <para>TimeMood does not take into account musical measures</para>
        /// <list type="bullet">
        /// <item><term>Progression</term> half Wholes, half Halfs notes</item>
        /// <item><term>Dull</term> All Quarter notes</item>
        /// <item><term>Chill</term> Mostly Eighths, small chance of Quarters</item>
        /// <item><term>Complex</term> Mix between Eighths, Quarters and Sixts</item>
        /// <item><term>Dissonant</term> Mix between Quarters and Sixts, small chance of Eighths</item>
        /// </list>
        /// </summary>
        public enum TimeMood { Progression, Dull, Chill, Complex, Dissonant }
        static MusicalTimeSpan GetTimeSpanFromMood(TimeMood mood, int rnd)
        {
            rnd = rnd > 100 ? 100 : rnd; //clamp
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

        public static Pattern ScaleChords(ChordQuality chordQuality, IEnumerable<Interval> scaleInterval, NoteName tonic)
        {
            Console.WriteLine("Generando midi con los acordes de la escala:");
            var chords = ExtraTheory.GenerateChordArray(chordQuality, scaleInterval, tonic);
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
        /// <summary>
        /// Generates an independent chord progression given various parameters
        /// <para> Includes an unique aditional parameter: ChordProgressionType, to determine the relation between chords</para>
        /// <para>Very similar to ParametricStandaloneMelody in the case of using ChordProgressionType = Random</para>
        /// </summary>
        /// <param name="chordsSeed">Seed for the chords</param>
        /// <param name="scaleInterval">Scale to be used for the chords</param>
        /// <param name="scaleQuality">Scale Quality</param>
        /// <param name="tonic">Base note for the scale</param>
        /// <param name="timeMood">TimeMood preset for chords timing</param>
        /// <param name="progressionType">Determines the relation between chords</param>
        /// <param name="chordsAmount">Only when ChordProgressionType is NOT Popular</param>
        /// <param name="stepVariance">Only relevant when ChordProgressionType is Random</param>
        /// <param name="octaves">Set of octaves to use</param>
        public static Pattern ParametricStandaloneChords(int chordsSeed,IEnumerable<Interval> scaleInterval, ChordQuality scaleQuality,
            NoteName tonic, TimeMood timeMood,ChordProgressionType progressionType,int chordsAmount, int stepVariance, int[] octaves)
        {
            Random random = new Random(chordsSeed);
            var patternBuilder = new PatternBuilder();
            patternBuilder.ProgramChange(GeneralMidiProgram.AcousticGuitar1);
            var scaleChords = ExtraTheory.GenerateChordArray(scaleQuality, scaleInterval, tonic);
            var chords = PopulateChordsArrayByProgressionType(scaleChords, progressionType, chordsAmount,stepVariance,random);

            Console.WriteLine("Placing chords:");
            //Builds the progression
            for(int i = 0; i < chords.Length; i++)
            {
                patternBuilder.SetNoteLength(GetTimeSpanFromMood(timeMood, random.Next(0, 101)));
                patternBuilder.Chord(chords[i], Octave.Get(octaves[random.Next(0, octaves.Length)]));

                //Debug:
                Console.WriteLine(chords[i].GetNames().ToArray<string>()[0]);
            }

            Console.WriteLine("Se creo una progresion de acordes en la escala de: " + tonic.ToString() + " " + scaleQuality.ToString());
            return patternBuilder.Build();
        }
        
        public static Pattern RandomParametricStandaloneChords(int seed, int notesAmount)
        {
            throw new NotImplementedException();
        }
        //Random: Same method used in ParametricMelodyGenerator
        public enum ChordProgressionType { Random, Coherent, Popular}

        static Melanchall.DryWetMidi.MusicTheory.Chord[] PopulateChordsArrayByProgressionType
            (Melanchall.DryWetMidi.MusicTheory.Chord[] availableChords,ChordProgressionType progressionType,int chordsAmount,
            int stepVariance, Random random)
        {
            Melanchall.DryWetMidi.MusicTheory.Chord[] chords = new Melanchall.DryWetMidi.MusicTheory.Chord[chordsAmount];
            switch (progressionType)
            {
                case ChordProgressionType.Random:
                {
                    int lastStep = 0;
                    for (int i = 0; i < chords.Length; i++)
                    {
                        lastStep += random.Next(-stepVariance, stepVariance);
                        chords[i] = availableChords[Math.Abs(lastStep % availableChords.Length)];
                    }
                    return chords;
                }
                case ChordProgressionType.Coherent:
                {
                    ExtraTheory.ChordOrder lastChord = ExtraTheory.ChordOrder.I;
                    int chordsSinceI = 0;
                    chords[0] = availableChords[0];
                    for (int i = 1; i < chords.Length; i++)
                    {
                        switch (lastChord)
                        {
                            case ExtraTheory.ChordOrder.I:
                            {
                                lastChord = (ExtraTheory.ChordOrder)random.Next(1,8);
                                break;
                            }
                            case ExtraTheory.ChordOrder.II:
                            {
                                if (chordsSinceI >= 4)
                                {
                                    lastChord = ExtraTheory.ChordOrder.V;
                                }
                                else
                                {
                                    if (random.Next(0, 2) == 0)
                                    {
                                        lastChord = ExtraTheory.ChordOrder.V;
                                    }
                                    else
                                    {
                                        lastChord = ExtraTheory.ChordOrder.III;
                                    }
                                }
                                break;
                            }
                            case ExtraTheory.ChordOrder.III:
                            {
                                if (random.Next(0, 2) == 0)
                                {
                                    lastChord = ExtraTheory.ChordOrder.VI;
                                }
                                else
                                {
                                    lastChord = ExtraTheory.ChordOrder.IV;
                                }
                                break;
                            }
                            case ExtraTheory.ChordOrder.IV:
                            {
                                if (random.Next(0, 3) == 0)
                                {
                                    lastChord = ExtraTheory.ChordOrder.II;
                                }
                                else
                                {
                                    lastChord = ExtraTheory.ChordOrder.V;
                                }
                                break;
                            }
                            case ExtraTheory.ChordOrder.V:
                            {
                                if(chordsSinceI >= 5)
                                {
                                    lastChord = ExtraTheory.ChordOrder.I;
                                }
                                else
                                {
                                    if (random.Next(0, 2) == 0)
                                    {
                                        lastChord = ExtraTheory.ChordOrder.VI;
                                    }
                                    else
                                    {
                                        lastChord = ExtraTheory.ChordOrder.III;
                                    }
                                }
                                break;
                            }
                            case ExtraTheory.ChordOrder.VI:
                            {
                                if (random.Next(0, 2) == 0)
                                {
                                    lastChord = ExtraTheory.ChordOrder.IV;
                                }
                                else
                                {
                                    lastChord = ExtraTheory.ChordOrder.II;
                                }
                                break;
                            }
                            case ExtraTheory.ChordOrder.VII:
                            {
                                if (random.Next(0, 2) == 0)
                                {
                                    lastChord = ExtraTheory.ChordOrder.VI;
                                }
                                else
                                {
                                    lastChord = ExtraTheory.ChordOrder.I;
                                }
                                break;
                            }
                        }

                        if(lastChord == ExtraTheory.ChordOrder.I)
                        {
                            chordsSinceI = 0;
                        }
                        else
                        {
                            chordsSinceI++;
                        }

                        chords[i] = availableChords[(int)lastChord-1];
                    }
                    return chords;
                }
                case ChordProgressionType.Popular:
                {
                    throw new NotImplementedException();
                }
            }
            return null;
        }
    }

}
