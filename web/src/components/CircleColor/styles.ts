import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const COLOR_CIRCLE_SIZE = 10;
const PADDING_CIRCLE_COLOR_WRAPPER = 2;

export const CircleColorElement = styled('div')<{ color: string }>(
	({ color }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',

		padding: 0,
		width: COLOR_CIRCLE_SIZE,
		height: COLOR_CIRCLE_SIZE,

		borderRadius: BorderRadiuses.half,

		background: color || 'transparent',
	})
);

export const CircleColorWrapper = styled('div')({
	paddingTop: `${PADDING_CIRCLE_COLOR_WRAPPER}px`,
});
