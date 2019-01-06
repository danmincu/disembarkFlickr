using FlickrNet;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DisembarkFlickr
{
    class Program
    {
        static string ApiKey = "8f13d9b0695d480a276018917ce1dd34";
        static string SharedSecret = "7082bbd2cc861c3f";
        static int retryCount = 3;

        private static OAuthRequestToken requestToken;


        static void Main1(string[] args)
        {
            string authFileName = "authentication.json";
            Flickr flickr = new Flickr(ApiKey, SharedSecret);

            var requestToken = flickr.OAuthGetRequestToken("oob");

            OAuthAccessToken accessToken = null;
            if (System.IO.File.Exists(authFileName))
            {
                var settings = System.IO.File.ReadAllText(authFileName);
                accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthAccessToken>(settings);
            }

            if (accessToken == null)
            {
                string url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

                System.Diagnostics.Process.Start(url);
                Console.WriteLine("Introduce the verification number and press <Enter>:");
                var verification = Console.ReadLine();

                accessToken = flickr.OAuthGetAccessToken(requestToken, verification);
                System.IO.File.WriteAllText(authFileName, JsonConvert.SerializeObject(accessToken));
                Console.WriteLine("The token is now saved");
            }

            flickr.OAuthAccessToken = accessToken.Token;
            flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;

            Console.WriteLine("Successfully authenticated as " + accessToken.FullName);
            //var accessToken = this.Flickr.OAuthGetAccessToken("72157633939557148-5055405cdf089fc5", "402a0be8ed9dbe1f", "440-205-244");
            //var accessToken = new Flickr("3e496589300dcfc9fe5ca1d322ed389d").OAuthGetAccessToken("72157633939557148-5055405cdf089fc5", "402a0be8ed9dbe1f", "440-205-244");
            //var m = accessToken;


            List<string> sb = new List<string>();
            StringBuilder sbUrl = new StringBuilder();


            var sets = flickr.PhotosetsGetList();
            int ii = 0;
            foreach (var item in sets)
            {
                var page = 0;
                var list = new List<Photo>();

                var p = flickr.PhotosetsGetPhotos(item.PhotosetId, page, 500);
                list.AddRange(p);

                while (p.Count == 500)
                {
                    p = flickr.PhotosetsGetPhotos(item.PhotosetId, ++page, 500);
                    list.AddRange(p);
                }

                var totalPhotosForAlbum = list;

                int pccount = 0;

                foreach (var photo in totalPhotosForAlbum)
                {
                    try
                    {
                        Policy
                        .Handle<Exception>()
                        .Retry(3)
                        .Execute(() =>
                        {
                            sb.Add(JsonConvert.SerializeObject(photo) + '\n');
                            Console.Write($"{++pccount}-");
                            var sizes = flickr.PhotosGetSizes(photo.PhotoId);
                            var original = sizes.FirstOrDefault(s => s.Label.Equals("Original", StringComparison.InvariantCultureIgnoreCase));
                            //var photoStaticURL = "https://farm" + photo.Farm + ".staticflickr.com/" + photo.Server + "/" + photo.PhotoId+ "_" + photo.Secret + "_b.jpg";
                            sbUrl.AppendLine(original?.Source);
                        });
                    }
                    catch (Exception e)
                    {
                        //bury
                    }

                }

                System.IO.File.WriteAllText($@"photoInfo-{++ii}.json", $"[{string.Join(",", sb)}]");
                System.IO.File.WriteAllText($@"Original-Url-{ii}.json", sbUrl.ToString());
            }

            return;


            for (int i = 154; i <= 168; i++)
            {
                Console.WriteLine($"Processing page {i}/{168}");
                sb.Clear();
                sbUrl.Clear();
                PhotoCollection pc = null;
                Policy
                    .Handle<Exception>()
                    .Retry(3)
                    .Execute(() =>
                    {
                        pc = flickr.PhotosGetNotInSet(i, 500);
                        Console.WriteLine($"Page {i} has {pc.Count} photos.");
                    });

                int pccount = 0;

                foreach (var photo in pc)
                {
                    try
                    {
                        Policy
                        .Handle<Exception>()
                        .Retry(3)
                        .Execute(() =>
                        {
                            sb.Add(JsonConvert.SerializeObject(photo) + '\n');
                            Console.Write($"{++pccount}-");
                            var sizes = flickr.PhotosGetSizes(photo.PhotoId);
                            var original = sizes.FirstOrDefault(s => s.Label.Equals("Original", StringComparison.InvariantCultureIgnoreCase));
                            //var photoStaticURL = "https://farm" + photo.Farm + ".staticflickr.com/" + photo.Server + "/" + photo.PhotoId+ "_" + photo.Secret + "_b.jpg";
                            sbUrl.AppendLine(original?.Source);
                        });
                    }
                    catch (Exception e)
                    {
                        //bury
                    }

                }

                System.IO.File.WriteAllText($@"photoInfo-{i}.json", $"[{string.Join(",", sb)}]");
                System.IO.File.WriteAllText($@"Original-Url-{i}.json", sbUrl.ToString());
            }
        }


        static void Main2(string[] args)
        {
            string authFileName = "authentication.json";
            Flickr flickr = new Flickr(ApiKey, SharedSecret);

            var requestToken = flickr.OAuthGetRequestToken("oob");

            OAuthAccessToken accessToken = null;
            if (System.IO.File.Exists(authFileName))
            {
                var settings = System.IO.File.ReadAllText(authFileName);
                accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthAccessToken>(settings);
            }

            if (accessToken == null)
            {
                string url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

                System.Diagnostics.Process.Start(url);
                Console.WriteLine("Introduce the verification number and press <Enter>:");
                var verification = Console.ReadLine();

                accessToken = flickr.OAuthGetAccessToken(requestToken, verification);
                System.IO.File.WriteAllText(authFileName, JsonConvert.SerializeObject(accessToken));
                Console.WriteLine("The token is now saved");
            }

            flickr.OAuthAccessToken = accessToken.Token;
            flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;

            Console.WriteLine("Successfully authenticated as " + accessToken.FullName);
            //var accessToken = this.Flickr.OAuthGetAccessToken("72157633939557148-5055405cdf089fc5", "402a0be8ed9dbe1f", "440-205-244");
            //var accessToken = new Flickr("3e496589300dcfc9fe5ca1d322ed389d").OAuthGetAccessToken("72157633939557148-5055405cdf089fc5", "402a0be8ed9dbe1f", "440-205-244");
            //var m = accessToken;


            List<string> sb = new List<string>();
            StringBuilder sbUrl = new StringBuilder();

            for (int i = 154; i <= 168; i++)
            {
                Console.WriteLine($"Processing page {i}/{168}");
                sb.Clear();
                sbUrl.Clear();
                PhotoCollection pc = null;
                Policy
                    .Handle<Exception>()
                    .Retry(3)
                    .Execute(() =>
                    {
                        pc = flickr.PhotosGetNotInSet(i, 500);
                        Console.WriteLine($"Page {i} has {pc.Count} photos.");
                    });

                int pccount = 0;

                foreach (var photo in pc)
                {
                    try
                    {
                        Policy
                        .Handle<Exception>()
                        .Retry(3)
                        .Execute(() =>
                        {
                            sb.Add(JsonConvert.SerializeObject(photo) + '\n');
                            Console.Write($"{++pccount}-");
                            var sizes = flickr.PhotosGetSizes(photo.PhotoId);
                            var original = sizes.FirstOrDefault(s => s.Label.Equals("Original", StringComparison.InvariantCultureIgnoreCase));
                            //var photoStaticURL = "https://farm" + photo.Farm + ".staticflickr.com/" + photo.Server + "/" + photo.PhotoId+ "_" + photo.Secret + "_b.jpg";
                            sbUrl.AppendLine(original?.Source);
                        });
                    }
                    catch (Exception e)
                    {
                        //bury
                    }

                }

                System.IO.File.WriteAllText($@"photoInfo-{i}.json", $"[{string.Join(",", sb)}]");
                System.IO.File.WriteAllText($@"Original-Url-{i}.json", sbUrl.ToString());
            }
        }

        static void Main2()
        {
            //DownloadUrisFromFile("Original-Url-32.json").Wait();
            for (int i = 152; i <= 168; i++)
            {
                var folderName = $@"e:\FFlickr100-168\{i}";
                System.IO.Directory.CreateDirectory(folderName);
                DownloadUrisFromFile($@"F:\FlickrMetadata\Original-Url-{i}.json", folderName).Wait();
            }

        }

        static void Main3()
        {
            //DownloadUrisFromFile("Original-Url-32.json").Wait();
            //for (int i = 152; i <= 168; i++)
            {
                var folderName = $@"e:\FFlickr100-168\Albums";
                System.IO.Directory.CreateDirectory(folderName);
                DownloadUrisFromFile($@"F:\FlickrMetadata\Original-Url-24.json", folderName).Wait();
            }

        }

        static void Main()
        {
            // GETTING THE TAGS

            string authFileName = "authentication.json";
            Flickr flickr = new Flickr(ApiKey, SharedSecret);

            var requestToken = flickr.OAuthGetRequestToken("oob");

            OAuthAccessToken accessToken = null;
            if (System.IO.File.Exists(authFileName))
            {
                var settings = System.IO.File.ReadAllText(authFileName);
                accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthAccessToken>(settings);
            }

            if (accessToken == null)
            {
                string url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

                System.Diagnostics.Process.Start(url);
                Console.WriteLine("Introduce the verification number and press <Enter>:");
                var verification = Console.ReadLine();

                accessToken = flickr.OAuthGetAccessToken(requestToken, verification);
                System.IO.File.WriteAllText(authFileName, JsonConvert.SerializeObject(accessToken));
                Console.WriteLine("The token is now saved");
            }

            flickr.OAuthAccessToken = accessToken.Token;
            flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;

            Console.WriteLine("Successfully authenticated as " + accessToken.FullName);

            for (int i = 31; i < 169; i++)
            {
                Console.WriteLine($"Processing {i}");
                try
                {
                    var fileName = $"photoInfo-{i}";
                    var photos = System.IO.File.ReadAllText($@"F:\FlickrMetadata\{fileName}.json");
                    var photosList = JsonConvert.DeserializeObject<Photo[]>(photos).Select(p => p.PhotoId).ToList();

                    var results = new StringBuilder();

                    photosList.AsParallel().ForAll(
                        new Action<string>(s =>
                        {
                            try
                            {
                                var tags = flickr.TagsGetListPhoto(s);//.Select(mm => mm.Raw).ToList<string>().Union(;

                                results.AppendLine(string.Join("|", new List<string> { s }.Union(tags.Select(mm => mm.Raw))));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"PhotoId:{s}. Exception {e}");
                            }
                        }));
                    System.IO.File.WriteAllText($@"F:\FlickrMetadata\{fileName}-tags.json", results.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.ReadLine();
            }
        }


        static async Task DownloadTags(List<string> results, Flickr flickr, string[] photoIds, string folderName)
        {
            const int CONCURRENCY_LEVEL = 20;    // Maximum of 4 requests at a time
            int nextIndex = 0;
            var downloadTasks = new List<Task>();
            while (nextIndex < CONCURRENCY_LEVEL && nextIndex < photoIds.Length)
            {

                var callback = new Action<FlickrResult<System.Collections.ObjectModel.Collection<PhotoInfoTag>>>(t =>
                {
                    results.Add(string.Join("-", t.Result.Select(m => m.Raw)));
                });

                flickr.TagsGetListPhotoAsync(photoIds[nextIndex], callback);
                nextIndex++;
            }
        }


        static async Task DownloadUrisFromFile(string fileName, string folderName)
        {
            var lines = System.IO.File.ReadAllLines(fileName);
            await Download(lines.Where(l => !string.IsNullOrEmpty(l)).Select(l => new Uri(l)).ToArray(), folderName).ConfigureAwait(false);
        }

        //private static async Task<string> RequestData(Uri uri, string fileName)
        //{
        //    await new WebClient().DownloadFileTaskAsync(uri, fileName).ConfigureAwait(false);
        //    return fileName;
        //}

        static async Task Download(Uri[] uris, string folderName)
        {
            const int CONCURRENCY_LEVEL = 20;    // Maximum of 4 requests at a time
            int nextIndex = 0;
            var downloadTasks = new List<Task<string>>();
            while (nextIndex < CONCURRENCY_LEVEL && nextIndex < uris.Length)
            {
                Console.WriteLine("Queuing up initial download #{0}.", nextIndex + 1);
                var fileName = $@"{folderName}\{System.IO.Path.GetFileName(uris[nextIndex].LocalPath)}";
                Func<Task<string>> del = async () =>
                {
                    await new WebClient().DownloadFileTaskAsync(uris[nextIndex], fileName).ConfigureAwait(false);
                    return fileName;
                };

                // not a good pattern to use as this would be appropriate for a CPU bound job
                //var dlg = Task.Run<string>(async () =>
                //{
                //    await new WebClient().DownloadFileTaskAsync(uris[nextIndex], fileName).ConfigureAwait(false);
                //    return fileName;
                //});


                //downloadTasks.Add(RequestData(uris[nextIndex], fileName));
                downloadTasks.Add(del.Invoke());
                nextIndex++;
            }

            while (downloadTasks.Count > 0)
            {
                try
                {
                    Task<string> downloadTask = await Task.WhenAny(downloadTasks);
                    downloadTasks.Remove(downloadTask);
                    var fn = await downloadTask.ConfigureAwait(false);
                    Console.WriteLine($"Downloaded {fn} file.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                if (nextIndex < uris.Length)
                {
                    Console.WriteLine("New download slot available.  Queuing up download #{0}.", nextIndex + 1);

                    //downloadTasks.Add(RequestData(uris[nextIndex], $@"F:\Flickr\{System.IO.Path.GetFileName(uris[nextIndex].LocalPath)}"));

                    var fileName = $@"{folderName}\{System.IO.Path.GetFileName(uris[nextIndex].LocalPath)}";
                    Func<Task<string>> del = async () =>
                    {
                        await new WebClient().DownloadFileTaskAsync(uris[nextIndex], fileName).ConfigureAwait(false);
                        return fileName;
                    };

                    downloadTasks.Add(del.Invoke());

                    nextIndex++;
                }
            }
        }

    }
}
