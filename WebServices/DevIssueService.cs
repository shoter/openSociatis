using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using WebServices.structs.Issues;
using Entities.enums;
using Entities.Extensions;
using WebServices.Helpers;
using Entities.Repository;
using System.Web;
using System.IO;
using Common.utilities;
using Weber.Html;
using WebServices.Html;

namespace WebServices
{
    public class DevIssueService : BaseService, IDevIssueService
    {
        private static string UploadFilesLocation = "~/Content/Issues/files/";
        

        public string[] AllowedExtensions { get; private set; } = new string[]
                {
                    "txt",
                    "png",
                    "jpg",
                    "jpeg",
                    "tiff"
                };


        private readonly IDevIssueRepository devIssueRepository;
        private readonly IDevIssueCommentRepository devIssueCommentRepository;
        private readonly IWarningService warningService;
        public DevIssueService(IDevIssueRepository devIssueRepository, IDevIssueCommentRepository devIssueCommentRepository,
            IWarningService warningService)
        {
            this.devIssueRepository = devIssueRepository;
            this.devIssueCommentRepository = devIssueCommentRepository;
            this.warningService = warningService;
        }


        public MethodResult CanCreateNewIssue(Citizen citizen, CreateIssueArgs args)
        {
            if (citizen == null)
                return new MethodResult("You do not exist!");

            if (string.IsNullOrWhiteSpace(args.Name))
                return new MethodResult("Name cannot be empty!");

            foreach (var file in args.UploadedFiles)
            {
                try
                {
                    if (file == null)
                        continue;

                    bool allowed = false;
                    foreach (var ext in AllowedExtensions)
                        if (file.FileName.EndsWith(ext))
                        {
                            allowed = true;
                            break;
                        }

                    if (allowed == false)
                        return new MethodResult($"File {file.FileName} have wrong extension!");

                    if (file.ContentLength > 5_000_000)
                        return new MethodResult("File is too big! Maximum 5MB");
                }
                catch
                {
                    return new MethodResult("There was error during an upload");
                }
            }

            return MethodResult.Success;
        }

        public MethodResult CanModerateIssue(Citizen citizen)
        {
            if (citizen.HaveRightsOfAtLeast(PlayerTypeEnum.Moderator) == false)
                return new MethodResult("You cannot do that!");
            return MethodResult.Success;
        }


        public DevIssue CreateNewIssue(Citizen citizen, CreateIssueArgs args)
        {
            var issue = new DevIssue()
            {
                Content = args.Content,
                Day = GameHelper.CurrentDay,
                Name = args.Name,
                VisibilityOptionID = (int)VisibilityOptionEnum.Author,
                Time = DateTime.Now,
                CreatedByID = citizen.ID
            };

            foreach (var file in args.UploadedFiles)
            {
                if (file == null)
                    continue;
                var extension = Path.GetExtension(file.FileName).Replace(".", "");
                var newFilename = GenerateFilenameForNewUpload(extension);
                var path = GetPathToUploadedFile(newFilename);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                file.SaveAs(path);
                issue.DevIssueUploadedFiles.Add(new DevIssueUploadedFile()
                {
                    Filename = newFilename,
                    OriginalFilename = file.FileName
                });
            }

            devIssueRepository.Add(issue);
            devIssueRepository.SaveChanges();

            return issue;
        }

        public void SetLabels(DevIssue issue, params DevIssueLabelTypeEnum[] labelTypes)
        {
            var labels = devIssueRepository.GetLabels(labelTypes);
            issue.DevIssueLabelTypes.Clear();
            issue.DevIssueLabelTypes = labels;
            ConditionalSaveChanges(devIssueRepository);
        }

        public void SetVisibility(DevIssue issue, VisibilityOptionEnum option)
        {
            issue.VisibilityOptionID = (int)option;
            ConditionalSaveChanges(devIssueRepository);
        }

        public MethodResult CanRemoveIssue(DevIssue issue, Citizen citizen)
        {
            if (citizen.HaveRightsOfAtLeast(PlayerTypeEnum.Admin) == false)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }
        public void RemoveIssue(DevIssue issue)
        {
            var files = issue.DevIssueUploadedFiles.ToList();

            foreach (var file in files)
            {
                var path = GetPathToUploadedFile(file);
                if (File.Exists(path))
                    File.Delete(path);
                devIssueRepository.RemoveSpecific(file);
            }

            devIssueRepository.Remove(issue);
            ConditionalSaveChanges(devIssueRepository);
        }

