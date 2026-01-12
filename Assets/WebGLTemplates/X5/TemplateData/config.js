var buildUrl = "Build";
var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
var config = {
    dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
    frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
    #if USE_THREADS
    workerUrl: buildUrl + "/{{{ WORKER_FILENAME }}}",
    #endif
    #if USE_WASM
    codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
    #endif
    #if MEMORY_FILENAME
    memoryUrl: buildUrl + "/{{{ MEMORY_FILENAME }}}",
    #endif
    #if SYMBOLS_FILENAME
    symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
    #endif
    streamingAssetsUrl: "StreamingAssets",
    companyName: {{{JSON.stringify(COMPANY_NAME)}}},
productName: {{{JSON.stringify(PRODUCT_NAME)}}},
productVersion: {{{JSON.stringify(PRODUCT_VERSION)}}},
};