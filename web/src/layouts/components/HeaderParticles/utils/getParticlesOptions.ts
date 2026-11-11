import { breakpoints } from '@mms/mms-ui-library/theme';
import type { IOptions, RecursivePartial, IShape } from '@tsparticles/engine';

import {
	defaultParticlesConfig,
	DISABLED_PARTICLES_COUNT,
	FPS_LIMIT,
	PARTICLE_IMAGE_DIMENSION,
} from '../constants';
import { ParticlesConfig } from '../types';

import { HEADER_HEIGHT } from '@/layouts/main/constants';

import {
	getAdaptiveParticlesCount,
	getManualParticles,
} from './particlesDistribution';

const { desktopL } = breakpoints;

export const getParticlesOptions = (
	particlesConfig: Partial<ParticlesConfig> = {},
	viewportWidth: number = desktopL
): RecursivePartial<IOptions> => {
	const resolvedParticlesConfig: ParticlesConfig = {
		...defaultParticlesConfig,
		...particlesConfig,
	};

	const {
		count,
		direction,
		images,
		initialRotation,
		opacity,
		rotationSpeed,
		size,
		speed,
	} = resolvedParticlesConfig;
	const resolvedCount = getAdaptiveParticlesCount(count, viewportWidth);

	const particleShape: RecursivePartial<IShape> =
		images.length > 0
			? {
					type: 'image',
					options: {
						image: images.map((src) => ({
							src,
							width: PARTICLE_IMAGE_DIMENSION,
							height: PARTICLE_IMAGE_DIMENSION,
						})),
					},
			  }
			: {
					type: 'circle',
			  };

	return {
		fullScreen: { enable: false },
		fpsLimit: FPS_LIMIT,
		background: {
			color: { value: 'transparent' },
		},
		particles: {
			number: {
				value: DISABLED_PARTICLES_COUNT,
				density: {
					enable: false,
					width: desktopL,
					height: HEADER_HEIGHT,
				},
			},
			move: {
				enable: true,
				speed: {
					min: speed[0],
					max: speed[1],
				},
				direction,
				straight: true,
				warp: true,
				outModes: 'out',
			},
			opacity: {
				value: {
					min: opacity[0],
					max: opacity[1],
				},
			},
			size: {
				value: {
					min: size[0],
					max: size[1],
				},
			},
			rotate: {
				value: {
					min: initialRotation[0],
					max: initialRotation[1],
				},
				animation: {
					enable: true,
					speed: {
						min: rotationSpeed[0],
						max: rotationSpeed[1],
					},
				},
			},
			shape: particleShape,
		},
		manualParticles: getManualParticles(resolvedCount),
		detectRetina: true,
	};
};
