﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using MAVN.Service.SmartVouchers.Domain.Enums;
using MAVN.Service.SmartVouchers.Domain.Models;
using MAVN.Service.SmartVouchers.Domain.Repositories;
using MAVN.Service.SmartVouchers.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.SmartVouchers.MsSqlRepositories.Repositories
{
    public class CampaignsRepository : ICampaignsRepository
    {
        private readonly IDbContextFactory<SmartVouchersContext> _contextFactory;
        private readonly IMapper _mapper;

        public CampaignsRepository(
            IDbContextFactory<SmartVouchersContext> contextFactory,
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(VoucherCampaign campaign)
        {
            var entity = _mapper.Map<VoucherCampaignEntity>(campaign);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.VoucherCampaigns.Add(entity);

                await context.SaveChangesAsync();

                return entity.Id;
            }
        }

        public async Task UpdateAsync(VoucherCampaign campaign)
        {
            var entity = _mapper.Map<VoucherCampaignEntity>(campaign);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.VoucherCampaigns.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(VoucherCampaign campaign)
        {
            campaign.State = CampaignState.Deleted;

            var entity = _mapper.Map<VoucherCampaignEntity>(campaign);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.VoucherCampaigns.Update(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task<VoucherCampaign> GetByIdAsync(Guid campaignId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.VoucherCampaigns
                    .Where(c => c.Id == campaignId)
                    .Include(c => c.LocalizedContents)
                    .FirstOrDefaultAsync();

                return _mapper.Map<VoucherCampaign>(entity);
            }
        }

        public async Task<IReadOnlyCollection<VoucherCampaign>> GetCampaignsByIdsAsync(Guid[] campaignIds)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = await context.VoucherCampaigns.AsNoTracking()
                    .Where(c => campaignIds.Contains(c.Id))
                    .ToListAsync();

                return _mapper.Map<List<VoucherCampaign>>(query);
            }
        }

        public async Task<CampaignsPage> GetCampaignsAsync(CampaignListRequest request)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.VoucherCampaigns
                    .Where(c => c.State != CampaignState.Deleted);

                if (!string.IsNullOrWhiteSpace(request.CampaignName))
                {
                    var name = request.CampaignName.Trim().ToLower();
                    query = query.Where(c => c.Name.ToLower().Contains(name));
                }
                if (request.OnlyActive)
                {
                    var now = DateTime.UtcNow;
                    query = query.Where(c => c.FromDate <= now && (!c.ToDate.HasValue || c.ToDate.Value > now));
                }

                var result = await query
                    .Skip(request.Skip)
                    .Take(request.Take)
                    .ToListAsync();

                var totalCount = await query.CountAsync();

                return new CampaignsPage
                {
                    Campaigns = _mapper.Map<List<VoucherCampaign>>(result),
                    TotalCount = totalCount,
                };
            }
        }
    }
}
