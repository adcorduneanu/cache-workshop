const cacheName = 'v1';

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches
            .open(cacheName)
            .then((cache) => cache.addAll(
                [    
                    'icon.png',               
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
                        .map((cacheName) => caches.delete(cacheName))
                );
            })
    );
});

self.addEventListener('fetch', (event) => {
    event.respondWith(
        fetch(event.request)
            .catch(() => caches.match(event.request))
            .catch(() => caches.match('/offline.html'))
    );
});