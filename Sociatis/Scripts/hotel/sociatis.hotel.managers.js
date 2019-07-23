window.Sociatis = window.Sociatis || {};
Sociatis.Hotel = Sociatis.Hotel || {};

$(function () {
    $.each($("div.hotelManager"), function (i, el) {
        new Sociatis.Hotel.Manager($(el), Sociatis.Global.HotelID);
    });
});

Sociatis.Hotel.Manager = (function () {
    var p = ctr.prototype;
    function ctr($frame, hotelID) {
        var self = this;
        this.$frame = $frame;
        this.hotelID = hotelID;
        this.managerID = this.$frame.data("managerid");
        this.$unblock = $frame.find("button[data-savechanges]");
        this.$priority = $frame.find("input[data-priority]");
        
        this.$unblock.click(function () { self.saveChanges.call(self); });

        this.rights = [];

        $.each($frame.find("input[data-rights]"), function (i, el) {
            self.rights.push(new Right($(el)));
        });

        this.$frame.on("change", ":input", function () { self.onInputChanged.call(self); });

    };

    p.onInputChanged = function () {
        this.unblockSaveChanges();
    }
   
    
    p.unblockSaveChanges = function () {
        this.$unblock.removeAttr("disabled").removeClass("disabled");
    };

    p.blockSaveChanges = function () {
        this.$unblock.attr("disabled", "true").addClass("disabled");
    };

    p.getPriority = function () {
        return this.$priority.val();
    }

    p.saveChanges = function () {
        Sociatis.UI.BlockUI();
        var data =
            {
                rights: this.getRights(),
                priority: this.getPriority(),
                managerID: this.managerID,
                hotelID: this.hotelID
            };
        var url = "/Hotel/UpdateManager";
        var self = this;
        $.postJSON(url, data, (data) => Sociatis.HandleJson(data, function (response) { self.onSavedChanges.call(self, response); }));
    };

    p.onSavedChanges = function (response) {
        $.notify(response.Message, "success");
        this.blockSaveChanges();
        Sociatis.UI.UnblockUI();
    };

    p.getRights = function () {
        var rights = [];
        $.each(this.rights, function (i, right) {
            rights.push(right.get());
        });
        return rights;
    }

    var Right = (function () {
        var p = ctr.prototype;
        function ctr($right) {
            this.$right = $right;
        }

        p.get = function () {
            return {
                RightID: this.getRightID(),
                Value: this.isEnabled()
            };
        };

        p.getRightID = function () {
            return this.$right.val();
        }

        p.isEnabled = function () {
            return this.$right.is(":checked");
        };

        return ctr;
    })();
    

    return ctr;
})();