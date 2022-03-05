//NAMETOOLONGEXCEPTION--CUSTOM CODE :: Sitecore Send
var boxeverService = {};
(function () {
    this._config = {
        "websiteId": "5c3fd852-b4f7-4b09-9103-9ee1cb22377c"
    }

    ///Initialize Service
    this.init = function () {
        //load TrackerJS
        !function (t, n, e, o, a) { function d(t) { var n = ~~(Date.now() / 3e5), o = document.createElement(e); o.async = !0, o.src = t + "?ts=" + n; var a = document.getElementsByTagName(e)[0]; a.parentNode.insertBefore(o, a) } t.MooTrackerObject = a, t[a] = t[a] || function () { return t[a].q ? void t[a].q.push(arguments) : void (t[a].q = [arguments]) }, window.attachEvent ? window.attachEvent("onload", d.bind(this, o)) : window.addEventListener("load", d.bind(this, o), !1) }(window, document, "script", "//cdn.stat-track.com/statics/moosend-tracking.min.js", "mootrack");

        //initialize tracker
        mootrack('init', this._config.websiteId);
        // track a view of the current page
        mootrack('trackPageView');
    };

    this.formSubmission = function () {
        if (!window.mootrack) { !function (t, n, e, o, a) { function d(t) { var n = ~~(Date.now() / 3e5), o = document.createElement(e); o.async = !0, o.src = t + "?ts=" + n; var a = document.getElementsByTagName(e)[0]; a.parentNode.insertBefore(o, a) } t.MooTrackerObject = a, t[a] = t[a] || function () { return t[a].q ? void t[a].q.push(arguments) : void (t[a].q = [arguments]) }, window.attachEvent ? window.attachEvent("onload", d.bind(this, o)) : window.addEventListener("load", d.bind(this, o), !1) }(window, document, "script", "https://cdn.stat-track.com/statics/moosend-tracking.min.js", "mootrack"); }
    }

    this.init();
    this.formSubmission();
}).apply(boxeverService);