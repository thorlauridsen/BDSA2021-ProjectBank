using Microsoft.EntityFrameworkCore;
using ProjectBank.Infrastructure;

namespace ProjectBank.Server.Model
{
    public static class SeedExtensions
    {
        public static async Task<IHost> SeedAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProjectBankContext>();
                await SeedData(context);
            }
            return host;
        }

        private static async Task SeedData(ProjectBankContext context)
        {
            await context.Database.MigrateAsync();

            var user1 = new User { oid = "1", Name = "Paolo" };
            var user2 = new User { oid = "2", Name = "Tue" };
            var generatedUser4 = new User { oid = "3", Name = "Aaron Duane" };
            var generatedUser5 = new User { oid = "4", Name = "P─▒nar T├Âz├╝n" };
            var generatedUser0 = new User { oid = "5", Name = "Veronika Cheplygina" };
            var generatedUser1 = new User { oid = "6", Name = "Sebastian B├╝ttrich" };
            var generatedUser2 = new User { oid = "7", Name = "Maria Astefanoaei" };
            var generatedUser3 = new User { oid = "8", Name = "Björn Þór Jónsson" };

            if (!await context.Users.AnyAsync())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(generatedUser4);
                context.Users.Add(generatedUser5);
                context.Users.Add(generatedUser0);
                context.Users.Add(generatedUser1);
                context.Users.Add(generatedUser2);
                context.Users.Add(generatedUser3);
            }

            var comment = new Comment
            {
                Content = "Nice post",
                User = user2,
                DateAdded = DateTime.Now
            };
            /*if (!await context.Comments.AnyAsync())
            {
                context.Comments.Add(comment);
            }*/
            var post = new Post
            {
                Title = "Biology Project",
                Content = "My Cool Biology Project",
                DateAdded = DateTime.Now,
                User = user1,
                Comments = new List<Comment>(){comment},
                Tags = new string[] { "Biology" }
            };
            if (!await context.Posts.AnyAsync())
            {
                var generatedPost0 = new Post
                {
                    Title = "Finding hidden features responsible for machine learning failures",
                    Content =
                        "There have been several situations where machine learning classifiers, trained to diagnose a particular disease (for example, lung cancer from chest x-rays), overfit on hidden features within the data. Examples include gridlines, surgical markers or evidence of treatment or text present in the images (see references for examples). This causes the classifier to fail on other type of images",
                    DateAdded = DateTime.Now,
                    User = generatedUser0,
                    Tags = new string[]
                        {"machine learning", "data science", "medical imaging"}
                };
                context.Posts.Add(generatedPost0);
                var generatedPost1 = new Post
                {
                    Title = "Parallels of medical imaging and natural language processing",
                    Content =
                        "Machine learning is used extensively in different applications, including medical imaging and natural language processing. As different types of data are involved, it is reasonable to assume that different methods are needed for each application. However, there are also opportunities in translating a method successful in one application, to the other application where it is not widely used. The",
                    DateAdded = DateTime.Now,
                    User = generatedUser0,
                    Tags = new string[]
                    {
                        "machine learning", "natural language processing", "medical imaging",
                        "literature review"
                    }
                };
                context.Posts.Add(generatedPost1);
                var generatedPost2 = new Post
                {
                    Title = "Predicting popularity of machine learning challenges",
                    Content =
                        "Machine learning challenges hosted on platforms such as Kaggle (general) or grand-challenge.org (medical imaging) have attracted a lot of attention, both from academia and industry researchers. Challenge designs vary widely [1], including what type of data is available, how the algorithms are evaluated, and the rewards for the winners. In medical imaging, there is some evidence that challenges",
                    DateAdded = DateTime.Now,
                    User = generatedUser0,
                    Tags = new string[]
                        {"machine learning", "data science", "medical imaging"}
                };
                context.Posts.Add(generatedPost2);
                var generatedPost3 = new Post
                {
                    Title = "Danish Student Cubesat",
                    Content =
                        "The Danish Student Cubesat Program is an inter university collaboration that will launch 3 cubesats into Low Earth Orbit over the next 4 years. The satellites will be designed, operated, programmed and built by students and the project offers an opportunity for MasterÔÇÖs students to take part in a live satellite project. ITU is partnering with Aarhus University on DISCOSAT2 which will be an",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                    {
                        "Satellite", "Cubesat", "Image processing",
                        "machine learning",
                        "edge", "constrained computing"
                    }
                };
                context.Posts.Add(generatedPost3);
                var generatedPost4 = new Post
                {
                    Title = "IoT and ML enhanced scarecrow",
                    Content =
                        "Invasive bird species can be a serious problem in cities, towns and in agriculture. The common pigeon is a very unwelcome guest on many balconies, roofs, terraces. Conventional scarecrows often show no effect, as these birds are known to be quite intelligent, and capable of learning fast. The idea is to built a sensor/camera enhanced scarecrow that - can recognize birds present within its",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                    {
                        "IoT", "machine learning", "sensors",
                        "security"
                    }
                };
                context.Posts.Add(generatedPost4);
                var generatedPost5 = new Post
                {
                    Title = "Machine learning for acoustic/vibrational analysis of wooden building materials",
                    Content =
                        "For this project, you would be working with a partner company who are looking to re-establish wood as a building material for sustainable architecture, and thus are using sensors for quality control - to detect damages and deterioration in buildings. Wood such as timber may be analyzed by non-intrusive acoustic impact testing and subsequent waveform analysis, and the expectation is that machine",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                        {"IoT", "sensors", "machine learning", "acoustics"}
                };
                context.Posts.Add(generatedPost5);
                var generatedPost6 = new Post
                {
                    Title = "Satellite LoRaWAN nodes for low bandwidth human communication",
                    Content =
                        "Recent progress in LoRaWAN development has made a new generation of satellite communications offerings available to IoT devices. In these, the LoRaWAN gateway is satellite born, and collecting data from small inexpensive ground stations. So far, this is predominantly seen as a means of communciation for remote sensor data, e.g. in agriculture, logistics or wildlife monitoring. However, one can",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                    {
                        "IoT", "LoRaWAN", "LPWAN", "Satellite", "networks",
                        "edge", "security"
                    }
                };
                context.Posts.Add(generatedPost6);
                var generatedPost7 = new Post
                {
                    Title = "Tiny embedded machine vision for metering (and beyond)",
                    Content =
                        "There is currently a lot of progress in really small, yet powerful visual machine learning / computer vision, on hardware like the OpenMV Cam H7, Arduino Portenta Vision Shield, Luxonis LUX-ESP32, Himax WE-I Plus, Arducam Pico4ML, and Raspberry Pi, and on software platforms such as TinyML or OpenMV IDE. While many popular use cases stem from fields like traffic analysis, wildlife monitoring, we",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                        {"IoT", "sensors", "machine learning", "computer vision"}
                };
                context.Posts.Add(generatedPost7);
                var generatedPost8 = new Post
                {
                    Title = "Sensors at Sea - Maritime IoT",
                    Content =
                        "Deliberately scoped very wide, this group contains a number of projects in different possible directions, from Location services via LPWAN time-of-flight and GPS/GNSS, Vessel tracking and management in fisheries, tourism and logistcs Water quality anc chemistry sensing for Aquaculure, specifically Mariculture, Wave and tidal dynamics, e.g. in energy harvesting and variations/combinations of",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                    {
                        "Satellite", "Image processing", "machine learning", "edge",
                        "constrained computing", "IoT", "sensors", "location"
                    }
                };
                context.Posts.Add(generatedPost8);
                var generatedPost9 = new Post
                {
                    Title = "Danish Student Cubesat",
                    Content =
                        "The Danish Student Cubesat Program is an inter university collaboration that will launch 3 cubesats into Low Earth Orbit over the next 4 years. The satellites will be designed, operated, programmed and built by students and the project offers an opportunity for MasterÔÇÖs students to take part in a live satellite project. ITU is partnering with Aarhus University on DISCOSAT2 which will be an",
                    DateAdded = DateTime.Now,
                    User = generatedUser1,
                    Tags = new string[]
                    {
                        "Satellite", "Cubesat", "Image processing",
                        "machine learning",
                        "edge", "constrained computing"
                    }
                };
                context.Posts.Add(generatedPost9);
                var generatedPost10 = new Post
                {
                    Title = "15-minutes cities visualisation",
                    Content =
                        "The idea behind ÔÇ£15-minutes citiesÔÇØ is that within a short walk or bike ride people should have access to all necessary facilities that constitute the essence of urban living, such as parks, shops, cafes, schools, hospitals. Initiatives to transform cities according to this paradigm are currently being implemented across the world, in an attempt to make urban spaces more liveable,",
                    DateAdded = DateTime.Now,
                    User = generatedUser2,
                    Tags = new string[]
                    {
                        "spatial data analysis", "visualisation", "Python",
                        "OSM data"
                    }
                };
                context.Posts.Add(generatedPost10);
                var generatedPost11 = new Post
                {
                    Title = "Algorithms for data-aware cycling network expansion",
                    Content =
                        "As a response to increased traffic congestion and the need to reduce carbon emissions, cities consider ways to modernise, build and extend transit systems. Transit network design solutions can benefit from analysing the large amount of crowd-sourced location data available, which provides valuable insights into population mobility needs. Designing efficient metro lines, bicycle paths, or bus",
                    DateAdded = DateTime.Now,
                    User = generatedUser2,
                    Tags = new string[]
                    {
                        "spatial data analysis", "Network Design", "Python",
                        "OSM data"
                    }
                };
                context.Posts.Add(generatedPost11);
                var generatedPost12 = new Post
                {
                    Title = "Graph summaries of accessibility maps",
                    Content =
                        "The idea behind ÔÇ£15-minutes citiesÔÇØ is that within a short walk or bike ride people should have access to all necessary facilities that constitute the essence of urban living, such as parks, shops, cafes, schools, hospitals. Initiatives to transform cities according to this paradigm are currently being implemented across the world, in an attempt to make urban spaces more liveable,",
                    DateAdded = DateTime.Now,
                    User = generatedUser2,
                    Tags = new string[]
                    {
                        "spatial data analysis", "graph summaries", "Python",
                        "OSM data"
                    }
                };
                context.Posts.Add(generatedPost12);
                var generatedPost13 = new Post
                {
                    Title = "Music genre embeddings",
                    Content =
                        "Musical genres are inherently ambiguous and difficult to define. Even more so is the task of establishing how genres relate to one another. Yet, genre is perhaps the most common and effective way of describing musical experience. The number of possible genre classifications (e.g. Spotify has over 4000 genre tags, LastFM over 500,000 tags) has made the idea of manually creating music taxonomies",
                    DateAdded = DateTime.Now,
                    User = generatedUser2,
                    Tags = new string[]
                    {
                        "Scalable Algorithms", "hyperbolic embeddings", "Python",
                        "Spotify data"
                    }
                };
                context.Posts.Add(generatedPost13);
                var generatedPost14 = new Post
                {
                    Title = "Spatiotemporal dependencies in wind energy production",
                    Content =
                        "The integration of wind power in the energy grid is dependent on accurate production forecasts. The power output curves between neighbouring wind farms are often correlated temporally and spatially, but currently, these spatiotemporal dependencies are under-utilised in prediction models. Graph neural networks allow for modelling these dependencies. In this project the student will implement a",
                    DateAdded = DateTime.Now,
                    User = generatedUser2,
                    Tags = new string[]
                    {
                        "spatial data analysis", "graph neural networks", "Python",
                        "timeseries data"
                    }
                };
                context.Posts.Add(generatedPost14);
                var generatedPost15 = new Post
                {
                    Title = "Diversity of Relevance Feedback",
                    Content =
                        "In relevance feedback, the choice of images to present to the user is a difficult problem, as a na├»ve approach may present too many similar images. The challenge addressed in this project is to ensure diversity (aka ÔÇ£one of eachÔÇØ) as well as relevance. A particularly interesting project for students interested in efficient algorithms. Read more",
                    DateAdded = DateTime.Now,
                    User = generatedUser3,
                    Tags = new string[]
                        {"multimedia analytics", "scalability", "diversity"}
                };
                context.Posts.Add(generatedPost15);
                var generatedPost16 = new Post
                {
                    Title = "Hash-Based Indexing for Relevance Feedback",
                    Content =
                        "In interactive learning systems, such as Exquisitor, the system presents potentially relevant images to users who label them as either relevant or irrelevant. Currently, Exquisitor uses a cluster-based index, which allows it to return results from a collection of 100 million images in 0.3 seconds. The goal of this project is to study the application of hash-based indexing to interactive learning",
                    DateAdded = DateTime.Now,
                    User = generatedUser3,
                    Tags = new string[] {"multimedia analytics", "diversity"}
                };
                context.Posts.Add(generatedPost16);
                var generatedPost17 = new Post
                {
                    Title = "Video Browser Showdown",
                    Content =
                        "The goal of this project is to enhance PhotoCube as a competior for the Video Browser Showdown, an international video retrieval competition where competing systems are judged based on speed, accuracy and recall. We propose to develop new versions of the C++-based media server and JS-based media browser, to expand the data model to videos and improve the performance sufficiently to take part in",
                    DateAdded = DateTime.Now,
                    User = generatedUser3,
                    Tags = new string[]
                        {"video search", "multimedia analytics", "photocube"}
                };
                context.Posts.Add(generatedPost17);
                var generatedPost18 = new Post
                {
                    Title = "Virtual Reality Analytics",
                    Content =
                        "We are actively developing a new prototype for analysing large multimedia collections in virtual reality, based on the ObjectCube data model. There are many ways in which students can contribute to the project, including work on the user interface and the back-end, and later on running large-scale user experiments. Read more",
                    DateAdded = DateTime.Now,
                    User = generatedUser4,
                    Tags = new string[] {"virtual reality", "multimedia analytics"}
                };
                context.Posts.Add(generatedPost18);

                var generatedPost20 = new Post
                {
                    Title = "Analysis of NVMe SSDs and the IO stack",
                    Content =
                        "NVMe SSDs are not a uniform class of devices. IO software stack is not uniform either. Understanding the performance characteristics of new-generation SSDs and the impact of the IO stack on their performance is crucial while determining how to design data-intensive systems. In this project, we would like to characterize the performance of a range of NVMe SSDs (e.g., Samsung Z-SSD, Intel Optane,",
                    DateAdded = DateTime.Now,
                    User = generatedUser5,
                    Tags = new string[] {"SSD", "benchmarking"}
                };
                context.Posts.Add(generatedPost20);
                var generatedPost21 = new Post
                {
                    Title = "Benchmarking ARM-based storage controllers for disaggregated storage",
                    Content =
                        "Disaggregated storage has gained acceptance in data centers. With disaggregated storage, storage resources are decoupled from compute resources, and made available through fabric. We are particularly interested in storage resources composed of an ARM-based smartNIC, which acts as fabric target as well as storage controller for a collection of SSDs. The performance characteristics of the storage",
                    DateAdded = DateTime.Now,
                    User = generatedUser5,
                    Tags = new string[]
                    {
                        "benchmarking", "ARM", "SoC", "fabric", "SSD",
                        "computational storage"
                    }
                };
                context.Posts.Add(generatedPost21);
                var generatedPost22 = new Post
                {
                    Title = "Exploiting Edge Devices for Data-Intensive Applications",
                    Content =
                        "The work on running data-intensive applications on very powerful, expensive, and power-hungry server hardware is very popular thanks to the growing size of data centers and high-performance computing (HPC) platforms. However, with the rise of new generation internet of things (IoT) applications, the lower-power and lower-budget hardware devices that specifically target IoT, the edge platforms,",
                    DateAdded = DateTime.Now,
                    User = generatedUser5,
                    Tags = new string[]
                        {"edge", "NVIDIA Jetson", "Raspberry Pi", "Odroid"}
                };
                context.Posts.Add(generatedPost22);
                var generatedPost23 = new Post
                {
                    Title = "Workload Characterization for Machine Learning",
                    Content =
                        "A data science infrastructure orchestrates the execution of widely used machine learning frameworks (e.g., TensorFlow , PyTorch) on a heterogeneous set of processing units (e.g., CPU, GPU, TPU, FPGA) while powering an increasingly diverse and complex range of applications (e.g., fraud detection, healthcare, virtual assistance, automatic driving). Understanding the resource consumption",
                    DateAdded = DateTime.Now,
                    User = generatedUser5,
                    Tags = new string[]
                    {
                        "benchmarking", "hardware resource consumption",
                        "deep learning frameworks"
                    }
                };
                context.Posts.Add(generatedPost23);


                context.Posts.Add(post);
            }

            await context.SaveChangesAsync();
        }
    }
}