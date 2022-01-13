using Data;
using Data.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patika2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OptimizationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<OptimizationController> _logger;

        public OptimizationController(ILogger<OptimizationController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("{id}/{n}")]
        public async Task<IActionResult> GetRoutes([FromRoute] long id, [FromRoute] int n)
        {
            /* 
             * Given vehicle id and the number of routes, returns n clusters of containers.
             */

            // Find the vehicle
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            // Get containers of the vehicle
            IEnumerable<Container> containerList = await unitOfWork.Vehicle.GetContainers(id);

            /* 
             * Prepare the input
             * Extract coordinates
             */
            List<Point> points = GetPointsFromContainers(containerList);


            /*
             * Apply the clustering algorithm
             * Split points into n clusters using k means
             * assignment[i] is the cluster no which ith point belongs to
             */
            List<int> assignment = KMeans(points, n);

            /*
             * Prepare the output
             * Gather containers that belong to the same cluster
             */
            List<List<Container>> clusters = GetClustersFromAssignment(containerList, assignment, n);


            return Ok(clusters);
        }

        private List<Point> GetPointsFromContainers(IEnumerable<Container> containers)
        {
            var points = new List<Point>();
            foreach (var container in containers)
            {
                points.Add(new Point(container.Latitude, container.Longitude));
            }
            return points;
        }

        private List<List<Container>> GetClustersFromAssignment(IEnumerable<Container> containers, List<int> assignment, int n)
        {
            var clusters = new List<List<Container>>();
            for (int i = 0; i < n; i++)
            {
                clusters.Add(new List<Container>());
            }

            for (int i = 0; i < assignment.Count(); i++)
            {
                var clusterNo = assignment[i];
                clusters[clusterNo].Add(containers.ElementAt(i));
            }
            return clusters;
        }

        private List<int> KMeans(List<Point> points, int n)
        {
            /*
             * Implementation of K means algorithm. n is the cluster number.
             */

            // Select n initial centroids
            List<Point> centroids = points.Take(n).ToList();

            // Assign each point to the closest cluster
            List<int> prevAssignment = AssignPointsToClusters(points, centroids, n);

            while (true)
            {
                centroids = UpdateCentroids(points, prevAssignment, n);

                List<int> assignment = AssignPointsToClusters(points, centroids, n);

                // Stop if assignments didn't change
                if (Enumerable.SequenceEqual(assignment, prevAssignment))
                {
                    break;
                }

                prevAssignment = assignment;
            }

            return prevAssignment;
        }



        private List<int> AssignPointsToClusters(List<Point> points, List<Point> centroids, int n)
        {
            /* 
             * One of the two main steps of K means algorithm.
             * Assign each point to the closest cluster. Similarity is measured by the squared Euclidean distance.
             */

            var assignment = new List<int>(points.Count());
            
            for(var i = 0; i < points.Count(); i++)
            {
                // For a point, find the closest centroid
                var point = points.ElementAt(i);

                double minDist = 0;

                for (var j = 0; j < centroids.Count(); j++)
                {
                    var centroid = centroids.ElementAt(j);

                    double dist = GetSquaredDistance(point, centroid);
                    if (j == 0 || dist < minDist)
                    {
                        // A closer centroid found. Assign.
                        minDist = dist;
                        if(assignment.Count() == i + 1)
                            assignment[i] = j;
                        else
                            assignment.Add(j);
                    }
                }
            }

            return assignment;
        }

        private List<Point> UpdateCentroids(List<Point> points, List<int> assignment, int n)
        {
            /*
             * One of the two main steps of K means algorithm.
             * Update the centroid of each cluster by averaging coordinates of points in that cluster.
             */

            var centroids = new List<Point>();
            var clusterPopulations = new List<int>();

            for (var i = 0; i < n; i++)
            {
                centroids.Add(new Point(0, 0));
                clusterPopulations.Add(0);
            }

            for (var i = 0; i < points.Count(); i++)
            {
                int clusterNo = assignment[i];
                clusterPopulations[clusterNo]++;
                centroids[clusterNo].X += points.ElementAt(i).X;
                centroids[clusterNo].Y += points.ElementAt(i).Y;
            }

            for (var i = 0; i < n; i++)
            {
                var count = clusterPopulations[i];
                centroids.ElementAt(i).X /= count;
                centroids.ElementAt(i).Y /= count;
            }

            return centroids;
        }

        public class Point
        {
            public Point(decimal x, decimal y)
            {
                X = x;
                Y = y;
            }
            public decimal X { get; set; }
            public decimal Y { get; set; }

        }

        private double GetSquaredDistance(Point p, Point q)
        {
            return Math.Pow((double)(p.X - q.X), 2) + Math.Pow((double)(p.Y - q.Y), 2);
        }


    }

}
