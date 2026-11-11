import styled from '@emotion/styled';

import { HEADER_HEIGHT } from '@/layouts/main/constants';

import { HEADER_DECORATIONS_Z_INDEX } from './constants';

export const ParticlesWrapper = styled('div')(
	() =>
		({
			position: 'absolute',
			top: 0,
			left: 0,
			width: '100%',
			height: HEADER_HEIGHT,
			zIndex: HEADER_DECORATIONS_Z_INDEX,
			pointerEvents: 'none',
			overflow: 'hidden',
		} as const)
);
