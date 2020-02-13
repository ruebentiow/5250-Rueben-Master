using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mine.Models;

namespace Mine.Services
{
    public class DatabaseService : IDataStore<ItemModel>
    { 
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        public async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ItemModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public void WipeDataList()
        {
            Database.DropTableAsync<ItemModel>().GetAwaiter().GetResult();
            Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<List<ItemModel>> IndexAsync(bool flag = false)
        {
            return Database.Table<ItemModel>().ToListAsync();
        }

        public Task<bool> CreateAsync(ItemModel item)
        {
            Database.InsertAsync(item);
            return Task.FromResult(true);
        }

        public Task<ItemModel> ReadAsync(string id)
        {
            return Database.Table<ItemModel>().Where(i => i.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public Task<bool> UpdateAsync(ItemModel Data)
        {
            var myRead = ReadAsync(Data.Id).GetAwaiter().GetResult();
            if (myRead == null)
            {
                return Task.FromResult(false);

            }

            Database.UpdateAsync(Data);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string id)
        {
            // Check if it exists...
            var myRead = ReadAsync(id).GetAwaiter().GetResult();
            if (myRead == null)
            {
                return Task.FromResult(false);

            }

            // Then delete...

            Database.DeleteAsync(myRead);
            return Task.FromResult(true);
        }

        // Delete the Datbase Tables by dropping them
        public async void DeleteTables()
        {
            await Database.DropTableAsync<ItemModel>();
        }

        // Create the Datbase Tables
        public async void CreateTables()
        {
            await Database.CreateTableAsync<ItemModel>();
        }
    }
}
