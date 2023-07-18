function Alert(isReload) {
    this.isReload = !!isReload;
    this.$alert = $(".calert");
    this.$alertPrompt = this.$alert.find(".calert-body .calert-prompt");
    this.$alertMsg = this.$alert.find(".calert-body .calert-msg");
    this.$alertHelp = this.$alert.find(".calert-body .calert-help");
    this.$alertAction = this.$alert.find(".calert-footer .calert-action");
    this.$alertClose = this.$alert.find(".calert-footer .calert-close");

    this.hide = function () {
        this.$alert.hide();
    };
    this.close();
}

Alert.prototype = {
    open: function (data) {
        data = data || {};
        var that = this;
        if (data.prompt) {
            that.$alertPrompt.html(data.prompt);
        }
        if (data.msg) {
            that.$alertMsg.html(data.msg);
        }
        if (data.help) {
            that.$alertHelp.html(data.help);
        }
        if (data.action) {
            that.$alertAction.html(data.action);
        }
        if (data.close) {
            that.$alertClose.html(data.close);
        }
        that.$alert.show();
    },
    close: function () {
        var that = this;
        that.$alert.on("click", function (event) {
            that.hide();
            if (that.isReload) {
                window.location.reload(true);
            }
        });
        that.$alertClose.on("click", function (event) {
            event.stopPropagation();
            that.hide();
            if (that.isReload) {
                window.location.reload(true);
            }
        });
    }
};