import { BrotliDecode as d } from './decode.min.js';
Blazor.start({
    loadBootResource: (type, name, defaultUri, integrity) => {
        if (type !== 'dotnetjs' && type !== "configuration" /*&& location.hostname !== 'localhost'*/)
        {
            return fetch(defaultUri + '.br', { cache: 'no-cache' })
                .then(response => {
                    if (!response.ok) throw new Error(response.statusText);
                    return response.arrayBuffer();
                })
                .then((originalResponseBuffer) => {
                    const originalResponseArray = new Int8Array(originalResponseBuffer);
                    const decompressedResponseArray = d(originalResponseArray);
                    const contentType = type === 'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
                    return new Response(decompressedResponseArray, { headers: { 'content-type': contentType } });
                });
        }
    }
});
