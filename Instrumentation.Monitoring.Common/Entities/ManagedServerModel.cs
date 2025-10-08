using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    public class ManagedServerModel
    {
        public string ServerName { get; set; }

        [Required(ErrorMessage = "Server IP Required")]
        public string PublicIp { get; set; }

        public string PrivateIp { get; set; }

        [Required(ErrorMessage = "NextPollTime Required")]
        public DateTime NextPollTime { get; set; }

        public DateTime? DeploymentTime { get; set; }

        [Required(ErrorMessage = "IsDisable Required")]
        public bool IsDisable { get; set; }

        [Required(ErrorMessage = "Server IDC Required")]
        public string ServerIDC { get; set; }

        [Required(ErrorMessage = "Region Required")]
        public int Region { get; set; }

        public string City { get; set; }

        [Required(ErrorMessage = "CPUModel Required")]
        public string CPUModel { get; set; }

        [Required(ErrorMessage = "CPUSpeed Required")]
        public decimal CPUSpeed { get; set; }

        [Required(ErrorMessage = "CPU number required")]
        public int CPUs { get; set; }

        [Required(ErrorMessage = "CPUCore Required")]
        public short CPUCore { get; set; }

        [Required(ErrorMessage = "RAM Required")]
        public int RAM { get; set; }

        [Required(ErrorMessage = "ManagedServer Id Required")]
        public int ManagedServerId2 { get; set; }

        public int? AllowedBandwidth { get; set; }

        public string Note { get; set; }

        public string Role { get; set; }

        public short? DiskSpace { get; set; }

        public decimal? Price { get; set; }
    }
}
