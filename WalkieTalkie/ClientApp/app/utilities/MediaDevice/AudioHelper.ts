export class AudioHelper {

    constructor() {
        this._isAudioAvailable = this.isAPIAvailable();
    }

    public get isAudioAvailable(): boolean {
        return this._isAudioAvailable;
    }

    private isAPIAvailable(): boolean {
        return navigator.mediaDevices !== null && navigator.mediaDevices.getUserMedia !== null;
    }

    private readonly _isAudioAvailable: boolean = false;

}