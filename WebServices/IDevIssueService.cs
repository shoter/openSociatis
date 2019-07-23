using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Issues;

namespace WebServices
{
    public interface IDevIssueService
    {
        string[] AllowedExtensions { get; }

        MethodResult CanCreateNewIssue(Citizen citizen, CreateIssueArgs args);
        DevIssue CreateNewIssue(Citizen citizen, CreateIssueArgs args);

        void SetVisibility(DevIssue issue, VisibilityOptionEnum option);

        MethodResult CanModerateIssue(Citizen citizen);
        void SetLabels(DevIssue issue, params DevIssueLabelTypeEnum[] labels);

        string GetPathToUploadedFile(DevIssueUploadedFile devIssueUploadedFile);
        string GetPathToUploadedFile(string filename);
        string GenerateFilenameForNewUpload(string extension);

        MethodResult CanSeeIssue(DevIssue issue, Citizen citizen);

        MethodResult CanWriteComment(DevIssue issue, Citizen citizen);

        void WriteComment(DevIssue issue, Citizen citizen, string content, VisibilityOptionEnum visibility);


        MethodResult CanVote(DevIssue issue, Citizen citizen, int score);
        void Vote(DevIssue issue, Citizen citizen, int score);
        MethodResult CanUnvote(DevIssue issue, Citizen citizen);
        void Unvote(DevIssue issue, Citizen citizen);
        VisibilityOptionEnum GetVisiblityOption(Citizen citizen);
    }
}
