using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sodum.API.DTOs;

namespace Sodum.API.Services
{
    public interface ILastFmServiceSearch
    {
        Task<List<MusicDto>> SearchTracksSearchAsync(string trackName);
    }
}