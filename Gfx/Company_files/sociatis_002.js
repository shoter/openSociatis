window.Sociatis = window.Sociatis || {}

Sociatis.Debug = function (printGroup, printType) {
    this.VERBOSE_SHOW = "VERBOSE_SHOW";
    this.NORMAL_PRINT = "NORMAL_PRINT";
    this.DEBUG_PRINT = "DEBUG_PRINT";
    this.NO_PRINT = "NO_PRINT";

    this.PrintGroups = ["General"];
    this.PrintTypes = ["NORMAL_PRINT"];

    this.Set = function (printGroup, printType) {
        var existingIndex = this.PrintGroups.indexOf(printGroup);

        if (existingIndex >= 0) {
            this.PrintTypes[existingIndex] = printType;
        }
        else {
            this.PrintGroups.push(printGroup);
            this.PrintTypes.push(printType);
        }
    }

    this.Log = function (originalMessage, printGroup) {
        var index = this.PrintGroups.indexOf(printGroup);
        if (index >= 0) {
            var type = this.PrintTypes[index];

            if (originalMessage !== null /*null may be an object*/ && typeof (originalMessage) === 'object' && type == StreamDebug.VERBOSE_SHOW) {
                originalMessage = JSON.stringify(originalMessage);

                originalMessage = originalMessage.replace("{", "{<br/>");
                originalMessage = originalMessage.replace("}", "<br/>}");
            }

            if (printGroup == undefined)
                printGroup = "General";


            var timeNow = new Date();
            var hours = timeNow.getHours();
            var minutes = timeNow.getMinutes();
            var seconds = timeNow.getSeconds();
            var timeString = "" + hours;
            timeString += ((minutes < 10) ? ":0" : ":") + minutes;
            timeString += ((seconds < 10) ? ":0" : ":") + seconds;
            timeString += ": ";

            message = timeString + "[" + printGroup + "]" + originalMessage;



            if (type === this.NORMAL_PRINT)
                console.log(message);
            else if (type === this.VERBOSE_SHOW)
                this.Verbose(message, originalMessage);
            else if (type === this.DEBUG_PRINT)
                console.log(message);
            else
                return;
        }
    }

    this.Verbose = function (message, originalMessage) {
        console.log(message);
    }

    this.Set(printGroup, printType);
    return message => {
        this.Log(message, printGroup);
    }
}