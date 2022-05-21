debugger
(async () => {
    const fragmentParams = window.location.hash.substr(1);
    if (fragmentParams) {
        console.log('Fragment supplied.');
        const params = new URLSearchParams(fragmentParams);
        params.append('response_mode', 'fragment');
        const newURL = 'redirect-delegate'; // relative to current URL
        let response
        try {
            response = await fetch(newURL,
                {
                    method: 'POST',
                    body: params,
                    mode: 'same-origin'
                });
            console.log(response);
            window.pageStatus = 'POST of fragment succeeded';
        } catch (error) {
            console.log(error)
            // Workaround for fact Chrome sometimes produces net::ERR_CERT_DATABASE_CHANGED
            // following successful fetch above on macOS.
            if (navigator.platform === "MacIntel") {
                window.pageStatus = 'POST of fragment succeeded';
            } else {
                window.pageStatus = 'POST of fragment failed';
            }
        }
    } else {
        window.pageStatus = 'No fragment supplied';
    }
    console.log(window.pageStatus);
})();
