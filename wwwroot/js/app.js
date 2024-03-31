class SiteUtility {
    isMobile() {
        return window.innerWidth <= 768;
    }

    scrollToTop() {
        window.scrollTo(0, 0);
    }
    
    scrollToBottom() {
        window.scrollTo(0, document.body.scrollHeight);
    }
    
    refreshPage() {
        window.location.reload(true); // The 'true' argument forces the browser to reload from the server, not the cache.
    }
}

// Create a global instance of the class
window.siteUtility = new SiteUtility();
