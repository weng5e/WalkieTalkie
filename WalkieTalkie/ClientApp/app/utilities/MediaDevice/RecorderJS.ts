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


const defaultConfig = {
    bufferLen: 4096,
    mimeType: 'audio/wav;codecs=pcm'
};

export class RecorderJS {
    private inputChannelNumber: number = 0;
    private sampleRate: number = 0;

    private recording: boolean = false;
    private buffer: Float32Array[] = [];
    private resultLength = 0;

    constructor() {

    }

    public setUp(source: GainNode) {
        let that = this;

        this.inputChannelNumber = source.channelCount;

        let audioContext = source.context;
        this.sampleRate = audioContext.sampleRate;

        // Set the output channel number to 1. We only want mono audio.
        let scriptNode = audioContext.createScriptProcessor(defaultConfig.bufferLen, this.inputChannelNumber, 1);

        scriptNode.onaudioprocess = (audioProcessingEvent) => {
            if (!that.recording) {
                return;
            }

            // Only keep the left channel (sound track).
            let leftChannelData = audioProcessingEvent.inputBuffer.getChannelData(0);
            that.buffer.push(leftChannelData);
            that.resultLength += leftChannelData.length;
        }

    }

    public start(): void {
        if (!this.recording) {
            this.recording = true;
            this.buffer = [];
            this.resultLength = 0;
        }
    }

    public stop(): void {
        if (this.recording) {
            this.recording = false;
        }
    }

    public exportWAV(): Blob {
        let allAudioData = this.mergeBuffers();
        let dataview = this.encodeWAV(allAudioData);
        let audioBlob = new Blob([dataview], { type: defaultConfig.mimeType });
        return audioBlob;
    }

    private mergeBuffers(): Float32Array {
        let result = new Float32Array(this.resultLength);
        let idx = 0;
        for (let i = 0; i < this.buffer.length; i++) {
            result.set(this.buffer[i], idx);
            idx += this.buffer[i].length;
        }
        return result;
    }

    private encodeWAV(samples: Float32Array) {
        let buffer = new ArrayBuffer(44 + samples.length * 2);
        let view = new DataView(buffer);

        /* RIFF identifier */
        this.writeString(view, 0, 'RIFF');
        /* RIFF chunk length */
        view.setUint32(4, 36 + samples.length * 2, true);
        /* RIFF type */
        this.writeString(view, 8, 'WAVE');
        /* format chunk identifier */
        this.writeString(view, 12, 'fmt ');
        /* format chunk length */
        view.setUint32(16, 16, true);
        /* sample format (raw) */
        view.setUint16(20, 1, true);
        /* channel count */
        view.setUint16(22, 1, true);
        /* sample rate */
        view.setUint32(24, this.sampleRate, true);
        /* byte rate (sample rate * block align) */
        view.setUint32(28, this.sampleRate * 4, true);
        /* block align (channel count * bytes per sample) */
        view.setUint16(32, 2, true);
        /* bits per sample */
        view.setUint16(34, 16, true);
        /* data chunk identifier */
        this.writeString(view, 36, 'data');
        /* data chunk length */
        view.setUint32(40, samples.length * 2, true);

        this.floatTo16BitPCM(view, 44, samples);

        return view;
    }

    private writeString(view: DataView, offset: number, input: string) {
        for (let i = 0; i < input.length; i++) {
            view.setUint8(offset + i, input.charCodeAt(i));
        }
    }

    private floatTo16BitPCM(output: DataView, offset: number, input: Float32Array) {
        for (let i = 0; i < input.length; i++ , offset += 2) {
            let s = Math.max(-1, Math.min(1, input[i]));
            output.setInt16(offset, s < 0 ? s * 0x8000 : s * 0x7FFF, true);
        }
    }
}