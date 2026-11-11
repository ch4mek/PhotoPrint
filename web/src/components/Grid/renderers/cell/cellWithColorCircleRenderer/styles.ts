import styled from '@emotion/styled';

import { SMALL_COLOR_CIRCLE_SIZE } from '@/components/Grid/constants';
import { BorderRadiuses } from '@/constants/borderRadiuses';

import { CellWithColorCircleProps } from './types';

export const ColorCircleCellContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	alignItems: 'center',
	gap: `${theme.spaces.xs}px`,
}));

export const ColorCircle = styled('div')<CellWithColorCircleProps>(
	({ color }) => ({
		minHeight: SMALL_COLOR_CIRCLE_SIZE,
		minWidth: SMALL_COLOR_CIRCLE_SIZE,
		width: SMALL_COLOR_CIRCLE_SIZE,
		height: SMALL_COLOR_CIRCLE_SIZE,
		background: color,
		borderRadius: BorderRadiuses.xxl,
	})
);
