using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SQLite;

namespace Mine.Models
{
    /// <summary>
    /// Item for the Game
    /// </summary>
    public class ItemModel : BaseModel
    {
        // Add Unique attributes for Item

        // The Value of the item
        public int Value { get; set; } = 0;

        [Ignore]
        public List<History> AuditHistory { get; set; } = new List<History>();

        // Holds the AuditHistory Json
        public string AuditHistoryString { get; set; } = string.Empty;

        public ItemModel() { }

        public ItemModel(ItemModel data)
        {
            AuditHistoryString = data.AuditHistoryString;
            AuditHistory = JsonConvert.DeserializeObject<List<History>>(AuditHistoryString);

            // Update the Base
            Id = data.Id;
            Name = data.Name;
            Description = data.Description;

            // Update the extended
            Value = data.Value;
        }

        public bool Update(ItemModel data)
        {
            var latest = JsonConvert.SerializeObject(data);
            var previous = JsonConvert.SerializeObject(this);

            AuditHistory = data.AuditHistory;

            if (AuditHistory == null)
            {
                AuditHistory = new List<History>();
            }

            AuditHistory.Add(new History
            {
                Note = "Updated",
                ChangedLatest = latest,
                ChangedPrevious = previous,
                ChangeSize = latest.Length - previous.Length
            });

            AuditHistoryString = JsonConvert.SerializeObject(AuditHistory);

            // Do not update the ID, if you do, the record will be orphaned
            // Id = data.Id;

            // Update the Base
            Name = data.Name;
            Description = data.Description;

            // Update the extended
            Value = data.Value;

            return true;
        }
    }

    public class History
    {
        // Change DateTime
        public DateTime ChangeDateTime { get; set; } = DateTime.Now;

        // Comments about the Change
        public string Note { get; set; } = "Note";

        // The Latest Record
        public string ChangedLatest { get; set; } = string.Empty;

        // The Previous Record
        public string ChangedPrevious { get; set; } = string.Empty;

        // The Size of the change between Latest - Prevous
        public int ChangeSize { get; set; } = 0;
    }
}