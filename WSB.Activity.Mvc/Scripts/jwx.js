function WeChat() {
    this.config = function (sign) {
        if (wx && sign) {
            wx.config({
                debug: false,
                appId: sign.appID,
                timestamp: sign.timestamp,
                nonceStr: sign.noncestr,
                signature: sign.signature,
                jsApiList: [
                    "onMenuShareAppMessage",
                    "onMenuShareTimeline",
                    "onMenuShareQQ",
                    "onMenuShareQZone",
                    "onMenuShareWeibo"
                ]
            });
        }
    };

    this.ready = function (data) {
        function shareSuccess() {
            alert("分享成功！");
        }

        function shareCancel() {
            alert("取消分享！");
        }

        if (wx && data) {
            wx.ready(function () {
                wx.error(function () { });
                //分享至朋友
                wx.onMenuShareAppMessage({
                    title: data.title,
                    desc: data.desc,
                    link: data.link,
                    imgUrl: data.imgUrl,
                    success: function () {
                        shareSuccess();
                    },
                    cancel: function () {
                        shareCancel();
                    }
                });
                //分享至朋友圈
                wx.onMenuShareTimeline({
                    title: data.title,
                    desc: data.desc,
                    link: data.link,
                    imgUrl: data.imgUrl,
                    success: function () {
                        shareSuccess();
                    },
                    cancel: function () {
                        shareCancel();
                    }
                });
                //分享至QQ
                wx.onMenuShareQQ({
                    title: data.title,
                    desc: data.desc,
                    link: data.link,
                    imgUrl: data.imgUrl,
                    success: function () {
                        shareSuccess();
                    },
                    cancel: function () {
                        shareCancel();
                    }
                });
                //分享至QQ空间
                wx.onMenuShareQZone({
                    title: data.title,
                    desc: data.desc,
                    link: data.link,
                    imgUrl: data.imgUrl,
                    success: function () {
                        shareSuccess();
                    },
                    cancel: function () {
                        shareCancel();
                    }
                });
                //分享至腾讯微博
                wx.onMenuShareWeibo({
                    title: data.title,
                    desc: data.desc,
                    link: data.link,
                    imgUrl: data.imgUrl,
                    success: function () {
                        shareSuccess();
                    },
                    cancel: function () {
                        shareCancel();
                    }
                });
            });
        }
    };
}

WeChat.prototype.init = function (url, link) {
    var that = this;
    $.ajax({
        url: url,
        type: "POST",
        data: { url: window.location.href },
        dataType: "json",
        success: function (result, status, xhr) {
            if (result) {
                that.config(result);
                that.ready({
                    title: "100天免费宽带，快来抢哦",
                    desc: "合肥有线微信营业厅福利大放送！",
                    link: link,
                    imgUrl: "http://test.jingkan.net/act/images/share.png",
                });
            }
        },
        error: function (xhr, status, error) {
            alert("微信signature获取失败！");
        }
    });
};