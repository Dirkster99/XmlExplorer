namespace SettingsModel.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Implement a simple file reverence model to allow XML persistence
    /// of a List <seealso cref="FileReference"/> via this class.
    /// </summary>
    public class FileReference
    {
        /// <summary>
        /// Gets/sets the path to a file.
        /// </summary>
        [XmlAttribute(AttributeName = "path")]
        public string path { get; set; }

        /// <summary>
        /// Gets/sets the date and time when this entry was updated for the last time.
        /// </summary>
        [XmlAttribute(AttributeName = "lastupdate")]
        public DateTime LastTimeOfEdit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "ispinned")]
        public int IsPinned { get; set; }
    }
}
