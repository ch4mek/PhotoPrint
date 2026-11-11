import { breakpoints } from '@mms/mms-ui-library/theme';

import {
	ADAPTIVE_COUNT_EXPONENT,
	LARGE_SCREEN_BOOST_FACTOR,
	LARGE_SCREEN_BOOST_START_RATIO,
	MANUAL_PARTICLE_JITTER_FACTOR,
	MIN_PARTICLES_COUNT,
	PARTICLE_PERCENT_RANGE,
	PARTICLE_SEGMENT_CENTER_OFFSET,
} from '../constants';

const { desktopL } = breakpoints;

const getLargeScreenBoost = (widthRatio: number) => {
	if (widthRatio <= LARGE_SCREEN_BOOST_START_RATIO) {
		return 1;
	}

	return (
		1 +
		(widthRatio - LARGE_SCREEN_BOOST_START_RATIO) * LARGE_SCREEN_BOOST_FACTOR
	);
};

export const getAdaptiveParticlesCount = (
	baseCount: number,
	viewportWidth: number
) => {
	if (baseCount <= 0) {
		return 0;
	}

	const widthRatio = viewportWidth / desktopL;
	const adjustedRatio = widthRatio ** ADAPTIVE_COUNT_EXPONENT;
	const largeScreenBoost = getLargeScreenBoost(widthRatio);

	return Math.max(
		MIN_PARTICLES_COUNT,
		Math.round(baseCount * adjustedRatio * largeScreenBoost)
	);
};

export const getManualParticles = (count: number) => {
	if (count <= 0) {
		return [];
	}

	return Array.from({ length: count }, (_, index) => {
		const baseX =
			((index + PARTICLE_SEGMENT_CENTER_OFFSET) / count) *
			PARTICLE_PERCENT_RANGE;

		const segmentWidth = PARTICLE_PERCENT_RANGE / count;

		const jitterOffset =
			(Math.random() - PARTICLE_SEGMENT_CENTER_OFFSET) *
			segmentWidth *
			MANUAL_PARTICLE_JITTER_FACTOR;

		return {
			position: {
				x: baseX + jitterOffset,
				y: Math.random() * PARTICLE_PERCENT_RANGE,
			},
		};
	});
};
