let currentUIScale = 1;
let requestId = null;
let stopped = false;

function createLoadingUI() {
    // Создаём root-контейнер
    const root = document.createElement('div');
    root.id = 'ui-root';

    // Статический фон - покрывает весь экран
    const background = document.createElement('div');
    background.className = 'loading-background';
    background.style.cssText = `
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background: #000000 url('TemplateData/Background.png') no-repeat center / cover;
    z-index: 1;
  `;

    const scaled = document.createElement('div');
    scaled.className = 'ui-scaled';

    // Прогрессбар (фиксированное позиционирование внизу экрана)
    const progress = document.createElement('div');
    progress.className = 'progress';

    progress.style.cssText = `
    position: fixed;
    bottom: 20px;
    left: 50%;
    transform: translateX(-50%);
    z-index: 100;
    width: 400px;
    max-width: 90vw;
  `;

    const box = document.createElement('div');
    box.className = 'box';

    const empty = document.createElement('div');
    empty.className = 'empty';
    empty.id = 'progress-bar-empty';

    const full = document.createElement('div');
    full.className = 'full';
    full.id = 'progress-bar-full';

    empty.appendChild(full);
    box.appendChild(empty);
    progress.appendChild(box);

    // Добавляем элементы в правильном порядке
    root.appendChild(background);  // Фон первым - покрывает весь экран
    root.appendChild(scaled);      // Масштабируемый контейнер (без прогрессбара)
    root.appendChild(progress);    // Прогрессбар отдельно - фиксированное позиционирование
    document.body.appendChild(root);

    // Масштабирование
    window.addEventListener('resize', scaleUIRoot);
    window.addEventListener('orientationchange', scaleUIRoot);
    scaleUIRoot();
}

// Масштаб по высоте экрана (контейнер всегда занимает всю высоту)
function scaleUIRoot() {
    const root = document.getElementById('ui-root');
    const scaled = root?.querySelector('.ui-scaled');
    if (!scaled || !root) return;

    const refWidth = 1920;
    const refHeight = 1080;
    const w = window.innerWidth;
    const h = window.innerHeight;

    // Масштабируем по высоте, чтобы контейнер всегда занимал весь экран по высоте
    const scale = h / refHeight;
    currentUIScale = scale;

    // Устанавливаем размеры контейнера и масштабируем
    scaled.style.width = `${refWidth}px`;
    scaled.style.height = `${refHeight}px`;
    scaled.style.transform = `scale(${scale})`;
    scaled.style.marginTop = `-${h * 0.05}px`; // Смещаем контейнер вверх на 5% от высоты экрана

    // Фон остается фиксированным и покрывает весь экран
    const background = root?.querySelector('.loading-background');
    if (background) {
        background.style.width = '100vw';
        background.style.height = '100vh';
    }
}


// Обновление прогресса (0.0 - 1.0)
function updateLoadingProgress(value) {
    const full = document.getElementById('progress-bar-full');
    if (full) {
        full.style.width = `${Math.min(Math.max(value, 0), 1) * 100}%`;
    }
}

// Очистка
function hideLoadingUI() {
    stopped = true;

    // Удаляем root контейнер и все его содержимое
    const root = document.getElementById('ui-root');
    if (root) {
        // Очищаем все дочерние элементы
        while (root.firstChild) {
            root.removeChild(root.firstChild);
        }
        root.remove();
    }

    // Удаляем обработчики событий
    window.removeEventListener('resize', scaleUIRoot);
    window.removeEventListener('orientationchange', scaleUIRoot);

    // Сбрасываем переменные
    currentUIScale = 1;

    // Принудительная очистка памяти
    if (window.gc) {
        window.gc();
    }
}

// Делаем глобально доступным
window.createLoadingUI = createLoadingUI;
window.updateLoadingProgress = updateLoadingProgress;
window.hideLoadingUI = hideLoadingUI;
