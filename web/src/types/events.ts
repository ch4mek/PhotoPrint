import { DecorativeIconConfig } from '@/layouts/components/HeaderParticles/components/DecorativeIcon/types';
import { StaticDecorationsConfig } from '@/layouts/components/HeaderParticles/components/StaticDecorations/types';
import { ParticlesConfig } from '@/layouts/components/HeaderParticles/types';

type EventName =
	| 'christmas'
	| 'valentinesDay'
	| 'defenderOfTheFatherland'
	| 'beginningOfSpring'
	| 'womensDay'
	| 'easter'
	| 'fifaWorldCup';

export interface EventSchedule {
	startDate: string;
	endDate: string;
}

export interface EventParticlesConfig {
	name: EventName;
	particles?: Partial<ParticlesConfig>;
	decorativeIcon?: DecorativeIconConfig;
	staticDecorations?: StaticDecorationsConfig;
	headerBackground?: string;
	schedule: EventSchedule;
}
