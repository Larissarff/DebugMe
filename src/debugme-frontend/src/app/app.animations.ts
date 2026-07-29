import { trigger, transition, style, query, animate, group } from '@angular/animations';

export const routeTransition = trigger('routeAnimations', [
  transition('* <=> *', [
    query(':enter, :leave', [
      style({
        position: 'absolute',
        left: 0,
        width: '100%',
        opacity: 0,
        transform: 'translateY(12px)'
      })
    ], { optional: true }),
    group([
      query(':leave', [
        animate('200ms ease-out', style({ opacity: 0, transform: 'translateY(-8px)' }))
      ], { optional: true }),
      query(':enter', [
        animate('350ms 150ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ], { optional: true })
    ])
  ])
]);
