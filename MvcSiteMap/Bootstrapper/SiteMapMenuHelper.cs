using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace MvcSiteMap.Bootstrapper
{
    /// <summary>
    /// HtmlHelper: SiteMapMenu, creates unordered list based on sitemap. Supports unlimited nested lists.
    /// </summary>
    public static class SiteMapMenuHelper
    {
        /// <summary>
        /// Creates menu from rootnode based on sitemap
        /// </summary>
        /// <param name="helper">Class which is extended</param>
        /// <param name="id">Id of generated menu</param>
        /// <returns>String containing menu as unordered list</returns>
        public static string SiteMapMenu(this HtmlHelper helper, string id, bool showNestedItems)
        {
            return createMenu(id, SiteMap.RootNode, showNestedItems, helper.ViewContext);
        }

        /// <summary>
        /// Creates menu from specified node based on sitemap
        /// </summary>
        /// <param name="helper">Class which is extended</param>
        /// <param name="id">Id of generated menu</param>
        /// <param name="siteMapNode">Sitemap node which childnodes will be displayed</param>
        /// <returns>String containing menu as unordered list</returns>
        public static string SiteMapMenu(this HtmlHelper helper, string id, SiteMapNode siteMapNode, bool showNestedItems)
        {
            return createMenu(id, siteMapNode, showNestedItems, helper.ViewContext);
        }

        /// <summary>
        /// Creates menu
        /// </summary>
        /// <param name="id">Id used in id attribute of main ul tag</param>
        /// <param name="siteMapNode">Starting point in sitemap</param>
        /// <param name="isRecursive">Determines if nested items are included</param>
        /// <param name="viewContext">Current viewcontext, used to retrieve current routing values</param>
        /// <returns>Unordered list of menu items</returns>
        private static string createMenu(string id, SiteMapNode siteMapNode, bool isRecursive, ViewContext viewContext)
        {
            // Only render when node has childnodes
            if (!siteMapNode.HasChildNodes)
            {
                return string.Empty;
            }

            // Create ul element
            var ul = new TagBuilder("ul");

            // Set id if specified
            if (!string.IsNullOrEmpty(id))
            {
                ul.Attributes.Add("id", id);
            }

            // Iterate through childnodes
            foreach (SiteMapNode childNode in siteMapNode.ChildNodes)
            {
                // Create listitem element
                var li = new TagBuilder("li");

                // Declare element
                TagBuilder element;

                if (childNode.HasChildNodes)
                {
                    // Create heading
                    element = new TagBuilder("span");
                }
                else
                {
                    // Create hyperlink
                    element = new TagBuilder("a");

                    // Set url
                    element.Attributes.Add("href", childNode.Url);
                }

                // Set text
                element.InnerHtml = childNode.Title;

                // When current sitemapnode is null, try to determine selected node based on matching of routevalues
                if (SiteMap.CurrentNode == null)
                {
                    bool Selected = true;

                    // Iterate route keys which are stored in custom attribute seperated by ','
                    foreach (string RouteKey in childNode["RouteKeys"].Split(','))
                    {
                        if (!string.IsNullOrEmpty(RouteKey))
                        {
                            string current = viewContext.RouteData.Values[RouteKey].ToString(); // Current value
                            string child = childNode[RouteKey]; // Sitemap value
                            
                            if (viewContext.RouteData.Values[RouteKey].ToString().ToLower() != childNode[RouteKey].ToLower())
                            {
                                Selected = false;
                            }
                        }
                        else
                        {
                            Selected = false;
                        }
                    }
                    if (Selected)
                    {
                        element.AddCssClass("selected");
                    }
                }
                else
                {
                    // Set 'selected' class on hyperlink if element equals current node
                    if (childNode.Equals(SiteMap.CurrentNode))
                    {

                        element.AddCssClass("selected");
                    }
                }

                // Add element to listitem
                li.InnerHtml = element.ToString();

                // Determine if nested items are to be displayed
                if (isRecursive)
                {
                    // Call function again for childnodes
                    li.InnerHtml += createMenu(string.Empty, childNode, true, viewContext);
                }

                // Add listitem to unordered list
                ul.InnerHtml += li.ToString();

            }

            // Return unordered list
            return ul.ToString();
        }
    }
}