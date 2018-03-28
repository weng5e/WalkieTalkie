// https://developer.mozilla.org/en-US/docs/Web/API/MediaStream_Recording_API/Using_the_MediaStream_Recording_API#Basic_app_setup

declare var MediaRecorder: any;

export class AudioHelper {

    constructor() {
        this._isAudioAvailable = this.isAPIAvailable();
    }

    public get isAudioAvailable(): boolean {
        return this._isAudioAvailable;
    }

    public setUp() {
        if (!this._isAudioAvailable) {
            console.log('getUserMedia not supported on your browser!');
            return;
        }

        if (this._mediaRecorder !== null) {
            console.log('Recorder already set up.');
            return;
        }

        navigator.mediaDevices.getUserMedia(AudioHelper._audioConstraints)
            .then((stream) => {
                this._mediaRecorder = new MediaRecorder(stream);
                this._mediaRecorder.onstop = (e: any) => {
                    const audio = new Audio();
                    const blob = new Blob(this._chunks, { 'type': 'audio/ogg; codecs=opus' });
                    this._chunks.length = 0;
                    audio.src = window.URL.createObjectURL(blob);
                    audio.load();
                    audio.play();
                };
                this._mediaRecorder.ondataavailable = (e: any) => this._chunks.push(e.data);
            })
            .catch((err) => {
                console.log('The following getUserMedia error occured: ' + err);
            });
    }

    public record() {
        if (this._isRecording) {
            console.log('Recorder is already recording.');
            return;
        }

        this._isRecording = true;
        this._mediaRecorder.start();
    }

    public stop() {
        if (!this._isRecording) {
            console.log('Recorder is already NOT recording.');
            return;
        }

        this._isRecording = false;
        this._mediaRecorder.stop();
    }

    private isAPIAvailable(): boolean {
        return navigator.mediaDevices !== null && navigator.mediaDevices.getUserMedia !== null;
    }

    private readonly _isAudioAvailable: boolean = false;

    private _isRecording: boolean = false;
    private _chunks: any = [];
    private _mediaRecorder: any = null;

    private static readonly _audioConstraints = { audio: true };
}