import { RefObject } from 'react';

import { DecorativeIconConfig } from './components/DecorativeIcon/types';
import { StaticDecorationsConfig } from './components/StaticDecorations/types';

type ParticleDirection = 'bottom' | 'left' | 'right' | 'top';

export interface ParticlesConfig {
	count: number;
	speed: [number, number];
	wind: [number, number];
	size: [number, number];
	opacity: [number, number];
	rotationSpeed: [number, number];
	initialRotation: [number, number];
	images: string[];
	direction: ParticleDirection;
}

export interface HeaderDecorationsProps {
	decorativeIcon?: DecorativeIconConfig;
	anchorRef?: RefObject<HTMLElement | null>;
	particles?: Partial<ParticlesConfig>;
	staticDecorations?: StaticDecorationsConfig;
}
