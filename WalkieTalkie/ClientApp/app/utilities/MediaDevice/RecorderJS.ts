/*License (MIT)

Copyright Â© 2013 Matt Diamond

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

// https://stackoverflow.com/questions/16413063/html5-record-audio-to-file

export calss RecorderJSConfig {
    /**
     * The length of the buffer that the internal JavaScriptNode uses to capture the audio. Can be tweaked if experiencing performance issues. Defaults to 4096.
     * 
     * @type {number}
     * @memberof RecorderJSConfig
     */
    bufferLen: number;

    /**
     *  A default callback to be used with exportWAV.
     * 
     * @type {ExportWAVCallback}
     * @memberof RecorderJSConfig
     */
    callback: (n: any) => void;

    /**
     * number of audio channels.
     * 
     * @type {number}
     * @memberof RecorderJSConfig
     */
    numChannels: number;

    /**
     * The type of the Blob generated by exportWAV. Defaults to 'audio/wav'.
     * 
     * @type {string}
     * @memberof RecorderJSConfig
     */
    mimeType: string;
}

let defaultConfig: RecorderJSConfig = {
    bufferLen: 4096
};

export class RecorderJS {
    private config: RecorderJSConfig;

    constructor(source: any, cfg: RecorderJSConfig) {
        if (cfg === null) {

        }
        else {
            this.config = cfg;
        }
    }
}