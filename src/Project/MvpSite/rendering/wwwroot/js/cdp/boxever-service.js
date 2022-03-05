﻿//NAMETOOLONGEXCEPTION--CUSTOM CODE :: Boxever view page events
var boxeverService = {};
(function () {
    this._config = {
        "channel": "WEB",
        "language": "EN",
        "currency": "EUR",
        "pos": "Orientation-GvT",
        "clientKey": "pqshkqqqoklsqwue4gs09b5idg0mv1d0",
        "domain": "demosite-rh.hackathon.localhost"
    }

    ///Initialize Service
    this.init = function () {
        // Define the Boxever queue
        var _boxeverq = _boxeverq || [];
        // Define the Boxever settings
        _boxever_settings = {
            client_key: this._config.clientKey, // Replace with your client key
            target: 'https://api.boxever.com/v1.2', // Replace with your API target endpoint specific to your data center region
            cookie_domain: this._config.domain // Replace with the top level cookie domain of the website that is being integrated e.g ".example.com" and not "www.example.com"
        };
        // Import the Boxever library asynchronously
        (function () {
            var s = document.createElement('script'); s.type = 'text/javascript'; s.async = true; s.id = "cdpSettings";
            s.src = 'https://d1mj578wat5n4o.cloudfront.net/boxever-1.4.8.min.js';
            var x = document.getElementsByTagName('script')[0]; x.parentNode.insertBefore(s, x);
        })();
    };

    this.trackViews = function () {
        // Place an anonymous function in the Boxever queue
        _boxeverq.push(function () {
            var viewEvent = {
                "channel": this._config.channel,
                "type": "VIEW",
                "language": this._config.language,
                "currency": this._config.currency,
                "page": window.location.pathname,
                "pos": this._config.pos,
                "browser_id": Boxever.getID()
            };
            // Invoke event create
            // (<event msg>, <callback function>, <format>)
            console.log(viewEvent);
            Boxever.eventCreate(viewEvent, function (data) { }, 'json');
        });
    }

    this.init();
    this.trackViews();
}).apply(boxeverService);