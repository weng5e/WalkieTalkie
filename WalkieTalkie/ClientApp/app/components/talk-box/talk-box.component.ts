import { Component } from '@angular/core';
import { AudioHelper } from "../../utilities/MediaDevice/AudioHelper";

@Component({
    selector: 'talk-box',
    templateUrl: './talk-box.component.html'
})
export class TalkBoxComponent {
    public isTalking: boolean = false;

    constructor() {
        this.audioHelper = new AudioHelper();
        this.audioHelper.setUp();
    }

    public toggleTalking(): void {
        if (this.isTalking) {
            this.stopTalking();
        }
        else {
            this.startTalking();
        }
    }

    private startTalking(): void {
        this.isTalking = true;
        this.audioHelper.record();
    }

    private stopTalking(): void {
        this.isTalking = false;
        this.audioHelper.stop();
    }

    private audioHelper: AudioHelper;

}
