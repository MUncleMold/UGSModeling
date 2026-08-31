using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using UGSModeling.Models;

namespace UGSModeling.Data
{
    public class UGSDataBase
    {
        private readonly SQLiteAsyncConnection _connection;
        public UGSDataBase()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "UGSDB.db");
            _connection = new SQLiteAsyncConnection(dbPath);
            _connection.CreateTableAsync<UGSParameter>().Wait();
            _connection.CreateTableAsync<User>().Wait();
            _connection.CreateTableAsync<UGSReport>().Wait();
            _connection.CreateTableAsync<Graph>().Wait();
            _connection.CreateTableAsync<Formula>().Wait();
            _connection.CreateTableAsync<Period>().Wait();
        }

        public async Task<User> Authenticate(string login, string password)
        {
            try
            {
                var user = await _connection.Table<User>().FirstOrDefaultAsync(u => u.Login == login);

                if (user != null && user.Password == password)
                {
                    return user;
                }
                else { return null; }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Period>> PeriodGetItems()
        {
            return await _connection.Table<Period>().ToListAsync();
        }

        public Task PeriodAddItem(Period period)
        {
            return _connection.InsertAsync(period);
        }

        public Task PeriodClean()
        {
            return _connection.DeleteAllAsync<Period>();
        }

        public async Task<List<UGSParameter>> ParamGetItems()
        {
            return await _connection.Table<UGSParameter>().ToListAsync();
        }

        public Task ParamAddItem(UGSParameter uGSParameter)
        {
            return _connection.InsertAsync(uGSParameter);
        }

        public Task ParamUpdateItem(UGSParameter uGSParameter)
        {
            return _connection.UpdateAsync(uGSParameter);
        }

        public Task ParamDeleteItem(UGSParameter uGSParameter)
        {
            return _connection.DeleteAsync(uGSParameter);
        }

        public async Task<List<Graph>> GraphGetItems()
        {
            return await _connection.Table<Graph>().ToListAsync();
        }

        public Task GraphAddItem(Graph graph)
        {
            return _connection.InsertAsync(graph);
        }

        public Task GraphUpdateItem(Graph graph)
        {
            return _connection.UpdateAsync(graph);
        }

        public Task GraphDeleteItem(Graph graph)
        {
            return _connection.DeleteAsync(graph);
        }

        public async Task<List<UGSReport>> RepGetItems()
        {
            return await _connection.Table<UGSReport>().ToListAsync();
        }

        public Task RepAddItem(UGSReport report)
        {
            return _connection.InsertAsync(report);
        }

        public Task RepUpdateItem(UGSReport report)
        {
            return _connection.UpdateAsync(report);
        }

        public Task RepDeleteItem(UGSReport report)
        {
            return _connection.DeleteAsync(report);
        }

        public async Task<List<User>> UserGetItems()
        {
            return await _connection.Table<User>().ToListAsync();
        }

        public Task UserAddItem(User user)
        {
            return _connection.InsertAsync(user);
        }

        public Task UserUpdateItem(User user)
        {
            return _connection.UpdateAsync(user);
        }

        public Task UserDeleteItem(User user)
        {
            return _connection.DeleteAsync(user);
        }

        public async Task<List<Formula>> FormulaGetItems()
        {
            return await _connection.Table<Formula>().ToListAsync();
        }

        public Task FormulaAddItem(Formula formula)
        {
            return _connection.InsertAsync(formula);
        }

        public Task FormulaUpdateItem(Formula formula)
        {
            return _connection.UpdateAsync(formula);
        }

        public Task FormulaDeleteItem(Formula formula)
        {
            return _connection.DeleteAsync(formula);
        }



        public async Task<List<UGSReport>> ReportGetItem()
        {
            return await _connection.Table<UGSReport>().ToListAsync();
        }

        public Task ReportAddItem(UGSReport report)
        {
            return _connection.InsertAsync(report);
        }

        public Task ReportDelete(UGSReport report)
        {
            return _connection.DeleteAsync(report);
        }
    }
}
