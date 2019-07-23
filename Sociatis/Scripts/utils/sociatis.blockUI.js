window.Sociatis = Sociatis || {};

Sociatis.UI = (function () {
    var logger = Sociatis.Debug("Sociatis.UI", "DEBUG_PRINT");
    var blockCount = 0;
    return {
        BlockUI : function (blockUIParams /*{}*/, blockCounts /*1*/) {
            if (blockCounts == undefined)
                blockCounts = 1;
            if (blockUIParams == undefined)
                blockUIParams = {};

            $.blockUI(blockUIParams);

            blockCount += blockCounts;
            logger("Block count(+ " + blockCounts + "), now = " + blockCount);
        },
        UnblockUI : function (unblockCounts /*1*/) {
            if (unblockCounts == undefined)
                unblockCounts = 1;

            blockCount -= unblockCounts;
            logger("Block count(- " + unblockCounts + "), now = " + blockCount);

            if (blockCount <= 0) {
                console.log("unblocked!");
                $.unblockUI();
                blockCount = 0;
            }
        }
    };
})();