using System.Xml;
using Serilog.Events;
using Serilog.Formatting;

namespace BackupUtil.Core.Util;

public class XmlLogFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        XmlWriterSettings settings = new() { OmitXmlDeclaration = true, Indent = true };

        using XmlWriter writer = XmlWriter.Create(output, settings);

        writer.WriteStartElement("LogEntry");

        writer.WriteElementString("Timestamp", logEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        writer.WriteElementString("Level", logEvent.Level.ToString());
        writer.WriteElementString("Message", logEvent.RenderMessage());

        if (logEvent.Exception != null)
        {
            writer.WriteElementString("Exception", logEvent.Exception.ToString());
        }

        if (logEvent.Properties.Count > 0)
        {
            writer.WriteStartElement("Properties");
            foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", property.Key);
                writer.WriteString(property.Value.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
}
