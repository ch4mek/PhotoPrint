import styled from '@emotion/styled';

import { TABLE_HEADER_HEIGHT } from '../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import {
	CURRENT_TIME_INDICATOR_WIDTH,
	CURRENT_TIME_INDICATOR_Z_INDEX,
	CURRENT_TIME_INDICATOR_TOP_ELLIPSIS,
} from './constants';

export const CurrentTimeIndicatorLine = styled('div')<{ posX: number }>(
	({ theme, posX }) => ({
		position: 'absolute',

		pointerEvents: 'none',
		top: TABLE_HEADER_HEIGHT - CURRENT_TIME_INDICATOR_WIDTH,
		left: posX,
		bottom: 0,
		zIndex: CURRENT_TIME_INDICATOR_Z_INDEX,
		width: CURRENT_TIME_INDICATOR_WIDTH,
		backgroundColor: theme.palette['graphics-accent'],

		'::before': {
			content: '""',
			position: 'absolute',
			left:
				-(CURRENT_TIME_INDICATOR_TOP_ELLIPSIS / 2) +
				CURRENT_TIME_INDICATOR_WIDTH / 2,
			top:
				-CURRENT_TIME_INDICATOR_TOP_ELLIPSIS / 2 +
				CURRENT_TIME_INDICATOR_WIDTH / 2,
			width: CURRENT_TIME_INDICATOR_TOP_ELLIPSIS,
			height: CURRENT_TIME_INDICATOR_TOP_ELLIPSIS,
			backgroundColor: theme.palette['graphics-accent'],
			borderRadius: BorderRadiuses.half,
		},
	})
);
