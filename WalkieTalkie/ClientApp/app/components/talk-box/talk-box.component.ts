import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { AudioHelper } from "../../utilities/MediaDevice/AudioHelper";

@Component({
    selector: 'talk-box',
    templateUrl: './talk-box.component.html'
})
export class TalkBoxComponent {
    public isTalking: boolean = false;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.audioHelper = new AudioHelper();
        this.audioHelper.setUp();
        this.audioHelper.recordsStream.subscribe((blob: any) => {
            const audio = new Audio();
            audio.src = window.URL.createObjectURL(blob);
            audio.load();
            audio.play();

            let formData = new FormData();
            formData.append("audioFile", blob);

            this.http.post(this.baseUrl + 'api/Speech/ASR', formData).subscribe(res => {
                console.log(res);
            }, err => {
                console.log(err);
            });
        })
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
    private http: Http;
    private baseUrl: string;

}
