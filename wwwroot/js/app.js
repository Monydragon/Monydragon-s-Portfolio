class SiteUtility {
    isMobile() {
        return window.innerWidth <= 768;
    }

    scrollToTop() {
        window.scrollTo(0, 0);
    }
}

// Create a global instance of the class
window.siteUtility = new SiteUtility();
