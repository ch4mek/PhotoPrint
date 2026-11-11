import { CSSObject } from '@emotion/react';
import styled from '@emotion/styled';

import { HEADER_DECORATIONS_Z_INDEX } from '../../constants';

import { HEADER_HEIGHT } from '@/layouts/main/constants';
import { ResponsiveValue } from '@/types/breakpoints';

import { ZoneContentAlign, ZonePosition } from './types';
import { getResponsiveStyles } from './utils';

export const DecorationsWrapper = styled('div')<{
	placement: 'top' | 'bottom';
	zonesGap?: ResponsiveValue<number>;
}>(({ theme, placement, zonesGap }) => ({
	position: 'fixed',
	top: 0,
	left: 0,
	right: 0,
	height: HEADER_HEIGHT,
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: placement === 'top' ? 'flex-start' : 'flex-end',
	pointerEvents: 'none',
	zIndex: HEADER_DECORATIONS_Z_INDEX,
	overflow: 'hidden',

	...getResponsiveStyles(theme, zonesGap, 'gap'),
}));

export const DecorationZone = styled('div')<{
	position: ZonePosition;
	contentAlign?: ZoneContentAlign;
	gap?: ResponsiveValue<number>;
	hideBelow?: number;
	customStyles?: ResponsiveValue<CSSObject>;
}>(({ theme, position, contentAlign, gap, hideBelow, customStyles }) => ({
	display: 'flex',
	alignItems: 'flex-end',
	justifyContent: contentAlign ?? 'flex-start',

	...(position === 'left' && { flexShrink: 0, marginRight: 'auto' }),
	...(position === 'center' && { flex: 1 }),
	...(position === 'right' && { marginLeft: 'auto' }),

	...getResponsiveStyles(theme, gap ?? { desktop: theme.spaces.xl }, 'gap'),
	...getResponsiveStyles(theme, customStyles),

	...(hideBelow && {
		[`@media (max-width: ${hideBelow}px)`]: {
			display: 'none',
		},
	}),
}));
