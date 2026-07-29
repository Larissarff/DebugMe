import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-cursor',
  template: `
    <div
      class="cursor-glow"
      [class.interactive]="isInteractive"
      [style.left.px]="x"
      [style.top.px]="y"
    ></div>
  `
})
export class CursorComponent {
  x = 0;
  y = 0;
  isInteractive = false;

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(event: MouseEvent): void {
    this.x = event.clientX;
    this.y = event.clientY;

    const target = event.target as HTMLElement;
    this.isInteractive =
      target.tagName === 'BUTTON' ||
      target.tagName === 'A' ||
      target.tagName === 'INPUT' ||
      target.tagName === 'SELECT' ||
      target.tagName === 'TEXTAREA' ||
      target.closest('.action-card') !== null ||
      target.closest('.event-card') !== null ||
      target.closest('.emotion-item') !== null;
  }
}
