﻿using System;
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

namespace ProceduralMusic
{
    public static class ExtraTheory
    {
        /// <summary>
        /// Triads used to generate chords with the following form:
        /// MusicTheory.Chord chord =
        /// new MusicTheory.Chord(NoteName.Transpose(triad[0]),
        /// NoteName.Transpose(triad[1]),
        /// NoteName.Transpose(triad[2]));
        /// </summary>
        public static readonly Interval[] MAJOR_TRIAD = { Interval.Four, Interval.Seven };
        public static readonly Interval[] AUGMENTED_TRIAD = { Interval.Four, Interval.Eight };
        public static readonly Interval[] MINOR_TRIAD = { Interval.Three, Interval.Seven };
        public static readonly Interval[] DIMINISHED_TRIAD = { Interval.Three, Interval.Six };
        public static readonly Interval[] SUS4_TRIAD = { Interval.Five, Interval.Seven };
        public static readonly Interval[] SUS2_TRIAD = { Interval.Two, Interval.Seven };

        //Used to create the chord array of a certain scale
        public static readonly ChordQuality[] MajorChords ={ChordQuality.Major,ChordQuality.Minor,ChordQuality.Minor,ChordQuality.Major,
                                                            ChordQuality.Major,ChordQuality.Minor,ChordQuality.Diminished};
        public static readonly ChordQuality[] NaturalMinorChords ={ChordQuality.Minor,ChordQuality.Diminished,ChordQuality.Major,
                                                        ChordQuality.Minor,ChordQuality.Minor,ChordQuality.Major,ChordQuality.Major};

        public static Interval[] ChordQualityToInterval(ChordQuality chordQuality)
        {
            switch (chordQuality)
            {
                case ChordQuality.Major:
                    return MAJOR_TRIAD;
                case ChordQuality.Minor:
                    return MINOR_TRIAD;
                case ChordQuality.Diminished:
                    return DIMINISHED_TRIAD;
                case ChordQuality.Augmented:
                    return AUGMENTED_TRIAD;
            }
            return null;
        }

        /// <summary>
        /// Only supports the Major and Minor scale so far
        /// </summary>
        public static Melanchall.DryWetMidi.MusicTheory.Chord[] GenerateChordArray(ChordQuality scaleQuality, IEnumerable<Interval> scaleInterval, NoteName tonic)
        {
            Melanchall.DryWetMidi.MusicTheory.Chord[] array = new Melanchall.DryWetMidi.MusicTheory.Chord[7];
            ChordQuality[] chordType = GetChordsFromScale(scaleQuality);
            Scale scale = new Scale(scaleInterval, tonic);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new Melanchall.DryWetMidi.MusicTheory.Chord(scale.GetStep(i),
                    scale.GetStep(i).Transpose(ChordQualityToInterval(chordType[i])[0]),
                    scale.GetStep(i).Transpose(ChordQualityToInterval(chordType[i])[1]));
            }
            return array;
        }
        public enum ChordOrder { I = 1, II, III, IV, V, VI, VII };
        private static ChordQuality[] GetChordsFromScale(ChordQuality scale)
        {
            if(scale == ChordQuality.Minor)
            {
                return NaturalMinorChords;
            }
            else
            {
                return MajorChords;
            }
        }
    }
}
