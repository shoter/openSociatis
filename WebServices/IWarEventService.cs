using Entities;
using Entities.enums;

namespace WebServices
{
    public interface IWarEventService
    {
        void AddEvent(War war, WarStatusEnum warStatus);
    }
}