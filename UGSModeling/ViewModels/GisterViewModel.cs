using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UGSModeling.Data;
using UGSModeling.Models;

namespace UGSModeling.ViewModels
{
    internal partial class GisterViewModel: ObservableObject
    {
        private readonly UGSDataBase _database;

        [ObservableProperty]
        private string g1;

        [ObservableProperty]
        private string g2;

        [ObservableProperty]
        private string g3;

        [ObservableProperty]
        private string g4;

        [ObservableProperty]
        private ObservableCollection<Per> perTable;

        [ObservableProperty]
        private ObservableCollection<MGraph> gisterData;

        [ObservableProperty]
        private ObservableCollection<MGraph> gVKData;

        [ObservableProperty]
        private ObservableCollection<MGraph> pressureData;

        [ObservableProperty]
        private ObservableCollection<MGraph> uploadAndSelectData;

        [ObservableProperty]
        private double step = 10;

        [ObservableProperty]
        private decimal qt = 0;

        [ObservableProperty]
        private string lb1;

        [ObservableProperty]
        private bool plastSwitch;

        [ObservableProperty]
        private string cost = "129";

        public GisterViewModel()
        {
            _database = new UGSDataBase();

            GisterData = new ObservableCollection<MGraph>();
            GVKData = new ObservableCollection<MGraph>();
            PressureData = new ObservableCollection<MGraph>();
            UploadAndSelectData = new ObservableCollection<MGraph>();

            PerTable = new ObservableCollection<Per>();

            LoadPeriod(10);
        }

        [RelayCommand]
        public async void LoadPeriod(int p)
        {
            decimal perN = 1;
            string perState = "";
            int perDuration = 30;
            decimal perCost = 0;

            if(PerTable.Count == 0)
            {
                for(int i = 1; i <= p; i++)
                {
                    perN = i;

                    switch (perN % 4)
                    {
                        case 1:
                            perState = "Закачка";
                            perCost = Convert.ToDecimal(Cost);
                            break;

                        case 2:
                            perState = "Простой";
                            perCost = 0;
                            break;

                        case 3:
                            perState = "Отбор";
                            perCost = Convert.ToDecimal(Cost);
                            break;

                        case 0:
                            perState = "Простой";
                            perCost = 0;
                            break;
                    }

                    PerTable.Add(new Per(perN, perState, perDuration, perCost));
                }
            }
            else
            {
                if(p > PerTable.Count)
                {
                    for (int i = PerTable.Count + 1; i <= p; i++)
                    {
                        perN = i;

                        switch (perN % 4)
                        {
                            case 1:
                                perState = "Закачка";
                                perCost = Convert.ToDecimal(Cost);
                                break;

                            case 2:
                                perState = "Простой";
                                perCost = 0;
                                break;

                            case 3:
                                perState = "Отбор";
                                perCost = Convert.ToDecimal(Cost);
                                break;

                            case 0:
                                perState = "Простой";
                                perCost = 0;
                                break;
                        }

                        PerTable.Add(new Per(perN, perState, perDuration, perCost));
                    }
                }
                else if(p < PerTable.Count)
                {
                    PerTable.RemoveAt(PerTable.Count - 1);
                }
            }
        }

