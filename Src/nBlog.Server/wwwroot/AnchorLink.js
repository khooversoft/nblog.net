
window.anchorLink = {
    scrollIntoView: function (id) {
        // This function is called from the AnchorLink component using JavaScript interop.
        // It will try to find an element using the ID given to the function, and scroll that
        // element into view, if an element is found.
        const element = document.getElementById(id);
        if (element instanceof HTMLElement) {
            element.scrollIntoView({
                behavior: "smooth",
                block: "start",
                inline: "nearest"
            });
        }
    }
}
