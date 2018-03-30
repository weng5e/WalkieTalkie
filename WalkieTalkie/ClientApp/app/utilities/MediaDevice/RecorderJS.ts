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
    mimeType: 'audio/wav',
    numOutputChannels: 2
};

export class RecorderJS {
    private inputChannelNumber: number = 0;
    private sampleRate: number = 0;

    private recording: boolean = false;
    private buffers: Float32Array[][] = [];
    private resultLength = 0;

    constructor() {

    }

    public setUp(source: GainNode) {
        this.initBuffers();

        let that = this;

        this.inputChannelNumber = source.channelCount;

        let audioContext = source.context;
        this.sampleRate = audioContext.sampleRate;

        let scriptNode = audioContext.createScriptProcessor(defaultConfig.bufferLen, this.inputChannelNumber, defaultConfig.numOutputChannels);

        scriptNode.onaudioprocess = (audioProcessingEvent) => {
            if (!that.recording) {
                return;
            }

            for (let i = 0; i < that.inputChannelNumber; i++) {
                let channelData = audioProcessingEvent.inputBuffer.getChannelData(i);
                let clone = new Float32Array(channelData.length);
                clone.set(channelData);

                that.buffers[i].push(clone);
                if (i === 0) {
                    that.resultLength += clone.length;
                }
            }
        }

        source.connect(scriptNode);
        scriptNode.connect(audioContext.destination);

    }

    public start(): void {
        if (!this.recording) {
            this.recording = true;
            this.initBuffers();
        }
    }

    public stop(): void {
        if (this.recording) {
            this.recording = false;
        }
    }

    public exportWAV() {
        if (this.resultLength === 0) return null;

        let mergedChannelData: Float32Array[] = [];
        for (let i = 0; i < this.inputChannelNumber; i++) {
            mergedChannelData.push(this.mergeBuffers(i));
        }

        let interleaved;
        if (this.inputChannelNumber === 2) {
            interleaved = this.interleave(mergedChannelData[0], mergedChannelData[1]);
        } else {
            interleaved = mergedChannelData[0];
        }

        let dataview = this.encodeWAV(interleaved);
        let audioBlob = new Blob([dataview], { type: defaultConfig.mimeType });

        let result = {
            sampleRate: this.sampleRate,
            channelCount: 1,
            blob: audioBlob
        };

        return result;
    }

    private mergeBuffers(channelNumber: number): Float32Array {
        let result = new Float32Array(this.resultLength);
        let idx = 0;
        let buff = this.buffers[channelNumber];
        for (let i = 0; i < buff.length; i++) {
            result.set(buff[i], idx);
            idx += buff[i].length;
        }
        return result;
    }

    private interleave(inputL: Float32Array, inputR: Float32Array) {
        let length = inputL.length + inputR.length;
        let result = new Float32Array(length);

        let index = 0,
            inputIndex = 0;

        while (index < length) {
            result[index++] = inputL[inputIndex];
            result[index++] = inputR[inputIndex];
            inputIndex++;
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
        view.setUint16(22, defaultConfig.numOutputChannels, true);
        /* sample rate */
        view.setUint32(24, this.sampleRate, true);
        /* byte rate (sample rate * block align) */
        view.setUint32(28, this.sampleRate * 4, true);
        /* block align (channel count * bytes per sample) */
        view.setUint16(32, defaultConfig.numOutputChannels * 2, true);
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

    private initBuffers() {
        this.buffers = [];
        this.resultLength = 0;
        for (let channel = 0; channel < this.inputChannelNumber; channel++) {
            this.buffers[channel] = [];
        }
    }
}