        public async void DrawGraphs(decimal k, decimal h, decimal L, decimal m, decimal R, decimal z, decimal T, decimal deg, decimal pk, decimal ρw, decimal μw, decimal pg, decimal A0, decimal st1, decimal st2, decimal ΔZ, decimal Rk)
        {
            GisterData.Clear();
            GVKData.Clear();
            PressureData.Clear();
            UploadAndSelectData.Clear();

            decimal Time = 0;

            for(int i = 0; i < perTable.Count; i++)
            {
                Time += Convert.ToDecimal(perTable[i].Duration * 86400);
            }

            int N = Convert.ToInt32(Math.Truncate(Convert.ToDecimal(Time) / Convert.ToDecimal(st2)));

            decimal τ = 0;

            decimal[] a = new decimal[N + 1]; a[0] = A0;
            decimal[] p = new decimal[N + 1]; p[0] = pg;
            decimal[] Mass = new decimal[N + 1];

            for (int i = 1; i <= N; i++)
            {
                τ += st2;

                q(τ);

                if(PlastSwitch == false)
                {
                    a[i] = a[i - 1] - st2 * ((k / (μw * m)) * ((p[i - 1] - pk) / (a[i - 1] - L) + ρw * Convert.ToDecimal(9.81) * Convert.ToDecimal(Math.Tan(Convert.ToDouble(deg)))));
                    p[i] = p[i - 1] - st2 * ((((a[i] - a[i - 1]) / st2) * p[i - 1] - Convert.ToDecimal(R) * Convert.ToDecimal(T) * Convert.ToDecimal(z) * Qt * Convert.ToDecimal(Math.Cos(Convert.ToDouble(deg))) / h / m) / (a[i - 1] + h / 2 / Convert.ToDecimal(Math.Sin(Convert.ToDouble(deg)))));
                    Mass[i] = p[i] / (z * R * T) * ((a[i] + h / 2 / Convert.ToDecimal(Math.Sin(Convert.ToDouble(deg)))) / Convert.ToDecimal(Math.Cos(Convert.ToDouble(deg))) * h * L * m);
                }
                else
                {
                    a[i] = a[i - 1] + st2 * ((k * pk) / (μw * m * a[i - 1] * Convert.ToDecimal(Math.Log(Convert.ToDouble((a[i - 1]) / (Rk)))))) * (((Convert.ToDecimal(9.81) * ρw) / (pk)) * (-ΔZ * (1 - ((Convert.ToDecimal(Math.Pow(Convert.ToDouble(a[i - 1]), 2))) / (Convert.ToDecimal(Math.Pow(Convert.ToDouble(Rk), 2)))))) + 1 - ((p[i - 1]) / (pk)));
                    p[i] = p[i - 1] + st2 * ((-(p[i - 1] * 2 * a[i - 1] * ((a[i] - a[i - 1]) / 86400)) / (z)) + ((R * T * Qt) / (h * m * Convert.ToDecimal(Math.PI)))) / ((Convert.ToDecimal(Math.Pow(Convert.ToDouble(a[i - 1]), 2))) / (z));
                    Mass[i] = Convert.ToDecimal(Math.PI) * h * ((m * Convert.ToDecimal(Math.Pow(Convert.ToDouble(a[i - 1]), 2)) * p[i - 1]) / (R * T * z));
                }

                GisterData.Add(new MGraph(p[i] / Convert.ToDecimal(1e6), Mass[i] / Convert.ToDecimal(1e6)));
                GVKData.Add(new MGraph(τ / 86400, a[i]));
                PressureData.Add(new MGraph(τ / 86400, p[i] / Convert.ToDecimal(1e6)));
                UploadAndSelectData.Add(new MGraph(τ / 86400, Qt*86400));
            }
        }

        public void q(decimal t)
        {
            decimal tt = 0;
            int i, j;

            for(i = 0; i < perTable.Count; i++)
            {
                if(t >= tt && t < tt + perTable[i].Duration*86400)
                {
                    j = i;
                    if (perTable[j].State == "Закачка")
                    {
                        Qt = perTable[j].Cost / 86400;
                    }
                    if (perTable[j].State == "Простой")
                    {
                        Qt = 0;
                    }
                    if (perTable[j].State == "Отбор")
                    {
                        Qt = -(perTable[j].Cost / 86400);
                    }
                }
                tt += perTable[i].Duration * 86400;
            }
        }
    }

    public class MGraph
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }

        public MGraph(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }
    }

    public class Per
    {
        public decimal PeriodNumber { get; set; }
        public string State { get; set; }
        public decimal Duration { get; set; }
        public decimal Cost { get; set; }

        public Per(decimal p, string s, decimal d, decimal c)
        {
            PeriodNumber = p;
            State = s;
            Duration = d;
            Cost = c;
        }
    }
}
