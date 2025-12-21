const ImageViewerFactory = (function () {

    function create(config) {

        const settings = Object.assign({
            enableZoomButtons: true,
            enableWheelZoom: true,
            enableDrag: true,
            enablePinchZoom: true,
            enableDoubleClickZoom: true,
            enableDownload: true,
            enforceBounds: true,
            resetOnClose: true,
            minScale: 1,
            maxScale: 4,
            zoomStep: 0.25
        }, config);

        const modal = document.querySelector(settings.modalId);
        const img = document.querySelector(settings.imageId);

        if (!modal || !img) {
            throw new Error('ImageViewer: modal or image element not found');
        }

        const zoomInBtn = document.querySelector(settings.zoomInBtnId);
        const zoomOutBtn = document.querySelector(settings.zoomOutBtnId);
        const resetBtn = document.querySelector(settings.resetBtnId);
        const downloadBtn = document.querySelector(settings.downloadBtnId);

        let scale = 1;
        let translateX = 0;
        let translateY = 0;
        let isDragging = false;
        let startX = 0;
        let startY = 0;
        let startDistance = 0;
        let startScale = 1;

        /* ------------------ CORE ------------------ */

        function clamp(v, min, max) {
            return Math.min(Math.max(v, min), max);
        }

        function applyTransform() {
            if (settings.enforceBounds) constrainBounds();
            img.style.transform =
                `translate(${translateX}px, ${translateY}px) scale(${scale})`;
        }

        function reset() {
            scale = 1;
            translateX = 0;
            translateY = 0;
            applyTransform();
        }

        function constrainBounds() {
            if (scale === 1) {
                translateX = translateY = 0;
                return;
            }

            const rect = img.getBoundingClientRect();
            const parent = img.parentElement.getBoundingClientRect();

            const maxX = Math.max((rect.width - parent.width) / 2, 0);
            const maxY = Math.max((rect.height - parent.height) / 2, 0);

            translateX = clamp(translateX, -maxX, maxX);
            translateY = clamp(translateY, -maxY, maxY);
        }

        /* ------------------ EVENTS ------------------ */

        img.addEventListener('dragstart', e => e.preventDefault());

        img.addEventListener('load', reset);

        if (settings.enableDoubleClickZoom) {
            img.addEventListener('dblclick', () => {
                scale = scale > 1 ? 1 : 2;
                translateX = translateY = 0;
                applyTransform();
            });
        }

        if (settings.enableWheelZoom) {
            img.addEventListener('wheel', e => {
                e.preventDefault();
                scale = clamp(
                    scale + (e.deltaY < 0 ? settings.zoomStep : -settings.zoomStep),
                    settings.minScale,
                    settings.maxScale
                );
                applyTransform();
            }, { passive: false });
        }

        if (settings.enableDrag) {
            img.addEventListener('mousedown', e => {
                img.style.cursor = 'grabbing'
                if (scale === 1) return;
                isDragging = true;
                startX = e.clientX - translateX;
                startY = e.clientY - translateY;
            });

            window.addEventListener('mousemove', e => {
                if (!isDragging) return;
                translateX = e.clientX - startX;
                translateY = e.clientY - startY;
                applyTransform();
            });

            window.addEventListener('mouseup', () => {
                isDragging = false;
                img.style.cursor = 'grab';
            });
        }

        if (settings.enablePinchZoom) {
            img.addEventListener('touchstart', e => {
                if (e.touches.length === 2) {
                    startDistance = getDistance(e.touches[0], e.touches[1]);
                    startScale = scale;
                }
            });

            img.addEventListener('touchmove', e => {
                if (e.touches.length === 2) {
                    e.preventDefault();
                    const newDist = getDistance(e.touches[0], e.touches[1]);
                    scale = clamp(
                        startScale * (newDist / startDistance),
                        settings.minScale,
                        settings.maxScale
                    );
                    applyTransform();
                }
            }, { passive: false });
        }

        if (settings.enableZoomButtons) {
            zoomInBtn?.addEventListener('click', () => {
                scale = clamp(scale + settings.zoomStep, settings.minScale, settings.maxScale);
                applyTransform();
            });

            zoomOutBtn?.addEventListener('click', () => {
                scale = clamp(scale - settings.zoomStep, settings.minScale, settings.maxScale);
                applyTransform();
            });

            resetBtn?.addEventListener('click', reset);

            zoomInBtn.classList.remove('d-none')
            zoomOutBtn.classList.remove('d-none')
        } else {
            resetBtn.classList.add('d-none')
        }

        if (settings.resetOnClose) {
            modal.addEventListener('hidden.bs.modal', reset);
        }

        function getDistance(t1, t2) {
            return Math.hypot(
                t2.clientX - t1.clientX,
                t2.clientY - t1.clientY
            );
        }

        /* ------------------ PUBLIC API ------------------ */

        return {
            open(url) {
                img.src = url;
                if (img.complete) reset();
            },
            download(url, filename = 'download') {
                if (settings.enableDownload && downloadBtn) {
                    downloadBtn.href = url;
                    downloadBtn.setAttribute('download', filename);
                    downloadBtn.classList.remove('d-none')
                }
            },
            reset,
            destroy() {
                img.src = '';
            }
        };
    }

    return { create };

})();
