import { zIndexes } from '@/constants/zIndexes';

import { ParticlesConfig } from './types';

export const defaultParticlesConfig = {
	count: 20,
	speed: [0.6, 1],
	wind: [0, 0],
	size: [6, 10],
	opacity: [1, 1],
	direction: 'bottom',
	images: [],
	initialRotation: [0, 360],
	rotationSpeed: [0, 0],
} satisfies ParticlesConfig;

export const FPS_LIMIT = 60;
export const HEADER_DECORATIONS_Z_INDEX = zIndexes.header + 2;
export const PARTICLE_IMAGE_DIMENSION = 100;
export const PARTICLE_PERCENT_RANGE = 100;
export const PARTICLE_SEGMENT_CENTER_OFFSET = 0.5;
export const MIN_PARTICLES_COUNT = 1;
export const DISABLED_PARTICLES_COUNT = 0;
export const MANUAL_PARTICLE_JITTER_FACTOR = 0.3;
export const ADAPTIVE_COUNT_EXPONENT = 1.35;
export const LARGE_SCREEN_BOOST_START_RATIO = 1.1;
export const LARGE_SCREEN_BOOST_FACTOR = 3;
