using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using concertTicketWebCoreMVC;
using concertTicketWebCoreMVC.App_Code;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace concertTicketWebCoreMVC.Controllers
{
    [Route("api/[controller]/")]
    public class TestApiController : Controller
    {

        [Route("GetCityVenues")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        public IActionResult GetCityVenues(string includeNulls = "false")
        {
            string qry = @"WITH a as (
                            --attach a latitude and longitude the the stubhub venues
	                        SELECT 
			                      v.[name]
		                        , v.[city]  
		                        , c.[latitude]
		                        , c.[longitude]
		                        , 'stubhub' as dataSource
	                        FROM [tblStubhubVenue] v                          
	                        LEFT JOIN [tblStubHubCity] c
		                        ON v.city=c.city
                            WHERE id in(
                                select distinct(venue_id)  from [tblStubhubVenueEvent]
                            )

		                        UNION --get ticketmaster venues with current events
                            
	                        SELECT
		                        [name]
		                        ,[city]
		                        ,[latitude]
		                        ,[longitude]
		                        , 'ticketmaster' as dataSource
	                        FROM [tblTicketmasterVenue]
                            WHERE id in(
                                select distinct(venue_id)  from [tblTicketmasterVenueEvent]
                               -- WHERE CAST(scrapeDate AS DATE) IN ( SELECT TOP (1) CAST(scrapeDate AS DATE) from [tblTicketmasterVenueEvent] order by CAST(scrapeDate AS DATE) desc )
                            )

                        --remove duplicate records(venues that appeared in both data sets with the same name)
                        ), b as ( --min is fine to use here because it will only be used on non duplicates
	
	                        select [name]
	                        , count(1) as duplicate
	                        , min(dataSource) as dataSource 
	                        from a 
	                        group by [name]

                        ), multiple as (
	                        select 
		                         a.[name]
		                        ,a.[city]
		                        ,a.[latitude]
		                        ,a.[longitude]
		                        , case duplicate when 2 Then 'multiple' ELSE b.dataSource end as dSource 
	                        from b 
	                        left join a on a.[name] = b.[name]
                        )
                        select [name]
                        , min(city) as city
                        , min(latitude) as latitude
                        , min(longitude) as longitude
                        , min(dSource) as dataSource
                        from multiple
                        WHERE dSource = 'multiple'
                        GROUP BY [name]
                        UNION
                        select * from multiple 
                        WHERE dSource != 'multiple'
                        AND [name] NOT LIKE  '%Parking Lots' --ticketmaster dataset contains parking lots
                        FOR JSON PATH " + (includeNulls == "false" ? "" : ", INCLUDE_NULL_VALUES");

            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return Ok(result);
        }
        public string GetSpecificStubhubVenueSummary(string venue)
        {
            string qry = @" SELECT 
	                         ve.[name] [Event Name]
	                        , eventDate [Event Date]
	                        , [minListPrice] [Min Cost]
	                        , [maxListPrice] [Max Cost]
                         FROM [tblStubhubVenueEvent] ve
                         LEFT JOIN [tblStubhubVenue] v
                         on ve.venue_id = v.id
                         WHERE eventDate is not null
                         AND  v.[name] = '" + venue + @"'
                         order by [eventDate]
                         FOR JSON PATH";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
            //return Ok(result);
        }

        public string GetSpecificTicketmasterVenueEventSummary(string venue)
        {
            string qry = @"with ve as 
                                (
                                    SELECT
                                     [venue_id]
                                    , [genre_name]
                                    , [name] as [Event]
                                    , [priceRanges_max] as [maxTicketCost]
                                    , [priceRanges_min] as [minTicketCost]
                                    , CAST
	                               (
			                             [dates_start_dateTime]
		                            AS DATE
		                            ) as [Event Date] 
                                     FROM tblTicketmasterVenueEvent ve
                                    -- filter to only include results from nost recent scrape
                                    WHERE CAST(scrapeDate AS DATE) 
                                    IN 
                                    (
                                        SELECT TOP (1) CAST(scrapeDate AS DATE) from [tblTicketmasterVenueEvent] order by CAST(scrapeDate AS DATE) desc
                                    )
                                )
                                SELECT 
                                ve.[Event] as [Event Name]
                                , ve.[Event Date]
                               -- , ve.[genre_name] as [Genre]
                                , ve.[maxTicketCost] as [Min Cost]
                                , ve.[minTicketCost] as [Max Cost]
                                FROM ve
                                 LEFT JOIN tblTicketmasterVenue v 
                                    ON v.id = ve.venue_id
                                 WHERE ve.genre_name is not null
                                 AND [name] = '" + venue + @"'
                                 GROUP BY
                                    ve.[Event]
                                    , ve.[Event Date]
                                    , ve.[genre_name]
                                    , ve.[maxTicketCost]
                                    , ve.[minTicketCost]
                                 ORDER BY 
                                     ve.[Event Date]
                                    , ve.[Event]
                                FOR JSON PATH";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
        }


        public string GetSpecificVenueEventSummary(string venue)
        {
            string qry = @"with ve as 
                                (
                                    SELECT
                                     [venue_id]
                                    , [genre_name]
                                    , [name] as [Event]
                                    , [priceRanges_max] as [maxTicketCost]
                                    , [priceRanges_min] as [minTicketCost]
                                    , CAST
	                               (
			                             [dates_start_dateTime]
		                            AS DATE
		                            ) as [Event Date] 
                                     FROM tblTicketmasterVenueEvent ve
                                    -- filter to only include results from nost recent scrape
                                    WHERE CAST(scrapeDate AS DATE) 
                                    IN 
                                    (
                                        SELECT TOP (1) CAST(scrapeDate AS DATE) from [tblTicketmasterVenueEvent] order by CAST(scrapeDate AS DATE) desc
                                    )
                                )
                                SELECT 
                                ve.[Event] as [Event Name]
                                , ve.[Event Date]
                               -- , ve.[genre_name] as [Genre]
                                , ve.[maxTicketCost] as [Min Cost]
                                , ve.[minTicketCost] as [Max Cost]
                                , 'ticketmaster' as dataSource
                                FROM ve
                                 LEFT JOIN tblTicketmasterVenue v 
                                    ON v.id = ve.venue_id
                                 WHERE ve.genre_name is not null
                                 AND [name] = '" + venue + @"'
                                 GROUP BY
                                    ve.[Event]
                                    , ve.[Event Date]
                                    , ve.[genre_name]
                                    , ve.[maxTicketCost]
                                    , ve.[minTicketCost]
                                 

                                 union


                                 SELECT 
	                         ve.[name] [Event Name]
	                        , ve.eventDate [Event Date]
	                        , ve.[minListPrice] [Min Cost]
	                        , ve.[maxListPrice] [Max Cost]
                            , 'stubhub' as dataSource
                         FROM [tblStubhubVenueEvent] ve
                         LEFT JOIN [tblStubhubVenue] v
                         on ve.venue_id = v.id
                         WHERE  eventDate is not null
                         AND  v.[name] = '" + venue + @"'
                         order by [Event Date]


                                FOR JSON PATH";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
        }









        public string GetSpecificTicketmasterVenueSummary(string venue)
        {
            string qry = @"
                                with ve as 
                               (
                                    select
                                     [venue_id]
                                    , [genre_name]
                                    , Count(1) as numberOfEvents
                                    , MAX([priceRanges_max]) as maxTicketCost
                                    , MIN([priceRanges_min]) as minTicketCost
                                    , AVG(CAST([priceRanges_max] AS NUMERIC)) as AVGmaxTicketCost
                                    , AVG(CAST([priceRanges_min] AS NUMERIC)) as AVGminTicketCost
                                     FROM tblTicketmasterVenueEvent
                                    WHERE venue_id IN ( 
                                        select id from tblTicketmasterVenue where [name] = '" + venue + @"'
                                    )
                                    AND CAST(scrapeDate AS DATE) IN (-- filter to only include results from nost recent scrape
                                        SELECT TOP (1) CAST(scrapeDate AS DATE) 
                                        FROM [tblTicketmasterVenueEvent] 
                                        ORDER BY CAST(scrapeDate AS DATE) desc
                                    )
                                    group by 
                                    [venue_id]
                                    , [genre_name]
                                )
                                SELECT 
                                v.[name] [Name]
                                , ve.[genre_name] [Genre]
                                , ve.[numberOfEvents] [Event Count]
                                , ve.maxTicketCost [Min Cost]
                                , ve.minTicketCost [Max Cost]
                                , ve.AVGminTicketCost [Avg Min Cost]
                                , ve.AVGmaxTicketCost [Avg Max Cost]
                                FROM ve
                                 LEFT JOIN tblTicketmasterVenue v ON v.id = ve.venue_id
                                 WHERE ve.genre_name is not null
                                 ORDER BY 
                                 ve.[venue_id]
                               , ve.[genre_name]
                                FOR JSON PATH";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
            //return Ok(result);
        }

        [Route("GetSpecificVenueSummary")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        //public IActionResult GetSpecificVenueEvent(string venue = "Paradise Rock Club", string dataSource = "stubhub")
        public IActionResult GetSpecificVenueSummary(string venue, string dataSource)
        {
            string result = "";
            if (dataSource == "stubhub")
            {
                result = GetSpecificStubhubVenueSummary(venue);

            }
            else if (dataSource == "ticketmaster")
            {
                result = GetSpecificTicketmasterVenueEventSummary(venue);

            }
            else
            {
               // result = $"[{GetSpecificStubhubVenueSummary(venue)}, {GetSpecificTicketmasterVenueEventSummary(venue)}]";
                result = GetSpecificVenueEventSummary(venue);
            }
            return Ok(result);
        }

        //
        [Route("GetSpecificVenue")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        //public IActionResult GetSpecificVenueEvent(string venue = "Paradise Rock Club", string dataSource = "stubhub")
        public IActionResult GetSpecificVenue(string venue, string dataSource)
        {
            string result = "";
            if (dataSource == "stubhub")
            {
                result = selectedStubhubVenueEvent(venue);
            }
            else if (dataSource == "ticketmaster")
            {
                result = selectedTicketmasterVenueEvent(venue);
            }
            else
            {
                result = $"[{selectedStubhubVenueEvent(venue)}, {selectedTicketmasterVenueEvent(venue)}]";
            }
            return Ok(result);
        }
        
        public string selectedStubhubVenueAddress(string venue)
        {
            string qry = @$"with a as (
                        SELECT
                               [name]
                              --,[url]
                              ,[postalCode]
                              ,[city]
                              ,[state]
                              ,[address1]
                              ,[address2]
                          FROM [stubhubApi].[dbo].[tblTicketmasterVenue]

	                        UNION

                          SELECT
                               [name]
                              --,[url]
                              ,[postalCode]
                              ,[city]
                              ,[state]
                              ,[address1]
                              ,[address2]
                          FROM [stubhubApi].[dbo].[tblStubhubVenue]
                          )
                          select top(1) 
                              [name]
                              ,[postalCode]
                              ,[city]
                              ,[state]
                              ,[address1]
                              ,[address2] from a where [name] = '{venue}'
                            for json path";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
        }

        [Route("GetSpecificVenueAddress")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        //public IActionResult GetSpecificVenueEvent(string venue = "Paradise Rock Club", string dataSource = "stubhub")
        public IActionResult GetSpecificVenueAddress(string venue)
        {
            string result = selectedStubhubVenueAddress(venue);
            return Ok(result);
        }

        public string selectedStubhubVenueEvent(string venue = "stubhub")
        {
            string qry = @"SELECT 
	              --[index]
                  --,[id]     
                  [name]
	              ,[status]
                 -- ,[description]
                  --,[webURI]
                  --,[eventDateLocal]
                  --,[eventDateUTC]
                  --,[createdDate]
                  --,[lastUpdatedDate]
                  --,[hideEventDate]
                  --,[hideEventTime]
                  --,[timezone]
                  --,[currencyCode]
                  --,[venue_id]
                  --,[venue_name]
                  --,[venue_city]
                  --,[venue_state]
                  --,[venue_postalCode]
                  --,[venue_country]
                 -- ,[venue.venueConfigId]
                  --,[venue.venueConfigName]
                  ,[minListPrice]
                  ,[maxListPrice]
                  ,[totalTickets]
                  ,[totalListings]
              FROM [stubhubApi].[dbo].[tblStubhubVenueEvent]
              Where venue_id in (select id from tblStubhubVenue where [name] ='" + venue + @"' and [name] not like '%Parking Lot%')
                           FOR JSON PATH";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
        }
        public string selectedTicketmasterVenueEvent(string venue)
        {
            string qry = @"SELECT [name]
                              --,[url]
                              --,[sales_public_startDateTime]
                              --,[sales_public_endDateTime]
                              --,[dates_start_localTime]
                              ,[dates_start_dateTime]
                              --,[dates_status_code]
                              --,[seatmap_staticUrl]
                              --,[promoter_id]
                              --,[promoter_name]
                              --,[promoter_description]
                              --,[ticketLimit]
                              --,[classifications]
                              --,[priceRanges]
                              --,[_embedded_venues]
                              ,[family]
                              ,[segment_name]
                              ,[genre_name]
                              ,[subGenre_name]
                              ,[priceRanges_min]
                              ,[priceRanges_max]
                              ,[venue_id]
                          FROM [stubhubApi].[dbo].[tblTicketmasterVenueEvent]
                            Where venue_id in (select id from tblTicketmasterVenue where [name] ='" + venue + @"' and [name] not like '%Parking Lot%')
                           FOR JSON PATH";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return result;
        }

        [Route("GetStubHubVenueCountAndLocation")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        public IActionResult GetStubHubVenueCountAndLocation()
        {
            string qry = @"with vc as
                            (
                                select count(1) as numberOfVenues, city as City from [tblStubHubVenue] 
                                group by city
                            )
                            , result as 
                            (
                                select vc.*
                                       ,c.latitude as Latitude 
                                       ,c.longitude as Longitude
                                from vc
                                left join tblStubHubCity c 
                                       on vc.city = c.city
                                where c.latitude is not null
                                  and c.longitude is not null
                            ) 
                            select * from result
                            for json path";

            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return Ok(result);
        }

        [Route("GetStubHubCityVenues")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        public IActionResult GetStubHubCityVenues()
        {
            string qry = @"with a as (
	                            select v.[city] as City
	                            , v.[name] as Venue
	                            , count(1) as numberOfEvents
	                            , MIN(ve.[minListPrice]) as LowestTicketPrice
	                            , max(ve.[maxListPrice]) as HighestTicketPrice
	                            from [tblStubHubVenueEvent] ve
                                LEFT JOIN [tblStubHubVenue] v
	                            where v.[name] not like '%Parking Lots%'
	                            group by v.[name], v.[city]
	                        )
	                        select a.*, c.latitude, c.longitude from a 
	                        left join tblStubHubCity c 
	                        on c.city = a.City
	                        order by a.numberOfEvents DESC
                            for json path";
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return Ok(result);
        }

        [Route("GetTicketMasterVenueCountAndLocation")]
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        public IActionResult GetTicketMasterVenueCountAndLocation()
        {
            string qry = @"with cityCount as 
                                (
                                SELECT 
                                      [city.name] as City
                                      , COUNT(1) as numberOfVenues
                                  FROM [stubhubApi].[dbo].[tblTicketMasterVenue]
                                  GROUP BY [city.name]
                                )
                                SELECT cc.City, cc.numberOfVenues, v.Latitude, v.Longitude FROM cityCount cc 
                                LEFT JOIN (
                                SELECT 
	                                [city.name] as City
	                                ,[location.longitude] as Longitude
	                                ,[location.latitude] as Latitude     
                                FROM [stubhubApi].[dbo].[tblTicketMasterVenue]
                                ) v
                                on v.City = cc.City
                            for json PATH";//use path instead of auto for flat json when joining tables
            dbConnector db = new dbConnector();
            string result = db.getQueryResultAsJsonString(qry);
            return Ok(result);
        }


    }
    
}
