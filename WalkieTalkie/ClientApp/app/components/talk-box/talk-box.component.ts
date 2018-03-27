import { Component } from '@angular/core';

@Component({
    selector: 'talk-box',
    templateUrl: './talk-box.component.html'
})
export class TalkBoxComponent {
    public isTalking: boolean = false;

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
    }

    private stopTalking(): void {
        this.isTalking = false;
    }

}
