using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using iTellerBranch.Service.Business;
using iTellerBranch.Repository;
using Newtonsoft.Json;
using iTellerBranch.Model.ViewModel;

namespace iTellerBranch.Service
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = null;
        public bool started;
        private TransactionServiceBusiness transactionService = new TransactionServiceBusiness();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            started = true;
            Utils.LogNO(Config.GetConfigValue("Timer"));
            this.timer.Interval = Convert.ToInt32(Config.GetConfigValue("Timer")) * 6000; //every 30 secs
            this.timer.Elapsed += new ElapsedEventHandler(this.ProcessTermination);
            timer.Enabled = true;
        }

        private  void ProcessTermination(object sender, ElapsedEventArgs e) 
        {
            try
            {
                if(started == true)
                {
                    Utils.LogNO("***********Termination Service has started******************");
                    IQueryable<TreasuryDealsMaster> treasuryDealsMasters = transactionService.GetMaturedDeals();
                    foreach (TreasuryDealsMaster treasuryDealsMaster in treasuryDealsMasters)
                    {
                        transactionService.UpdateTreasuryDeal(treasuryDealsMaster);
                    }


                    Utils.LogNO("***********Termination Service has started executing unsuccessful double entries******************");
                    //IQueryable<TreasuryDealsMaster> treasuryDeals = transactionService
                    //                    .GetTreasuryDealsWithoutSuccessfulDoubleEntries();
                    //foreach (TreasuryDealsMaster treasuryDeal in treasuryDeals)
                    //{
                    //    transactionService.CreateDoubleEntriesForUnsuccesfulDoubleEntries(treasuryDeal);
                    //}
                    //started = false;
                }
               
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for Processing Termination: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }
    }
}
