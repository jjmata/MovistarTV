﻿// Copyright (C) 2014-2017, GitHub/Codeplex user AlphaCentaury
// 
// All rights reserved, except those granted by the governing license of this software.
// See 'license.txt' file in the project root for complete license information.
// 
// http://movistartv.alphacentaury.org/ https://github.com/AlphaCentaury

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Xml.Serialization;

namespace IpTviewr.Services.EpgDiscovery
{
    [Serializable()]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(TypeName = "Program", Namespace = Common.XmlNamespace)]
    public class EpgProgram
    {
        [XmlAttribute("id")]
        public string Id
        {
            get;
            set;
        } // Id

        [XmlElement("Title")]
        public string Title
        {
            get;
            set;
        } // Tile

        [XmlElement("Genre")]
        public EpgCodedValue Genre
        {
            get;
            set;
        } // Genre

        [XmlElement("ParentalRating")]
        public EpgCodedValue ParentalRating
        {
            get;
            set;
        } // ParentalRating

        [XmlElement("StartTime")]
        public DateTime UtcStartTime
        {
            get;
            set;
        } // UtcStartTime

        [XmlIgnore]
        public DateTime LocalStartTime
        {
            get { return UtcStartTime.ToLocalTime(); }
        } // LocalStartTime

        [XmlIgnore]
        public DateTime UtcEndTime
        {
            get { return UtcStartTime + Duration; }
        } // UtcEndTime

        [XmlIgnore]
        public DateTime LocalEndTime
        {
            get { return UtcEndTime.ToLocalTime(); }
        } // LocalEndTime

        [XmlIgnore]
        public TimeSpan Duration
        {
            get;
            set;
        } // Duration

        [XmlElement("Duration")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string XmlDuration
        {
            get { return SoapDuration.ToString(Duration); }
            set { Duration = string.IsNullOrEmpty(value) ? new TimeSpan() : SoapDuration.Parse(value); }
        } // XmlDuration

        [XmlAttribute("isBlank")]
        public bool IsBlank
        {
            get;
            set;
        } // IsBlank

        public EpgProgramEpisode Episode
        {
            get;
            set;
        } // Episode

        public bool IsCurrent(DateTime referenceTime)
        {
            if (referenceTime.Kind != DateTimeKind.Utc)
            {
                referenceTime = referenceTime.ToUniversalTime();
            } // if

            return (referenceTime >= UtcStartTime) && (referenceTime < UtcEndTime);
        } // IsCurrent

        public bool IsOld(DateTime referenceTime)
        {
            if (referenceTime.Kind != DateTimeKind.Utc)
            {
                referenceTime = referenceTime.ToUniversalTime();
            } // if

            return (referenceTime >= UtcEndTime);
        } // IsOld

        public bool IsAfter(DateTime referenceTime)
        {
            if (referenceTime.Kind != DateTimeKind.Utc)
            {
                referenceTime = referenceTime.ToUniversalTime();
            } // if

            return (referenceTime < UtcStartTime);
        } // IsAfter

        public static EpgProgram FromScheduleEvent(TvAnytime.TvaScheduleEvent item)
        {
            if (item == null) return null;

            var utcStartTime = item.StartTime ?? item.PublishedStartTime;
            if (utcStartTime == null) return null;

            var result = new EpgProgram()
            {
                Id = item.Program.CRID,
                Duration = (item.Duration.TotalSeconds > 0) ? item.Duration : item.PublishedDuration,
                UtcStartTime = utcStartTime.Value
            };

            if (item.Description == null)
            {
                result.Title = Properties.Texts.EpgBlankTitle;
                result.IsBlank = true;
                return result;
            }

            result.Title = item.Description.Title;
            result.Genre = EpgCodedValue.ToCodedValue(item.Description.Genre);
            result.ParentalRating = (item.Description.ParentalGuidance != null)? EpgCodedValue.ToCodedValue(item.Description.ParentalGuidance.ParentalRating) : null;

            if (item.Description.ReleaseInfo?.ReleaseDate != null)
            {
                result.Episode = new EpgProgramEpisode()
                {
                    Number = item.Description.ReleaseInfo.ReleaseDate.Episode.Nullable,
                    Season = item.Description.ReleaseInfo.ReleaseDate.Season.Nullable,
                    Year = item.Description.ReleaseInfo.ReleaseDate.Year.Nullable,
                };
            } // if
            if (item.EpisodeOf != null)
            {
                var episode = (result.Episode == null) ? new EpgProgramEpisode() : result.Episode;
                episode.SeriesId = item.EpisodeOf.CRID;
                episode.SeriesName = item.EpisodeOf.Title;
                result.Episode = episode;
            } // if

            return result;
        } // FromScheduleEvent

        public override string ToString()
        {
            return Title;
        } // ToString
    } // EPGEvent
} // namespace