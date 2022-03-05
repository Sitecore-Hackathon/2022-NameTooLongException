//NAMETOOLONGEXCEPTION--CUSTOM CODE :: Boxever init tracker
var boxeverService = {};
(function () {
    window.boxeverService._config = {
        "channel": "WEB",
        "language": "EN",
        "currency": "EUR",
        "pos": "NTLE-MVP-Site",
        "clientKey": "psfu6uh05hsr9c34rptlr06dn864cqrx",
        "domain": "mvp.sc-mvp.localhost"
    }
   

    ///Initialize Service
    this.init = function () {
        // Define the Boxever queue
        var boxeverq = boxeverq || [];
        window._boxeverq = boxeverq;
        
        // Define the Boxever settings
        _boxever_settings = {
            client_key: window.boxeverService._config.clientKey, // Replace with your client key
            target: 'https://api.boxever.com/v1.2', // Replace with your API target endpoint specific to your data center region
            cookie_domain: window.boxeverService._config.domain // Replace with the top level cookie domain of the website that is being integrated e.g ".example.com" and not "www.example.com"
        };
        // Import the Boxever library asynchronously
        (function () {
            var s = document.createElement('script'); s.type = 'text/javascript'; s.async = true; s.id = "cdpSettings";
            s.src = 'https://d1mj578wat5n4o.cloudfront.net/boxever-1.4.8.min.js';
            var x = document.getElementsByTagName('script')[0]; x.parentNode.insertBefore(s, x);
        })();        
    };
    this.init();
}).apply(boxeverService);