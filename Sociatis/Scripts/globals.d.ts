
declare module Sociatis {

    interface Popups {

        AddMessage(message: string, messageClass: Sociatis.Popups.MessageClasses): void;
    }


    interface UI {

    }
}

declare module Sociatis.Utils {
    function GenerateGUID(): string;
}

declare module Sociatis.Popups {
    export enum MessageClasses {
        Error,
        Warning,
        Info,
        Success
    }
}
