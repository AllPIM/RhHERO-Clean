﻿using BlazorHero.CleanArchitecture.Application.Features.Products.Queries.GetAllPaged;
using BlazorHero.CleanArchitecture.Application.Requests.Catalog;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Client.Pages.Catalog
{
    public partial class Products
    {
        private IEnumerable<GetAllPagedProductsResponse> pagedData;
        private MudTable<GetAllPagedProductsResponse> table;
        private int totalItems;
        private int currentPage;
        private string searchString = null;
        private async Task<TableData<GetAllPagedProductsResponse>> ServerReload(TableState state)
        {            
            await LoadData(state.Page, state.PageSize);
            return new TableData<GetAllPagedProductsResponse>() { TotalItems = totalItems, Items = pagedData };
        }
        async Task LoadData(int pageNumber, int pageSize)
        {
            var request = new GetAllPagedProductsRequest { PageSize = pageSize, PageNumber = pageNumber + 1 };
            var response = await _productManager.GetProductsAsync(request);
            totalItems = response.TotalCount;
            currentPage = response.CurrentPage;
            var data = response.Data;
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Barcode.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToList();
            pagedData = data;
        }
        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
    }
}
