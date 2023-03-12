window["uuidv4"] = function () {
    // @ts-ignore
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
};

window["get_location_async"] = (promise_handler) => {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(position => {
            let value = position.coords.latitude + "," + position.coords.longitude;
            promise_handler.invokeMethodAsync('SetResult', value);
        });
    } else {
        let value = "0,0";
        promise_handler.invokeMethodAsync('SetResult', value);
    }
};

window["init_window_resize_event"] = (event_handler) => {
    let a = () => {
        let w = window.innerWidth;
        let h = window.innerHeight;

        setTimeout(() => {
            event_handler.invokeMethodAsync('SetSize', w, h);
        }, 0);
    };
    window.addEventListener('resize', a);
    setTimeout(a, 0);
};

window["add_intersection_observer"] = (event_handler, element) => {
    let a = (entry: IntersectionObserverEntry) => {
        setTimeout(() => {
            console.log('intersection_observer: SendIntersectionChanged', entry.isIntersecting, entry.intersectionRatio);
            event_handler.invokeMethodAsync('SendIntersectionChanged', entry.isIntersecting, entry.intersectionRatio);
        }, 0);
    };
    let observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            a(entry);
        });
    });
    element["intersection_observer"] = observer;
    observer.observe(element);
};

window["remove_intersection_observer"] = (element) => {
    let observer = element["intersection_observer"];
    if (observer) {
        observer.disconnect();
    } else {
        console.error('intersection_observer: does not exist', element)
    }
};

window["get_window_size"] = () => {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

window["remaining_height_set"] = (element: HTMLElement) => {
    setTimeout(() => {
        let offset_parent_element = <HTMLElement>element.offsetParent;
        if (offset_parent_element) {
            let offset_bottom = offset_parent_element.offsetHeight - element.offsetTop - element.offsetHeight;
            element.style.height = `${window.innerHeight - element.offsetTop - offset_bottom}px`;
        }
    }, 0);
};

window["send_window_resize"] = () => {
    setTimeout(() => {
        window.dispatchEvent(new Event('resize'));
    }, 0);
};




