using Common.Exceptions;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Json.Shoutbox;
using Sociatis.Helpers;
using Sociatis.Models.Shoutbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Controllers
{
    public class ShoutboxController : ControllerBase
    {
        private readonly IShoutboxMessageRepository shoutboxMessageRepository;
        private readonly IShoutBoxService shoutBoxService;
        private readonly IShoutboxChannelRepository shoutboxChannelRepository;


        public ShoutboxController(IShoutboxMessageRepository shoutboxMessageRepository, IPopupService popupService,
            IShoutBoxService shoutBoxService, IShoutboxChannelRepository shoutboxChannelRepository)
            :base(popupService)
        {
            this.shoutboxMessageRepository = shoutboxMessageRepository;
            this.shoutBoxService = shoutBoxService;
            this.shoutboxChannelRepository = shoutboxChannelRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult GetMessages(ShoutboxRequestJson request)
        {

            var query = shoutboxMessageRepository
                .Where(message => message.ChannelID == request.ChannelID);
            if (request.LastMessageID.HasValue)
                query = query.Where(message => message.ID < request.LastMessageID);

            query = query.OrderByDescending(msg => msg.ID)
                .Take(request.PageSize);

            var response = new ShoutboxResponseModel(query.ToList());
            return Json(response);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult GetChannels()
        {
            var channels = shoutboxChannelRepository
                .Select(c => new
                {
                    ChannelID = c.ID,
                    ChannelName = c.Name
                }).ToList();

            return JsonData(channels);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult WriteMessage(int channelID, int? parentID, string content)
        {
            try
            {
                var channel = shoutboxChannelRepository.GetById(channelID);
                var author = SessionHelper.CurrentEntity;

                MethodResult result = shoutBoxService.CanSendMessage(content, channel, author);
                if (result.IsError)
                    return JsonError(result);

                ShoutboxMessage message = null;

                if (parentID.HasValue)
                {
                    var parent = shoutboxMessageRepository.GetById(parentID.Value);
                    if (parent == null)
                        return JsonError("Error");

                    message = shoutBoxService.SendMessage(content, author, parent);
                }
                else
                {
                    message = shoutBoxService.SendMessage(content, author, channel);
                }

                return JsonData(new ShoutboxMessageViewModel(message));
            }
            catch (UserReadableException e)
            {
                return JsonError(e);
            }
            catch (Exception)
            {
                return JsonError("Unspecified error ocurred");
                throw;
            }
        }
    }
}