﻿// The MIT License (MIT)
// 
// Copyright (c) 2015 Xamarin
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;
using XamarinCRM.Services;
using XamarinCRM.Models;
using XamarinCRM.Models.Local;

[assembly: Dependency(typeof(ChartDataService))]

namespace XamarinCRM.Services
{
    public class ChartDataService : IChartDataService
    {
        IDataService _DataClient;

        const int defaultNumberOfWeks = 6;

        public ChartDataService()
        {
            _DataClient = DependencyService.Get<IDataService>();
        }

        #region IChartDataService implementation

        public async Task<List<WeeklySalesDataPoint>> GetWeeklySalesDataPointsAsync(IEnumerable<Order> orders, int numberOfWeeks = defaultNumberOfWeks, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            DateTime dateStart = DateTime.UtcNow;

            DateTime dateWkStart = dateStart.Subtract(new TimeSpan(dateStart.DayOfWeek.GetHashCode(), 0, 0, 0));
            DateTime dateWkEnd = dateWkStart.AddDays(6);

            var enumerableOrders = orders as IList<Order> ?? orders.ToList();
            double total = GetOrderTotalForPeriod(enumerableOrders, dateWkStart, dateWkEnd, statusOption);

            List<WeeklySalesDataPoint> weeklySalesDataPoints = new List<WeeklySalesDataPoint>();

            weeklySalesDataPoints.Add(new WeeklySalesDataPoint() { DateStart = dateWkStart, DateEnd = dateWkEnd, Amount = total });

            for (int i = 1; i < numberOfWeeks; i++)
            {
                dateWkStart = dateWkStart.AddDays(-7);
                dateWkEnd = dateWkStart.AddDays(6);
                total = GetOrderTotalForPeriod(enumerableOrders, dateWkStart, dateWkEnd);
                weeklySalesDataPoints.Add(new WeeklySalesDataPoint() { DateStart = dateWkStart, DateEnd = dateWkEnd, Amount = total });
            }

            return weeklySalesDataPoints;
        }

        public async Task<List<ChartDataPoint>> GetCategorySalesDataPointsAsync(IEnumerable<Order> orders, Account account = null, int numberOfWeeks = defaultNumberOfWeks, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            // get top-level categories by passing no parent categoryId
            var categories = await _DataClient.GetCategoriesAsync();

            List<ChartDataPoint> categorySalesDataPoints = new List<ChartDataPoint>();

            orders = (account == null) ? orders : orders.Where(order => order.AccountId == account.Id);

            foreach (var category in categories)
            {
                double amount = await GetOrderTotalForCategoryAsync(orders, category, numberOfWeeks, statusOption);
                categorySalesDataPoints.Add(new ChartDataPoint(category.Name, amount));
            }

            return categorySalesDataPoints;
        }

        #endregion

        async Task<double> GetOrderTotalForCategoryAsync(IEnumerable<Order> orders, Category category, int numberOfWeeks = defaultNumberOfWeks, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            double total = 0;

            var categoryProducts = await _DataClient.GetAllChildProductsAsync(category.Id);

            DateTime dateEnd = DateTime.UtcNow;
            DateTime dateStart = dateEnd.AddDays(-numberOfWeeks * 7);

            IEnumerable<Order> results;

            switch (statusOption)
            {
                case OrderStatusOption.Open:
                    results = orders.Where(
                        order => 
                        order.IsOpen &&
                        order.OrderDate >= dateStart &&
                        order.OrderDate <= dateEnd &&
                        categoryProducts.Any(product => product.Name.ToLower() == order.Item.ToLower()));
                    break;
                case OrderStatusOption.Closed:
                    results = orders.Where(
                        order => 
                        !order.IsOpen &&
                        order.ClosedDate >= dateStart &&
                        order.ClosedDate <= dateEnd &&
                        categoryProducts.Any(product => product.Name.ToLower() == order.Item.ToLower()));
                    break;
                default:
                    results = orders.Where(
                        order => 
                        order.ClosedDate >= dateStart &&
                        order.ClosedDate <= dateEnd &&
                        categoryProducts.Any(product => product.Name.ToLower() == order.Item.ToLower()));
                    break;
            }
                
            foreach (var order in results)
            {
                total += order.Price;
            }

            return total;
        }

        static double GetOrderTotalForPeriod(IEnumerable<Order> orders, DateTime dateStart, DateTime dateEnd, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            double total = 0;

            IEnumerable<Order> results;

            switch (statusOption)
            {
                case OrderStatusOption.Open:
                    results = orders.Where(
                        order => 
                        order.IsOpen &&
                        order.OrderDate >= dateStart &&
                        order.OrderDate <= dateEnd);
                    break;
                case OrderStatusOption.Closed:
                    results = orders.Where(
                        order => 
                        !order.IsOpen &&
                        order.ClosedDate >= dateStart &&
                        order.ClosedDate <= dateEnd);
                    break;
                default:
                    results = orders.Where(
                        order => 
                        order.OrderDate >= dateStart &&
                        order.OrderDate <= dateEnd);
                    break;
            }

            foreach (var order in results)
            {
                total += order.Price;
            }

            return total;
        }
    }
}

