import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { RecorderJS } from "./RecorderJS";

// https://developer.mozilla.org/en-US/docs/Web/API/MediaStream_Recording_API/Using_the_MediaStream_Recording_API#Basic_app_setup

export class AudioHelper {
    private audioContext: AudioContext;

    private isRecording: boolean = false;
    private recorder: RecorderJS = new RecorderJS();
    private audioStream?: MediaStream;

    private static readonly _audioConstraints = { audio: true };

    public readonly recordsStream: Subject<any>;

    constructor() {
        this.recordsStream = new Subject();
        this.audioContext = new AudioContext();
    }

    public setUp() {
        if (!this.isAPIAvailable()) {
            console.log('getUserMedia not supported on your browser!');
            return;
        }

        let that = this;

        navigator.mediaDevices.getUserMedia(AudioHelper._audioConstraints)
            .then((stream) => {
                that.audioStream = stream;
                let gainNode = this.audioContext.createGain();
                let audioSourceNode = that.audioContext.createMediaStreamSource(stream);

                audioSourceNode.connect(gainNode);
                that.recorder.setUp(gainNode);
            })
            .catch((err) => {
                console.log('The following getUserMedia error occured: ' + err);
            });
    }

    public record(): void {
        if (this.isRecording) {
            console.log('Recorder is already recording.');
            return;
        }

        this.isRecording = true;
        this.recorder.start();
    }

    public stop() {
        if (!this.isRecording) {
            console.log('Recorder is already NOT recording.');
            return;
        }

        this.isRecording = false;
        this.recorder.stop();

        let audioBlob = this.recorder.exportWAV();
        if (audioBlob !== null) {
            this.recordsStream.next(audioBlob);
        }

    }


    private isAPIAvailable(): boolean {
        return navigator.mediaDevices !== null && navigator.mediaDevices.getUserMedia !== null;
    }

}