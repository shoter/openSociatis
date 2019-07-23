window.Sociatis = window.Sociatis || {};

Sociatis.WalletChooser = (function () {

    function c($walletChooser, chosenHandler) {
        var self = this;
        this.chosenHandler = chosenHandler;
        this.$walletChooser = $walletChooser;
        $(function () {
            $walletChooser.change(function (e) { self.onWalletChange(e); });
        });
    }
    var prot = c.prototype;

    prot.onWalletChange = function () {
        this.walletID = this.$walletChooser.val();
        this.requestAmountData(this.walletID, this.currencyID)
    };


    prot.requestAmountData = function (walletID, currencyID) {
        var url = "/Wallet/GetWalletCurrencyAmount";
        var data = {
            walletID: walletID,
            currencyID: currencyID
        };
        var self = this;

        $.postJSON(url, data, data => Sociatis.HandleJson(data, function (data) { self.onResponse(data); }, currencyID));
    };

    prot.onResponse = function(response, currencyID) {
        var amount = response.Data;
        this.chosenHandler(amount, currencyID);
    };

    prot.SetCurrencyID = function (currencyID) {
        this.currencyID = currencyID;
    }

    return c;
})();