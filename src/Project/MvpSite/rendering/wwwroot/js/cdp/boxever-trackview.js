//NAMETOOLONGEXCEPTION--CUSTOM CODE :: Boxever Tracking View
$(document).ready(function () {
    function delayUntilBoxeverIsReady() {
        if (window.Boxever && window.Boxever.getID() !== 'anonymous' && window._boxeverq) {
            window._boxeverq.push(function () {
                var viewEvent = {
                    "channel": window.boxeverService._config.channel,
                    "type": "VIEW",
                    "language": window.boxeverService._config.language,
                    "currency": window.boxeverService._config.currency,
                    "page": window.location.pathname,
                    "pos": window.boxeverService._config.pos,
                    "browser_id": window.Boxever.getID()
                };
                // Invoke event create
                // (<event msg>, <callback function>, <format>)
                console.log(viewEvent);
                window.Boxever.eventCreate(viewEvent, function (data) { }, 'json');
            });
        } else {
            const timeToWaitInMilliseconds = 1000;
            console.log(`Boxever is not ready yet. Waiting ${timeToWaitInMilliseconds}ms before retrying.`);
            window.setTimeout(delayUntilBoxeverIsReady, timeToWaitInMilliseconds);
        }
    }
    delayUntilBoxeverIsReady();

});
