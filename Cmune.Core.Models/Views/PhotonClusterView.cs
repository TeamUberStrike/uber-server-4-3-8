using System;
using System.Collections.Generic;

namespace Cmune.Core.Models.Views
{
    public class PhotonsClusterView
    {
        #region Properties

        public int PhotonsClusterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PhotonView> Photons { get; set; }

        #endregion

        #region Constructors

        public PhotonsClusterView(int photonsClusterId, string name, string description, List<PhotonView> photons)
        {
            PhotonsClusterId = photonsClusterId;
            Name = name;
            Description = description;
            Photons = photons;
        }

        public PhotonsClusterView(int photonsClusterId, string name, List<PhotonView> photons)
            : this(photonsClusterId, name, String.Empty, photons)
        {
        }

        #endregion
    }
}