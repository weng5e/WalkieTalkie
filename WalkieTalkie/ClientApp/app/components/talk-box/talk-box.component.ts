import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { Http, RequestOptionsArgs, ResponseContentType } from '@angular/http';
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
    private recognizing: boolean = false;

    public isTalking: boolean = false;

    @ViewChild('audioFileInput') audioFileInput: ElementRef;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.audioHelper = new AudioHelper();
        this.audioHelper.setUp();
        this.audioHelper.recordsStream.subscribe((data: any) => {



            this.postBlobToRecognize(data.blob);
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

            this.postBlobToRecognize(fileToUpload);
        }
    }

    private postBlobToRecognize(blob: any) {
        let formData = new FormData();
        formData.append("audioFile", blob);

        this.recognizing = true;

        this.http.post(this.baseUrl + 'api/Speech/ASR', formData).subscribe(res => {
            this.recognizing = false;
            console.log(res);
            this.appendResults(res);

            // Read the response.
            let jsonObj = res.json();
            if (jsonObj && jsonObj.recognitionStatus && jsonObj.recognitionStatus === "RecognitionSuccess"
                && jsonObj.results && jsonObj.results.length > 0) {
                this.readText(jsonObj.results[0].displayText);
            }

        }, err => {
            this.recognizing = false;
            console.log(err);
            this.appendResults(err);
        });
    }

    private readText(text: string) {
        let options: RequestOptionsArgs = { responseType: ResponseContentType.Blob };
        this.http.get(this.baseUrl + 'api/TTS/GetAudio?text=' + btoa(text), options).subscribe(res => {
            // this.recognizing = false;
            console.log(res);

            const audio = new Audio();
            audio.src = window.URL.createObjectURL(res.blob());
            audio.load();
            audio.play();

        }, err => {
            this.recognizing = false;
            console.log(err);
            this.appendResults(err);
        });



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
