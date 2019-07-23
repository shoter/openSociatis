using Common.Tasks;
using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Messages;
using Sociatis.Validators.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.BigParams.messages;
using WebServices.Helpers;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    public class MessageController : ControllerBase
    {
        IMessageRepository messageRepository;
        IMessageService messageService;
        IEntityRepository entityRepository;

        public MessageController(IMessageRepository messageRepository, IEntityRepository entityRepository,
            IPopupService popupService) : base(popupService)
        {
            this.messageRepository = messageRepository;
            this.entityRepository = entityRepository;
            this.messageService = new MessageService(messageRepository, entityRepository);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ViewResult Index()
        {
            var user = SessionHelper.CurrentEntity;
            var mailboxMessages = messageRepository.MailboxMessages.Where(mb => mb.Viewers_EntityID == user.EntityID);
               



            var vm = new MessageIndexViewModel(mailboxMessages);

            return View(vm);
        }

        [Route("Message/{threadID:int}/{page:int=0}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ViewResult Message(int threadID, int page)
        {
            var thread = messageRepository
                .Where((MessageThread t) => t.ID == threadID)
                .First();

            var vm = new ViewThreadViewModel(thread, 10, page);
            messageService.MarkAsRead(threadID, SessionHelper.CurrentEntity.EntityID);

            return View(vm);
        }

        [Route("Message/{threadID:int}/last")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Message(int threadID)
        {
            var thread = messageRepository
                .Where((MessageThread t) => t.ID == threadID)
                .FirstOrDefault();

            if (thread == null)
                return RedirectToHomeWithError("Thread does not exist!");

            var vm = new ViewThreadViewModel();
            vm.InitPagingParams(thread, 10, 0);
            vm.Paging.PageNumber = vm.Paging.PageCount - 1;
            vm.Initialize(thread, 10);

            messageService.MarkAsRead(threadID, SessionHelper.CurrentEntity.EntityID);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult AddRecipient(int threadID, int recipientID)
        {
            var entity = SessionHelper.CurrentEntity;
            var thread = messageRepository.Where((MessageThread t) => t.ID == threadID).FirstOrDefault();

            if (thread == null)
                return RedirectToHomeWithError("Thread does not exist!");

            if (messageService.IsMemberOfThread(entity, thread) == false)
                return RedirectToHomeWithError("You cannot do that!");

            var newEntity = entityRepository.GetById(recipientID);

            if (newEntity == null)
                return RedirectToHomeWithError("Recipient does not exist!");


            messageService.AddRecipient(newEntity, thread);
            AddSuccess($"Successfully added {newEntity.Name}");
            return RedirectBack();
        }



        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public RedirectToRouteResult SendMessage(int threadID, string content)
        {
            var entity = SessionHelper.CurrentEntity;

            var ps = new SendMessageParams()
            {
                AuthorID = entity.EntityID,
                Content = content,
                Date = DateTime.Now,
                Day = GameHelper.CurrentDay,
                ThreadID = threadID
            };

            messageService.SendMessage(ps);

            return RedirectToAction("Message", new { threadID = threadID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ViewResult Send()
        {
            var user = SessionHelper.CurrentEntity;

            var vm = new SendMessageViewModel();

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult GetEligibleRecipients(Select2Request request)
        {
            var currentEntityID = SessionHelper.CurrentEntity.EntityID;

            var query = request.Query?.Trim();

            var entities = entityRepository.Where(entity => entity.EntityID != currentEntityID)
                .Where(entity => entity.Name.ToLower().Contains(query))
                .OrderBy(entity => entity.Name)
                .Select(entity => new Select2Item()
                {
                    id = entity.EntityID,
                    text = entity.Name
                });

            return Response(entities, request);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Send(SendMessageViewModel vm)
        {
            SendMessageValidator validator = new SendMessageValidator(ModelState, entityRepository);

            validator.Validate(vm);

            if(validator.IsValid)
            {
                var user = SessionHelper.CurrentEntity;

                List<int> recipients = new List<int>()
                {
                    user.EntityID,
                };

                if (vm.RecipientID.HasValue)
                {
                    recipients.Add(vm.RecipientID.Value);
                }
                else
                {
                    var recipientID = entityRepository.Where(e => e.Name.ToLower() == vm.RecipientName.ToLower()).Select(e => e.EntityID).FirstOrDefault();
                    recipients.Add(recipientID);

                }



                var thread = messageService.CreateNewThread(recipients, vm.Title);

                var param = new SendMessageParams()
                {
                    AuthorID = user.EntityID,
                    Content = vm.Content,
                    Date = DateTime.Now,
                    Day = GameHelper.CurrentDay,
                    ThreadID = thread.ID
                };

                messageService.SendMessage(param);

                return RedirectToAction("Message", new { threadID = thread.ID });
            }

            return View(vm);
        }
    }
}