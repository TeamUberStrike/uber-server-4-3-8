using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using SendGridSdk.Models;

namespace SendGridSdk
{
    [DataContract]
    internal class SmtpApiHeader
    {
        [DataMember(Name = "to")]
        internal IList<string> Recipients { get; private set; }
        [DataMember(Name = "sub")]
        internal Dictionary<string, List<string>> SubstitutionValues { get; private set; }
        [DataMember(Name = "section")]
        internal Dictionary<string, string> Sections { get; private set; }
        [DataMember(Name = "category")]
        internal IList<string> Categories { get; private set; }
        [DataMember(Name = "unique_args")]
        internal Dictionary<string, string> UniqueArguments { get; private set; }
        // TODO: Filters

        internal SmtpApiHeader()
        {
            Recipients = new List<string>();
            SubstitutionValues = new Dictionary<string, List<string>>();
            Sections = new Dictionary<string, string>();
            Categories = new List<string>();
            UniqueArguments = new Dictionary<string, string>();
        }

        internal void AddRecipients(List<string> recipients)
        {
            List<string> newRecipients = recipients.Where(r => !Recipients.Contains(r)).ToList();

            foreach (string recipient in newRecipients)
            {
                Recipients.Add(recipient);
            }

            if (Recipients.Count > SendGridConfig.MaxSmtpRecipientsCount)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "It is best to split up large jobs at around {0} recipients (currently {1} recipients) .", SendGridConfig.MaxSmtpRecipientsCount, Recipients.Count));
            }
        }

        internal void AddSubstitutionValue(string tag, List<string> values)
        {
            if (!SubstitutionValues.ContainsKey(tag))
            {
                SubstitutionValues.Add(tag, new List<string>());
            }

            List<string> newValues = values.Where(v => !SubstitutionValues[tag].Contains(v)).ToList();
            SubstitutionValues[tag].AddRange(newValues);
        }

        internal void AddSection(string tag, string value)
        {
            if (!Sections.ContainsKey(tag))
            {
                Sections.Add(tag, value);
            }
        }

        internal void SetUniqueArguments(Dictionary<string, string> arguments)
        {
            UniqueArguments = arguments;
        }

        internal void SetCategories(IList<string> categories)
        {
            if (categories.Count <= SendGridConfig.MaxCategoriesCount)
            {
                Categories = categories;
            }
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "You may insert up to {0} categories.", SendGridConfig.MaxCategoriesCount));
            }
        }

        internal string ToJson()
        {
            foreach (string tag in SubstitutionValues.Keys)
            {
                if (Recipients.Count != SubstitutionValues[tag].Count)
                {
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "{0} recipients but only {1} \"{2}\" tag", Recipients.Count, SubstitutionValues[tag].Count, tag));
                }
            }

            string json = JsonConvert.SerializeObject(this);

            Regex spacer = new Regex(@"([""\]}]{1})([,:]{1})([""\[{]{1})");
            json = spacer.Replace(json, "$1$2 \n$3");

            // TODO: Cut line at 72

            return json;
        }
    }
}