const cacheName = 'v1';

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches
            .open(cacheName)
            .then((cache) => cache.addAll(
                [
                    '/offline.html'
                ]
            ))
    );
});

self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches
            .keys()
            .then((cacheNames) => {
                return Promise.all(
                    cacheNames
                        .filter((cache) => cacheName != cache)
                        .map((cache) => caches.delete(cache))
                );
            })
    );
});

self.addEventListener('fetch', (event) => {
    event.respondWith(
        caches
            .match(event.request)
            .then((cacheResponse) => {
                if (cacheResponse) {
                    return cacheResponse;
                }

                return fetch(event.request)
                    .then(function (response) {
                        const clonedResponse = response.clone();

                        caches
                            .open(cacheName)
                            .then((cache) => {
                                cache.put(event.request, clonedResponse);
                            });

                        return response;
                    })
            })
            .catch(() => {
                return caches.match('/offline.html');
            })
    );
});