        public string GetPathToUploadedFile(DevIssueUploadedFile devIssueUploadedFile)
        {
            return GetPathToUploadedFile(devIssueUploadedFile.Filename);
        }
        public string GetPathToUploadedFile(string filename)
        {
            return Path.Combine(HttpContext.Current.Server.MapPath(UploadFilesLocation), filename);
        }



        public string GenerateFilenameForNewUpload(string extension)
        {
            string randomName;
            string path;
            do
            {
                randomName = RandomGenerator.GenerateString(40);
                path = GetPathToUploadedFile($"{randomName}.{extension}");
            } while (File.Exists(path) == true);

            return $"{randomName}.{extension}";
        }

        public MethodResult CanSeeIssue(DevIssue issue, Citizen citizen)
        {
            if (issue == null)
                return new MethodResult("Issue does not exist!");
            if (citizen.HaveRightsOfAtLeast(issue.GetVisibilityOption().GetMinimumPlayerTypeToView()) == false)
                return new MethodResult("You cannot do that!");

            if (citizen.GetPlayerType() == PlayerTypeEnum.Player && issue.GetVisibilityOption() == VisibilityOptionEnum.Author && citizen.ID != issue.CreatedByID)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public VisibilityOptionEnum GetVisiblityOption(Citizen citizen)
        {
            VisibilityOptionEnum visiblity = VisibilityOptionEnum.Everyone;
            if (citizen.GetPlayerType() == PlayerTypeEnum.Moderator)
                visiblity = VisibilityOptionEnum.Moderators;
            else if (citizen.GetPlayerType() >= PlayerTypeEnum.Admin)
                visiblity = VisibilityOptionEnum.Admins;
            return visiblity;
        }

        public MethodResult CanWriteComment(DevIssue issue, Citizen citizen)
        {
            return CanSeeIssue(issue, citizen);
        }

        public void WriteComment(DevIssue issue, Citizen citizen, string content, VisibilityOptionEnum visibility)
        {
            var comment = new DevIssueComment()
            {
                CitizenID = citizen.ID,
                Day = GameHelper.CurrentDay,
                Time = DateTime.Now,
                VisibilityOptionID = (int)visibility,
                DevIssueID = issue.ID,
                Content = content
            };

            var entityLink = EntityLinkCreator.Create(citizen.Entity);
            var issueLink = DevIssueLinkCreator.Create(issue);
            var msg = $"{entityLink} commented your issue - {issueLink}";

            warningService.AddWarning(issue.CreatedByID, msg);

            devIssueCommentRepository.Add(comment);
            devIssueCommentRepository.SaveChanges();
        }

        public MethodResult CanVote(DevIssue issue, Citizen citizen, int score)
        {
            var result = CanSeeIssue(issue, citizen);
            if (result.IsError)
                return result;

            if (citizen.GetPlayerType() == PlayerTypeEnum.Player && score != 1)
                return new MethodResult("You cannot do that!");

            if (issue.DevIssueVotes.Any(v => v.CitizenID == citizen.ID))
                return new MethodResult("You already voted!");

            return MethodResult.Success;
        }

        public void Vote(DevIssue issue, Citizen citizen, int score)
        {
            issue.DevIssueVotes.Add(new DevIssueVote()
            {
                CitizenID = citizen.ID,
                DevIssueID = issue.ID,
                Score = score
            });
            devIssueRepository.SaveChanges();
        }

        public MethodResult CanUnvote(DevIssue issue, Citizen citizen)
        {
            var result = CanSeeIssue(issue, citizen);
            if (result.IsError)
                return result;

            if (issue.DevIssueVotes.Any(v => v.CitizenID == citizen.ID) == false)
                return new MethodResult("You did not voted!");

            return MethodResult.Success;
        }

        public void Unvote(DevIssue issue, Citizen citizen)
        {
            var vote = issue.DevIssueVotes.First(v => v.CitizenID == citizen.ID);
            devIssueRepository.RemoveSpecific(vote);
            devIssueRepository.SaveChanges();
        }


    }
}
