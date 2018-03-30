import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { Http } from '@angular/http';
import { AudioHelper } from "../../utilities/MediaDevice/AudioHelper";

@Component({
    selector: 'talk-box',
    templateUrl: './talk-box.component.html'
})
export class TalkBoxComponent {
    private audioHelper: AudioHelper;
    private http: Http;
    private baseUrl: string;
    private results: string[] = [];

    public isTalking: boolean = false;

    @ViewChild('audioFileInput') audioFileInput: ElementRef;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.audioHelper = new AudioHelper();
        this.audioHelper.setUp();
        this.audioHelper.recordsStream.subscribe((data: any) => {
            const audio = new Audio();
            audio.src = window.URL.createObjectURL(data.blob);
            audio.load();
            audio.play();

            let formData = new FormData();
            formData.append("audioFile", data.blob);

            // this.http.post(this.baseUrl + 'api/Speech/ASR', formData).subscribe(res => {
            //     console.log(res);
            //     this.appendResults(res);
            // }, err => {
            //     console.log(err);
            //     this.appendResults(err);
            // });
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

    public uploadAudioFile(): void {
        let fi = this.audioFileInput.nativeElement;
        if (fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];

            let formData = new FormData();
            formData.append("audioFile", fileToUpload);

            this.http.post(this.baseUrl + 'api/Speech/ASR', formData).subscribe(res => {
                console.log(res);
                this.appendResults(res);
            }, err => {
                console.log(err);
                this.appendResults(err);
            });
        }
    }

    private appendResults(res: any) {
        this.results.push(JSON.stringify(res.json(), null, 4));
    }

    private startTalking(): void {
        this.isTalking = true;
        this.audioHelper.record();
    }

    private stopTalking(): void {
        this.isTalking = false;
        this.audioHelper.stop();
    }

}
